using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data.Models;
public class Camera : Entity
{
    [MaxLength(200)]
    public required string Name { get; set; }
    [MaxLength(500)]
    public required string Description { get; set; }
    public required string RTSPUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastChecked { get; set; } = DateTime.Now;

    // Parâmetros para a configuração de vídeo
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Framerate { get; set; }
    public int? Bitrate { get; set; }
    [MaxLength(5)]
    public string? AnalyzeDuration { get; set; }      
    public string? Probesize { get; set; }

    //Tunning
    public bool? ZeroLatency { get; set; }
    public bool? NoBuffer { get; set; }
    public int? GOP { get; set; }
    public int? BufferSize { get; set; }
    public int? Threads { get; set; }
    public bool? Vsync { get; set; }
    public bool? UseServerTimestamps { get; set; }

    // Opção para configuração de codificação de vídeo
    [MaxLength(30)]
    public string VideoCodec { get; set; } = "copy";
    [MaxLength(30)]
    public string Preset { get; set; } = "ultrafast";
    public int? CRF { get; set; }

    // Opções para configuração de áudio
    public bool AudioEnabled { get; set; } = false;
    [MaxLength(10)]
    public string? AudioCodec { get; set; }
    public int? AudioBitrate { get; set; }
    public int? AudioChannels { get; set; }

    // Configurações de HLS
    public int HLSTime { get; set; } = 2;
    public int HLSListSize { get; set; } = 5;

    // Parametros de Rede
    [MaxLength(3)]
    public string RtspTransport { get; set; } = "udp";

    //Parametros do Container
    public string? ServiceId { get; set; }
    public bool IsRunning { get; set; } = false;
    public bool IsRequested { get; set; } = false;
    public DateTime? LastRequested { get; set; } = DateTime.Now;

    //Snapshot
    public string? CameraSnapshotUrl { get; set; }
    public bool GenerateSnapshot { get; set; } = false;
    public double SnapshotFPS { get; set; } = 2;

    // Urls
    public string? HLSUrl { get; set; }
    public string? SnapshotUrl { get; set; }
}
