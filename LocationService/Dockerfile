FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BackEnd/LocationService/LocationService.csproj", "BackEnd/LocationService/"]
COPY ["BackEnd/EventBus/EventBus.csproj", "BackEnd/EventBus/"]
COPY ["Protos/", "Protos/"]
RUN dotnet restore "BackEnd/LocationService/LocationService.csproj"
COPY ["BackEnd/", "BackEnd/"]
WORKDIR "/src/BackEnd/LocationService"
RUN dotnet build "LocationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LocationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "LocationService.dll"]