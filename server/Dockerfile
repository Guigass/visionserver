FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VisionAPI/VisionAPI.csproj", "VisionAPI/"]
COPY ["Vision.Data/Vision.Data.csproj", "Vision.Data/"]
RUN dotnet restore "VisionAPI/VisionAPI.csproj"
COPY . .
WORKDIR "/src/VisionAPI"
RUN dotnet build "./VisionAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VisionAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VisionAPI.dll"]