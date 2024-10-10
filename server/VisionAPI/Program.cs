using Microsoft.EntityFrameworkCore;
using Vision.Data.Context;
using VisionAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VisionContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddControllers();

builder.Services.WebApiConfig();

builder.Services.ResolveDependencies();

var app = builder.Build();

await app.Migrate();

await app.AddRoles();

if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("Production");
    app.UseHsts();
}

app.UseAppConfiguration();

app.MapControllers();

app.Run();
