FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем ВСЕ файлы проектов
COPY *.sln .
COPY Dashboards/*.csproj Dashboards/
COPY Infrastructure/*.csproj Infrastructure/
COPY Application/*.csproj Application/
COPY Domain/*.csproj Domain/

# Восстанавливаем ВСЕ проекты
RUN dotnet restore Dashboards/Dashboards.csproj

# Копируем все исходники
COPY . .

# Публикуем Dashboards (включая зависимости)
WORKDIR /src/Dashboards
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Dashboards.dll"]