# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build Image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FinanceManagement.Api/FinanceManagement.Api.csproj", "FinanceManagement.Api/"]

# Generate and trust the development certificate
RUN dotnet dev-certs https && dotnet dev-certs https --trust

RUN dotnet restore "./FinanceManagement.Api/./FinanceManagement.Api.csproj"
COPY . .
WORKDIR "/src/FinanceManagement.Api"
RUN dotnet build "./FinanceManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish Image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinanceManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinanceManagement.Api.dll"]