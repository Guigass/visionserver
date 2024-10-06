using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionOrchestrator.Models;
public class CameraContainer
{
    // Identificador único da câmera (relacionado ao banco de dados)
    public string CameraId { get; set; }

    // Nome ou identificador único do container Docker que está processando a câmera
    public string? ContainerId { get; set; }

    // Nome da câmera
    public string? CameraName { get; set; }

    // Status atual do container (Running, Stopped, etc.)
    public string? Status { get; set; }

    // Data e hora de criação do container
    public DateTime CreatedAt { get; set; }

    // Data e hora da última verificação do status do container
    public DateTime LastCheckedAt { get; set; }
}
