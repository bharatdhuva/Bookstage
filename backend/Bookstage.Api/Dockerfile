FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Bookstage.Api.csproj", "./"]
RUN dotnet restore "Bookstage.Api.csproj"

COPY . .
RUN dotnet publish "Bookstage.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-bookworm-slim AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://0.0.0.0:10000 \
    PORT=10000

EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Bookstage.Api.dll"]
