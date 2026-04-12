# Story 1.4: Next.js BFF Auth Layer & i18n Routing

Status: done

<!-- Note: Validation is optional. Run validate-create-story for quality check before dev-story. -->

## Story

As a developer,
I want the Next.js BFF auth layer and i18n URL-prefix routing in place,
So that the browser communicates securely via HttpOnly cookies and all app routes are available in Bulgarian and English.

## Acceptance Criteria

**AC1 — BFF login**
**Given** a user submits credentials via the login form
**When** the Next.js BFF `POST /api/auth/login` Route Handler is called
**Then** it forwards the credentials to the ASP.NET Core API
**And** on success, stores the JWT in an HttpOnly, Secure, SameSite=Strict cookie named `jwt` on the SWA domain
**And** returns the user's role and `account_state` to the client (not the raw JWT)

**AC2 — BFF Bearer forwarding**
**Given** an authenticated page component calls `lib/api.ts`
**When** a request is made to the ASP.NET Core API
**Then** the JWT is read from the `jwt` HttpOnly cookie server-side and forwarded as a `Bearer` header
**And** the browser never has direct JavaScript access to the JWT string

**AC3 — BFF logout**
**Given** a user calls `POST /api/auth/logout`
**When** the Route Handler executes
**Then** the `jwt` HttpOnly cookie is cleared

**AC4 — i18n root redirect**
**Given** next-intl is configured with `bg` as the default locale
**When** a user visits the root path `/`
**Then** `proxy.ts` redirects them to `/bg/`

**AC5 — Language toggle routing**
**Given** a user is on a `/bg/` route
**When** they switch the language toggle to EN
**Then** they are navigated to the equivalent `/en/` route without a full page reload

**AC6 — Landlord auth guard**
**Given** any route under `/[locale]/(landlord)/*`
**When** an unauthenticated user (no `jwt` cookie) attempts to access it
**Then** `(landlord)/layout.tsx` redirects them to `/[locale]/login`

**AC7 — Tenant auth guard**
**Given** any route under `/[locale]/(tenant)/*`
**When** an unauthenticated user (no `jwt` cookie) or a non-Tenant user attempts to access it
**Then** `(tenant)/layout.tsx` redirects them to `/[locale]/login`

**AC8 — BFF `/api/auth/me`**
**Given** a valid `jwt` cookie is present
**When** `GET /api/auth/me` is called
**Then** it decodes the JWT payload (no signature verification — API handles that) and returns `{ role, accountState }` as JSON
**And** if the cookie is absent or malformed, returns 401

## Tasks / Subtasks

- [x] **Task 1: Install test framework (Vitest)** (AC: all — prerequisite for TDD)
  - [x] `npm install -D vitest @vitejs/plugin-react jsdom @testing-library/react @testing-library/dom vite-tsconfig-paths`
  - [x] Create `vitest.config.mts` in `renteasy-web/` root with jsdom environment
  - [x] Add `"test": "vitest"` script to `package.json`

- [x] **Task 2: Add environment variables** (AC: 1, 2)
  - [x] Create `.env.example` with `RENTEASY_API_URL=http://localhost:5164` (no `NEXT_PUBLIC_` prefix — server-side only)
  - [x] Create `.env.local` (git-ignored) with the actual dev API URL
  - [x] Add `.env.local` to `.gitignore` if not already there

- [x] **Task 3: Configure next-intl plugin in `next.config.ts`** (AC: 4, 5)
  - [x] Import `createNextIntlPlugin` from `next-intl/plugin`
  - [x] Wrap `nextConfig` with `withNextIntl('./src/i18n/request.ts')`
  - [x] Export the wrapped config as default

- [x] **Task 4: Create `src/i18n/request.ts`** (AC: 4, 5)
  - [x] Import `getRequestConfig` from `next-intl/server`
  - [x] Export default config with `locales: ['bg', 'en']` and `defaultLocale: 'bg'`
  - [x] In `getRequestConfig`, await `requestLocale`, validate it's in the locales list, fall back to `'bg'` if invalid
  - [x] Return `{ locale, messages: (await import(\`../messages/${locale}.json\`)).default }`

