# 🎬 Bookstage - Premium Movie & Live Event Booking Platform

Bookstage is a premium, modern ticket booking application for movies and live events. It is built with a high-performance **ASP.NET Core Web API (.NET 10.0)** backend and a state-of-the-art **React + Vite** frontend. The platform provides a rich user experience with dark-themed aesthetics, responsive design, interactive seat selection with 5-minute locking mechanisms, discount validation, dynamic search, PDF ticket generation, and comprehensive administrator dashboards.

---

## 📂 Project Structure

Bookstage consists of a decoupled backend API and a React SPA frontend:

```text
├── backend/                             # .NET 10.0 Web API Solution
│   ├── Bookstage.Api/                   # Core Web API Project
│   │   ├── Controllers/                 # REST Controller Endpoints
│   │   ├── Domain/                      # Domain Entities & Models
│   │   │   └── Entities/                # Database entities (User, Movie, Event, etc.)
│   │   ├── DTOs/                        # Request/Response Data Transfer Objects
│   │   ├── Infrastructure/              # EF Core DB Context & Initial Seeder
│   │   │   ├── Data/
│   │   │   │   ├── BookstageDbContext.cs# EF Core DbContext with model configurations
│   │   │   │   └── DataSeeder.cs        # Seeds test data for movies, events, and bookings
│   │   │   └── HealthChecks/            # Custom health checks (PostgreSQL connectivity)
│   │   ├── Migrations/                  # EF Core Database Migrations
│   │   ├── Services/                    # Token Services & Security logic
│   │   ├── Program.cs                   # API Startup & Configuration
│   │   ├── Dockerfile                   # Deployment Dockerfile for backend service
│   │   ├── RENDER_DEPLOYMENT.md         # Detailed instructions for Render cloud setup
│   │   └── RENDER_ENVIRONMENT_VARIABLES_CHECKLIST.md
│   └── Bookstage.sln                    # Visual Studio Solution File
│
├── frontend/                            # React + Vite Client Application
│   ├── src/
│   │   ├── components/                  # Shared Layout and UI components
│   │   │   ├── layout/                  # Navbar, Footer
│   │   │   └── ui/                      # Loading screen, custom UI components
│   │   ├── pages/                       # Application Pages & View logic
│   │   │   ├── auth/                    # Login, Register
│   │   │   ├── BookingConfirm.jsx       # Invoice generation, PDF ticket download & QR Code
│   │   │   ├── Checkout.jsx             # Payment page with discount codes and tax calculations
│   │   │   ├── EventDetail.jsx          # Event summary, show times & selection
│   │   │   ├── Events.jsx               # Categorized live events list (Concerts, Sports, etc.)
│   │   │   ├── Home.jsx                 # Dashboard with now-playing movies & upcoming events
│   │   │   ├── MovieDetail.jsx          # Movie details, cast, reviews & showtimes list
│   │   │   ├── Movies.jsx               # Movies catalog with category filtering
│   │   │   ├── MyBookingsImpl.jsx       # Booking list and cancellation triggers
│   │   │   ├── Profile.jsx              # Profile details editor
│   │   │   └── Search.jsx               # Dynamic global search
│   │   ├── services/                    # Axios API client integrations
│   │   │   └── api.js                   # REST API requests for all modules
│   │   ├── store/                       # Zustand State Stores
│   │   │   └── index.js                 # Auth, Theme, Booking & Search states
│   │   ├── utils/
│   │   │   └── supabase.js              # Supabase Client setup for media preview/db
│   │   ├── App.jsx                      # Router & Route guards definition
│   │   ├── index.css                    # Global CSS styling & Tailwind config integration
│   │   └── main.jsx                     # Client entry point
│   ├── tailwind.config.js               # Tailwind CSS theme customization
│   └── vite.config.js                   # Vite configuration (Proxy rules)
│
├── Dockerfile                           # Root-level multi-stage Dockerfile
└── .dockerignore                        # Docker exclude configuration
```

---

## 🛠️ Tech Stack & Dependencies

### Backend (API Service)
* **Framework:** ASP.NET Core (.NET 10.0 Web API)
* **Database & ORM:** PostgreSQL with Entity Framework Core (`Npgsql.EntityFrameworkCore.PostgreSQL`)
* **Security & Tokens:** JWT Bearer authentication (`Microsoft.AspNetCore.Authentication.JwtBearer`)
* **Password Hashing:** ASP.NET Core Identity (`Microsoft.AspNetCore.Identity`)
* **Health Checks:** Core Diagnostics (`Microsoft.Extensions.Diagnostics.HealthChecks`)
* **API Documentation:** Microsoft OpenApi support (`Microsoft.AspNetCore.OpenApi`)

