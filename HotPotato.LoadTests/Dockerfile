FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HotPotato.LoadTests/HotPotato.LoadTests.csproj", "HotPotato.LoadTests/"]
RUN dotnet restore "HotPotato.LoadTests/HotPotato.LoadTests.csproj"
COPY . .
WORKDIR "/src/HotPotato.LoadTests"
RUN dotnet build "HotPotato.LoadTests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HotPotato.LoadTests.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotPotato.LoadTests.dll"]
