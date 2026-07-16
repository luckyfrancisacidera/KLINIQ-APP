# KLINIQ

KLINIQ is a full-stack healthcare clinic discovery and appointment-booking application. It provides patients with searchable clinic and practitioner discovery, location-aware map results, AI-assisted symptom-to-specialty matching, live practitioner availability, appointment booking and clinic-queue tracking, secure account workflows, and an installable Progressive Web App experience.

The product uses original KLINIQ branding and a responsive, accessibility-conscious healthcare interface. The repository preserves the existing ASP.NET Core clean architecture and React feature structure while repairing authentication, authorization, API consistency, pagination, query performance, appointment integrity, PWA behavior, and production configuration.

> **Verification note**
>
> Source-level validation is included in `IMPLEMENTATION_REPORT.md`. A machine with the .NET 10 SDK and project dependencies must run the documented restore, migration, build, and test commands before deployment. Do not treat a source review as a substitute for a successful CI build.

## Main capabilities

### Public discovery

- Search active clinic records by clinic name or practitioner specialization
- Backend pagination with deterministic sorting
- Nearby-clinic search using browser geolocation after an explicit user action
- Radius filtering and distance sorting
- Synchronized clinic cards and Google Maps markers
- Mobile list/map switching with preserved search state
- Clinic and practitioner detail pages
- AI-assisted symptom search that suggests specialties and matching verified physicians
- Emergency-warning detection with clear in-person care guidance
- Map-independent list fallback when the Maps API is unavailable

### Patient experience

- Registration, login, logout, session restoration, forgot/reset password, and password change
- Patient dashboard and profile management
- Live availability loaded from the backend
- Appointment booking with final server-side slot validation
- Appointment rescheduling and cancellation without deleting history
- Paginated appointment history with status filters
- Patient-visible progress from confirmation through queue, active checkup, and completion
- Offline safeguards that prevent appointment mutations without a network connection

### Practitioner experience

- Practitioner onboarding request with validated document uploads
- Invitation-based password setup after administrator approval
- Profile management
- Recurring schedule creation and removal
- Assigned appointment list with backend pagination and filtering
- Full workflow: confirm → add to queue → start checkup → finish checkup
- Queue ordering by check-in timestamp
- Backend-enforced transition and ownership rules

### Administrator experience

- Administrator dashboard
- Paginated patient and practitioner management
- Server-side patient search
- Paginated practitioner account requests with status and search filters
- Account-request review, approval, rejection, and email delivery

### Platform foundations

- ASP.NET Core Identity
- Short-lived JWT access tokens in HttpOnly cookies
- Rotating, hashed refresh tokens with revocation on logout
- Role and resource-ownership enforcement in the API
- FluentValidation and MediatR pipeline validation
- RFC-style `ProblemDetails` responses
- Rate limiting for authentication endpoints
- Health check endpoint
- Response compression
- Serilog request logging without request bodies or sensitive healthcare content
- EF Core SQL Server migrations and indexes
- OpenAPI plus Scalar API reference in development
- Installable PWA manifest, service worker, offline page, update notification, and network status feedback

## Technology stack

| Area | Technology |
|---|---|
| API | ASP.NET Core Web API, .NET 10 |
| Architecture | Domain, Application, Infrastructure, API projects |
| Data access | Entity Framework Core 10, SQL Server |
| Authentication | ASP.NET Core Identity, JWT, rotating refresh tokens |
| Application patterns | MediatR, FluentValidation, DTO projections |
| API documentation | OpenAPI and Scalar |
| Frontend | React 19, TypeScript, Vite |
| Data fetching | TanStack Query, centralized Axios client |
| Styling | Tailwind CSS 4 and reusable UI primitives |
| Maps | Google Maps loaded dynamically behind a map component (not Leaflet) |
| PWA | Web app manifest and production service worker |
| Backend tests | xUnit |

## Architecture

