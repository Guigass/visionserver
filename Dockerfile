# Utilizando a imagem base do Ubuntu mais recente
FROM ubuntu:20.04

# Definir argumentos e variáveis de ambiente
ARG DEBIAN_FRONTEND=noninteractive
ENV TZ=America/Sao_Paulo

# Atualizar pacotes e instalar dependências essenciais
RUN apt-get update && apt-get install -y \
    autoconf \
    automake \
    build-essential \
    cmake \
    git \
    libass-dev \
    libfreetype6-dev \
    libgnutls28-dev \
    libmp3lame-dev \
    libsdl2-dev \
    libtool \
    libva-dev \
    libvdpau-dev \
    libvorbis-dev \
    libxcb1-dev \
    libxcb-shm0-dev \
    libxcb-xfixes0-dev \
    pkg-config \
    texinfo \
    wget \
    yasm \
    zlib1g-dev \
    nasm \
    libx264-dev \
    libx265-dev \
    libnuma-dev \
    libvpx-dev \
    libfdk-aac-dev \
    libopus-dev \
    libaom-dev \
    libssl-dev \
    libva-drm2 \
    libva-glx2 \
    libva-wayland2 \
    libva-x11-2 \
    vainfo \
    clinfo \
    && apt-get clean

# Definir diretório de trabalho
WORKDIR /root

# Clonar e compilar o FFmpeg com suporte a GPUs e codecs relevantes
RUN git clone https://git.ffmpeg.org/ffmpeg.git ffmpeg \
    && cd ffmpeg \
    && ./configure \
        --disable-debug \
        --disable-doc \
        --disable-ffplay \
        --enable-gpl \
        --enable-libass \
        --enable-libfdk_aac \
        --enable-libfreetype \
        --enable-libmp3lame \
        --enable-libopus \
        --enable-libvorbis \
        --enable-libvpx \
        --enable-libx264 \
        --enable-libx265 \
        --enable-nvenc \
        --enable-nonfree \
        --enable-openssl \
        --enable-vaapi \
    && make -j$(nproc) \
    && make install \
    && make distclean

# Limpar cache e arquivos temporários
RUN apt-get remove --purge -y \
    build-essential \
    git \
    && apt-get autoremove -y \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Testar o FFmpeg
RUN ffmpeg -version

# Definir o ponto de entrada padrão
ENTRYPOINT ["/bin/sh", "-c"]
