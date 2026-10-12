FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BackEnd/UserRepoService/UserRepoService.csproj", "BackEnd/UserRepoService/"]
COPY ["BackEnd/EventBus/EventBus.csproj", "BackEnd/EventBus/"]
COPY ["Protos/", "Protos/"]
RUN dotnet restore "BackEnd/UserRepoService/UserRepoService.csproj"
COPY ["BackEnd/", "BackEnd/"]
WORKDIR "/src/BackEnd/UserRepoService"
RUN dotnet build "UserRepoService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserRepoService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "UserRepoService.dll"]