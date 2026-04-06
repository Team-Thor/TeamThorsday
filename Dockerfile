# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY shared/ shared/
COPY backend/ backend/

WORKDIR /src/backend
RUN dotnet publish Backend.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 10000

CMD ASPNETCORE_URLS=http://+:${PORT:-10000} dotnet Backend.dll
