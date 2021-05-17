#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
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
RUN dotnet build "Fanex.NCB.Main.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fanex.NCB.Main.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fanex.NCB.Main.dll"]