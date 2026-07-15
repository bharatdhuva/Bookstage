# Render Deployment Guide

## Root Directory
`backend/Bookstage.Api`

## Dockerfile Path
`backend/Bookstage.Api/Dockerfile`

## Build Configuration
- Docker Web Service on Render
- Build from the repository root using the backend subdirectory as the root directory
- Multi-stage Docker build handles restore, build, and publish
- Runtime image: `mcr.microsoft.com/dotnet/aspnet:10.0-bookworm-slim`
- The application listens on `0.0.0.0:10000` inside the container
- Render should route traffic to port `10000`

## Expected Port
`10000`

## Health Check Path
`/health`

## Environment Variables
Set these in Render:
- `ASPNETCORE_ENVIRONMENT=Production`
- `PORT=10000`
- `ConnectionStrings__DefaultConnection=Host=db.fdywyftilmopumgkiqwo.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=<SUPABASE_DB_PASSWORD>;SSL Mode=Require;Trust Server Certificate=true`
- `Jwt__Key=<long-random-secret>`
- `Jwt__Issuer=<issuer-value>`
- `Jwt__Audience=<audience-value>`
- `Cors__AllowedOrigins=https://YOUR-VERCEL-DOMAIN.vercel.app`
- `Cloudinary__CloudName=<if used>`
- `Cloudinary__ApiKey=<if used>`
- `Cloudinary__ApiSecret=<if used>`
- `Database__ApplyMigrationsOnStartup=true` for first deploys, or `false` if you manage migrations separately
- `Database__SeedDataOnStartup=false` in production

## Render Notes
- The app listens on `0.0.0.0:10000` in the container.
- Swagger/OpenAPI is enabled only in Development.
- Health checks are available at `/health`.
- No SQL Server dependency is required.
- Database migrations are controlled by `Database__ApplyMigrationsOnStartup`.
- Static files are served from the container image; do not rely on local disk persistence for uploads.

## CORS Notes
- Use `Cors__AllowedOrigins` as a comma-separated list if you want multiple origins.
- Example: `Cors__AllowedOrigins=https://app1.vercel.app,https://app2.vercel.app`

## Local Docker Test Commands
```bash
docker build -t bookstage-api .
docker run -p 10000:10000 bookstage-api
```

## Final Checklist
- [ ] Push the backend to GitHub
- [ ] Create a Render Docker Web Service
- [ ] Set root directory to `backend/Bookstage.Api`
- [ ] Add environment variables
- [ ] Confirm `/health` returns healthy
- [ ] Confirm the frontend origin is allowed by CORS
- [ ] Confirm Supabase connection string is valid
