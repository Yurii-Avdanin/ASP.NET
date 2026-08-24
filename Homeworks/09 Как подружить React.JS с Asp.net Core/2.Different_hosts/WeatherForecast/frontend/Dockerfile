FROM nginx:stable-alpine AS base

ENV API_URL=http://backend:80
ENV NGINX_ENVSUBST_FILTER=API_URL

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

FROM node:22-alpine AS build
WORKDIR /app

COPY ./WeatherForecast/frontend/package*.json ./
RUN npm ci

COPY ./WeatherForecast/frontend/. ./
RUN npm run build


FROM base AS final
COPY ./WeatherForecast/config/nginx.conf /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist /usr/share/nginx/html