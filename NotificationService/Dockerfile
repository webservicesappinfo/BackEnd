FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BackEnd/NotificationService/NotificationService.csproj", "BackEnd/NotificationService/"]
COPY ["BackEnd/EventBus/EventBus.csproj", "BackEnd/EventBus/"]
COPY ["BackEnd/Globals/Globals.csproj", "BackEnd/Globals/"]
COPY ["Protos/", "Protos/"]
RUN dotnet restore "BackEnd/NotificationService/NotificationService.csproj"
COPY ["BackEnd/", "BackEnd/"]
WORKDIR "/src/BackEnd/NotificationService"
RUN dotnet build "NotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "NotificationService.dll"]