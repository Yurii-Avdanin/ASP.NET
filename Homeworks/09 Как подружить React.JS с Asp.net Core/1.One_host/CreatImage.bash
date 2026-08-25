#!/bin/bash

IMAGE_NAME=homework/weather-forecast:1.0.1

PATH_PROJECT=.

PATH_DOCKEFILE=${PATH_PROJECT}/WeatherForecast/WeatherForecast.Server/Dockerfile

echo "IMAGE_NAME = ${IMAGE_NAME}"

echo "docker build -t [${IMAGE_NAME}] -f [${PATH_DOCKEFILE}] [${PATH_PROJECT}]"

docker build -t ${IMAGE_NAME} -f ${PATH_DOCKEFILE} ${PATH_PROJECT}
# docker build --no-cache --progress=plain -t ${IMAGE_NAME} -f ${PATH_DOCKEFILE} ${PATH_PROJECT} 2>&1 | tee "./image_create_$(date +%Y-%m-%d).log"