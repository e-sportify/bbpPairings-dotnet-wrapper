﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /bbp-bin
RUN wget https://github.com/e-sportify/bbpPairings/releases/download/v4.1.4/bbpPairings-linux

WORKDIR /src
COPY ["bbpPairings-dotnet-wrapper/bbpPairings-dotnet-wrapper.csproj", "bbpPairings-dotnet-wrapper/"]
RUN dotnet restore "bbpPairings-dotnet-wrapper/bbpPairings-dotnet-wrapper.csproj"
COPY . .
WORKDIR "/src/bbpPairings-dotnet-wrapper"
RUN dotnet build "bbpPairings-dotnet-wrapper.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "bbpPairings-dotnet-wrapper.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /bbp-bin .
RUN chmod +x bbpPairings-linux

ENTRYPOINT ["dotnet", "bbpPairings-dotnet-wrapper.dll"]
