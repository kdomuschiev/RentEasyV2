# Story 1.5: Login & Authentication UI

Status: done

## Story

As a landlord or tenant,
I want a login screen, a forced first-login password change screen, and account settings password change,
So that I can securely access my account and manage my credentials from any device.

## Acceptance Criteria

**AC1 — Unauthenticated redirect**
**Given** an unauthenticated user visits any authenticated route
**When** the route guard evaluates their session
**Then** they are redirected to `/[locale]/login`
*(Already implemented in Story 1.4 — guard layouts exist. Verify it still works.)*

**AC2 — Login → role-appropriate redirect**
**Given** the login page at `/[locale]/login`
**When** a user enters valid credentials and submits
**Then** they are redirected to: landlord → `/[locale]/dashboard`, tenant → `/[locale]/billing`

**AC3 — RequiresPasswordChange redirect**
**Given** a user with `account_state = 'RequiresPasswordChange'` successfully logs in
**When** the login page processes the response
**Then** they are redirected to `/[locale]/change-password` and cannot navigate to any other authenticated route until the password is changed
*(The auth guard layouts already handle the cannot-navigate part from Story 1.4 patch.)*

**AC4 — Forced password change (single new-password field)**
**Given** the forced password change screen at `/[locale]/change-password`
**When** the user enters a new password meeting minimum requirements (8+ chars, mix of letters + numbers)
**Then** the password is changed and they are redirected to their role-appropriate default screen
**And** the new password field has a show/hide eye icon toggle
**And** there is NO current-password field and NO confirmation field on this screen

**AC5 — Account settings password change**
**Given** the account settings page for an authenticated user
**When** they submit a password change with the correct current password and a valid new password
**Then** the password is updated and an inline success message is shown (no page reload)
**And** the session remains active (new JWT issued and stored in cookie)

**AC6 — Language toggle**
**Given** the language toggle is present in the authenticated app header
**When** a user taps BG or EN
**Then** the active locale switches instantly without a reload or re-authentication
**And** the `<html lang="...">` attribute updates to match (because `[locale]/layout.tsx` already derives it from the URL locale)

**AC7 — Inline field validation**
**Given** any login or password form field has a validation error
**When** the user moves focus away from the field (blur event)
**Then** an inline error message appears beneath the field in text (not colour alone)

## Tasks / Subtasks

- [x] **Task 1: New API endpoint — `POST /api/auth/forced-change-password`** (AC: 4)
  - [x] In `renteasy-api/Controllers/AuthController.cs`: add `[HttpPost("forced-change-password")]` action
  - [x] Policy: requires valid JWT + `account_state === "RequiresPasswordChange"` (403 otherwise)
  - [x] Body DTO: `ForcedChangePasswordRequest { NewPassword: string }`
  - [x] Validate `NewPassword` length ≥ 8 and contains at least one letter and one digit (return 400 ProblemDetails otherwise)
  - [x] Call `UserManager.ChangePasswordAsync` — but `RequiresPasswordChange` users have a temp password they don't know → use `UserManager.RemovePasswordAsync` then `AddPasswordAsync` to set new password without requiring current
  - [x] After success: update `user.TokenValidFrom = DateTime.UtcNow` and `user.AccountState = AccountState.Active`, save
  - [x] Issue a new JWT (call `AuthService.GenerateJwtAsync(user)`) and return `{ token, role, accountState }` (same shape as login response)
  - [x] Add unit test co-located: `ForcedChangePasswordTests.cs`

- [x] **Task 2: Update `POST /api/auth/change-password` to return a new JWT** (AC: 5)
  - [x] In `renteasy-api/Controllers/AuthController.cs`: after successful password change, issue new JWT and return `{ token, role, accountState }`
  - [x] This is backward-compatible (existing callers that ignored the body still work)
  - [x] Update the existing test to assert the new response shape

- [x] **Task 3: BFF route handler — `POST /api/auth/forced-change-password`** (AC: 4)
  - [x] Create `renteasy-web/src/app/api/auth/forced-change-password/route.ts`
  - [x] `POST` handler: read `jwt` cookie (if absent → 401), forward `{ newPassword }` to `${RENTEASY_API_URL}/api/auth/forced-change-password` as Bearer
  - [x] On success: set new `jwt` cookie (same options as login handler), return `{ role, accountState }` JSON
  - [x] On non-200: forward status + ProblemDetails body unchanged
  - [x] Write `route.test.ts` (mock `cookies()` and `fetch`)

