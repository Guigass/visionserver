
# VisionServer

**VisionServer** is an open-source security camera management software designed to convert RTSP streams into HLS (HTTP Live Streaming). It facilitates easy video streaming over HTTP using a scalable architecture based on **Docker** to manage multiple servers, and **Nginx** as the API Gateway and Load Balancer.

> ⚠️ **Project Status**: VisionServer is under construction and not yet ready for use. The core features are still in development, and implementations may change drastically. Feel free to follow the progress and contribute, but note that the project is not functional yet.

## Key Features (Planned)

- **RTSP to HLS Conversion**: Uses FFmpeg to convert RTSP streams into HLS, enabling easy video playback in browsers.
- **Management Interface**: A web interface built with Angular to register, monitor, and manage cameras.
- **Automatic Scalability with Docker**: VisionServer uses Docker to manage multiple containers, ensuring dynamic workload balancing.
- **Integrated Nginx**: Nginx runs within the Docker orchestration, acting as the API Gateway and Load Balancer for stream distribution.
- **High Availability**: Supports replicas and multiple Nginx servers to ensure resilience and redundancy.
- **Monitoring and Logging**: Integrates with Prometheus and Grafana for system health monitoring, and uses the ELK Stack (Elasticsearch, Logstash, Kibana) for centralized logging.
- **Security**: Implements JWT authentication and SSL/TLS support to ensure secure communication between all services.

## How It Works (Planned)

1. The user registers a new camera via the web interface.
2. The system generates an HLS URL for the camera.
3. Stream processing (RTSP to HLS) is automatically distributed across Docker containers.
4. Nginx, integrated into Docker orchestration, routes streaming requests to the correct containers, balancing the load.

## Technologies Used

- **Backend**: .NET Core with FFmpeg
- **Frontend**: Angular
- **Orchestration**: Docker (Docker Compose)
- **Load Balancing**: Nginx
- **Database**: PostgreSQL
- **Monitoring**: Prometheus + Grafana
- **Logging**: Elasticsearch, Logstash, Kibana (ELK Stack)

## How to Run VisionServer

> ⚠️ **Important**: This project is still under construction and is not yet ready for use. The following instructions are preliminary and subject to change.

### Prerequisites

- **Docker** and **Docker Compose** installed.

### Steps to Run

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/visionserver.git
   cd visionserver
   ```

2. Build and start the Docker containers:
   ```bash
   docker-compose up --build
   ```

3. Access the frontend at `http://localhost:4200` and the API at `http://localhost:5000`.

> **Note**: These instructions are for local development and may change as the project progresses.

## Contributing

The project is still in its early development stages. Feel free to open issues or pull requests to discuss improvements, bug fixes, and new features.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