### Frontend (SPA Client)
* **Build System & Tooling:** Vite, ESLint
* **Libraries:** React 18, React Router DOM (v6)
* **Styling & UI:** Tailwind CSS, Headless UI, Radix UI Icons & Elements, Lucide Icons, Framer Motion (premium animations)
* **State Management:** Zustand (Persisted Auth, Theme, Booking & Search stores)
* **HTTP Client:** Axios (Interceptors for Bearer JWT injection and 401 token refresh/eviction)
* **Visuals & Charts:** Recharts (For analytics on the Admin Dashboard)
* **Utility Tools:** date-fns (Date Formatting), qrcode (QR ticket code generation), html2canvas & jsPDF (Render & download receipts/PDF tickets)

---

## 🗄️ Database Schema & Data Tally

The schema maps 10 Entity tables managed via Entity Framework Core:

| Entity Table | Purpose / Description | Primary Key | Key Fields & Configuration |
| :--- | :--- | :--- | :--- |
| **Users** | Core accounts with profiles | `Guid` | `Email` (Unique, Max 256), `PasswordHash`, `FullName` (Max 200), `Phone`, `City` (Max 100), `DateOfBirth`, `Role` (Admin/User) |
| **Movies** | Movie catalog | `Guid` | `Title` (Max 500), `Genre` (Max 200), `Language` (Max 50), `Rating`, `Duration`, `ReleaseDate`, `PosterUrl`, `YoutubeTrailerId` |
| **ShowTimes** | Movie screening schedules | `Guid` | `MovieId` (FK), `VenueName` (Max 300), `VenueCity` (Max 100), `ShowDate`, `ShowTimeOfDay`, `Price`, `TotalSeats`, `AvailableSeats` |
| **Events** | Live events (Concerts, Sports, etc.) | `Guid` | `Title` (Max 500), `Category` (Max 100), `VenueName`, `VenueCity`, `EventDate`, `EventTime`, `Price`, `Rating`, `YoutubeTrailerId` |
| **Seats** | Individual seat state per showtime | `Guid` | `ShowTimeId` (FK), `SeatNumber` (Max 50), `Row` (Max 10), `Category` (VIP/Premium/Standard), `Price`, `Status` (Available/Locked/Booked) |
| **SeatLocks**| Temporary 5-minute locks | `Guid` | `ShowTimeId`/`EventId` (FK), `SeatId`, `LockedByUserId` (FK), `LockedAt`, `ExpiresAt`, `IsConfirmed` (Unique constraints to prevent double locks) |
| **Bookings** | Confirmed bookings for movies/events| `Guid` | `UserId` (FK), `BookingType` (Movie/Event), `EventOrMovieTitle`, `SeatsBooked` (JSON/Comma-delimited), `TotalPrice`, `Status`, `PaymentId` |
| **Payments** | Log of payment confirmations | `Guid` | `BookingId` (FK), `UserId` (FK), `Amount`, `PaymentMethod` (Max 50), `TransactionId` (Unique, Max 200), `Status` |
| **Offers**   | Discount coupon lookup | `Guid` | `Code` (Unique, Max 50), `Type` (Percentage/Flat), `DiscountValue`, `ValidFrom`, `ValidTo` |
| **Reviews**  | Movie & Event ratings | `Guid` | `UserId` (FK), `MovieId`/`EventId` (FK), `Rating` (1-5 stars), `Title`, `Comment`, `CreatedAt` |

### 🌱 Seed Data Stats
When running in `Development` or with the configuration `Database:SeedDataOnStartup=true`, the system seeds:
* **Users:** 3 initial profiles (`john@example.com` / User, `jane@example.com` / User, `admin@bookstage.com` / Admin).
* **Movies:** 4 featured films (*Pushpa 2, Dune 2, Mohan Lal, Kalki*).
* **ShowTimes:** 5 distinct screenings distributed across venues.
* **Seats:** 150 configurable seats per showtime with custom VIP/Premium/Standard pricing.
* **Events:** 4 live events categorized under Concerts, Sports, Comedy, and Theatre.
* **Bookings & Payments:** 3 sample completed orders to visualize client stats instantly.
* **Discount Offers:** 3 promotional coupon codes.

---

## 🔌 API Endpoint Inventory

All endpoints are hosted under `/api` and secured with JWT where indicated:

### 🔑 Authentication & Profile (`/api/auth` & `/api/users`)
* `POST /api/auth/register` — Register a new account
* `POST /api/auth/login` — Login & acquire JWT token
* `GET /api/auth/me` — Retrieve active user details from token *(Authorized)*
* `GET /api/users/me` — Read detailed profile *(Authorized)*
* `PUT /api/users/me` — Update profile details *(Authorized)*

