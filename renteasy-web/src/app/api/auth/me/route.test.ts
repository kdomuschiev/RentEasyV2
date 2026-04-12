import { describe, expect, it, vi, beforeEach } from 'vitest'

// JWT with payload: { role: 'Landlord', account_state: 'Active', sub: 'uuid', email: 'a@b.com' }
const VALID_TOKEN =
  'header.' +
  Buffer.from(
    JSON.stringify({ role: 'Landlord', account_state: 'Active', sub: 'uuid-1', email: 'a@b.com' })
  ).toString('base64url') +
  '.sig'

const mockCookiesGet = vi.fn()
vi.mock('next/headers', () => ({
  cookies: vi.fn().mockResolvedValue({ get: mockCookiesGet }),
}))

describe('GET /api/auth/me', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('returns role and accountState from a valid jwt cookie', async () => {
    mockCookiesGet.mockReturnValue({ value: VALID_TOKEN })

    const { GET } = await import('./route')
    const response = await GET()

    expect(response.status).toBe(200)
    const body = await response.json()
    expect(body).toEqual({ role: 'Landlord', accountState: 'Active' })
  })

  it('returns 401 when jwt cookie is absent', async () => {
    mockCookiesGet.mockReturnValue(undefined)

    const { GET } = await import('./route')
    const response = await GET()

    expect(response.status).toBe(401)
    const body = await response.json()
    expect(body).toEqual({ error: 'Unauthenticated' })
  })

  it('returns 401 when jwt cookie is malformed', async () => {
    mockCookiesGet.mockReturnValue({ value: 'not-a-valid-jwt' })

    const { GET } = await import('./route')
    const response = await GET()

    expect(response.status).toBe(401)
    const body = await response.json()
    expect(body).toEqual({ error: 'Unauthenticated' })
  })
})