```text
KLINIQ-APP-master/
├── client/
│   ├── public/
│   │   ├── icons/
│   │   ├── manifest.webmanifest
│   │   ├── offline.html
│   │   └── sw.js
│   └── src/
│       ├── app/                 # bootstrap, providers, layout, routes
│       ├── features/            # auth, clinics, symptom search, patients, practitioners, admin
│       └── shared/              # API client, components, hooks, types, utilities
├── server/Kliniq/
│   ├── src/
│   │   ├── Kliniq.Domain/       # entities, value objects, domain rules
│   │   ├── Kliniq.Application/  # use cases, DTOs, validation, interfaces
│   │   ├── Kliniq.Infrastructure/# EF Core, Identity, repositories, email, files
│   │   └── Kliniq.Api/          # controllers, auth, middleware, OpenAPI
│   └── tests/
│       ├── Kliniq.Domain.Tests/
│       ├── Kliniq.Application.Tests/
│       └── Kliniq.Tests.Integration/
├── tools/Kliniq.SymptomCatalogBuilder/
│   ├── Sources/                # one parser per local public-dataset export
│   ├── data/                   # reviewed map/baseline; raw exports are Git-ignored
│   └── catalog-diff.md         # human-review summary
├── CHANGELOG.md
└── IMPLEMENTATION_REPORT.md
```

### Backend request flow

```text
HTTP request
  -> ASP.NET authentication/authorization
  -> controller
  -> MediatR command/query
  -> FluentValidation pipeline
  -> application handler
  -> repository/service abstraction
  -> EF Core / Identity / external service
  -> DTO or ProblemDetails response
```

Complex business rules remain outside controllers. Read endpoints use no-tracking queries, database filtering, deterministic ordering, and pagination before materialization where the current model supports them.

### Frontend request flow

```text
Route-level lazy page
  -> feature component
  -> TanStack Query hook
  -> centralized typed API module
  -> configured Axios client
  -> API
```

The Axios client owns the base URL, credentials, timeout, refresh-token retry queue, and session-expiration event. Feature components do not create independent Axios instances.

## Database overview

The current domain includes:

- Identity users and roles
- Patients
- Practitioners
- Clinics
- Practitioner schedules and schedule breaks
- Appointments
- Practitioner account requests and uploaded verification documents
- Refresh-token metadata stored on the Identity user

### Appointment integrity and queue workflow

KLINIQ interprets schedule dates and times in the configured platform timezone and stores appointment instants as UTC. The visit lifecycle is:

```text
Pending → Confirmed → In Queue → Checkup in Progress → Completed
                   ↘ Cancelled (before an active checkup)
```

Queue entry is allowed only on the scheduled local appointment date. The application records queue, consultation-start, and completion timestamps. Once a checkup starts, it cannot be cancelled or rescheduled through normal patient/practitioner actions.

It validates appointment creation and rescheduling against:

- Future date/time
- Positive duration
- Existing practitioner schedule
- Schedule breaks
- Existing non-cancelled appointment overlap
- Patient/practitioner ownership and role permissions
- Allowed status transitions

The migration `20260710000100_PreventAppointmentDoubleBooking.cs` adds a filtered unique index for non-cancelled appointments sharing a practitioner and start time. `20260711000100_AddAppointmentQueueWorkflow.cs` adds queue, consultation-start, and completion timestamps. The application also checks interval overlap and returns a conflict result when a slot is no longer available.

### Pagination format

Growing list endpoints use the shared response shape:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalItems": 0,
  "totalPages": 0,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

Page sizes are normalized and capped at 100. Filtering and sorting occur before `Skip`/`Take`.

## Authentication flow

1. A patient registers or a practitioner accepts an approved invitation.
2. ASP.NET Core Identity hashes and stores the password.
3. Login returns safe user metadata and writes:
   - a short-lived access token to an HttpOnly cookie;
   - a cryptographically random refresh token to a narrower HttpOnly cookie.
4. Only the refresh-token hash is persisted.
5. The frontend restores the session through `GET /api/auth/me`.
6. On `401`, the centralized client makes one queued refresh request and retries eligible requests.
7. Refresh rotates the token and revokes the previous value.
8. Logout revokes the stored refresh token, clears cookies, and asks the service worker to clear runtime caches.

Production should serve the frontend and API from the same site (for example `app.example.com` and `api.example.com`) so the configured cookie policies work predictably.

## Clinic discovery and map flow

1. Search, specialty, page, radius, coordinates, and sort state are stored in the URL.
2. Text input is debounced.
3. TanStack Query cancels stale requests through `AbortSignal`.
4. The API applies database filters, stable ordering, and pagination.
5. When coordinates are supplied, the API calculates approximate Haversine distance and supports radius/nearest behavior.
6. The map library is dynamically loaded only on the discovery page.
7. Selected card and marker IDs share state.
8. Results remain usable if geolocation is denied or the map fails.

The current clinic entity stores a clinic name and coordinates. Address, services, operating hours, active-state management, and clinic-administrator location editing require a future schema expansion; see **Known limitations**.


## AI-assisted symptom search

The `/symptom-search` experience is a privacy-first, explainable physician-matching tool:

