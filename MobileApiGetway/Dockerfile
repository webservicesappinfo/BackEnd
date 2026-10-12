FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BackEnd/MobileApiGetway/MobileApiGetway.csproj", "BackEnd/MobileApiGetway/"]
COPY ["Protos/", "Protos/"]
RUN dotnet restore "BackEnd/MobileApiGetway/MobileApiGetway.csproj"
COPY ["BackEnd/", "BackEnd/"]
WORKDIR "/src/BackEnd/MobileApiGetway"
RUN dotnet build "MobileApiGetway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MobileApiGetway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MobileApiGetway.dll"]