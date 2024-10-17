ARG CI_REGISTRY_IMAGE

FROM $CI_REGISTRY_IMAGE:builder as deps

RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

ENV NUGET_PACKAGES=/src/packages

COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR /src

COPY ["Fanex.NCB.Main/Fanex.NCB.Main.csproj", "Fanex.NCB.Main/"]
COPY ["Fanex.NCB.NAPResolver/Fanex.NCB.NAPResolver.csproj", "Fanex.NCB.NAPResolver/"]
COPY ["Fanex.NCB.Core/Fanex.NCB.Core.csproj", "Fanex.NCB.Core/"]
COPY ["Fanex.NCB.Criteria/Fanex.NCB.Criteria.csproj", "Fanex.NCB.Criteria/"]
COPY ["Fanex.NCB.Shared/Fanex.NCB.Shared.csproj", "Fanex.NCB.Shared/"]
COPY ["Fanex.NCB.Services/Fanex.NCB.Services.csproj", "Fanex.NCB.Services/"]
COPY ["Fanex.NCB.Repository/Fanex.NCB.Repository.csproj", "Fanex.NCB.Repository/"]
COPY ["Fanex.NCB.Models/Fanex.NCB.Models.csproj", "Fanex.NCB.Models/"]
COPY ["Fanex.NCB.Resources/Fanex.NCB.Resources.csproj", "Fanex.NCB.Resources/"]
COPY ["nuget.config", ""]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

RUN dotnet restore -v n -r linux-x64 "Fanex.NCB.Main/Fanex.NCB.Main.csproj"

COPY . .

WORKDIR "/src/Fanex.NCB.Main"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.NCB.Main.dll"]