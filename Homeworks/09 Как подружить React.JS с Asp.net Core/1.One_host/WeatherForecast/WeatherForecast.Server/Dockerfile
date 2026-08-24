
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-all
RUN apt-get update -yq && \
    apt-get install -yq curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -yq nodejs

WORKDIR /src

COPY ./WeatherForecast/WeatherForecast.Server/WeatherForecast.Server.csproj ./WeatherForecast/WeatherForecast.Server/
RUN dotnet restore "./WeatherForecast/WeatherForecast.Server/WeatherForecast.Server.csproj"

COPY ./WeatherForecast ./WeatherForecast/
RUN dotnet build "./WeatherForecast/WeatherForecast.Server/WeatherForecast.Server.csproj" -c Release --no-restore
RUN dotnet publish "./WeatherForecast/WeatherForecast.Server/WeatherForecast.Server.csproj" -c Release -o /app/publish -p:UseAppHost=false --no-restore --no-build


FROM base AS final
WORKDIR /app
COPY --from=build-all /app/publish ./
ENTRYPOINT ["dotnet", "WeatherForecast.Server.dll"]
