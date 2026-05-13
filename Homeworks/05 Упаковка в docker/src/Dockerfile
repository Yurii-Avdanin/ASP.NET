FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY . .
RUN dotnet restore ./PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj
RUN dotnet build ./PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj -c $BUILD_CONFIGURATION --no-restore
RUN dotnet publish ./PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore --no-build


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
LABEL maintainer="Yurii Avdanin"
LABEL description="PromoCode Factory API"
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8091
ENTRYPOINT ["dotnet", "PromoCodeFactory.WebHost.dll"]
