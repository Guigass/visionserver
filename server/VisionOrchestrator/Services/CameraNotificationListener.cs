using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using Vision.Data.Interfaces;
using Vision.Data.Models;
using VisionOrchestrator.Interfaces;

public class CameraNotificationListener : ICameraNotificationListener
{
    private readonly string _connectionString;
    private readonly IServiceScopeFactory _scopeFactory;

    public CameraNotificationListener(string connectionString,
                                      IServiceScopeFactory scopeFactory)
    {
        _connectionString = connectionString;
        _scopeFactory = scopeFactory;
    }

    public void StartListening()
    {
        Task.Run(async () => await ListenForNotifications());
    }

    private async Task ListenForNotifications()
    {
        while (true)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new NpgsqlCommand("LISTEN camera_is_requested;", conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }

                    conn.Notification += async (o, e) =>
                    {
                        try
                        {
                            await HandleNotification(e.Payload);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error handling notification: {ex.Message}");
                        }
                    };

                    while (true)
                    {
                        await conn.WaitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}. Retrying in 10 seconds...");

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }

    private async Task HandleNotification(string payload)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var cameraRepository = scope.ServiceProvider.GetRequiredService<ICameraRepository>();
            var dockerService = scope.ServiceProvider.GetRequiredService<IDockerService>();

            var cameraId = new Guid(payload);
            var camera = await cameraRepository.GetByIdAsync(cameraId);

            if (camera.IsRequested && !camera.IsRunning)
            {
                camera.ServiceId = await dockerService.StartCameraService(camera);
                camera.IsRunning = true;
            }
            else if (!camera.IsRequested && camera.IsRunning)
            {
                await dockerService.StopCameraService(camera.Id.ToString());
                camera.ServiceId = null;
                camera.IsRunning = false;
            }

            camera.LastRequested = DateTime.Now;

            await cameraRepository.SaveChangesAsync();
        }
    }
}