### 🎬 Movies & ShowTimes (`/api/movies` & `/api/showtimes`)
* `GET /api/movies` — Query all movies (supports filtering by `nowShowing=true|false`)
* `GET /api/movies/{id}` — Get single movie details
* `GET /api/movies/{id}/showtimes` — Get all showtimes for a movie by city

### 🎭 Live Events (`/api/events`)
* `GET /api/events` — Retrieve event list (supports category and city filters)
* `GET /api/events/{id}` — Fetch specific event details
* `POST /api/events` — Create new live event *(Admin Authorized)*
* `PUT /api/events/{id}` — Edit event details *(Admin Authorized)*
* `DELETE /api/events/{id}` — Delete live event *(Admin Authorized)*

### 💺 Seat Management & Locks (`/api/seats`)
* `GET /api/seats/{showtimeId}` — Get availability layout and active locks/bookings
* `POST /api/seats/{showtimeId}/lock` — Acquire 5-minute temporary seat locks *(Authorized)*
* `POST /api/seats/{showtimeId}/unlock` — Release held seat locks *(Authorized)*
* `POST /api/seats/{showtimeId}/confirm` — Update status of locked seats to confirmed *(Authorized)*

### 🎟️ Bookings & Offers (`/api/bookings` & `/api/offers`)
* `POST /api/bookings` — Create booking invoice *(Authorized)*
* `GET /api/bookings/my` — Fetch bookings for the logged-in user *(Authorized)*
* `GET /api/bookings/{id}` — Retrieve details of specific booking *(Authorized)*
* `PUT /api/bookings/{id}/cancel` — Cancel booking and issue 80% refund *(Authorized)*
* `POST /api/offers/validate` — Validate promo coupon and apply discount *(Authorized)*

---

## 🚀 Local Setup Guide

### 1. Database Configuration
1. Host a PostgreSQL database instance on [Supabase](https://supabase.com/) or run PostgreSQL locally.
2. Retrieve your connection string.

### 2. Backend Startup
1. Navigate to the api directory:
   ```bash
   cd backend/Bookstage.Api
   ```
2. Update `appsettings.Development.json` or define environment variables:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "YOUR_POSTGRES_CONNECTION_STRING"
     },
     "Jwt": {
       "Key": "A_SECURE_256BIT_RANDOM_SECRET_KEY",
       "Issuer": "Bookstage.Api",
       "Audience": "Bookstage.Client"
     },
     "Database": {
       "ApplyMigrationsOnStartup": true,
       "SeedDataOnStartup": true
     }
   }
   ```
3. Run migrations and launch:
   ```bash
   dotnet run
   ```
   *The api will start listening on `http://localhost:5054`.*

### 3. Frontend Startup
1. Navigate to the client directory:
   ```bash
   cd frontend
   ```
2. Install the necessary node modules:
   ```bash
   npm install
   ```
3. Create a `.env` file in the `frontend/` directory:
   ```text
   VITE_API_URL=http://localhost:5054/api
   VITE_SUPABASE_URL=https://your-supabase-url.supabase.co
   VITE_SUPABASE_PUBLISHABLE_KEY=your-supabase-pub-key
   ```
4. Start Vite development server:
   ```bash
   npm run dev
   ```
   *Open `http://localhost:5173` to test locally.*

---

## 🌐 Production Deployment Guide

### Backend Cloud Deployment (Render)
The backend is optimized for deployment as a **Docker Web Service** on Render.

1. Connect your repository to Render.
2. Select **Web Service** and choose **Docker** runtime.
3. Configure target build settings:
   * **Root Directory:** *(Keep blank)*
   * **Dockerfile Path:** `Dockerfile` (or `backend/Bookstage.Api/Dockerfile` depending on the build origin)
4. Add the following **Environment Variables**:
   * `ASPNETCORE_ENVIRONMENT` = `Production`
   * `PORT` = `10000`
   * `ConnectionStrings__DefaultConnection` = `YOUR_SUPABASE_PRODUCTION_DB_STRING`
   * `Jwt__Key` = `SECURE_JWT_SIGNING_KEY`
   * `Jwt__Issuer` = `Bookstage.Api`
   * `Jwt__Audience` = `Bookstage.Client`
   * `Cors__AllowedOrigins` = `https://your-app-frontend.vercel.app` (Separate multiple with commas, without trailing slash)
   * `Database__ApplyMigrationsOnStartup` = `true` (Triggers DB updates automatically)
   * `Database__SeedDataOnStartup` = `false`

### Frontend Cloud Deployment (Vercel)
1. Import the project repository into Vercel.
2. Set **Root Directory** to `frontend`.
3. Select **Vite** preset.
4. Set the **Environment Variable**:
   * `VITE_API_URL` = `https://your-render-backend-service.onrender.com/api`
5. Click **Deploy**.
