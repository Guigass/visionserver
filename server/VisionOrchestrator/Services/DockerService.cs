using Docker.DotNet.Models;
using Docker.DotNet;
using Vision.Data.Models;
using VisionOrchestrator.Interfaces;
using VisionOrchestrator.Models;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace VisionOrchestrator.Services;
public class DockerService : IDockerService
{
    private readonly DockerClient _client;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public DockerService(IHostApplicationLifetime hostApplicationLifetime)
    {
        _hostApplicationLifetime = hostApplicationLifetime;

        var defaultLocalApiUri = new Uri(Environment.GetEnvironmentVariable("DOCKER_HOST")!);

        _client = new DockerClientConfiguration(defaultLocalApiUri).CreateClient();

        _hostApplicationLifetime!.ApplicationStopping.Register(OnShutdown);
    }

    private async void OnShutdown()
    {
        var cameras = await GetRunningCameraServices();

        foreach (var camera in cameras)
        {
            await StopCameraService(camera.CameraId);
        }
    }

    // Verifica se o serviço da câmera já está rodando
    public async Task<bool> IsCameraServiceRunning(string cameraId)
    {

        var services = await _client.Swarm.ListServicesAsync(new ServicesListParameters
        {
            Filters = new ServiceFilter
            {
                Label = new[] { $"cameraId={cameraId}" }, // Filtra o serviço pelo ID da câmera
            }
        });

        return services.Any();
    }

