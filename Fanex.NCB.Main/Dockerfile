FROM registry.nexdev.net:5050/docker/dotnet/core/sdk:5.0.102-ca-patch-buster-slim AS builder

WORKDIR /src

COPY ["Fanex.NCB.Main/Fanex.NCB.Main.csproj", "Fanex.NCB.Main/"]
COPY ["Fanex.NCB.NAPResolver/Fanex.NCB.NAPResolver.csproj", "Fanex.NCB.NAPResolver/"]
COPY ["Fanex.NCB.Core/Fanex.NCB.Core.csproj", "Fanex.NCB.Core/"]
COPY ["Fanex.NCB.Criteria/Fanex.NCB.Criteria.csproj", "Fanex.NCB.Criteria/"]
COPY ["Fanex.NCB.Shared/Fanex.NCB.Shared.csproj", "Fanex.NCB.Shared/"]
COPY ["Fanex.NCB.Services/Fanex.NCB.Services.csproj", "Fanex.NCB.Services/"]
COPY ["Fanex.NCB.Repository/Fanex.NCB.Repository.csproj", "Fanex.NCB.Repository/"]
COPY ["Fanex.NCB.Models/Fanex.NCB.Models.csproj", "Fanex.NCB.Models/"]
RUN dotnet restore "Fanex.NCB.Main/Fanex.NCB.Main.csproj"

COPY . .

WORKDIR "/src/Fanex.NCB.Main"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM registry.nexdev.net:5050/docker/dotnet/core/aspnet:5.0-buster-slim AS base

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fanex.NCB.Main.dll"]