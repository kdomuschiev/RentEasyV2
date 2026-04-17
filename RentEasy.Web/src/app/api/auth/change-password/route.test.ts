import { describe, expect, it, vi, beforeEach } from 'vitest'

const mockCookieGet = vi.fn()
const mockCookieSet = vi.fn()

vi.mock('next/headers', () => ({
  cookies: vi.fn().mockResolvedValue({ get: mockCookieGet, set: mockCookieSet }),
}))

const mockFetch = vi.fn()
global.fetch = mockFetch

describe('POST /api/auth/change-password', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    process.env.RENTEASY_API_URL = 'http://localhost:5164'
  })

  it('returns 401 when jwt cookie is absent', async () => {
    mockCookieGet.mockReturnValue(undefined)

    const { POST } = await import('./route')
    const response = await POST(new Request('http://localhost:3000/api/auth/change-password', {
      method: 'POST',
      body: JSON.stringify({ currentPassword: 'old', newPassword: 'NewPassword1' }),
      headers: { 'Content-Type': 'application/json' },
    }))

    expect(response.status).toBe(401)
    expect(mockFetch).not.toHaveBeenCalled()
  })

  it('sets new jwt cookie and returns ok:true on success', async () => {
    mockCookieGet.mockReturnValue({ value: 'mock.jwt.token' })
    mockFetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => ({ token: 'new.jwt.token', role: 'Landlord', accountState: 'Active' }),
    })

    const { POST } = await import('./route')
    const response = await POST(new Request('http://localhost:3000/api/auth/change-password', {
      method: 'POST',
      body: JSON.stringify({ currentPassword: 'OldPassword1', newPassword: 'NewPassword1' }),
      headers: { 'Content-Type': 'application/json' },
    }))

    const body = await response.json()
    expect(response.status).toBe(200)
    expect(body).toEqual({ ok: true })
    expect(mockFetch).toHaveBeenCalledWith(
      'http://localhost:5164/api/auth/change-password',
      expect.objectContaining({
        method: 'POST',
        headers: expect.objectContaining({ Authorization: 'Bearer mock.jwt.token' }),
      })
    )
  })

  it('forwards non-200 API responses unchanged', async () => {
    mockCookieGet.mockReturnValue({ value: 'mock.jwt.token' })
    const problemDetails = {
      type: 'https://tools.ietf.org/html/rfc7807',
      title: 'Bad Request',
      status: 400,
      detail: 'Incorrect password.',
    }
    mockFetch.mockResolvedValueOnce({
      ok: false,
      status: 400,
      json: async () => problemDetails,
    })

    const { POST } = await import('./route')
    const response = await POST(new Request('http://localhost:3000/api/auth/change-password', {
      method: 'POST',
      body: JSON.stringify({ currentPassword: 'wrong', newPassword: 'NewPassword1' }),
      headers: { 'Content-Type': 'application/json' },
    }))

    const body = await response.json()
    expect(response.status).toBe(400)
    expect(body).toEqual(problemDetails)
  })

  it('returns 400 when request body is invalid JSON', async () => {
    mockCookieGet.mockReturnValue({ value: 'mock.jwt.token' })

    const { POST } = await import('./route')
    const response = await POST(new Request('http://localhost:3000/api/auth/change-password', {
      method: 'POST',
      body: 'not-json',
      headers: { 'Content-Type': 'application/json' },
    }))

    expect(response.status).toBe(400)
    expect(mockFetch).not.toHaveBeenCalled()
  })
})
