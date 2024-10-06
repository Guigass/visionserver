using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Vision.Data.Context;
using Microsoft.Extensions.Configuration;
using Vision.Data.Interfaces;
using Vision.Data.Repositories;
using VisionOrchestrator.Workers;
using VisionOrchestrator.Interfaces;
using VisionOrchestrator.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Vision Orchestrator is starting...");

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<VisionContext>(options => options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));

                services.AddScoped<ICameraRepository, CameraRepository>();

                services.AddSingleton<IDockerService, DockerService>();
                services.AddSingleton<ICameraNotificationListener>(sp =>
                {
                    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")!;
                    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

                    return new CameraNotificationListener(connectionString, scopeFactory);
                });

                services.AddHostedService<CameraOrchestratorWorker>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build();

        await host.RunAsync();
    }
}