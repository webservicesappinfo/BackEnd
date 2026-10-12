FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BackEnd/UserService/UserService.csproj", "BackEnd/UserService/"]
COPY ["BackEnd/EventBus/EventBus.csproj", "BackEnd/EventBus/"]
COPY ["BackEnd/Globals/Globals.csproj", "BackEnd/Globals/"]
COPY ["Protos/", "Protos/"]
RUN dotnet restore "BackEnd/UserService/UserService.csproj"
COPY ["BackEnd/", "BackEnd/"]
WORKDIR "/src/BackEnd/UserService"
RUN dotnet build "UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "UserService.dll"]