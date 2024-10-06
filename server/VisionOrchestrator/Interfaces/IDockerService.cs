using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Data.Models;
using VisionOrchestrator.Models;

namespace VisionOrchestrator.Interfaces;
public interface IDockerService
{
    // Verifica se o serviço da câmera já está rodando
    Task<bool> IsCameraServiceRunning(string cameraId);

    // Inicia um serviço no Docker Swarm para processar o stream da câmera com FFmpeg
    Task<string> StartCameraService(Camera camera);

    // Para e remove o serviço associado à câmera
    Task StopCameraService(string cameraId);

    // Retorna uma lista dos serviços de câmeras atualmente ativos
    Task<List<CameraContainer>> GetRunningCameraServices();
}
