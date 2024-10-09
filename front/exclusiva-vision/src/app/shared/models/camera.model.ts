export interface Camera {
    id: string;
    name: string;
    description: string;
    rtspUrl: string;
    isActive: boolean;
    lastChecked: Date;
  
    // Parâmetros para a configuração de vídeo
    width?: number;
    height?: number;
    framerate?: number;
    bitrate?: number;
    analyzeDuration?: number;
    probesize?: number;
  
    // Tunning
    zeroLatency?: boolean;
    noBuffer?: boolean;
    gop?: number;
    bufferSize?: number;
    threads?: number;
    vsync?: boolean;
    useServerTimestamps?: boolean;
  
    // Opção para configuração de codificação de vídeo
    videoCodec: string;
    preset: string;
    crf?: number;
  
    // Opções para configuração de áudio
    audioEnabled: boolean;
    audioCodec?: string;
    audioBitrate?: number;
    audioChannels?: number;
  
    // Configurações de HLS
    hlsTime: number;
    hlsListSize: number;
  
    // Parâmetros de Rede
    rtspTransport: string;
  
    // Parâmetros do Container
    serviceId?: string;
    isRunning: boolean;
    isRequested: boolean;
    lastRequested?: Date;
  
    // Snapshot
    cameraSnapshotUrl?: string;
    generateSnapshot: boolean;
    snapshotFPS: number;
  
    // Urls
    hlsUrl?: string;
    snapshotUrl?: string;
  }
  