- [x] **Task 4: BFF route handler — `POST /api/auth/change-password`** (AC: 5)
  - [x] Create `renteasy-web/src/app/api/auth/change-password/route.ts`
  - [x] `POST` handler: read `jwt` cookie (if absent → 401), forward `{ currentPassword, newPassword }` to `${RENTEASY_API_URL}/api/auth/change-password` as Bearer
  - [x] On success: set new `jwt` cookie with token from API response, return `{ ok: true }`
  - [x] On non-200: forward status + ProblemDetails body unchanged
  - [x] Write `route.test.ts`

- [x] **Task 5: Login page** (AC: 2, 3, 7)
  - [x] Create `renteasy-web/src/app/[locale]/(auth)/login/page.tsx`
  - [x] Client Component (`'use client'`) — needs `useState` for form state, errors, loading
  - [x] Fields: `email` (type="email") + `password` (type="password", show/hide toggle)
  - [x] Both fields: validate on blur — email must be non-empty + valid format, password non-empty
  - [x] Submit: `POST /api/auth/login` (BFF — already exists) with `{ email, password }`
  - [x] On success `{ role, accountState }`:
    - `accountState === 'RequiresPasswordChange'` → `router.push('/[locale]/change-password')` (use current locale from `useLocale()`)
    - `role === 'Landlord'` → `router.push('/[locale]/dashboard')`
    - `role === 'Tenant'` → `router.push('/[locale]/billing')`
  - [x] On 401: show inline API error message (ProblemDetails `detail` field, or generic "Invalid email or password")
  - [x] Disable submit button and show loading state during request
  - [x] All strings from `messages/` via `useTranslations('auth')`
  - [x] Write `page.test.tsx` (render, fill fields, submit, assert redirect calls)

- [x] **Task 6: Password show/hide utility component** (AC: 4, 7)
  - [x] Create `renteasy-web/src/components/ui/PasswordInput.tsx`
  - [x] Client Component wrapping `<input type={showPassword ? 'text' : 'password'}>`
  - [x] Eye / eye-off toggle button inside the input wrapper (position: absolute right)
  - [x] Props: `id`, `name`, `value`, `onChange`, `onBlur`, `placeholder`, `error?: string`
  - [x] Renders error text beneath field when `error` is set
  - [x] `aria-describedby` pointing to error element for screen readers
  - [x] Write `PasswordInput.test.tsx`

- [x] **Task 7: Forced change-password page** (AC: 4, 7)
  - [x] Create `renteasy-web/src/app/[locale]/(auth)/change-password/page.tsx`
  - [x] Client Component
  - [x] Single field: new password using `PasswordInput` component (no current-password field, no confirm field)
  - [x] Blur validation: 8+ chars, at least one letter + one digit
  - [x] Submit: `POST /api/auth/forced-change-password` (BFF from Task 3) with `{ newPassword }`
  - [x] On success `{ role, accountState }`: redirect to `/[locale]/dashboard` or `/[locale]/billing` based on role
  - [x] On non-200: show inline API error (ProblemDetails `detail`)
  - [x] All strings from `messages/` via `useTranslations('auth')`
  - [x] Write `page.test.tsx`

- [x] **Task 8: Language toggle + minimal app header** (AC: 6)
  - [x] Create `renteasy-web/src/components/ui/LanguageToggle.tsx`
    - Client Component (`'use client'`)
    - `useLocale()` from `next-intl` for current locale
    - `useRouter()` and `usePathname()` from `next/navigation`
    - Locale switch: replace first path segment → `router.push(segments.join('/'))`
    - Renders `<button>BG</button>` and `<button>EN</button>`, active locale visually marked
    - `aria-pressed` attribute on each button
  - [x] Create `renteasy-web/src/components/ui/AppHeader.tsx`
    - Client Component — contains `<LanguageToggle />` + logout button
    - Logout button: calls `POST /api/auth/logout` (BFF) and redirects to `/[locale]/login`
    - Minimal styling (Story 1.6 will apply design system)
  - [x] Update `(landlord)/layout.tsx` to wrap `{children}` with `<AppHeader />` above content
  - [x] Update `(tenant)/layout.tsx` to wrap `{children}` with `<AppHeader />` above content
  - [x] Write `LanguageToggle.test.tsx`

