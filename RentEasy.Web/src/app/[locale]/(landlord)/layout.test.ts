import { describe, expect, it, vi, beforeEach } from 'vitest'

const mockRedirect = vi.fn()
const mockCookiesGet = vi.fn()

vi.mock('next/navigation', () => ({
  redirect: mockRedirect,
}))

vi.mock('next/headers', () => ({
  cookies: vi.fn().mockResolvedValue({ get: mockCookiesGet }),
}))

// JWT payload helpers
function makeToken(payload: object): string {
  return `header.${Buffer.from(JSON.stringify(payload)).toString('base64url')}.sig`
}

describe('LandlordLayout auth guard', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('redirects to login when jwt cookie is absent', async () => {
    mockCookiesGet.mockReturnValue(undefined)
    const { default: LandlordLayout } = await import('./layout')

    await LandlordLayout({
      children: null,
      params: Promise.resolve({ locale: 'bg' }),
    })

    expect(mockRedirect).toHaveBeenCalledWith('/bg/login')
  })

  it('redirects to login when role is not Landlord', async () => {
    mockCookiesGet.mockReturnValue({
      value: makeToken({ role: 'Tenant', account_state: 'Active' }),
    })
    const { default: LandlordLayout } = await import('./layout')

    await LandlordLayout({
      children: null,
      params: Promise.resolve({ locale: 'en' }),
    })

    expect(mockRedirect).toHaveBeenCalledWith('/en/login')
  })

  it('allows through when role is Landlord', async () => {
    mockCookiesGet.mockReturnValue({
      value: makeToken({ role: 'Landlord', account_state: 'Active' }),
    })
    const { default: LandlordLayout } = await import('./layout')

    await LandlordLayout({
      children: null,
      params: Promise.resolve({ locale: 'bg' }),
    })

    expect(mockRedirect).not.toHaveBeenCalled()
  })

  it('redirects to change-password when accountState is RequiresPasswordChange', async () => {
    mockCookiesGet.mockReturnValue({
      value: makeToken({ role: 'Landlord', account_state: 'RequiresPasswordChange' }),
    })
    const { default: LandlordLayout } = await import('./layout')

    await LandlordLayout({
      children: null,
      params: Promise.resolve({ locale: 'bg' }),
    })

    expect(mockRedirect).toHaveBeenCalledWith('/bg/change-password')
  })
})
