# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for efficient layer caching
COPY ["src/FlowDesk.Api/FlowDesk.Api.csproj", "src/FlowDesk.Api/"]
COPY ["src/FlowDesk.Application/FlowDesk.Application.csproj", "src/FlowDesk.Application/"]
COPY ["src/FlowDesk.Domain/FlowDesk.Domain.csproj", "src/FlowDesk.Domain/"]
COPY ["src/FlowDesk.Infrastructure/FlowDesk.Infrastructure.csproj", "src/FlowDesk.Infrastructure/"]

RUN dotnet restore "src/FlowDesk.Api/FlowDesk.Api.csproj"

# Copy all source files and publish
COPY . .
RUN dotnet publish "src/FlowDesk.Api/FlowDesk.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Install curl for Docker health checks
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .

# Docker health check
HEALTHCHECK --interval=30s \
    --timeout=5s \
    --start-period=20s \
    --retries=3 \
    CMD curl -f http://localhost:8080/health/ready || exit 1

ENTRYPOINT ["dotnet", "FlowDesk.Api.dll"]