    // Inicia um serviço no Docker Swarm para processar o stream da câmera com FFmpeg
    public async Task<string> StartCameraService(Camera camera)
    {
        if (await IsCameraServiceRunning(camera.Id.ToString()))
        {
            Console.WriteLine($"Service for camera {camera.Name} is already running.");
            return null!;
        }

        var camFolder = GetHlsFolderPath(camera.Id.ToString()); // Caminho do volume para os segmentos HLS

        #region Stream Command
        // Define o arquivo de playlist HLS como um arquivo .m3u8
        string playlistOutput = $"{camFolder}/playlist.m3u8"; // Arquivo .m3u8 principal

        // Parâmetros do FFmpeg para o processamento do stream
        string segmentPath = $"{camFolder}/segment_%03d.ts";  // Define o caminho de saída para os segmentos HLS

        List<string> hlsOptions = new List<string>
        {
            "-i", $"{camera.RTSPUrl}", // URL RTSP da câmera
            "-c:v", camera.VideoCodec, // Codec de vídeo
            "-preset", camera.Preset, // Preset de codificação
            "-f", "hls", // Formato HLS
            "-hls_time", camera.HLSTime.ToString(), // Tempo por segmento HLS
            "-hls_list_size", camera.HLSListSize.ToString(), // Tamanho da lista de segmentos HLS
            "-hls_flags", "delete_segments",  // Deletar segmentos antigos
            "-hls_segment_filename", segmentPath, // Arquivo de saída dos segmentos
            "-rtsp_transport", camera.RtspTransport,  // Transporte RTSP
            //"-loglevel", "debug"
        };

        // Adicionar Bitrate se estiver definido
        if (camera.Bitrate != null & camera.Bitrate > 0)
        {
            hlsOptions.Add("-b:v");
            hlsOptions.Add($"{camera.Bitrate}k");
        }

        // Adicionar Probesize se estiver definido
        if (camera.Probesize != null)
        {
            hlsOptions.Add("-probesize");
            hlsOptions.Add(camera.Probesize.ToString());
        }

        // Adicionar AnalyzeDuration se estiver definido
        if (camera.AnalyzeDuration != null)
        {
            hlsOptions.Add("-analyzeduration");
            hlsOptions.Add(camera.AnalyzeDuration.ToString());
        }

        // Otimização para baixa latência
        if (camera.ZeroLatency == true)
        {
            hlsOptions.Add("-tune");
            hlsOptions.Add("zerolatency");
        }

        // Sem buffer
        if (camera.NoBuffer == true)
        {
            hlsOptions.Add("-fflags");
            hlsOptions.Add("nobuffer+genpts");
        }
        else
        {
            hlsOptions.Add("-fflags");
            hlsOptions.Add("genpts");
        }

        if (camera.Threads != null && camera.Threads > 0)
        {
            hlsOptions.Add("-threads");
            hlsOptions.Add(camera.Threads.ToString()!);
        }

        if (camera.Vsync == true)
        {
            hlsOptions.Add("-vsync");
            hlsOptions.Add("1");
        }

        if (camera.UseServerTimestamps == true)
        {
            hlsOptions.Add("-use_wallclock_as_timestamps");
            hlsOptions.Add("1");
        }

        // Adicionar GOP se estiver definido
        if (camera.GOP != null && camera.GOP > 0)
        {
            hlsOptions.Add("-g");
            hlsOptions.Add(camera.GOP.ToString());
        }

        if(camera.BufferSize != null && camera.BufferSize > 0)
        {
            hlsOptions.Add("-buffer_size");
            hlsOptions.Add($"{camera.BufferSize}k");
        }

        // Adicionar resolução e framerate se o VideoCodec não for "copy" e se estiverem definidos
        if (camera.VideoCodec != "copy")
        {
            if (camera.CRF != null)
            {
                hlsOptions.Add("-crf");
                hlsOptions.Add(camera.CRF.ToString()!);
            }

            if (camera.Width > 0 && camera.Height > 0 && camera.Framerate > 0)
            {
                hlsOptions.Add("-vf");
                hlsOptions.Add($"scale={camera.Width}:{camera.Height},fps={camera.Framerate}");
            }
            else if (camera.Width > 0 && camera.Height > 0)
            {
                hlsOptions.Add("-vf");
                hlsOptions.Add($"scale={camera.Width}:{camera.Height}");
            }
            else if (camera.Framerate > 0)
            {
                hlsOptions.Add("-vf");
                hlsOptions.Add($"fps={camera.Framerate}");
            }
        }

        // Verifica se o áudio está habilitado e adiciona parâmetros de áudio
        if (camera.AudioEnabled)
        {
            if (!string.IsNullOrEmpty(camera.AudioCodec))
            {
                hlsOptions.Add("-c:a");
                hlsOptions.Add(camera.AudioCodec);
            }

            if (camera.AudioBitrate != null && camera.AudioBitrate > 0)
            {
                hlsOptions.Add("-b:a");
                hlsOptions.Add($"{camera.AudioBitrate}k");
            }

            if (camera.AudioChannels != null && camera.AudioChannels > 0)
            {
                hlsOptions.Add("-ac");
                hlsOptions.Add(camera.AudioChannels.ToString()!);
            }
        }
        else
        {
            hlsOptions.Add("-an");
        }

        #endregion

        hlsOptions.Add(playlistOutput); // Arquivo de playlist HLS

        Console.WriteLine($"Starting service for camera {camera.Name}...");
        Console.WriteLine($"FFmpeg Command: ffmpeg {string.Join(" ", hlsOptions)}");

        var service = new ServiceCreateParameters
        {
            Service = new ServiceSpec
            {
                Name = $"visionserver_ffmpeg_{camera.Id}", // Nome único para o serviço
                TaskTemplate = new TaskSpec
                {
                    ContainerSpec = new ContainerSpec
                    {
                        Image = "visionserver/ffmpeg", // Imagem base do FFmpeg
                        Env = new List<string>
                        {
                            "TZ=America/Sao_Paulo",
                            $"FFMPEG1_ARGS={string.Join(" ", hlsOptions)}", // Passa os argumentos do HLS como variável de ambiente
                        },
                        Mounts = new List<Mount>
                        {
                            new Mount
                            {
                                Source = "visionserver_cam_data", // Nome do volume
                                Target = "/app/cam", // Caminho dentro do container
                                Type = "volume", // Volume Docker
                                ReadOnly = false // Certifica-se de que o volume não está em modo somente leitura
                            }
                        }
                    },
                    Networks = new List<NetworkAttachmentConfig>
                    {
                        new NetworkAttachmentConfig
                        {
                            Target = "host" // Rede Host
                        }
                    },
                    RestartPolicy = new SwarmRestartPolicy
                    {
                        Condition = "on-failure", // Tenta reiniciar em caso de falha
                        MaxAttempts = 3 // Limita a 3 tentativas
                    }
                },
                Mode = new ServiceMode
                {
                    Replicated = new ReplicatedService
                    {
                        Replicas = 1 // Um serviço por câmera
                    }
                },
                Labels = new Dictionary<string, string>
                {
                    { "com.docker.stack.namespace", "visionserver" }, // Adiciona o serviço à stack 'visionserver'
                    { "cameraId", camera.Id.ToString() },
                    { "cameraName", camera.Name }
                }
            }
        };

        if (camera.GenerateSnapshot)
        {
            var camSnapShotFolder = GetSnapshotFolderPath(camera.Id.ToString()); // Caminho do volume para os snapshots

            string snapshotPath = $"{camSnapShotFolder}/snapshot.jpg"; // Define o caminho de saída para os snapshots

            var snapshotOptions = new List<string>
            {
                "-rtsp_transport", camera.RtspTransport,
                "-i", $"{camera.RTSPUrl}", // URL RTSP da câmera
                "-vf", $"fps={camera.SnapshotFPS}", // FPS
                "-q:v", "2", // Qualidade do snapshot
                "-s", $"{camera.Width}x{camera.Height}", // Resolução do snapshot
                "-update", "1", // Atualiza a mesma imagem
                "-loglevel", "debug",
                snapshotPath // Caminho do snapshot
            };

            Console.WriteLine($"Snapshot Command: ffmpeg {string.Join(" ", snapshotOptions)}");

            service.Service.TaskTemplate.ContainerSpec.Env.Add($"FFMPEG2_ARGS={string.Join(" ", snapshotOptions)}");
        }

        // Cria o serviço Docker Swarm
        var createServiceResponse = await _client.Swarm.CreateServiceAsync(service);

        Console.WriteLine($"Started service for camera {camera.Name}, Service ID: {createServiceResponse.ID}");

        if (createServiceResponse.Warnings != null && createServiceResponse.Warnings.Count > 0)
            Console.WriteLine($"Service for camera {camera.Name} has warnings: {string.Join(" ", createServiceResponse.Warnings)}");

        return createServiceResponse.ID;
    }

