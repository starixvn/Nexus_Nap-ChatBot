ARG CI_REGISTRY_IMAGE

FROM $CI_REGISTRY_IMAGE:builder AS deps

RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

ENV NUGET_PACKAGES=/src/packages

COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Fanex.NCB.AutoJobs/Fanex.NCB.AutoJobs.csproj", "Fanex.NCB.AutoJobs/"]
COPY ["Fanex.NCB.BackgroundJob.Core/Fanex.NCB.AutoJobs.Core.csproj", "Fanex.NCB.BackgroundJob.Core/"]
COPY ["Fanex.NCB.Resources/Fanex.NCB.Resources.csproj", "Fanex.NCB.Resources/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

RUN dotnet restore -v n -r linux-x64 "Fanex.NCB.AutoJobs/Fanex.NCB.AutoJobs.csproj"

COPY . .

WORKDIR "/src/Fanex.NCB.AutoJobs"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-alpine-25h1 AS base

WORKDIR /app

COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.NCB.AutoJobs.dll"]