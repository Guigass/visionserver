using Npgsql;
using Vision.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace VisionAPI.Configuration;


public static class Migration
{
    public static async Task Migrate(this WebApplication app)
    {
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
    }
}
