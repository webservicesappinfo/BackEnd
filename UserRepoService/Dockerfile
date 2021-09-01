#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["UserRepoService/UserRepoService.csproj", "UserRepoService/"]
RUN dotnet restore "UserRepoService/UserRepoService.csproj"
COPY . .
WORKDIR "/src/UserRepoService"
RUN dotnet build "UserRepoService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserRepoService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS http://+:6001
EXPOSE 6001
ENTRYPOINT ["dotnet", "UserRepoService.dll"]