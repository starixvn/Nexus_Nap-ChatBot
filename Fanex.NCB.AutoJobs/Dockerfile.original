#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Fanex.NCB.AutoJobs/Fanex.NCB.AutoJobs.csproj", "Fanex.NCB.AutoJobs/"]

RUN dotnet restore "Fanex.NCB.AutoJobs/Fanex.NCB.AutoJobs.csproj"
COPY . .
WORKDIR "/src/Fanex.NCB.AutoJobs"
RUN dotnet build "Fanex.NCB.AutoJobs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fanex.NCB.AutoJobs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fanex.NCB.AutoJobs.dll"]