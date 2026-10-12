FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BackEnd/CompanyService/CompanyService.csproj", "BackEnd/CompanyService/"]
COPY ["BackEnd/EventBus/EventBus.csproj", "BackEnd/EventBus/"]
COPY ["BackEnd/Globals/Globals.csproj", "BackEnd/Globals/"]
COPY ["Protos/", "Protos/"]
RUN dotnet restore "BackEnd/CompanyService/CompanyService.csproj"
COPY ["BackEnd/", "BackEnd/"]
WORKDIR "/src/BackEnd/CompanyService"
RUN dotnet build "CompanyService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CompanyService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CompanyService.dll"]