
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ./WeatherForecast/WeatherForecast.Server/WeatherForecast.csproj ./
RUN dotnet restore "./WeatherForecast.csproj"

COPY ./WeatherForecast/WeatherForecast.Server/. ./
RUN dotnet build "./WeatherForecast.csproj" -c Release --no-restore
RUN dotnet publish "./WeatherForecast.csproj" -c Release -o /app/publish -p:UseAppHost=false --no-restore --no-build


FROM base AS final
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "WeatherForecast.dll"]