1. The user describes symptoms in free text without adding identity details.
2. A backend analysis service normalizes the text, checks a reviewed emergency-warning catalog, and scores specialty signals.
3. The API returns up to three specialty suggestions with the exact matched signals.
4. Matching verified practitioners are loaded through server-side specialty filtering and pagination.
5. The response always states that the result is not a diagnosis or treatment plan.

The current implementation is an explainable local expert-system matcher rather than an external generative-AI call. This avoids sending symptom text to a third-party model and works without an AI API key. The `ISymptomAnalysisService` abstraction remains unchanged. Its reviewed English catalog is loaded from an embedded JSON resource, explicit negations are suppressed with a small NegEx-style detector, and misspellings use deterministic Levenshtein/Jaro-Winkler fallback matching only when the description contains no affirmed exact catalog signal. Fuzzy results are visibly marked as “Did you mean …?” rather than presented as exact evidence.

The catalog can be refreshed offline with `tools/Kliniq.SymptomCatalogBuilder`. Raw datasets and HPO are downloaded manually and are never needed by the running API:

```bash
dotnet run --project tools/Kliniq.SymptomCatalogBuilder
```

The builder refuses to run without a local dataset export and writes a review diff plus an unmapped-disease list. Review the generated JSON before committing it. Configure the fallback threshold with `SymptomMatching:FuzzyThreshold` (default `85`; allowed range `70`–`100`). Emergency and urgent phrases remain exact-only and negation-aware.

Safety and privacy controls:

- Symptom descriptions are not persisted.
- Request payloads are not serialized into application logs.
- The endpoint is rate limited.
- The PWA does not cache API requests or POST bodies.
- Emergency signals return immediate-care guidance instead of an appointment recommendation presented as sufficient care.
- Negated emergency, urgent, and specialty phrases are excluded from scoring.
- Fuzzy matching is not used for emergency or urgent classification.
- The feature recommends physician types; it never claims to diagnose a condition.

## PWA architecture

### Installability

- Original KLINIQ 192px, 512px, and maskable icons
- Standalone manifest
- Theme and background colors
- Service-worker registration only for production builds
- Install prompt handling on the Settings page
- Non-intrusive update prompt

### Caching policy

The service worker intentionally does **not** intercept:

- `/api` requests
- authentication requests
- appointment reads or mutations
- administrator requests
- non-GET requests
- cross-origin requests

Strategies:

- Precache the public shell, offline page, logo, icons, and local fonts
- Cache-first for same-origin hashed static assets
- Network-first navigation with an offline fallback
- Delete old named caches during activation
- Clear runtime caches on logout

KLINIQ does not queue appointment bookings while offline. Live availability and mutation controls are disabled or rejected until connectivity returns.

## Performance choices

### Frontend

- Route-level lazy loading
- Dynamic map loading
- Typed centralized requests
- Debounced search
- Abortable queries
- Query-key based caching and invalidation
- Previous page data retained during pagination
- Loading skeletons and localized errors instead of blank screens
- Local font assets
- No large charting framework

### Backend

- Database-side filtering and pagination
- `AsNoTracking()` for read-only query paths
- DTO/list shaping rather than exposing public EF entities
- Appointment and query indexes
- Asynchronous operations with cancellation tokens
- Response compression
- Authentication rate limiting
- Health endpoint for orchestration

## Accessibility and responsive behavior

The implemented UI uses semantic buttons and links, visible labels, keyboard-accessible controls, focus-visible styles from the component system, live regions for connection/session feedback, status text in addition to color, 44px-class interactive targets, mobile drawers/navigation, scroll-safe filter rows, and map-independent clinic results.

Manual WCAG 2.1 AA review with screen readers and automated browser tooling is still required before declaring formal conformance.

## Requirements

- .NET 10 SDK
- SQL Server reachable by the API
- Node.js 22 or another version supported by the installed Vite release
- pnpm through Corepack
- SMTP account for password reset and practitioner invitation email
- Google Maps JavaScript API browser key for map display

## Local setup

### 1. Configure the API

From `server/Kliniq`:

```bash
cp src/Kliniq.Api/appsettings.Development.json.example \
   src/Kliniq.Api/appsettings.Development.json
```

For secrets, prefer ASP.NET user secrets rather than committing them:

