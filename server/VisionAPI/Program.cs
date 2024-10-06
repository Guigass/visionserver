using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Vision.Data.Context;
using VisionAPI.Configuration;
using VisionAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VisionContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ResolveDependencies();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VisionContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var retryCount = 5;
    var delayBetweenRetries = TimeSpan.FromSeconds(5);

    for (var i = 0; i < retryCount; i++)
    {
        try
        {
            logger.LogInformation("Tentando realizar a migração...");
            dbContext.Database.Migrate();
            logger.LogInformation("Migração realizada com sucesso!");
            break; // Se a migração funcionar, sai do loop.
        }
        catch (PostgresException ex)
        {
            logger.LogError(ex, "Erro ao tentar aplicar a migração. Tentativa {i} de {retryCount}", i + 1, retryCount);

            if (i == retryCount - 1)
            {
                throw; // Se atingiu o número máximo de tentativas, lança a exceção.
            }

            logger.LogInformation($"Esperando {delayBetweenRetries.TotalSeconds} segundos antes de tentar novamente.");
            await Task.Delay(delayBetweenRetries);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAntiXssMiddleware();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
