FROM registry.nexdev.net:5050/docker/dotnet/core/sdk:5.0.102-ca-patch-buster-slim AS builder

WORKDIR /src

COPY ["Fanex.NCB.Core", "Fanex.NCB.Core"]
COPY ["Fanex.NCB.Criteria", "Fanex.NCB.Criteria"]
COPY ["Fanex.NCB.Models", "Fanex.NCB.Models"]
COPY ["Fanex.NCB.Shared/Fanex.NCB.Shared", "Fanex.NCB.Shared"]
COPY ["Fanex.NCB.NAPResolver", "Fanex.NCB.NAPResolver"]
COPY ["Fanex.NCB.Services", "Fanex.NCB.Services"]
COPY ["Fanex.NCB.Repository", "Fanex.NCB.Repository"]
COPY ["Fanex.NCB.Main.csproj", "./"]
COPY ["nuget.config", ""]

WORKDIR "/Fanex.NCB.Main"
RUN dotnet restore -v n

COPY ./Fanex.NCB.Main .

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM registry.nexdev.net:5050/docker/dotnet/core/aspnet:5.0-buster-slim AS base

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.NCB.Main.dll"]
