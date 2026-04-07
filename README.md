# RentEasyV2

A monorepo containing the RentEasy rental management platform.

## Projects

- `renteasy-web/` — Next.js 16 frontend (Azure Static Web Apps)
- `renteasy-api/` — ASP.NET Core 10 Web API (Azure App Service B1)

## Cloud Prerequisites

These resources must be created manually before CI/CD and the app will work end-to-end. One-time setup.

### 1. Azure App Service (API)

1. Create a **Resource Group** (e.g. `renteasy-rg`)
2. Create an **App Service** — runtime: `.NET 10`, tier: **B1**, name: `renteasy-api`
3. In the App Service → **Deployment Center** → download the **Publish Profile**
4. Add it as a GitHub secret: `AZURE_WEBAPP_PUBLISH_PROFILE`

### 2. Azure Static Web Apps (Frontend)

1. Create a **Static Web App** in the same resource group, name: `renteasy-web`
   - Build preset: **Next.js**
   - Link to the `kdomuschiev/RentEasyV2` GitHub repo, branch: `main`
2. In the Static Web App → **Manage deployment token** → copy the token
3. Add it as a GitHub secret: `AZURE_STATIC_WEB_APPS_API_TOKEN`

### 3. Azure Blob Storage (Files & PDFs)

1. Create a **Storage Account** in the same resource group (LRS, standard tier)
2. Create a container named `renteasy-files` (private access)
3. Copy the **connection string** from Access Keys
4. Add to `appsettings.Development.json` (local) and App Service → **Configuration** (production):
   ```
   AzureStorage__ConnectionString = <connection string>
   AzureStorage__ContainerName = renteasy-files
   ```

### 4. Neon PostgreSQL (Database)

1. Create a **Neon** project at [neon.tech](https://neon.tech) — region: EU (Frankfurt)
2. Confirm **encryption at rest** is enabled in the Neon dashboard (required — NFR-S)
3. Copy two connection strings from the Neon dashboard:
   - **Pooled** (for runtime queries — via PgBouncer)
   - **Direct** (for EF Core migrations only)
4. Add to `appsettings.Development.json` (local) and App Service → **Configuration** (production):
   ```
   ConnectionStrings__DefaultConnection = <pooled connection string>
   ConnectionStrings__MigrationConnection = <direct connection string>
   ```

### 5. Resend (Transactional Email)

1. Create a **Resend** account at [resend.com](https://resend.com) — select EU region
2. Add and verify your sending domain
3. Create an **API key**
4. Add to `appsettings.Development.json` (local) and App Service → **Configuration** (production):
   ```
   Resend__ApiKey = <api key>
   Resend__FromEmail = noreply@yourdomain.com
   ```

### 6. GitHub Secrets Summary

| Secret | Where to get it |
|---|---|
| `AZURE_WEBAPP_PUBLISH_PROFILE` | App Service → Deployment Center → Download publish profile |
| `AZURE_STATIC_WEB_APPS_API_TOKEN` | Static Web App → Manage deployment token |

---

## Getting Started (Local Development)

### Backend

```bash
cd renteasy-api
# Create appsettings.Development.json with your local connection strings (git-ignored)
dotnet run
```

### Frontend

```bash
cd renteasy-web
npm install
npm run dev
```

## CI/CD

Both projects deploy automatically to Azure on push to `main`:
- `api-deploy.yml` — builds and deploys `renteasy-api` to Azure App Service B1
- `web-deploy.yml` — builds and deploys `renteasy-web` to Azure Static Web Apps
