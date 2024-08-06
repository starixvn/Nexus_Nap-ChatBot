FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Fanex.NCB.AutoJobs/Fanex.NCB.AutoJobs.csproj", "Fanex.NCB.AutoJobs/"]
COPY ["Fanex.NCB.BackgroundJob.Core/Fanex.NCB.AutoJobs.Core.csproj", "Fanex.NCB.BackgroundJob.Core/"]
COPY ["Fanex.NCB.Resources/Fanex.NCB.Resources.csproj", "Fanex.NCB.Resources/"]
RUN dotnet restore "Fanex.NCB.AutoJobs/Fanex.NCB.AutoJobs.csproj"

COPY . .

WORKDIR "/src/Fanex.NCB.AutoJobs"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.NCB.AutoJobs.dll"]