- [x] **Task 5: Create `src/proxy.ts`** (AC: 4, 5) ⚠️ CRITICAL — NOT `middleware.ts`
  - [x] Import `createMiddleware` from `next-intl/middleware`
  - [x] Import `defineRouting` from `next-intl/routing`
  - [x] Define routing: `locales: ['bg', 'en']`, `defaultLocale: 'bg'`, `localePrefix: 'always'`
  - [x] Create `intlProxy` using `createMiddleware(routing)`
  - [x] Export `export function proxy(request: NextRequest)` wrapping `intlProxy(request)`
  - [x] Export `config.matcher` excluding `api`, `_next/static`, `_next/image`, `favicon.ico`
  - [x] Write unit tests in `src/proxy.test.ts` using `unstable_doesMiddlewareMatch` from `next/experimental/testing/server` (note: `unstable_doesProxyMatch` is documented but not yet exported; using the actual exported function)

- [x] **Task 6: Create `[locale]` routing structure** (AC: 4, 5, 6, 7)
  - [x] Create `src/app/[locale]/layout.tsx` — root locale layout:
    - Set `<html lang={locale}>` using `await params` to get locale
    - Keep existing Geist font setup from `src/app/layout.tsx`
    - Remove `src/app/layout.tsx` root layout (it conflicts with `[locale]` layout)
    - Remove/replace `src/app/page.tsx` with redirect to `/bg/` (or rely on proxy)
  - [x] Move messages to `src/messages/` (already there — confirm path)
  - [x] Add minimal translation keys to `bg.json` and `en.json`: `{ "common": { "loading": "...", "error": "..." } }`

- [x] **Task 7: Create BFF Route Handlers** (AC: 1, 2, 3, 8) ⚠️ Files go in `src/app/api/auth/` NOT `src/api/auth/`
  - [x] **`src/app/api/auth/login/route.ts`**:
    - `POST` handler: forward `{ email, password }` to `${RENTEASY_API_URL}/api/auth/login`
    - On 200: set `jwt` cookie (HttpOnly, Secure, SameSite=Strict, Path=/, Max-Age=604800)
    - Return `{ role, accountState }` as JSON (strip the raw token)
    - On non-200: forward the status and ProblemDetails body from the API unchanged
  - [x] **`src/app/api/auth/logout/route.ts`**:
    - `POST` handler: delete the `jwt` cookie (set Max-Age=0 or use `cookieStore.delete()`)
    - Return 200 JSON `{ ok: true }`
  - [x] **`src/app/api/auth/me/route.ts`**:
    - `GET` handler: read `jwt` cookie
    - If absent → 401 JSON `{ error: 'Unauthenticated' }`
    - Decode payload: `Buffer.from(token.split('.')[1], 'base64url').toString('utf-8')` then `JSON.parse`
    - Return `{ role: payload.role, accountState: payload.account_state }`
    - If malformed → 401
  - [x] Write unit tests for each handler in co-located `*.test.ts` files

- [x] **Task 8: Create `src/lib/api.ts`** (AC: 2)
  - [x] Export `async function apiRequest(path: string, init?: RequestInit): Promise<Response>`
  - [x] Read `jwt` cookie using `await cookies()` from `next/headers` (server-side only)
  - [x] Build request: `Authorization: Bearer ${token}`, `Content-Type: application/json`
  - [x] Fetch `${process.env.RENTEASY_API_URL}${path}` with merged headers
  - [x] Return the raw `Response` — callers handle parsing
  - [x] Write unit test mocking `cookies()` and `fetch`

- [x] **Task 9: Create auth guard layouts** (AC: 6, 7)
  - [x] **`src/app/[locale]/(landlord)/layout.tsx`**:
    - Read `jwt` cookie with `await cookies()`
    - If absent → `redirect(\`/${locale}/login\`)` using `redirect` from `next/navigation`
    - Decode JWT payload (same base64 logic as `/me` handler)
    - If role ≠ `'Landlord'` → `redirect(\`/${locale}/login\`)`
    - Render `{children}`
  - [x] **`src/app/[locale]/(tenant)/layout.tsx`**:
    - Read `jwt` cookie
    - If absent → redirect to login
    - Decode JWT payload, check role = `'Tenant'`
    - If `accountState === 'Expired'` → redirect to `/[locale]/expired` (stub — just render a placeholder page for now)
    - Render `{children}`
  - [x] Write tests for redirect behavior using mocked `cookies()`

