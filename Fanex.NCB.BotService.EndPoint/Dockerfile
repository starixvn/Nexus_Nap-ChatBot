ARG CI_REGISTRY_IMAGE

FROM $CI_REGISTRY_IMAGE:builder as deps

RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

ENV NUGET_PACKAGES=/src/packages

COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Fanex.NCB.BotService.EndPoint/Fanex.NCB.BotService.EndPoint.csproj", "Fanex.NCB.BotService.EndPoint/"]
COPY ["Fanex.NCB.BotService.Core/Fanex.NCB.BotService.Core.csproj", "Fanex.NCB.BotService.Core/"]
COPY ["Fanex.NCB.Shared/Fanex.NCB.Shared.csproj", "Fanex.NCB.Shared/"]
COPY ["Fanex.NCB.Resources/Fanex.NCB.Resources.csproj", "Fanex.NCB.Resources/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

RUN dotnet restore -v n -r linux-x64 "Fanex.NCB.BotService.EndPoint/Fanex.NCB.BotService.EndPoint.csproj"

COPY . .

WORKDIR "/src/Fanex.NCB.BotService.EndPoint"

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.NCB.BotService.EndPoint.dll"]