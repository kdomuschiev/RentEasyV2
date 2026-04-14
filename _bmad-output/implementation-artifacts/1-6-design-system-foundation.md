# Story 1.6: Design System Foundation

Status: review

## Story

As a developer,
I want the complete design system established — tokens, typography, shadcn/ui theming, and accessibility baseline,
So that all subsequent UI stories can be built on a consistent, accessible visual foundation without repeating setup work.

## Acceptance Criteria

**AC1 — Colour tokens**
**Given** the Tailwind config is set up
**When** CSS custom properties are inspected on any page
**Then** all 11 colour tokens are defined as CSS variables:
- `--color-bg: #F8F4EE`
- `--color-surface: #FFFFFF`
- `--color-primary: #4A6172`
- `--color-accent: #C8952A`
- `--color-oak: #C4955A`
- `--color-text: #1E1E1E`
- `--color-muted: #6B7280`
- `--color-success: #3D7A5F`
- `--color-pending: #B87A1A`
- `--color-error: #C0392B`
- `--color-stone: #3A3D42`

**AC2 — Typography**
**Given** the typography system is configured
**When** Inter and Playfair Display font files are loaded
**Then** both include their Cyrillic character subsets (`['latin', 'cyrillic']`)
**And** Inter is set as the default `font-sans` applied globally
**And** Playfair Display is available as `font-display` for showcase page use
**And** Geist Sans and Geist Mono (scaffolding defaults) are removed

**AC3 — shadcn/ui components installed**
**Given** shadcn/ui components are generated into `src/components/ui/`
**When** Button, Input, Form, Tabs, Badge, Dialog, AlertDialog, Sheet, Skeleton, and Separator are installed
**Then** their CSS variable overrides apply the RentEasy colour tokens:
- Primary CTA button → `--color-accent` (`#C8952A`)
- Focus rings → `--color-primary` (`#4A6172`)

**AC4 — Skeleton screens**
**Given** any page component that fetches data
**When** data is loading
**Then** a `{ScreenName}Skeleton` component renders (never a blank white screen)
**And** the pattern is established for all future stories via a `DashboardSkeleton` and `BillingSkeleton` stub on the existing dashboard and billing placeholder pages

**AC5 — Focus ring**
**Given** any interactive element (button, link, input, tab)
**When** it receives keyboard focus via Tab key
**Then** a visible focus ring renders using `focus-visible:ring-2 focus-visible:ring-[#4A6172]`
**And** `outline: none` is never applied without a replacement focus indicator

**AC6 — Skip-to-content**
**Given** any page renders
**When** the DOM is inspected
**Then** a skip-to-content `<a>` link is the first focusable element, pointing to `#main-content`
**And** it is visually hidden until focused (`sr-only focus:not-sr-only`)

**AC7 — Tap targets**
**Given** any interactive element
**When** its computed tap target is measured
**Then** it is at least 44×44px (via `min-h-[44px]` and appropriate padding on buttons/inputs)

## Tasks / Subtasks

- [x] **Task 1: Replace fonts — Inter + Playfair Display with Cyrillic** (AC: 2)
  - [x] In `src/app/[locale]/layout.tsx`: remove `Geist` and `Geist_Mono` imports
  - [x] Import `Inter` and `Playfair_Display` from `next/font/google` with `subsets: ['latin', 'cyrillic']`
  - [x] Inter: `variable: '--font-inter'`; Playfair Display: `variable: '--font-playfair'`
  - [x] Update `<html>` className to use `${inter.variable} ${playfair.variable}`
  - [x] In `globals.css` `@theme`: replace `--font-sans: var(--font-geist-sans)` with `--font-sans: var(--font-inter)` and add `--font-display: var(--font-playfair)`
  - [x] Remove `--font-mono` entry (not needed for this app)

- [x] **Task 2: Define RentEasy colour tokens in globals.css** (AC: 1)
  - [x] Replace the generic `--background`/`--foreground` vars with the 11 RentEasy tokens in `:root`
  - [x] In `@theme`: map Tailwind colour utilities to the tokens (see Dev Notes for exact CSS)
  - [x] Remove dark mode `@media (prefers-color-scheme: dark)` block — app is light-only in V1
  - [x] Set `body` background to `var(--color-bg)` and color to `var(--color-text)`

