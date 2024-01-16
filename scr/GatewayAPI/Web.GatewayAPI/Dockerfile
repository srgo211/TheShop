#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GatewayAPI/Web.GatewayAPI/Web.GatewayAPI.csproj", "GatewayAPI/Web.GatewayAPI/"]
RUN dotnet restore "./GatewayAPI/Web.GatewayAPI/./Web.GatewayAPI.csproj"
COPY . .
WORKDIR "/src/GatewayAPI/Web.GatewayAPI"
RUN dotnet build "./Web.GatewayAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.GatewayAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.GatewayAPI.dll"]