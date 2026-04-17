import { describe, expect, it, vi, beforeEach } from 'vitest'

const mockRedirect = vi.fn()
const mockCookieGet = vi.fn()

vi.mock('next/navigation', () => ({
  redirect: mockRedirect,
}))

vi.mock('next/headers', () => ({
  cookies: vi.fn().mockResolvedValue({ get: mockCookieGet }),
}))

vi.mock('next-intl/server', () => ({
  getTranslations: vi.fn().mockResolvedValue((key: string) => key),
}))

vi.mock('@/components/ui/PasswordChangeForm', () => ({
  PasswordChangeForm: () => null,
}))

vi.mock('@/components/ui/AppHeader', () => ({
  AppHeader: () => null,
}))

// Build a minimal valid JWT payload (base64url-encoded JSON)
function buildToken(payload: Record<string, unknown>): string {
  const encoded = Buffer.from(JSON.stringify(payload)).toString('base64url')
  return `header.${encoded}.signature`
}

describe('SettingsPage', () => {
  beforeEach(() => {
    mockRedirect.mockReset()
    mockCookieGet.mockReset()
  })

  it('redirects to login when jwt cookie is absent', async () => {
    mockCookieGet.mockReturnValue(undefined)
    const { default: SettingsPage } = await import('./page')
    await SettingsPage({ params: Promise.resolve({ locale: 'bg' }) }).catch(() => {})
    expect(mockRedirect).toHaveBeenCalledWith('/bg/login')
  })

  it('redirects to login when jwt is malformed', async () => {
    mockCookieGet.mockReturnValue({ value: 'not.a.valid.jwt' })
    const { default: SettingsPage } = await import('./page')
    await SettingsPage({ params: Promise.resolve({ locale: 'bg' }) }).catch(() => {})
    expect(mockRedirect).toHaveBeenCalledWith('/bg/login')
  })

  it('redirects to change-password when accountState is RequiresPasswordChange', async () => {
    mockCookieGet.mockReturnValue({ value: buildToken({ account_state: 'RequiresPasswordChange' }) })
    const { default: SettingsPage } = await import('./page')
    await SettingsPage({ params: Promise.resolve({ locale: 'bg' }) }).catch(() => {})
    expect(mockRedirect).toHaveBeenCalledWith('/bg/change-password')
  })

  it('redirects to expired page when accountState is Expired', async () => {
    mockCookieGet.mockReturnValue({ value: buildToken({ account_state: 'Expired' }) })
    const { default: SettingsPage } = await import('./page')
    await SettingsPage({ params: Promise.resolve({ locale: 'bg' }) }).catch(() => {})
    expect(mockRedirect).toHaveBeenCalledWith('/bg/expired')
  })

  it('renders page without redirect when token is valid and active', async () => {
    mockCookieGet.mockReturnValue({ value: buildToken({ account_state: 'Active', role: 'Tenant' }) })
    const { default: SettingsPage } = await import('./page')
    await SettingsPage({ params: Promise.resolve({ locale: 'bg' }) })
    expect(mockRedirect).not.toHaveBeenCalled()
  })
})
