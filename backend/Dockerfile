FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY ["Bookstage.Api/Bookstage.Api.csproj", "Bookstage.Api/"]
RUN dotnet restore "Bookstage.Api/Bookstage.Api.csproj"

# Copy the remaining API files and publish
COPY Bookstage.Api/ Bookstage.Api/
WORKDIR "/src/Bookstage.Api"
RUN dotnet publish "Bookstage.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-bookworm-slim AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://0.0.0.0:10000 \
    PORT=10000

EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Bookstage.Api.dll"]