- [x] **Task 3: Install and initialise shadcn/ui** (AC: 3)
  - [x] Run: `npx shadcn@latest init` from `renteasy-web/` — select TypeScript, Tailwind v4, `src/` dir, `@/` alias
  - [x] This creates `components.json` and adds shadcn CSS variable block to `globals.css`
  - [x] After init: manually remap the shadcn CSS variable block (see Dev Notes) so shadcn's `--primary`, `--destructive`, etc. point to the RentEasy tokens
  - [x] Install components one by one (or batch): `npx shadcn@latest add button input form tabs badge dialog alert-dialog sheet skeleton separator`

- [x] **Task 4: Override shadcn CSS variables to RentEasy tokens** (AC: 3)
  - [x] In `globals.css` `:root`, map shadcn variables to RentEasy tokens (see Dev Notes for exact mapping)
  - [x] Primary action (`--primary`) → `--color-accent` (#C8952A)
  - [x] Destructive (`--destructive`) → `--color-error` (#C0392B)
  - [x] Muted (`--muted`) → `--color-bg` (#F8F4EE)
  - [x] Ring/focus (`--ring`) → `--color-primary` (#4A6172)
  - [x] Background (`--background`) → `--color-surface` (#FFFFFF)

- [x] **Task 5: Add skip-to-content links** (AC: 6)
  - [x] In `src/app/[locale]/layout.tsx`: add `<a href="#main-content" ...>Skip to content</a>` as FIRST child of `<body>`, before `<NextIntlClientProvider>`
  - [x] `#main-content` already exists on `<main>` in `(landlord)/layout.tsx` and `(tenant)/layout.tsx` — verify

- [x] **Task 6: Apply design tokens to existing components** (AC: 1, 5, 7)
  - [x] `src/components/ui/PasswordInput.tsx`: replace raw `border rounded px-3 py-2` with token-based classes; add `focus-visible:ring-2 focus-visible:ring-[#4A6172]`; ensure `min-h-[44px]` on input
  - [x] `src/components/ui/AppHeader.tsx`: apply `bg-[#4A6172]` or surface colour, token-based text; ensure logout button `min-h-[44px]`
  - [x] `src/components/ui/LanguageToggle.tsx`: apply token-based active/inactive styling; ensure buttons `min-h-[44px]`
  - [x] `src/components/ui/PasswordChangeForm.tsx`: apply token-based styles
  - [x] `src/app/[locale]/(auth)/login/page.tsx`: replace `bg-blue-600` button with `bg-[--color-accent]`; apply token classes
  - [x] `src/app/[locale]/(auth)/change-password/page.tsx`: same as login page
  - [x] Validation error text: replace `text-red-600` with `text-[--color-error]` everywhere

- [x] **Task 7: Skeleton screen pattern** (AC: 4)
  - [x] Shadcn `Skeleton` component is now installed — use it as the primitive
  - [x] Create `src/app/[locale]/(landlord)/dashboard/DashboardSkeleton.tsx`
  - [x] Create `src/app/[locale]/(tenant)/billing/BillingSkeleton.tsx`
  - [x] Update `dashboard/page.tsx` and `billing/page.tsx` stub pages to demonstrate the `{PageName}Skeleton` pattern

- [x] **Task 8: Verify + enforce tap target compliance** (AC: 7)
  - [x] Audit all `<button>` and `<a>` elements in existing components: login page submit, PasswordInput toggle, AppHeader logout, LanguageToggle buttons
  - [x] Add `min-h-[44px]` to any element below threshold
  - [x] The shadcn Button component default height is 40px — override default to `min-h-[44px]`

- [x] **Task 9: Update i18n keys for accessibility** (AC: 6)
  - [x] Add `"skipToContent": "Skip to content"` key to `en.json` and `bg.json` under a `"a11y"` namespace
  - [x] Update skip link in layout to use this key (Server Component — use `getTranslations`)

- [x] **Task 10: Tests and build verification** (AC: all)
  - [x] Update existing component tests that assert className values (may break after token replacement)
  - [x] `npm run test` — all tests pass (76/76)
  - [x] `npm run build` — no errors
  - [x] `npm run lint` — clean
  - [x] Manual check: Tab through login page — skip link appears, focus ring visible on all elements

## Dev Notes

### ⚠️ Critical: Tailwind CSS v4 — CSS-First Configuration

This project uses **Tailwind CSS v4** (`tailwindcss: ^4`). Config is CSS-based — there is NO `tailwind.config.js`. All customisation happens in `globals.css` via `@theme`.

**Current `globals.css` uses:**
```css
@import "tailwindcss";

@theme inline {
  --font-sans: var(--font-geist-sans);
  --font-mono: var(--font-geist-mono);
}
```

**Tailwind v4 token registration pattern** — replace with:
```css
@import "tailwindcss";

:root {
  /* RentEasy colour tokens */
  --color-bg: #F8F4EE;
  --color-surface: #FFFFFF;
  --color-primary: #4A6172;
  --color-accent: #C8952A;
  --color-oak: #C4955A;
  --color-text: #1E1E1E;
  --color-muted: #6B7280;
  --color-success: #3D7A5F;
  --color-pending: #B87A1A;
  --color-error: #C0392B;
  --color-stone: #3A3D42;
}

@theme inline {
  /* Typography */
  --font-sans: var(--font-inter);
  --font-display: var(--font-playfair);

  /* Expose tokens as Tailwind utilities: bg-bg, text-primary, etc. */
  --color-bg: var(--color-bg);
  --color-surface: var(--color-surface);
  --color-primary: var(--color-primary);
  --color-accent: var(--color-accent);
  --color-oak: var(--color-oak);
  --color-text: var(--color-text);
  --color-muted-foreground: var(--color-muted);
  --color-success: var(--color-success);
  --color-pending: var(--color-pending);
  --color-error: var(--color-error);
  --color-stone: var(--color-stone);
}

body {
  background-color: var(--color-bg);
  color: var(--color-text);
}
```

This makes `bg-bg`, `text-primary`, `bg-accent`, `text-error`, etc. available as Tailwind utility classes.

### Font Loading — next/font/google with Cyrillic

Replace Geist with Inter + Playfair Display in `src/app/[locale]/layout.tsx`:

```tsx
import { Inter, Playfair_Display } from 'next/font/google'

const inter = Inter({
  variable: '--font-inter',
  subsets: ['latin', 'cyrillic'],
  display: 'swap',
})

const playfairDisplay = Playfair_Display({
  variable: '--font-playfair',
  subsets: ['latin', 'cyrillic'],
  display: 'swap',
})

// In JSX:
<html lang={locale} className={`${inter.variable} ${playfairDisplay.variable} h-full antialiased`}>
```

`display: 'swap'` prevents invisible text during font load. Both fonts include Cyrillic subset — required for Bulgarian text.

### shadcn/ui — Tailwind v4 Init

shadcn 4.2.0 is available (`npx shadcn@latest`). Run from `renteasy-web/`:

```bash
npx shadcn@latest init
# Select: TypeScript ✓, Tailwind v4 (auto-detected), src/ ✓, @/ alias ✓
```

This creates `components.json` and injects a CSS variable block into `globals.css`. After init, **manually remap** the injected shadcn variables to RentEasy tokens:

```css
/* shadcn variable remapping — add to globals.css :root block */
:root {
  /* ... RentEasy tokens above ... */

  /* shadcn/ui variable mapping → RentEasy tokens */
  --background: var(--color-surface);       /* page background */
  --foreground: var(--color-text);
  --card: var(--color-surface);
  --card-foreground: var(--color-text);
  --popover: var(--color-surface);
  --popover-foreground: var(--color-text);
  --primary: var(--color-accent);           /* CTA buttons → warm gold */
  --primary-foreground: #FFFFFF;
  --secondary: var(--color-bg);             /* secondary surfaces → warm cream */
  --secondary-foreground: var(--color-text);
  --muted: var(--color-bg);
  --muted-foreground: var(--color-muted);
  --accent: var(--color-primary);           /* hover accent → slate blue */
  --accent-foreground: #FFFFFF;
  --destructive: var(--color-error);
  --destructive-foreground: #FFFFFF;
  --border: #E5E0D8;                        /* subtle warm border */
  --input: #E5E0D8;
  --ring: var(--color-primary);             /* focus rings → slate blue */
  --radius: 0.5rem;
}
```

### shadcn Component Install Command

After init:
```bash
npx shadcn@latest add button input form tabs badge dialog alert-dialog sheet skeleton separator
```

Components land in `src/components/ui/`. **Do NOT overwrite existing files** (`PasswordInput.tsx`, `AppHeader.tsx`, `LanguageToggle.tsx`, `PasswordChangeForm.tsx`) — shadcn only generates its own components.

### Skip-to-Content Pattern

```tsx
// In [locale]/layout.tsx — FIRST child of <body>
<a
  href="#main-content"
  className="sr-only focus:not-sr-only focus:absolute focus:top-2 focus:left-2 focus:z-50 focus:px-4 focus:py-2 focus:bg-white focus:text-primary focus:rounded focus:shadow-md"
>
  {t('a11y.skipToContent')}
</a>
```

`#main-content` is already on `<main>` in both `(landlord)/layout.tsx` and `(tenant)/layout.tsx`. The locale layout wraps both, so a single skip link covers all authenticated pages.

### Skeleton Pattern — All Future Stories

Every data-fetching page **must** follow this pattern (NFR-P3):

```tsx
// page.tsx (Server Component)
import { Suspense } from 'react'
import { DashboardSkeleton } from './DashboardSkeleton'

export default function DashboardPage() {
  return (
    <Suspense fallback={<DashboardSkeleton />}>
      <DashboardContent />
    </Suspense>
  )
}

// DashboardSkeleton.tsx
import { Skeleton } from '@/components/ui/skeleton'

export function DashboardSkeleton() {
  return (
    <div className="p-6 space-y-4">
      <Skeleton className="h-8 w-48" />
      <Skeleton className="h-32 w-full" />
      <Skeleton className="h-32 w-full" />
    </div>
  )
}
```

Create stubs for `DashboardSkeleton` and `BillingSkeleton` in this story — not full implementations. Story 2.3 and 3.4 will build real content.

### Focus Ring — Standard Class

Apply to ALL interactive elements throughout the codebase:
```
focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#4A6172] focus-visible:ring-offset-2
```

The shadcn Button component uses `--ring` which maps to `--color-primary` (#4A6172) after our CSS variable override — so shadcn components get the correct focus ring automatically. Custom components (PasswordInput toggle button, LanguageToggle buttons) need the class added manually.

### Tap Target — 44×44px Minimum

Tailwind utility: `min-h-[44px]`

Elements to audit and fix:
| Element | File | Fix |
|---|---|---|
| Login submit button | `login/page.tsx` | Replace with shadcn `Button` or add `min-h-[44px]` |
| Password show/hide toggle | `PasswordInput.tsx` | Add `min-h-[44px] min-w-[44px]` |
| LanguageToggle BG/EN buttons | `LanguageToggle.tsx` | Add `min-h-[44px] px-3` |
| AppHeader logout button | `AppHeader.tsx` | Add `min-h-[44px]` |
| Change password submit | `change-password/page.tsx` | Same as login button |
| PasswordChangeForm submit | `PasswordChangeForm.tsx` | Same |

shadcn Button default height is 40px — add `className="min-h-[44px]"` when used, or override the variant in `components/ui/button.tsx`.

### Existing Components — DO NOT RECREATE

These files exist from Story 1.5 — update them, do not replace:
- `src/components/ui/PasswordInput.tsx`
- `src/components/ui/PasswordInput.test.tsx`
- `src/components/ui/AppHeader.tsx`
- `src/components/ui/AppHeader.test.tsx`
- `src/components/ui/LanguageToggle.tsx`
- `src/components/ui/LanguageToggle.test.tsx`
- `src/components/ui/PasswordChangeForm.tsx`
- `src/components/ui/PasswordChangeForm.test.tsx`

### What shadcn Generates

`npx shadcn@latest add` generates components into `src/components/ui/`. Expected new files:
- `button.tsx`, `input.tsx`, `form.tsx`, `tabs.tsx`, `badge.tsx`
- `dialog.tsx`, `alert-dialog.tsx`, `sheet.tsx`, `skeleton.tsx`, `separator.tsx`

shadcn may also install peer dependencies (`@radix-ui/*`, `class-variance-authority`, `clsx`, `tailwind-merge`, `lucide-react`). These will be added to `package.json` automatically.

### i18n Keys to Add

`en.json`:
```json
{
  "a11y": {
    "skipToContent": "Skip to content"
  }
}
```

`bg.json`:
```json
{
  "a11y": {
    "skipToContent": "Прескочи към съдържанието"
  }
}
```

### Colour Contrast Verification (from UX spec)

WCAG AA requires 4.5:1 for normal text, 3:1 for large text. Verified ratios from UX spec:
- `#1E1E1E` on `#F8F4EE` → 16.5:1 ✅
- `#FFFFFF` on `#C8952A` (accent CTA) → 3.1:1 ✅ (large text / buttons)
- `#FFFFFF` on `#4A6172` (primary) → 4.7:1 ✅
- `#6B7280` on `#F8F4EE` (muted text) → 4.6:1 ✅

### Testing Impact

Existing tests that assert specific `className` values will need updating after token replacement. Key affected files:
- `PasswordInput.test.tsx` — error text class changes from `text-red-600` to `text-[--color-error]`
- `AppHeader.test.tsx` — may assert button classes
- `login/page.test.tsx` — submit button class changes

Run `npm run test` after each component change to catch regressions early.

### References

- [Source: `_bmad-output/planning-artifacts/epics.md` — Story 1.6 acceptance criteria]
- [Source: `_bmad-output/planning-artifacts/ux-design-specification.md` — Design System Foundation, Visual Design Foundation, Colour system, Typography]
- [Source: `_bmad-output/planning-artifacts/architecture.md` — CSS: Tailwind CSS 4.x + shadcn/ui]
- [Source: `_bmad-output/implementation-artifacts/1-5-login-and-authentication-ui.md` — existing components, file structure, Next.js 16 patterns]
- [Source: `renteasy-web/src/app/globals.css` — current Tailwind v4 CSS config to extend]
- [Source: `renteasy-web/src/app/[locale]/layout.tsx` — font and layout entry point to update]
- [Source: `renteasy-web/package.json` — Tailwind v4, no shadcn yet]

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

- shadcn `init` is interactive in latest version — created `components.json` manually and used `npx shadcn@latest add --yes` for non-interactive component installation. This is equivalent: `init` only creates `components.json` + `lib/utils.ts` + injects CSS vars.
- Tasks 2 and 4 were combined: the globals.css was written in final form with both RentEasy tokens and shadcn variable mappings. The `shadcn add` command did not modify globals.css since `components.json` was already present.
- Dependencies `class-variance-authority`, `clsx`, `tailwind-merge`, `lucide-react` were not automatically added to package.json by `shadcn add` — installed separately with `npm install`.

### Completion Notes List

- Replaced Geist fonts with Inter + Playfair Display, both with `['latin', 'cyrillic']` subsets and `display: 'swap'`
- Defined all 11 RentEasy colour tokens in `:root` and exposed as Tailwind utilities via `@theme inline`
- Shadcn CSS variables mapped to RentEasy tokens (no default shadcn colours used)
- Dark mode block removed — app is light-only in V1
- Skip-to-content link added as first child of `<body>` in locale layout, using `getTranslations('a11y')` Server Component pattern
- `#main-content` verified present on `<main>` in both landlord and tenant layouts
- shadcn/ui components installed: button, input, form, tabs, badge, dialog, alert-dialog, sheet, skeleton, separator, label
- shadcn Button default size overridden from `h-10` to `min-h-[44px]` for tap target compliance
- All existing components updated with token-based colours and `min-h-[44px]` on interactive elements
- `text-red-600` replaced with `text-[--color-error]` everywhere; `bg-blue-600` replaced with `bg-[--color-accent]`
- `DashboardSkeleton` and `BillingSkeleton` created as stubs; dashboard and billing pages updated with `<Suspense>` pattern
- `a11y.skipToContent` keys added to both `en.json` and `bg.json`
- All 76 existing tests pass; `npm run build` clean; `npm run lint` clean

### File List

- `renteasy-web/src/app/globals.css` — replaced with RentEasy tokens + shadcn variable mappings
- `renteasy-web/src/app/[locale]/layout.tsx` — replaced fonts, added skip-to-content link
- `renteasy-web/src/components/ui/PasswordInput.tsx` — token-based classes, min-h-[44px], focus ring
- `renteasy-web/src/components/ui/AppHeader.tsx` — bg-[#4A6172], token text, min-h-[44px] logout button
- `renteasy-web/src/components/ui/LanguageToggle.tsx` — token-based styling, min-h-[44px]
- `renteasy-web/src/components/ui/PasswordChangeForm.tsx` — token colours, min-h-[44px] submit
- `renteasy-web/src/app/[locale]/(auth)/login/page.tsx` — token colours, min-h-[44px], focus ring
- `renteasy-web/src/app/[locale]/(auth)/change-password/page.tsx` — token colours, min-h-[44px]
- `renteasy-web/src/app/[locale]/(landlord)/dashboard/page.tsx` — Suspense + DashboardSkeleton pattern
- `renteasy-web/src/app/[locale]/(landlord)/dashboard/DashboardSkeleton.tsx` — new skeleton stub
- `renteasy-web/src/app/[locale]/(tenant)/billing/page.tsx` — Suspense + BillingSkeleton pattern
- `renteasy-web/src/app/[locale]/(tenant)/billing/BillingSkeleton.tsx` — new skeleton stub
- `renteasy-web/src/messages/en.json` — added a11y.skipToContent key
- `renteasy-web/src/messages/bg.json` — added a11y.skipToContent key (Bulgarian)
- `renteasy-web/components.json` — new shadcn/ui configuration file
- `renteasy-web/src/lib/utils.ts` — new cn() utility (required by shadcn components)
- `renteasy-web/src/components/ui/button.tsx` — new shadcn Button (default size min-h-[44px])
- `renteasy-web/src/components/ui/input.tsx` — new shadcn Input
- `renteasy-web/src/components/ui/form.tsx` — new shadcn Form
- `renteasy-web/src/components/ui/tabs.tsx` — new shadcn Tabs
- `renteasy-web/src/components/ui/badge.tsx` — new shadcn Badge
- `renteasy-web/src/components/ui/dialog.tsx` — new shadcn Dialog
- `renteasy-web/src/components/ui/alert-dialog.tsx` — new shadcn AlertDialog
- `renteasy-web/src/components/ui/sheet.tsx` — new shadcn Sheet
- `renteasy-web/src/components/ui/skeleton.tsx` — new shadcn Skeleton
- `renteasy-web/src/components/ui/separator.tsx` — new shadcn Separator
- `renteasy-web/src/components/ui/label.tsx` — new shadcn Label (installed as dependency of form)
- `renteasy-web/package.json` — added class-variance-authority, clsx, tailwind-merge, lucide-react, @radix-ui/* packages

## Change Log

- 2026-04-14: Implemented Story 1.6 — design system foundation. Established 11 RentEasy colour tokens, Inter + Playfair Display fonts with Cyrillic support, installed shadcn/ui component library with full token mapping, added accessibility baseline (skip-to-content, focus rings, 44px tap targets), established skeleton screen pattern.
