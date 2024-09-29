
# VisionServer Roadmap

This roadmap outlines the development stages planned for VisionServer, prioritizing essential features, performance improvements, and security enhancements.

## Phase 1: Core Features (Completed)
- [x] Camera registration implementation via Angular frontend.
- [x] RTSP to HLS conversion using FFmpeg.
- [x] Load balancing across multiple Docker containers.
- [x] Nginx integration with Docker for load balancing.

## Phase 2: Security and Monitoring (In Progress)
- [ ] Implement JWT authentication for the API.
- [ ] Set up SSL/TLS in Nginx to ensure secure communication.
- [ ] Configure Prometheus to monitor CPU, memory usage, and container loads.
- [ ] Configure Grafana to display performance and health dashboards.
- [ ] Centralize logging with Elasticsearch, Logstash, and Kibana (ELK Stack).

## Phase 3: Scalability and Performance Improvements
- [ ] Implement caching with Redis to improve query performance.
- [ ] Add Docker container auto-scaling based on increasing load.
- [ ] Implement read replication or sharding in PostgreSQL for database scalability.

## Phase 4: Advanced Features
- [ ] Support for multiple codecs (H.264, H.265).
- [ ] Synchronize container states for seamless camera migration.
- [ ] Support for audio streams alongside video streams.

## Phase 5: Testing and Optimization
- [ ] Implement automated testing to ensure code quality.
- [ ] Optimize performance for RTSP to HLS conversion.
- [ ] Enhance frontend for real-time metric visualization.

## Phase 6: Version 1.0 Release
- [ ] Official release of VisionServer version 1.0 with all core and advanced features implemented.
