FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src
COPY PromoCodeFactory.Core/PromoCodeFactory.Core.csproj PromoCodeFactory.Core/PromoCodeFactory.Core.csproj
COPY PromoCodeFactory.DataAccess/PromoCodeFactory.DataAccess.csproj PromoCodeFactory.DataAccess/PromoCodeFactory.DataAccess.csproj
COPY PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj
COPY PromoCodeFactory.UnitTests/PromoCodeFactory.UnitTests.csproj PromoCodeFactory.UnitTests/PromoCodeFactory.UnitTests.csproj
RUN dotnet restore PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj
RUN dotnet restore PromoCodeFactory.UnitTests/PromoCodeFactory.UnitTests.csproj


FROM restore AS test
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet build -c $BUILD_CONFIGURATION PromoCodeFactory.UnitTests/PromoCodeFactory.UnitTests.csproj --no-restore
RUN dotnet test PromoCodeFactory.UnitTests/PromoCodeFactory.UnitTests.csproj -c $BUILD_CONFIGURATION --no-build --verbosity normal


FROM test AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN dotnet build -c $BUILD_CONFIGURATION PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj --no-restore


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN dotnet publish PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore --no-build


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PromoCodeFactory.WebHost.dll"]