```bash
dotnet user-secrets init --project src/Kliniq.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<sql-server-connection>" --project src/Kliniq.Api
dotnet user-secrets set "JwtSettings:Key" "<at-least-32-random-bytes>" --project src/Kliniq.Api
dotnet user-secrets set "SmtpSettings:Host" "<smtp-host>" --project src/Kliniq.Api
dotnet user-secrets set "SmtpSettings:Port" "587" --project src/Kliniq.Api
dotnet user-secrets set "SmtpSettings:Username" "<smtp-user>" --project src/Kliniq.Api
dotnet user-secrets set "SmtpSettings:Password" "<smtp-password>" --project src/Kliniq.Api
dotnet user-secrets set "SmtpSettings:FromEmail" "<verified-sender>" --project src/Kliniq.Api
```

Optional development-only initial administrator:

```bash
dotnet user-secrets set "SeedSettings:AdminEmail" "admin@example.test" --project src/Kliniq.Api
dotnet user-secrets set "SeedSettings:AdminPassword" "<strong-temporary-password>" --project src/Kliniq.Api
```

No fake clinics, practitioners, patients, or appointments are seeded. Admin seeding is skipped unless both values are explicitly configured.

### 2. Restore and migrate the API

```bash
cd server/Kliniq
dotnet restore
dotnet tool restore  # only when a local tool manifest is added
dotnet ef database update \
  --project src/Kliniq.Infrastructure \
  --startup-project src/Kliniq.Api
```

When `dotnet-ef` is not installed:

```bash
dotnet tool install --global dotnet-ef --version 10.*
```

### 3. Run the API

```bash
dotnet run --project src/Kliniq.Api --launch-profile http
```

Default development API URL: `http://localhost:5178`  
Health check: `http://localhost:5178/health`  
Scalar API reference: `http://localhost:5178/scalar/v1`

### 4. Configure and run the frontend

```bash
cd client
cp .env.example .env.local
corepack enable
pnpm install --frozen-lockfile
pnpm dev
```

Default frontend URL: `http://localhost:5173`

`VITE_GOOGLE_MAPS_API_KEY` is a browser key. Restrict it in Google Cloud by allowed HTTP referrers and only enable required APIs. Never place a server-only geocoding secret in the frontend.

## Validation commands

### Backend

```bash
cd server/Kliniq
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

### Frontend

```bash
cd client
pnpm install --frozen-lockfile
pnpm lint
pnpm typecheck
pnpm build
pnpm preview
```

The repository currently has backend xUnit projects. A frontend test runner was not present in the original package manifest and was not added without being installable and verifiable in the execution environment.

## Environment variables

ASP.NET Core supports the following as environment variables by replacing `:` with `__`.

| Variable | Required in production | Purpose |
|---|---:|---|
| `ASPNETCORE_ENVIRONMENT` | Yes | Set to `Production` |
| `ASPNETCORE_URLS` | Yes | Kestrel binding behind the proxy |
| `ConnectionStrings__DefaultConnection` | Yes | SQL Server connection |
| `JwtSettings__Key` | Yes | High-entropy signing secret |
| `JwtSettings__Issuer` | Yes | JWT issuer |
| `JwtSettings__Audience` | Yes | JWT audience |
| `JwtSettings__ExpiryMinutes` | Recommended | Access-token lifetime |
| `FileStorage__BasePath` | Yes | Persistent non-public upload directory |
| `DataProtection__KeysPath` | Yes | Persistent shared Identity token keys |
| `App__BaseUrl` | Yes | Public frontend origin used in email links |
| `App__TimeZoneId` | Yes | Platform clinic timezone, default example `Asia/Manila` |
| `SymptomMatching__FuzzyThreshold` | No | Typo similarity threshold, default `85` (valid `70`–`100`) |
| `Cors__AllowedOrigins__0` | Yes | Exact frontend origin |
| `SmtpSettings__Host` | Yes | SMTP server |
| `SmtpSettings__Port` | Yes | SMTP port |
| `SmtpSettings__Username` | Yes | SMTP login |
| `SmtpSettings__Password` | Yes | SMTP secret |
| `SmtpSettings__FromEmail` | Yes | Verified sender |
| `SmtpSettings__FromName` | Recommended | Display name |
| `SeedSettings__AdminEmail` | No | Explicit first-run admin only |
| `SeedSettings__AdminPassword` | No | Strong temporary admin password |
| `VITE_API_BASE_URL` | Yes at build time | Public API base ending in `/api` |
| `VITE_GOOGLE_MAPS_API_KEY` | Optional | Restricted browser Maps key |

## Production build

### API

```bash
cd server/Kliniq
dotnet restore
dotnet test
dotnet publish src/Kliniq.Api -c Release -o ./publish/api
```

Apply migrations as an explicit release step using the same production configuration:

```bash
dotnet ef database update \
  --project src/Kliniq.Infrastructure \
  --startup-project src/Kliniq.Api \
  --configuration Release
