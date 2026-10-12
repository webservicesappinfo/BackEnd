FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BackEnd/SkillService/SkillService.csproj", "BackEnd/SkillService/"]
COPY ["BackEnd/EventBus/EventBus.csproj", "BackEnd/EventBus/"]
COPY ["BackEnd/Globals/Globals.csproj", "BackEnd/Globals/"]
COPY ["Protos/", "Protos/"]
RUN dotnet restore "BackEnd/SkillService/SkillService.csproj"
COPY ["BackEnd/", "BackEnd/"]
WORKDIR "/src/BackEnd/SkillService"
RUN dotnet build "SkillService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SkillService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SkillService.dll"]