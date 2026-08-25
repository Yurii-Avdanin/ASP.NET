#!/bin/bash

run_script_build_image() {
    local IMAGE_NAME="$1"
    local PATH_PROJECT="$2"
    local PATH_DOCKEFILE="$3"

    echo "[INF] IMAGE_NAME = ${IMAGE_NAME}"
    echo "[INF] docker build -t [${IMAGE_NAME}] -f [${PATH_DOCKEFILE}] [${PATH_PROJECT}]"

    docker build -t ${IMAGE_NAME} -f ${PATH_DOCKEFILE} ${PATH_PROJECT}
}

# Create image backend:
run_script_build_image "homework/weather-api:1.0.1" "." "./WeatherForecast/WeatherForecast.Server/Dockerfile"

# Create image frontend:
run_script_build_image "homework/weather-front:1.0.1" "." "./WeatherForecast/frontend/Dockerfile"