```

Do not let every horizontally scaled API instance race to apply migrations at startup.

### Frontend

```bash
cd client
corepack enable
pnpm install --frozen-lockfile
pnpm lint
pnpm typecheck
pnpm build
```

Deploy `client/dist` as immutable static files. Configure the web server to return `index.html` for application routes while serving `/offline.html`, `/manifest.webmanifest`, `/sw.js`, icons, fonts, and hashed assets directly.

## Reverse proxy checklist

- Terminate HTTPS at the proxy or load balancer
- Forward `X-Forwarded-For` and `X-Forwarded-Proto`
- Proxy `/api/*` and `/health` to Kestrel
- Serve frontend static files with long cache headers only for content-hashed assets
- Serve `sw.js` with `Cache-Control: no-cache`
- Serve `manifest.webmanifest` with the correct manifest content type
- Keep API and frontend on the same registrable site for cookie behavior
- Set a strict production CORS origin rather than `*`
- Limit request body size at both the proxy and API
- Ensure the upload and Data Protection directories are mounted on persistent storage

## Operational security

- Store secrets in a secret manager, not source control or container images
- Rotate JWT and SMTP credentials through a controlled deployment
- Persist and back up Data Protection keys; all instances must share the same key ring
- Back up SQL Server using tested full, differential, and transaction-log policies appropriate to the deployment
- Back up practitioner verification uploads separately from the database
- Restrict upload storage from direct execution and public directory listing
- Use a restricted Google Maps browser key
- Monitor `/health`, 5xx rates, authentication 429s, database latency, and email-delivery failures
- Do not log access tokens, cookies, passwords, reset tokens, appointment reasons, or patient records

## Service-worker release behavior

The cache version in `client/public/sw.js` should be advanced when shell assets or caching rules change. Deploy `sw.js` without a long immutable cache header. The UI lets users choose when to activate an available version, preventing an automatic refresh in the middle of an appointment form.

## Troubleshooting

### Browser repeatedly returns 401

- Confirm frontend requests include credentials.
- Confirm frontend and API are deployed on a cookie-compatible site.
- Verify `JwtSettings` issuer, audience, key, and server clock.
- Clear old cookies after changing cookie paths or domains.

### CORS failure

- Add the exact scheme, host, and port to `Cors__AllowedOrigins__0`.
- Do not include a trailing slash.
- Do not combine credentials with wildcard origins.

### Map panel shows a fallback

- Set `VITE_GOOGLE_MAPS_API_KEY` before the frontend build.
- Restrict the key to the deployed referrer.
- Confirm Maps JavaScript API is enabled.
- Clinic list functionality remains available without the map.

### Password reset or invitation email fails

- Verify SMTP host, port, sender, credentials, and STARTTLS support.
- Confirm `App__BaseUrl` is the frontend URL.
- Persist `DataProtection__KeysPath`; otherwise reset tokens may become invalid after restart.

### Migration reports a duplicate appointment index conflict

Resolve existing duplicate non-cancelled practitioner/start-time data before applying the filtered uniqueness migration. Preserve cancelled history rather than deleting it.

## Known limitations and next milestones

The implementation substantially completes the workflows supported by the repository’s current schema, but the following are not represented fully enough to claim completion:

- The clinic entity currently contains only name and coordinates. Structured address, contact details, services, conditions, operating hours, images, active state, and clinic-owned editing require a new clinic schema and management feature.
- The role enum contains Patient, Practitioner, and Admin. Separate Clinic Staff and Clinic Administrator roles require a clinic-membership/ownership model.
- There is no persisted notification or audit-log domain yet.
- Appointment services, variable service duration, per-clinic time zones, booking cutoffs, availability exceptions, and idempotency keys are not yet modeled. The current deployment uses the configured platform timezone (`Asia/Manila` by default).
- The unique index prevents identical active starts, while interval overlap is enforced in application queries; a stronger database serialization strategy is recommended for high-concurrency multi-instance production.
- Frontend automated tests were not added because no frontend test framework existed in the repository and dependencies could not be installed and executed in the implementation environment.
- Formal browser, device, Lighthouse, screen-reader, map-provider, SMTP, and production database verification remains required.

See `IMPLEMENTATION_REPORT.md` for the exact changes, validation evidence, and acceptance gaps.