- [x] **Task 10: Clean up scaffold placeholder files** (AC: all)
  - [x] Remove or replace `src/app/page.tsx` default Next.js scaffold content
  - [x] Update `src/app/layout.tsx` or remove it if `[locale]/layout.tsx` takes over fully
  - [x] Delete `src/api/auth/.gitkeep` — route handlers are now correctly in `src/app/api/auth/`
  - [x] Verify `npm run build` passes with 0 errors

- [x] **Task 11: Run full test suite and verify build** (AC: all)
  - [x] `npm run test` — all tests pass
  - [x] `npm run build` — no type errors, no lint errors
  - [x] `npm run lint` — clean

## Dev Notes

### ⚠️ CRITICAL: Next.js 16 Breaking Changes (DO NOT USE TRAINING DATA DEFAULTS)

**1. `middleware.ts` is DEPRECATED — renamed to `proxy.ts`**
- File: `src/proxy.ts` (NOT `src/middleware.ts`)
- Exported function: `export function proxy(request: NextRequest)` (NOT `middleware`)
- `export const config` works the same
- Migration codemod: `npx @next/codemod@canary middleware-to-proxy .` (reference only — don't run it; just use proxy.ts directly)

**2. Cookies API is async**
```ts
import { cookies } from 'next/headers'
const cookieStore = await cookies()          // MUST await
const token = cookieStore.get('jwt')?.value
cookieStore.set('jwt', value, { httpOnly: true, ... })
cookieStore.delete('jwt')
```

**3. Route params are async in layouts/pages**
```ts
export default async function Layout({ params }: { params: Promise<{ locale: string }> }) {
  const { locale } = await params             // MUST await
}
```

### next-intl 4.9.0 Setup (App Router)

**Plugin in `next.config.ts`:**
```ts
import createNextIntlPlugin from 'next-intl/plugin'
const withNextIntl = createNextIntlPlugin('./src/i18n/request.ts')
export default withNextIntl({ /* nextConfig */ })
```

**`src/i18n/request.ts`** (required by plugin):
```ts
import { getRequestConfig } from 'next-intl/server'

export default getRequestConfig(async ({ requestLocale }) => {
  let locale = await requestLocale
  if (!locale || !['bg', 'en'].includes(locale)) locale = 'bg'
  return {
    locale,
    messages: (await import(`../messages/${locale}.json`)).default
  }
})
```

**`src/proxy.ts`** (wrapping next-intl's createMiddleware):
```ts
import createMiddleware from 'next-intl/middleware'
import { defineRouting } from 'next-intl/routing'
import type { NextRequest } from 'next/server'

const routing = defineRouting({ locales: ['bg', 'en'], defaultLocale: 'bg' })
const intlProxy = createMiddleware(routing)

export function proxy(request: NextRequest) {
  return intlProxy(request)
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)']
}
```

### BFF Route Handler Pattern

**Cookie options for the `jwt` cookie:**
```ts
{
  httpOnly: true,
  secure: process.env.NODE_ENV === 'production',
  sameSite: 'strict' as const,
  path: '/',
  maxAge: 60 * 60 * 24 * 7    // 7 days — matches JWT ExpiryDays from Story 1.3
}
```

**JWT payload decode (no library needed):**
```ts
function decodeJwtPayload(token: string): Record<string, unknown> {
  const base64 = token.split('.')[1]
  return JSON.parse(Buffer.from(base64, 'base64url').toString('utf-8'))
}
```

**Claim names in JWT** (from Story 1.3 AuthService — these are the actual claim names in the token):
- `role` → `'Landlord'` or `'Tenant'`
- `account_state` → `'Active'`, `'ReadOnly'`, `'Expired'`, `'RequiresPasswordChange'`
- `sub` → user UUID
- `email`

**API URL env var (server-side only — no `NEXT_PUBLIC_` prefix):**
```
RENTEASY_API_URL=http://localhost:5164    # dev
RENTEASY_API_URL=https://renteasy-api.azurewebsites.net    # production
```

### Route Handler Location

**⚠️ WRONG (scaffold placeholder — delete this):**
```
src/api/auth/.gitkeep    ← NOT a valid Next.js location
```

**CORRECT (Next.js App Router requires route handlers inside `app/`):**
```
src/app/api/auth/login/route.ts
src/app/api/auth/logout/route.ts
src/app/api/auth/me/route.ts
```

### File Structure to Create

```
renteasy-web/
├── vitest.config.mts                         ← NEW
├── .env.example                              ← NEW
├── .env.local                                ← NEW (git-ignored)
├── next.config.ts                            ← UPDATE (add next-intl plugin)
└── src/
    ├── proxy.ts                              ← NEW (NOT middleware.ts)
    ├── proxy.test.ts                         ← NEW
    ├── i18n/
    │   └── request.ts                        ← NEW
    ├── app/
    │   ├── layout.tsx                        ← UPDATE or REMOVE (merge into [locale]/layout.tsx)
    │   ├── page.tsx                          ← REMOVE/REPLACE (locale redirect or delete)
    │   ├── api/
    │   │   └── auth/
    │   │       ├── login/
    │   │       │   ├── route.ts              ← NEW
    │   │       │   └── route.test.ts         ← NEW
    │   │       ├── logout/
    │   │       │   ├── route.ts              ← NEW
    │   │       │   └── route.test.ts         ← NEW
    │   │       └── me/
    │   │           ├── route.ts              ← NEW
    │   │           └── route.test.ts         ← NEW
    │   └── [locale]/
    │       ├── layout.tsx                    ← NEW (root locale layout)
    │       ├── (auth)/
    │       │   └── .gitkeep                  ← KEEP (placeholder for Story 1.5)
    │       ├── (landlord)/
    │       │   └── layout.tsx                ← NEW (auth guard)
    │       └── (tenant)/
    │           └── layout.tsx                ← NEW (auth guard)
    ├── api/
    │   └── auth/
    │       └── .gitkeep                      ← DELETE (wrong location)
    ├── lib/
    │   ├── api.ts                            ← NEW
    │   └── api.test.ts                       ← NEW
    └── messages/
        ├── bg.json                           ← UPDATE (add common keys)
        └── en.json                           ← UPDATE (add common keys)
```

### Vitest Config

```ts
// vitest.config.mts
import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'
import tsconfigPaths from 'vite-tsconfig-paths'

export default defineConfig({
  plugins: [tsconfigPaths(), react()],
  test: {
    environment: 'jsdom',
  },
})
```

### Auth Guard Layout Pattern

```tsx
// src/app/[locale]/(landlord)/layout.tsx
import { cookies } from 'next/headers'
import { redirect } from 'next/navigation'

export default async function LandlordLayout({
  children,
  params,
}: {
  children: React.ReactNode
  params: Promise<{ locale: string }>
}) {
  const { locale } = await params
  const cookieStore = await cookies()
  const token = cookieStore.get('jwt')?.value

  if (!token) redirect(`/${locale}/login`)

  try {
    const payload = JSON.parse(
      Buffer.from(token.split('.')[1], 'base64url').toString('utf-8')
    ) as { role: string; account_state: string }

    if (payload.role !== 'Landlord') redirect(`/${locale}/login`)
  } catch {
    redirect(`/${locale}/login`)
  }

  return <>{children}</>
}
```

### What Already Exists (DO NOT RECREATE)

From Story 1.1 scaffold:
- `renteasy-web/` — Next.js 16 project with App Router, Tailwind 4, TypeScript strict
- `next-intl@4.9.0` — already installed in `package.json`
- `src/app/layout.tsx` — root layout with Geist fonts (merge into `[locale]/layout.tsx`, don't lose fonts)
- `src/app/globals.css` — keep as-is
- `src/messages/bg.json` and `src/messages/en.json` — exist but empty `{}`
- `src/api/auth/.gitkeep` — wrong location placeholder (DELETE in Task 10)
- Route group folders `(auth)`, `(landlord)`, `(tenant)`, `(public)` — exist with `.gitkeep` files

From Story 1.3 (ASP.NET Core API):
- `POST /api/auth/login` — returns `{ token, role, accountState }` JSON
- `POST /api/auth/change-password` — authenticated endpoint
- CORS configured to allow the SWA origin from `appsettings.json`
- JWT contains claims: `sub`, `email`, `role`, `account_state`, `iat`, `landlord_id` (landlords only)
- Seeded landlord: email and password from `appsettings.Development.json`

### Testing Strategy

**Proxy tests** (`src/proxy.test.ts`) — use `next/experimental/testing/server`:
- `/bg/dashboard` → proxy runs and allows
- `/api/auth/login` → proxy does NOT run (excluded by matcher)
- `/` → proxy runs and redirects to `/bg/`

**Route Handler tests** — import and call handler functions directly with mock `NextRequest`:
```ts
// route.test.ts
import { POST } from './route'
import { NextRequest } from 'next/server'
// Mock global fetch and next/headers
```

**Auth guard tests** — mock `cookies()` from `next/headers` and `redirect` from `next/navigation`:
```ts
vi.mock('next/headers', () => ({
  cookies: vi.fn().mockResolvedValue({ get: () => undefined })
}))
vi.mock('next/navigation', () => ({
  redirect: vi.fn()
}))
```

### References

- [Source: _bmad-output/planning-artifacts/epics.md — Story 1.4 acceptance criteria]
- [Source: _bmad-output/planning-artifacts/architecture.md — BFF Auth Layer, Frontend Architecture, Frontend project organisation]
- [Source: _bmad-output/implementation-artifacts/1-3-authentication-api.md — JWT claim names, CORS config, seeded accounts]
- [Source: renteasy-web/node_modules/next/dist/docs/01-app/03-api-reference/03-file-conventions/proxy.md — proxy.ts convention, Next.js 16 migration]
- [Source: renteasy-web/node_modules/next/dist/docs/01-app/01-getting-started/15-route-handlers.md — Route Handler file convention]
- [Source: renteasy-web/node_modules/next/dist/docs/01-app/03-api-reference/04-functions/cookies.md — async cookies() API]
- [Source: renteasy-web/node_modules/next-intl/dist/types/middleware/middleware.d.ts — createMiddleware type]
- [Source: renteasy-web/node_modules/next-intl/dist/types/plugin/createNextIntlPlugin.d.ts — plugin API]
- [Source: renteasy-web/node_modules/next-intl/dist/types/server/react-server/getRequestConfig.d.ts — getRequestConfig API]
- [Source: renteasy-web/node_modules/next/dist/docs/01-app/02-guides/testing/vitest.md — Vitest setup for Next.js 16]

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

- `unstable_doesProxyMatch` is documented in Next.js 16 proxy.md but is NOT exported from `next/experimental/testing/server`. Used `unstable_doesMiddlewareMatch` (the actual exported function) in proxy tests instead.
- `next-intl/middleware` (ESM build) imports `next/server` without `.js` extension, which fails Vitest's module resolution. Resolved by `vi.mock()`-ing `next-intl/middleware`, `next-intl/routing`, and `next/server` in proxy tests.
- `app/layout.tsx` and `app/page.tsx` deleted (not just replaced) — Next.js 16 supports omitting the top-level root layout when `[locale]/layout.tsx` serves as a dynamic root layout with html/body tags.
- `variable named 'module'` triggers `@next/next/no-assign-module-variable` ESLint rule in proxy.test.ts — renamed to `proxyModule`.

### Completion Notes List

- Installed Vitest + testing library with jsdom environment; added `vitest.config.mts` with resolve aliases for Next.js 16 ESM compatibility.
- Created `src/proxy.ts` using Next.js 16 proxy convention (NOT `middleware.ts`), wrapping next-intl's `createMiddleware`. 5 proxy matcher tests pass.
- Created BFF Route Handlers at `src/app/api/auth/{login,logout,me}/route.ts` — JWT stored in HttpOnly cookie, never exposed to browser. 6 tests pass.
- Created `src/lib/api.ts` — server-side API client that reads `jwt` cookie and forwards as Bearer token. 4 tests pass.
- Created `[locale]/layout.tsx` as the root layout (html/body with `lang={locale}`) replacing the scaffold `app/layout.tsx`.
- Created landlord and tenant auth guard layouts with role-based redirect logic. 8 tests pass.
- Full suite: 23 tests passing, `npm run build` clean, `npm run lint` clean.

### File List

- `renteasy-web/package.json` (modified — added test script, devDependencies)
- `renteasy-web/vitest.config.mts` (new)
- `renteasy-web/.env.example` (new)
- `renteasy-web/.env.local` (new, git-ignored)
- `renteasy-web/next.config.ts` (modified — added next-intl plugin)
- `renteasy-web/src/i18n/request.ts` (new)
- `renteasy-web/src/proxy.ts` (new)
- `renteasy-web/src/proxy.test.ts` (new)
- `renteasy-web/src/app/layout.tsx` (deleted)
- `renteasy-web/src/app/page.tsx` (deleted)
- `renteasy-web/src/api/auth/.gitkeep` (deleted)
- `renteasy-web/src/app/[locale]/layout.tsx` (new)
- `renteasy-web/src/app/[locale]/expired/page.tsx` (new)
- `renteasy-web/src/app/[locale]/(landlord)/layout.tsx` (new)
- `renteasy-web/src/app/[locale]/(landlord)/layout.test.ts` (new)
- `renteasy-web/src/app/[locale]/(tenant)/layout.tsx` (new)
- `renteasy-web/src/app/[locale]/(tenant)/layout.test.ts` (new)
- `renteasy-web/src/app/api/auth/login/route.ts` (new)
- `renteasy-web/src/app/api/auth/login/route.test.ts` (new)
- `renteasy-web/src/app/api/auth/logout/route.ts` (new)
- `renteasy-web/src/app/api/auth/logout/route.test.ts` (new)
- `renteasy-web/src/app/api/auth/me/route.ts` (new)
- `renteasy-web/src/app/api/auth/me/route.test.ts` (new)
- `renteasy-web/src/lib/api.ts` (new)
- `renteasy-web/src/lib/api.test.ts` (new)
- `renteasy-web/src/messages/bg.json` (modified — added common keys)
- `renteasy-web/src/messages/en.json` (modified — added common keys)
- `_bmad-output/implementation-artifacts/sprint-status.yaml` (modified — 1-4 in-progress → review)

### Review Findings

- [x] [Review][Defer] Language toggle AC5 — LanguageToggle UI component deferred to Story 1.5/1.6; routing infrastructure (proxy, i18n config) is in place — deferred, intentional
- [x] [Review][Patch] `RequiresPasswordChange` account state not handled in layout guards — neither `(tenant)/layout.tsx` nor `(landlord)/layout.tsx` redirect when `account_state === 'RequiresPasswordChange'`; the user lands in the full authenticated app without being forced to change their password [src/app/[locale]/(tenant)/layout.tsx, src/app/[locale]/(landlord)/layout.tsx]
- [x] [Review][Patch] `request.json()` in login route has no try/catch — malformed JSON body throws an unhandled exception and returns 500 instead of a clean 400 [src/app/api/auth/login/route.ts]
- [x] [Review][Patch] `RENTEASY_API_URL` undefined causes cryptic fetch error — no startup or runtime guard; missing env var silently produces `"undefined/api/auth/..."` URL and a non-descriptive TypeError [src/app/api/auth/login/route.ts, src/lib/api.ts]
- [x] [Review][Defer] JWT decode logic duplicated — `decodeJwtPayload` in `me/route.ts` and inline `Buffer.from` decode in both layout guards are independent implementations; shape changes will diverge silently — deferred, pre-existing
- [x] [Review][Defer] `/api/auth/me` returns HTTP 200 with `undefined` role/accountState when JWT payload lacks expected claims — low risk (token always issued by API), but no defensive check — deferred, pre-existing
- [x] [Review][Defer] `apiRequest` unconditionally sets `Content-Type: application/json` on GET requests — technically invalid header for bodyless requests — deferred, pre-existing

### Change Log

- 2026-04-12: Implemented Story 1.4 — Next.js BFF auth layer and i18n routing. Added Vitest test framework, BFF route handlers (login/logout/me), next-intl proxy routing, auth guard layouts, and server-side API client. 23 tests, 0 lint errors, build passes.
