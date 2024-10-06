using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Vision.Data.Interfaces;
using Vision.Data.Models;
using VisionOrchestrator.Interfaces;

namespace VisionOrchestrator.Workers;
public class CameraOrchestratorWorker : BackgroundService
{
    private readonly IDockerService _dockerService;
    private readonly ICameraNotificationListener _cameraNotificationListener;
    private readonly IServiceScopeFactory _scopeFactory;

    public CameraOrchestratorWorker(
                                    IDockerService dockerService, 
                                    ICameraNotificationListener cameraNotificationListener,
                                    IServiceScopeFactory scopeFactory)
    {
        _dockerService = dockerService;
        _cameraNotificationListener = cameraNotificationListener;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cameraNotificationListener.StartListening();

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var cameraRepository = scope.ServiceProvider.GetRequiredService<ICameraRepository>();

                var cameras = await cameraRepository.GetAllAsync();

                foreach (var camera in cameras)
                {
                    camera.IsRunning = await _dockerService.IsCameraServiceRunning(camera.Id.ToString());

                    if (camera.IsRunning)
                    {
                        var timeSinceLastRequest = DateTime.Now - camera.LastRequested;
                        if (timeSinceLastRequest >= TimeSpan.FromMinutes(30))
                        {
                            await _dockerService.StopCameraService(camera.Id.ToString());
                            camera.ServiceId = null;
                            camera.IsRequested = false;
                            camera.IsRunning = false;
                        }
                    }

                    await cameraRepository.SaveChangesAsync();
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}

