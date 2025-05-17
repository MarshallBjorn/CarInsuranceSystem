FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Core/Core.csproj", "src/Core/"]
RUN dotnet restore "src/Infrastructure/Infrastructure.csproj" --source https://api.nuget.org/v3/index.json
COPY . .
WORKDIR "/src/src/Infrastructure"
RUN dotnet build "Infrastructure.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Infrastructure.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Infrastructure.dll"]