- [x] **Task 9: Account settings page** (AC: 5)
  - [x] Create `renteasy-web/src/components/ui/PasswordChangeForm.tsx`
    - Client Component
    - Fields: current password (`PasswordInput`), new password (`PasswordInput`)
    - No confirm-password field
    - Blur validation on both fields; new password must be 8+ chars with letter + digit
    - Submit: `POST /api/auth/change-password` (BFF from Task 4)
    - On success: show inline success message, clear form
    - On error: show inline API error
  - [x] Create `renteasy-web/src/app/[locale]/settings/page.tsx` (shared for landlord+tenant — Next.js route groups can't have duplicate URL paths)
  - [x] Write `PasswordChangeForm.test.tsx`

- [x] **Task 10: Redirect-target stub pages** (AC: 2, 3)
  - [x] Create `renteasy-web/src/app/[locale]/(landlord)/dashboard/page.tsx` — placeholder (Story 2.3 will implement)
  - [x] Create `renteasy-web/src/app/[locale]/(tenant)/billing/page.tsx` — placeholder (Story 3.4 will implement)
  - [x] Each stub returns a simple `<main>` with page title (no styling needed, Story 1.6+)

- [x] **Task 11: Add i18n translation keys** (AC: all)
  - [x] Update `renteasy-web/src/messages/bg.json` — add `auth` namespace
  - [x] Update `renteasy-web/src/messages/en.json` — add same `auth` namespace in English

- [x] **Task 12: Run full test suite and verify build** (AC: all)
  - [x] `cd renteasy-web && npm run test` — 68 tests pass (14 test files)
  - [x] `cd renteasy-web && npm run build` — no errors
  - [x] `cd renteasy-web && npm run lint` — clean
  - [x] `cd renteasy-api && dotnet test` — 27 tests pass

## Dev Notes

### ⚠️ CRITICAL: Next.js 16 Breaking Changes (carry forward from Story 1.4)

**1. `proxy.ts` not `middleware.ts`** — already in place, do not rename.

**2. Cookies API is async (MUST await):**
```ts
import { cookies } from 'next/headers'
const cookieStore = await cookies()          // MUST await
const token = cookieStore.get('jwt')?.value
```

**3. Route params are async in layouts/pages:**
```ts
export default async function Layout({ params }: { params: Promise<{ locale: string }> }) {
  const { locale } = await params             // MUST await
}
```

**4. Client Components for interactive forms** — all login/password forms are Client Components (`'use client'`). Use `useState` for form values, errors, and loading state. No global state library.

**5. Read Next.js docs before writing code** — `renteasy-web/node_modules/next/dist/docs/` is authoritative. Training-data defaults are wrong for Next.js 16.

### What Already Exists (DO NOT RECREATE)

From Story 1.4:
- `src/proxy.ts` — next-intl locale routing (handles `/` → `/bg/` redirect)
- `src/app/api/auth/login/route.ts` — BFF login handler (sets `jwt` cookie, returns `{ role, accountState }`)
- `src/app/api/auth/logout/route.ts` — BFF logout handler (clears `jwt` cookie)
- `src/app/api/auth/me/route.ts` — BFF me handler (decodes JWT, returns `{ role, accountState }`)
- `src/lib/api.ts` — server-side API client (reads `jwt` cookie, adds Bearer header)
- `src/app/[locale]/(landlord)/layout.tsx` — auth guard: checks JWT, role===Landlord, handles RequiresPasswordChange → redirect
- `src/app/[locale]/(tenant)/layout.tsx` — auth guard: checks JWT, role===Tenant, handles Expired + RequiresPasswordChange → redirect
- `src/app/[locale]/expired/page.tsx` — stub for Expired account state
- Vitest + Testing Library configured in `vitest.config.mts`
- JWT cookie options: `{ httpOnly: true, secure: process.env.NODE_ENV === 'production', sameSite: 'strict', path: '/', maxAge: 60 * 60 * 24 * 7 }`

From Story 1.3 (ASP.NET Core API):
- `POST /api/auth/login` → returns `{ token, role, accountState }`
- `POST /api/auth/change-password` → requires `{ currentPassword, newPassword }` (Task 2 updates this to also return `{ token, role, accountState }`)
- `AuthService.GenerateJwtAsync(user)` — call this after forced password change to issue new token
- JWT claims: `role` → `'Landlord'`/`'Tenant'`, `account_state` → string enum value, `sub` → UUID, `email`, `landlord_id` (landlords only)
- `AccountState` enum: `Active`, `ReadOnly`, `Expired`, `RequiresPasswordChange`

**Important:** `(auth)` route group has NO layout — it is intentionally unauthenticated. The login and change-password pages must be publicly reachable.

### BFF Pattern — New Route Handlers

Cookie JWT options (same for all BFF handlers — copy exactly from Story 1.4):
```ts
const cookieOptions = {
  httpOnly: true,
  secure: process.env.NODE_ENV === 'production',
  sameSite: 'strict' as const,
  path: '/',
  maxAge: 60 * 60 * 24 * 7,
}
```

**`forced-change-password` BFF handler pattern:**
```ts
// src/app/api/auth/forced-change-password/route.ts
import { cookies } from 'next/headers'
import { NextResponse } from 'next/server'

const API_URL = process.env.RENTEASY_API_URL
if (!API_URL) throw new Error('RENTEASY_API_URL is not set')

export async function POST(request: Request) {
  const cookieStore = await cookies()
  const token = cookieStore.get('jwt')?.value
  if (!token) return NextResponse.json({ error: 'Unauthenticated' }, { status: 401 })

  let body: unknown
  try { body = await request.json() } catch { return NextResponse.json({ error: 'Invalid JSON' }, { status: 400 }) }

  const apiRes = await fetch(`${API_URL}/api/auth/forced-change-password`, {
    method: 'POST',
    headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })

  if (!apiRes.ok) {
    const errorBody = await apiRes.json().catch(() => ({}))
    return NextResponse.json(errorBody, { status: apiRes.status })
  }

  const data = await apiRes.json() as { token: string; role: string; accountState: string }
  const response = NextResponse.json({ role: data.role, accountState: data.accountState })
  response.cookies.set('jwt', data.token, cookieOptions)
  return response
}
```

### Login Page — Client Component Pattern

```tsx
// src/app/[locale]/(auth)/login/page.tsx
'use client'
import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { useLocale, useTranslations } from 'next-intl'

export default function LoginPage() {
  const locale = useLocale()
  const router = useRouter()
  const t = useTranslations('auth.login')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [errors, setErrors] = useState<{ email?: string; password?: string; api?: string }>({})
  const [loading, setLoading] = useState(false)

  const validateEmail = (v: string) => {
    if (!v) return t('validation.required')            // from messages/
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v)) return t('validation.invalidEmail')
    return ''
  }
  const validatePassword = (v: string) => !v ? t('validation.required') : ''

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const emailErr = validateEmail(email)
    const passErr = validatePassword(password)
    if (emailErr || passErr) { setErrors({ email: emailErr, password: passErr }); return }

    setLoading(true)
    setErrors({})
    const res = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    })

    if (!res.ok) {
      const body = await res.json().catch(() => ({}))
      setErrors({ api: body.detail ?? t('invalidCredentials') })
      setLoading(false)
      return
    }

    const { role, accountState } = await res.json() as { role: string; accountState: string }
    if (accountState === 'RequiresPasswordChange') { router.push(`/${locale}/change-password`); return }
    router.push(role === 'Landlord' ? `/${locale}/dashboard` : `/${locale}/billing`)
  }

  // ... render form with email field, PasswordInput, api error, submit button
}
```

### PasswordInput Component Pattern

```tsx
// src/components/ui/PasswordInput.tsx
'use client'
import { useState } from 'react'

interface PasswordInputProps {
  id: string
  name: string
  value: string
  onChange: (v: string) => void
  onBlur?: () => void
  placeholder?: string
  error?: string
}

export function PasswordInput({ id, name, value, onChange, onBlur, placeholder, error }: PasswordInputProps) {
  const [show, setShow] = useState(false)
  const errorId = `${id}-error`

  return (
    <div className="relative">
      <input
        id={id} name={name} type={show ? 'text' : 'password'}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        onBlur={onBlur}
        placeholder={placeholder}
        aria-invalid={!!error}
        aria-describedby={error ? errorId : undefined}
        className="w-full pr-10 ..."
      />
      <button
        type="button"
        onClick={() => setShow(!show)}
        className="absolute right-3 top-1/2 -translate-y-1/2"
        aria-label={show ? 'Hide password' : 'Show password'}
      >
        {show ? <EyeOffIcon /> : <EyeIcon />}
      </button>
      {error && <p id={errorId} className="text-red-600 text-sm mt-1">{error}</p>}
    </div>
  )
}
```
*Use inline SVGs for EyeIcon / EyeOffIcon (no additional icon package). Story 1.6 will standardize icons.*

### Language Toggle — next-intl Locale Switching

```tsx
// src/components/ui/LanguageToggle.tsx
'use client'
import { useLocale } from 'next-intl'
import { useRouter, usePathname } from 'next/navigation'

export function LanguageToggle() {
  const locale = useLocale()
  const router = useRouter()
  const pathname = usePathname()

  const switchLocale = (newLocale: 'bg' | 'en') => {
    if (newLocale === locale) return
    const segments = pathname.split('/')
    segments[1] = newLocale   // pathname is /{locale}/... — replace segment 1
    router.push(segments.join('/'))
  }

  return (
    <div className="flex gap-2" role="group" aria-label="Language">
      <button onClick={() => switchLocale('bg')} aria-pressed={locale === 'bg'}
        className={locale === 'bg' ? 'font-semibold' : 'text-gray-500'}>BG</button>
      <button onClick={() => switchLocale('en')} aria-pressed={locale === 'en'}
        className={locale === 'en' ? 'font-semibold' : 'text-gray-500'}>EN</button>
    </div>
  )
}
```

Note: `useLocale()` and client-side navigation from `next/navigation` work in Client Components within the `[locale]` App Router tree. The `<html lang>` attribute in `[locale]/layout.tsx` is already derived from the URL locale, so it updates automatically when the route changes.

### Forced Password Change — API Design

The `POST /api/auth/forced-change-password` endpoint (Task 1) must:
- Be protected by the standard JWT middleware (user must be authenticated)
- Additionally guard: if `account_state !== 'RequiresPasswordChange'` → return 403 ProblemDetails
- Use `RemovePasswordAsync` + `AddPasswordAsync` to bypass current-password requirement
- Update `user.AccountState = AccountState.Active` and `user.TokenValidFrom = DateTime.UtcNow` before saving
- Issue and return a new JWT (call existing `AuthService.GenerateJwtAsync`) so the BFF can update the cookie and the session remains valid

### AppHeader Layout Integration

```tsx
// Update (landlord)/layout.tsx — add header above children
import { AppHeader } from '@/components/ui/AppHeader'

// ... existing auth guard logic ...
return (
  <>
    <AppHeader />
    <main id="main-content">{children}</main>
  </>
)
```

Same for `(tenant)/layout.tsx`.

Keep `AppHeader` minimal for now (Story 1.6 applies design tokens). It just needs: `<LanguageToggle />` and a logout button that calls `POST /api/auth/logout` (fetch, then `router.push('/[locale]/login')`). Logout button in `AppHeader` must be a Client Component since it uses `onClick`.

### Existing API Auth Routes (Landlord-scoped Entities — for reference)

No new landlord-scoped entities are created in this story. This story is pure auth + UI.

### Validation Rules (client-side, consistent with API)

| Field | Rule |
|---|---|
| email | Non-empty, regex `/^[^\s@]+@[^\s@]+\.[^\s@]+$/` |
| password (login) | Non-empty |
| newPassword | ≥ 8 chars, at least 1 letter (`/[a-zA-Z]/`), at least 1 digit (`/[0-9]/`) |
| currentPassword | Non-empty |

Validate on blur. Re-validate on submit before calling API. Blur-first means no red highlighting before the user has interacted with a field.

### Testing Strategy

**BFF handler tests** — same pattern as Story 1.4:
```ts
// route.test.ts
import { POST } from './route'
vi.mock('next/headers', () => ({ cookies: vi.fn().mockResolvedValue({ get: () => ({ value: 'mock.jwt.token' }), set: vi.fn() }) }))
global.fetch = vi.fn()
```

**Login page tests:**
```tsx
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
vi.mock('next-intl', () => ({ useLocale: () => 'bg', useTranslations: () => (key: string) => key }))
vi.mock('next/navigation', () => ({ useRouter: () => ({ push: vi.fn() }) }))
global.fetch = vi.fn()

// Test: invalid email shows error on blur
// Test: submit calls /api/auth/login with correct body
// Test: role=Landlord → redirects to /bg/dashboard
// Test: role=Tenant → redirects to /bg/billing
// Test: RequiresPasswordChange → redirects to /bg/change-password
// Test: 401 response → shows inline error
```

**Auth guard layout tests** — already exist in `(landlord)/layout.test.ts` and `(tenant)/layout.test.ts`. Verify RequiresPasswordChange redirect still tests correctly (it was patched in Story 1.4 review).

### File Structure

```
renteasy-api/Controllers/
└── AuthController.cs              ← UPDATE: add forced-change-password, update change-password response

renteasy-web/src/
├── app/
│   ├── api/auth/
│   │   ├── change-password/
│   │   │   ├── route.ts          ← NEW (BFF, forwards { currentPassword, newPassword })
│   │   │   └── route.test.ts     ← NEW
│   │   └── forced-change-password/
│   │       ├── route.ts          ← NEW (BFF, forwards { newPassword }, issues new cookie)
│   │       └── route.test.ts     ← NEW
│   └── [locale]/
│       ├── (auth)/
│       │   ├── login/
│       │   │   ├── page.tsx      ← NEW (Client Component)
│       │   │   └── page.test.tsx ← NEW
│       │   └── change-password/
│       │       ├── page.tsx      ← NEW (Client Component, single new-password field)
│       │       └── page.test.tsx ← NEW
│       ├── (landlord)/
│       │   ├── layout.tsx        ← UPDATE: wrap children with <AppHeader />
│       │   ├── dashboard/
│       │   │   └── page.tsx      ← NEW (stub — "Landlord Dashboard" placeholder)
│       │   └── settings/
│       │       └── page.tsx      ← NEW (renders <PasswordChangeForm />)
│       └── (tenant)/
│           ├── layout.tsx        ← UPDATE: wrap children with <AppHeader />
│           ├── billing/
│           │   └── page.tsx      ← NEW (stub — "Billing" placeholder)
│           └── settings/
│               └── page.tsx      ← NEW (renders <PasswordChangeForm />)
├── components/ui/
│   ├── PasswordInput.tsx         ← NEW (show/hide, aria-describedby for errors)
│   ├── PasswordInput.test.tsx    ← NEW
│   ├── LanguageToggle.tsx        ← NEW (Client Component, useLocale + useRouter)
│   ├── LanguageToggle.test.tsx   ← NEW
│   ├── AppHeader.tsx             ← NEW (LanguageToggle + logout)
│   └── PasswordChangeForm.tsx    ← NEW (Client Component, current + new password)
│   └── PasswordChangeForm.test.tsx ← NEW
└── messages/
    ├── bg.json                   ← UPDATE (add auth namespace)
    └── en.json                   ← UPDATE (add auth namespace)
```

### Project Structure Notes

- `components/ui/` — shared UI components (not role-specific). Correct location for PasswordInput, LanguageToggle, AppHeader, PasswordChangeForm.
- `(auth)` route group has no layout — intentional. Pages inside are publicly reachable (no JWT required). The BFF handlers protect the actual password change operations.
- `(landlord)/dashboard/page.tsx` and `(tenant)/billing/page.tsx` are stubs. Story 2.3 and Story 3.4 will implement them.
- No new EF Core entities or migrations in this story.

### References

- [Source: _bmad-output/planning-artifacts/epics.md — Story 1.5 acceptance criteria]
- [Source: _bmad-output/planning-artifacts/ux-design-specification.md — Experience Principles, Form Design, Forced password change UX]
- [Source: _bmad-output/implementation-artifacts/1-4-nextjs-bff-auth-layer-and-i18n-routing.md — BFF patterns, cookie options, JWT decode, Next.js 16 breaking changes]
- [Source: _bmad-output/implementation-artifacts/1-3-authentication-api.md — JWT claim names, AuthService, AccountState enum, change-password endpoint]
- [Source: _bmad-output/implementation-artifacts/deferred-work.md — Language toggle deferred from 1.4]
- [Source: renteasy-web/src/app/api/auth/login/route.ts — BFF login pattern to replicate in new handlers]
- [Source: renteasy-web/src/app/[locale]/(landlord)/layout.tsx — existing auth guard with RequiresPasswordChange redirect]

### Review Findings

- [x] [Review][Decision] Deployment config removed — confirmed intentional: migrating from Azure Static Web Apps to Vercel; `output: 'standalone'` not needed for Vercel, `web-deploy.yml` intentionally deleted
- [x] [Review][Patch] Password-less account corruption if `AddPasswordAsync` fails after `RemovePasswordAsync` succeeds — fixed: pre-validate via Identity's `PasswordValidators` before removing; also moved `TokenValidFrom`/`AccountState` assignments before `AddPasswordAsync` to eliminate redundant `UpdateAsync` call [`AuthService.cs:ForcedChangePasswordAsync`]
- [x] [Review][Patch] Double `UpdateAsync` in `ForcedChangePasswordAsync` — fixed: see above [`AuthService.cs`]
- [x] [Review][Patch] BFF route handlers throw at module-level if `RENTEASY_API_URL` is missing — fixed: moved check inside handler, returns 500 at request time [`forced-change-password/route.ts`, `change-password/route.ts`]
- [x] [Review][Patch] Unhandled `fetch` network errors in login/change-password pages and `PasswordChangeForm` — fixed: wrapped fetch in try/catch, shows `common.error` on network failure [`login/page.tsx`, `change-password/page.tsx`, `PasswordChangeForm.tsx`]
- [x] [Review][Patch] `setLoading(false)` not called on successful navigation — fixed: called before `router.push` on success path [`login/page.tsx`, `change-password/page.tsx`]
- [x] [Review][Patch] Settings page does not check `account_state` — fixed: parses payload, redirects `RequiresPasswordChange` → change-password, `Expired` → expired [`settings/page.tsx`]
- [x] [Review][Patch] `t('common.error')` resolves to non-existent `auth.common.error` key — fixed: added `tCommon = useTranslations('common')` and use `tCommon('error')` [`change-password/page.tsx`, `PasswordChangeForm.tsx`]
- [x] [Review][Patch] `t('common.loading', { ns: 'common' })` uses unsupported next-intl API — fixed: use `tCommon('loading')` [`login/page.tsx`]
- [x] [Review][Patch] `PasswordInput` show/hide aria-labels hardcoded English — fixed: added optional `showPasswordLabel`/`hidePasswordLabel` props; callers pass translated strings [`PasswordInput.tsx`, `login/page.tsx`, `change-password/page.tsx`, `PasswordChangeForm.tsx`]
- [x] [Review][Patch] Settings page `<h1>` hardcoded English — fixed: added `settings.title` key to messages, use `getTranslations` [`settings/page.tsx`, `en.json`, `bg.json`]
- [x] [Review][Patch] Loading text hardcoded `'...'` — fixed: use `tCommon('loading')` [`change-password/page.tsx`, `PasswordChangeForm.tsx`]
- [x] [Review][Patch] `AppHeader.tsx` missing co-located test file — fixed: created `AppHeader.test.tsx` with render, logout, and error-resilience tests [`AppHeader.test.tsx`]
- [x] [Review][Patch] `settings/page.tsx` missing co-located test file — fixed: created `settings/page.test.ts` covering all redirect branches [`settings/page.test.ts`]
- [x] [Review][Patch] Missing `account_state` claim in JWT returns 403 instead of 401 — fixed: null claim check returns 401 before the RequiresPasswordChange check [`AuthController.cs`]
- [x] [Review][Patch] `RemovePasswordAsync` failure path not tested — fixed: added `ForcedChangePasswordAsync_PasswordValidationFails_ReturnsFailureWithoutRemovingPassword` test [`ForcedChangePasswordTests.cs`]
- [x] [Review][Patch] `AppHeader.handleLogout` has no try/catch — fixed: fetch wrapped in try/catch; redirect fires regardless so user is never stuck [`AppHeader.tsx`]
- [x] [Review][Defer] Stub pages (dashboard, billing) have hardcoded English strings — Story 2.3 and 3.4 will implement; deferred, pre-existing [`dashboard/page.tsx`, `billing/page.tsx`]
- [x] [Review][Defer] `RequiresPasswordChangeMiddleware` also whitelists regular `change-password` for `RequiresPasswordChange` users — pre-existing behavior from Story 1.4, not introduced by this diff; deferred, pre-existing
- [x] [Review][Defer] BFF handlers forward raw JSON body to API — API validates; defense-in-depth concern only; deferred, pre-existing
- [x] [Review][Defer] Settings/layout JWT parsing without signature verification — consistent with Story 1.4 pattern; deferred, pre-existing
- [x] [Review][Defer] Duplicate password validation in controller and Identity validators — intentional: controller rejects invalid passwords before `RemovePasswordAsync` runs; deferred, pre-existing

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

None.

### Completion Notes List

- Implemented `POST /api/auth/forced-change-password` API endpoint using `RemovePasswordAsync` + `AddPasswordAsync` pattern to bypass current-password requirement for temp-password accounts.
- Updated `ChangePasswordResponse` DTO and `GenerateNewTokenAsync` to return `{ token, role, accountState }` (was just `token`) for both change-password endpoints.
- Updated `RequiresPasswordChangeMiddleware` to also allow `POST /api/auth/forced-change-password` (previously only `POST /api/auth/change-password` was whitelisted).
- All BFF route handlers follow the established pattern from Story 1.4 (async cookies, Bearer forwarding, identical cookie options).
- Settings page placed at `/[locale]/settings` (shared, outside role groups) rather than duplicated in `(landlord)` and `(tenant)` — Next.js does not allow two route groups to resolve to the same URL path.
- `AppHeader` implemented as a Client Component (not Server) because it calls `useLocale()` and `useRouter()` transitively via `LanguageToggle`.
- Test cleanup: vitest module cache means `mockClear()` doesn't clear queued `mockResolvedValueOnce` values — used `mockReset()` instead, and added explicit `afterEach(cleanup)` for React component tests.
- 27 API tests + 68 web tests, all green. Build and lint clean.

### File List

renteasy-api/Controllers/AuthController.cs
renteasy-api/Application/DTOs/Auth/ForcedChangePasswordRequest.cs
renteasy-api/Application/DTOs/Auth/ChangePasswordResponse.cs
renteasy-api/Application/Services/AuthService.cs
renteasy-api/Common/Middleware/RequiresPasswordChangeMiddleware.cs
renteasy-api.Tests/Application/Services/ForcedChangePasswordTests.cs
renteasy-web/src/app/api/auth/forced-change-password/route.ts
renteasy-web/src/app/api/auth/forced-change-password/route.test.ts
renteasy-web/src/app/api/auth/change-password/route.ts
renteasy-web/src/app/api/auth/change-password/route.test.ts
renteasy-web/src/app/[locale]/(auth)/login/page.tsx
renteasy-web/src/app/[locale]/(auth)/login/page.test.tsx
renteasy-web/src/app/[locale]/(auth)/change-password/page.tsx
renteasy-web/src/app/[locale]/(auth)/change-password/page.test.tsx
renteasy-web/src/app/[locale]/(landlord)/layout.tsx
renteasy-web/src/app/[locale]/(landlord)/dashboard/page.tsx
renteasy-web/src/app/[locale]/(tenant)/layout.tsx
renteasy-web/src/app/[locale]/(tenant)/billing/page.tsx
renteasy-web/src/app/[locale]/settings/page.tsx
renteasy-web/src/components/ui/PasswordInput.tsx
renteasy-web/src/components/ui/PasswordInput.test.tsx
renteasy-web/src/components/ui/LanguageToggle.tsx
renteasy-web/src/components/ui/LanguageToggle.test.tsx
renteasy-web/src/components/ui/AppHeader.tsx
renteasy-web/src/components/ui/PasswordChangeForm.tsx
renteasy-web/src/components/ui/PasswordChangeForm.test.tsx
renteasy-web/src/messages/bg.json
renteasy-web/src/messages/en.json

### Change Log

- 2026-04-13: Story 1.5 implemented — login UI, forced password change flow, account settings, language toggle, app header, BFF handlers, i18n keys. All 95 tests pass.
