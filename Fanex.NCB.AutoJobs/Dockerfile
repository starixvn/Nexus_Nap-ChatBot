FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Fanex.NCB.AutoJobs/Fanex.NCB.AutoJobs.csproj", "Fanex.NCB.AutoJobs/"]
COPY ["Fanex.NCB.Services/Fanex.NCB.Services.csproj", "Fanex.NCB.Services/"]
COPY ["Fanex.NCB.Repository/Fanex.NCB.Repository.csproj", "Fanex.NCB.Repository/"]
COPY ["Fanex.NCB.Shared/Fanex.NCB.Shared.csproj", "Fanex.NCB.Shared/"]
COPY ["Fanex.NCB.Models/Fanex.NCB.Models.csproj", "Fanex.NCB.Models/"]
COPY ["Fanex.NCB.Core/Fanex.NCB.Core.csproj", "Fanex.NCB.Core/"]
COPY ["Fanex.NCB.Criteria/Fanex.NCB.Criteria.csproj", "Fanex.NCB.Criteria/"]
COPY ["Fanex.NCB.Resources/Fanex.NCB.Resources.csproj", "Fanex.NCB.Resources/"]
RUN dotnet restore "Fanex.NCB.AutoJobs/Fanex.NCB.AutoJobs.csproj"

COPY . .

WORKDIR "/src/Fanex.NCB.AutoJobs"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-final AS base

WORKDIR /app

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.NCB.AutoJobs.dll"]