# 🎬 Bookstage - Movie & Live Event Booking Platform

Bookstage is a premium, modern movie and live event booking application built with a robust C# .NET Web API backend and a responsive, high-performance React + Vite frontend.

---

## 📂 Project Structure

```text
├── backend/                  # .NET 10.0 Web API
│   ├── Bookstage.Api/        # Core API project (Controllers, DTOs, Domain, Infrastructure)
│   └── Bookstage.sln         # Visual Studio Solution file
│
├── frontend/                 # React + Vite Frontend
│   ├── src/                  # React components, pages, stores, and utilities
│   ├── tailwind.config.js    # Tailwind CSS Configuration
│   └── vite.config.js        # Vite build and proxy settings
│
├── Dockerfile                # Root-level Dockerfile for Render/Docker deployments
└── .dockerignore             # Excludes unnecessary build artifacts from Docker context
```

---

## 🛠️ Tech Stack

### Backend
* **Runtime:** .NET 10.0
* **Framework:** ASP.NET Core Web API
* **Database & ORM:** PostgreSQL with Entity Framework Core
* **Authentication:** JWT Bearer Token Authentication
* **Third-Party Integrations:** Cloudinary (Media upload), Supabase (Database hosting)

### Frontend
* **Build Tool:** Vite
* **Library:** React (JSX)
* **Styling:** Tailwind CSS (Modern, premium dark-theme dashboard)
* **State Management:** React Stores & Context API
* **HTTP Client:** Axios

---

## 🚀 Local Setup Guide

### 1. Database Setup (Supabase)
1. Create a free PostgreSQL database on [Supabase](https://supabase.com/).
2. Copy your **Session Pooler** connection string from your Supabase Project Settings.

### 2. Backend Setup
1. Navigate to the backend directory:
   ```bash
   cd backend/Bookstage.Api
   ```
2. Configure your environment variables or update `appsettings.Development.json` (do not commit secrets):
   * `ConnectionStrings:DefaultConnection` — Your Supabase connection string.
   * `Jwt:Key` — A secure 256-bit key.
   * `Jwt:Issuer` / `Jwt:Audience` — e.g. `Bookstage.Api` / `Bookstage.Client`.
3. Run migrations and start the API:
   ```bash
   dotnet run
   ```
   *The API will start locally on `http://localhost:5054`.*

### 3. Frontend Setup
1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Create a `.env` file based on your credentials:
   ```text
   VITE_API_URL=http://localhost:5054/api
   VITE_SUPABASE_URL=https://your-project.supabase.co
   VITE_SUPABASE_PUBLISHABLE_KEY=your-supabase-key
   ```
4. Start the frontend development server:
   ```bash
   npm run dev
   ```
   *Open `http://localhost:5173` in your browser.*

---

## 🌐 Production Deployment

### Backend (Render)
The backend is configured to be deployed as a **Docker Web Service** on Render.

1. **Create a Web Service** on Render and link your GitHub repository.
2. Configure the following settings:
   * **Environment**: `Docker`
   * **Root Directory**: *(Leave blank)*
   * **Dockerfile Path**: `Dockerfile`
3. Add the following **Environment Variables** under the Environment tab:
   * `ConnectionStrings__DefaultConnection` — Supabase session pooler connection string (IPv4 compatible).
   * `Jwt__Key` — A secure random string.
   * `Jwt__Issuer` — `Bookstage.Api`
   * `Jwt__Audience` — `Bookstage.Client`
   * `Cors__AllowedOrigins` — `https://your-frontend-domain.vercel.app` (without a trailing slash `/`).
   * `Database__SeedDataOnStartup` — Set to `true` on the first run to populate the database, then switch to `false`.

### Frontend (Vercel)
1. Import your repository into **Vercel**.
2. Set the **Framework Preset** to `Vite`.
3. Set the **Root Directory** to `frontend`.
4. Add the following **Environment Variable**:
   * `VITE_API_URL` — `https://your-backend-app.onrender.com/api`
5. Deploy the project.
