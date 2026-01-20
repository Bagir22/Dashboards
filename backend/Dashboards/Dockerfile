FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Dashboards.sln .
COPY Dashboards/Dashboards.csproj Dashboards/

RUN dotnet restore Dashboards/Dashboards.csproj

COPY . .

WORKDIR /src/Dashboards
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Dashboards.dll"]
