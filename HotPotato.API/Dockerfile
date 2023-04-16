FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Ping.API/Ping.API.csproj", "Ping.API/"]
RUN dotnet restore "Ping.API/Ping.API.csproj"
COPY . .
WORKDIR "/src/Ping.API"
RUN dotnet build "Ping.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ping.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ping.API.dll"]
