# RentEasyV2

A monorepo containing the RentEasy rental management platform.

## Projects

- `renteasy-web/` — Next.js 16 frontend (Azure Static Web Apps)
- `renteasy-api/` — ASP.NET Core 10 Web API (Azure App Service B1)

## Cloud Prerequisites

These resources must be created manually before CI/CD and the app will work end-to-end. One-time setup.

Azure resource names follow the [CAF naming convention](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming): `{type}-{workload}-{env}`.

### 1. Resource Group

Create a resource group to contain all resources:

| Field | Value |
|---|---|
| Name | `rg-renteasy-prod` |
| Region | West Europe |

### 2. Azure App Service (API)

| Field | Value |
|---|---|
| Name | `app-renteasy-api-prod` |
| Runtime | .NET 10 |
| SKU | B1 |
| Resource group | `rg-renteasy-prod` |

After creation:
1. App Service → **Deployment Center** → download the **Publish Profile**
2. Add it as a GitHub secret: `AZURE_WEBAPP_PUBLISH_PROFILE`

### 3. Azure Static Web Apps (Frontend)

| Field | Value |
|---|---|
| Name | `swa-renteasy-web-prod` |
| Resource group | `rg-renteasy-prod` |
| Build preset | Next.js |
| Branch | `main` |

After creation:
1. Static Web App → **Manage deployment token** → copy the token
2. Add it as a GitHub secret: `AZURE_STATIC_WEB_APPS_API_TOKEN`

### 4. Azure Storage Account (Files & PDFs)

Storage account names must be globally unique, lowercase, alphanumeric only, max 24 chars.

| Field | Value |
|---|---|
| Name | `strenteasy` (adjust if taken) |
| Resource group | `rg-renteasy-prod` |
| Redundancy | LRS |
| Access tier | Hot |

After creation:
1. Create a private blob container named `files`
2. Copy the **connection string** from Access Keys
3. Add to App Service → **Environment variables**:
   ```
   AzureStorage__ConnectionString = <connection string>
   AzureStorage__ContainerName   = files
   ```
4. Add the same values to your local `renteasy-api/appsettings.Development.json` (git-ignored)

### 5. Neon PostgreSQL (Database)

1. Create a project at [neon.tech](https://neon.tech) — region: **EU (Frankfurt)**
2. Confirm **encryption at rest** is enabled in the Neon dashboard
3. Copy two connection strings from the Neon dashboard:
   - **Pooled** (PgBouncer) — for runtime queries
   - **Direct** — for EF Core migrations only
4. Add to App Service → **Environment variables**:
   ```
   ConnectionStrings__DefaultConnection  = <pooled connection string>
   ConnectionStrings__MigrationConnection = <direct connection string>
   ```
5. Add the same values to your local `renteasy-api/appsettings.Development.json`

### 6. Resend (Transactional Email)

1. Create an account at [resend.com](https://resend.com) — select **EU region**
2. Add and verify your sending domain
3. Create an **API key**
4. Add to App Service → **Environment variables**:
   ```
   Resend__ApiKey    = <api key>
   Resend__FromEmail = noreply@yourdomain.com
   ```
5. Add the same values to your local `renteasy-api/appsettings.Development.json`

### GitHub Secrets Summary

| Secret | Where to get it |
|---|---|
| `AZURE_WEBAPP_PUBLISH_PROFILE` | App Service → Deployment Center → Download publish profile |
| `AZURE_STATIC_WEB_APPS_API_TOKEN` | Static Web App → Manage deployment token |

---

## Local Development

### Backend

```bash
cd renteasy-api
# appsettings.Development.json is git-ignored — create it with your local secrets
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

- `api-deploy.yml` — builds and deploys `renteasy-api` to `app-renteasy-api-prod` (Azure App Service B1)
- `web-deploy.yml` — builds and deploys `renteasy-web` to `swa-renteasy-web-prod` (Azure Static Web Apps)
