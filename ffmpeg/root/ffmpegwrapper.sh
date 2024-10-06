#! /bin/bash

FULL_ARGS=( "$@" )

set_uidgid () {
  # setup abc based on file perms
  PUID=$(stat -c %u "${INPUT_FILE}")
  PGID=$(stat -c %g "${INPUT_FILE}")
  groupmod -o -g "$PGID" abc
  usermod -o -u "$PUID" abc
}

run_ffmpeg () {
  # Verificar se ambos os comandos (HLS e Snapshot) estão presentes
  if [[ ! -z "$FFMPEG1_ARGS" && ! -z "$FFMPEG2_ARGS" ]]; then
    echo "Executing HLS stream and snapshot capture..."
    
    # Executar o comando de HLS
    echo "Starting HLS stream: /usr/local/bin/ffmpeg $FFMPEG1_ARGS"
    exec /usr/local/bin/ffmpeg $FFMPEG1_ARGS &
    
    # Executar o comando de captura de snapshot
    echo "Starting snapshot capture: /usr/local/bin/ffmpeg $FFMPEG2_ARGS"
    exec /usr/local/bin/ffmpeg $FFMPEG2_ARGS &
    
    # Aguardar ambos os processos
    wait
  # Se apenas o HLS estiver presente, rodar apenas ele
  elif [[ ! -z "$FFMPEG1_ARGS" ]]; then
    echo "Executing HLS stream only..."
    exec /usr/local/bin/ffmpeg $FFMPEG1_ARGS
  # Se apenas o snapshot estiver presente, rodar apenas ele
  elif [[ ! -z "$FFMPEG2_ARGS" ]]; then
    echo "Executing snapshot capture only..."
    exec exec /usr/local/bin/ffmpeg $FFMPEG2_ARGS
  else
    echo "No valid FFmpeg arguments provided."
    exit 1
  fi
}

# look for input file value
for i in "$@"
do
  if [ ${KILL+x} ]; then
    INPUT_FILE=$i
    break
  fi
  if [ "$i" == "-i" ]; then
    KILL=1
  fi
done

## hardware support ##
FILES=$(find /dev/dri /dev/dvb /dev/snd -type c -print 2>/dev/null)

for i in $FILES
do
    VIDEO_GID=$(stat -c '%g' "${i}")
    VIDEO_UID=$(stat -c '%u' "${i}")
    # check if user matches device
    if id -u abc | grep -qw "${VIDEO_UID}"; then
        echo "**** permissions for ${i} are good ****"
    else
        # check if group matches and that device has group rw
        if id -G abc | grep -qw "${VIDEO_GID}" && [ $(stat -c '%A' "${i}" | cut -b 5,6) = "rw" ]; then
            echo "**** permissions for ${i} are good ****"
        # check if device needs to be added to video group
        elif ! id -G abc | grep -qw "${VIDEO_GID}"; then
            # check if video group needs to be created
            VIDEO_NAME=$(getent group "${VIDEO_GID}" | awk -F: '{print $1}')
            if [ -z "${VIDEO_NAME}" ]; then
                VIDEO_NAME="video$(head /dev/urandom | tr -dc 'a-z0-9' | head -c4)"
                groupadd "${VIDEO_NAME}"
                groupmod -g "${VIDEO_GID}" "${VIDEO_NAME}"
                echo "**** creating video group ${VIDEO_NAME} with id ${VIDEO_GID} ****"
            fi
            echo "**** adding ${i} to video group ${VIDEO_NAME} with id ${VIDEO_GID} ****"
            usermod -a -G "${VIDEO_NAME}" abc
        fi
        # check if device has group rw
        if [ $(stat -c '%A' "${i}" | cut -b 5,6) != "rw" ]; then
            echo -e "**** The device ${i} does not have group read/write permissions, attempting to fix inside the container. ****"
            chmod g+rw "${i}"
        fi
    fi
done

run_ffmpeg