    // Para e remove o serviço associado à câmera
    public async Task StopCameraService(string cameraId)
    {
        var services = await _client.Swarm.ListServicesAsync(new ServicesListParameters
        {
            Filters = new ServiceFilter
            {
                Label = new[] { $"cameraId={cameraId}" }, // Filtra o serviço pelo ID da câmera
            }
        });

        if (services.Any())
        {
            var service = services.First();

            var serviceId = service.ID;
            await _client.Swarm.RemoveServiceAsync(serviceId);

            //TODO Remove HLS Files and Snapshot Files

            Console.WriteLine($"Stopped and removed service for camera {cameraId}.");
        }
        else
        {
            Console.WriteLine($"No running service found for camera {cameraId}.");
        }
    }

    // Retorna uma lista dos serviços de câmeras atualmente ativos
    public async Task<List<CameraContainer>> GetRunningCameraServices()
    {
        var services = await _client.Swarm.ListServicesAsync(new ServicesListParameters());

        var cameraServices = services
            .Where(s => s.Spec.Name.Contains("camera_ffmpeg_"))
            .Select(s => new CameraContainer
            {
                ContainerId = s.ID,
                CameraName = s.Spec.Name,
                CameraId = s.Spec.Labels["cameraId"],
                CreatedAt = s.CreatedAt,
                LastCheckedAt = DateTime.Now,
                Status = s.Spec.Mode.Replicated.Replicas == 0 ? "Stopped" : "Running"
            })
            .ToList();

        return cameraServices;
    }

    private string GetFolderName(string name)
    {
        return name.Replace(" ", "_").ToLower();
    }

    private string GetHlsFolderPath(string name)
    {
        var camPath = GetFolderName(name);

        var path = $"/app/cam/hls/{camPath}";

        //Cria o Diretorio para salvar os segmentos
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    private string GetSnapshotFolderPath(string name)
    {
        var camPath = GetFolderName(name);

        var path = $"/app/cam/snapshot/{camPath}";

        //Cria o Diretorio para salvar os segmentos
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }
}
