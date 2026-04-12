import { describe, expect, it, vi, beforeEach } from 'vitest'

const mockRedirect = vi.fn()
const mockCookiesGet = vi.fn()

vi.mock('next/navigation', () => ({
  redirect: mockRedirect,
}))

vi.mock('next/headers', () => ({
  cookies: vi.fn().mockResolvedValue({ get: mockCookiesGet }),
}))

function makeToken(payload: object): string {
  return `header.${Buffer.from(JSON.stringify(payload)).toString('base64url')}.sig`
}

describe('TenantLayout auth guard', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('redirects to login when jwt cookie is absent', async () => {
    mockCookiesGet.mockReturnValue(undefined)
    const { default: TenantLayout } = await import('./layout')

    await TenantLayout({
      children: null,
      params: Promise.resolve({ locale: 'bg' }),
    })

    expect(mockRedirect).toHaveBeenCalledWith('/bg/login')
  })

  it('redirects to login when role is not Tenant', async () => {
    mockCookiesGet.mockReturnValue({
      value: makeToken({ role: 'Landlord', account_state: 'Active' }),
    })
    const { default: TenantLayout } = await import('./layout')

    await TenantLayout({
      children: null,
      params: Promise.resolve({ locale: 'en' }),
    })

    expect(mockRedirect).toHaveBeenCalledWith('/en/login')
  })

  it('redirects to expired page when accountState is Expired', async () => {
    mockCookiesGet.mockReturnValue({
      value: makeToken({ role: 'Tenant', account_state: 'Expired' }),
    })
    const { default: TenantLayout } = await import('./layout')

    await TenantLayout({
      children: null,
      params: Promise.resolve({ locale: 'bg' }),
    })

    expect(mockRedirect).toHaveBeenCalledWith('/bg/expired')
  })

  it('allows through when role is Tenant and accountState is Active', async () => {
    mockCookiesGet.mockReturnValue({
      value: makeToken({ role: 'Tenant', account_state: 'Active' }),
    })
    const { default: TenantLayout } = await import('./layout')

    await TenantLayout({
      children: null,
      params: Promise.resolve({ locale: 'bg' }),
    })

    expect(mockRedirect).not.toHaveBeenCalled()
  })

  it('allows through when role is Tenant and accountState is ReadOnly', async () => {
    mockCookiesGet.mockReturnValue({
      value: makeToken({ role: 'Tenant', account_state: 'ReadOnly' }),
    })
    const { default: TenantLayout } = await import('./layout')

    await TenantLayout({
      children: null,
      params: Promise.resolve({ locale: 'bg' }),
    })

    expect(mockRedirect).not.toHaveBeenCalled()
  })
})
