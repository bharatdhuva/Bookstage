FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY ["backend/Bookstage.Api/Bookstage.Api.csproj", "backend/Bookstage.Api/"]
RUN dotnet restore "backend/Bookstage.Api/Bookstage.Api.csproj"

# Copy the remaining backend files and publish
COPY backend/Bookstage.Api/ backend/Bookstage.Api/
WORKDIR "/src/backend/Bookstage.Api"
RUN dotnet publish "Bookstage.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://0.0.0.0:10000 \
    PORT=10000

EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Bookstage.Api.dll"]
