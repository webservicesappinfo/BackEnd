#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OfferService/OfferService.csproj", "OfferService/"]
RUN dotnet restore "OfferService/OfferService.csproj"
COPY . .
WORKDIR "/src/OfferService"
RUN dotnet build "OfferService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OfferService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS http://+:6004
EXPOSE 6004
ENTRYPOINT ["dotnet", "OfferService.dll"]