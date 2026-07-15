# Render Environment Variables Checklist

Required:
- `ASPNETCORE_ENVIRONMENT=Production`
- `PORT=10000`
- `ConnectionStrings__DefaultConnection=`
- `Jwt__Key=`
- `Jwt__Issuer=`
- `Jwt__Audience=`
- `Cors__AllowedOrigins=`

Optional:
- `Cloudinary__CloudName=`
- `Cloudinary__ApiKey=`
- `Cloudinary__ApiSecret=`
- `Database__ApplyMigrationsOnStartup=true`
- `Database__SeedDataOnStartup=false`

Recommended values:
- `Cors__AllowedOrigins=https://YOUR-VERCEL-DOMAIN.vercel.app`
- `ConnectionStrings__DefaultConnection=Host=...;Port=5432;Database=postgres;Username=postgres;Password=...;SSL Mode=Require;Trust Server Certificate=true`