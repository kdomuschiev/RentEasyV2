import { describe, expect, it, vi, beforeEach } from 'vitest'
import { NextRequest } from 'next/server'

// Mock next/headers (cookies)
const mockCookieSet = vi.fn()
vi.mock('next/headers', () => ({
  cookies: vi.fn().mockResolvedValue({ set: mockCookieSet }),
}))

// Mock global fetch
const mockFetch = vi.fn()
global.fetch = mockFetch

describe('POST /api/auth/login', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    process.env.RENTEASY_API_URL = 'http://localhost:5164'
  })

  it('sets jwt cookie and returns role+accountState on successful login', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => ({
        token: 'header.eyJyb2xlIjoiTGFuZGxvcmQiLCJhY2NvdW50X3N0YXRlIjoiQWN0aXZlIn0.sig',
        role: 'Landlord',
        accountState: 'Active',
      }),
    })

    const { POST } = await import('./route')
    const request = new NextRequest('http://localhost:3000/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email: 'test@example.com', password: 'pass' }),
      headers: { 'Content-Type': 'application/json' },
    })

    const response = await POST(request)
    const body = await response.json()

    expect(response.status).toBe(200)
    expect(body).toEqual({ role: 'Landlord', accountState: 'Active' })
    expect(body.token).toBeUndefined()
    expect(mockCookieSet).toHaveBeenCalledWith(
      'jwt',
      'header.eyJyb2xlIjoiTGFuZGxvcmQiLCJhY2NvdW50X3N0YXRlIjoiQWN0aXZlIn0.sig',
      expect.objectContaining({
        httpOnly: true,
        sameSite: 'strict',
        path: '/',
      })
    )
  })

  it('forwards non-200 responses from the API unchanged', async () => {
    const problemDetails = {
      type: 'https://tools.ietf.org/html/rfc7807',
      title: 'Unauthorized',
      status: 401,
      detail: 'Invalid credentials',
    }
    mockFetch.mockResolvedValueOnce({
      ok: false,
      status: 401,
      json: async () => problemDetails,
    })

    const { POST } = await import('./route')
    const request = new NextRequest('http://localhost:3000/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email: 'bad@example.com', password: 'wrong' }),
      headers: { 'Content-Type': 'application/json' },
    })

    const response = await POST(request)
    const body = await response.json()

    expect(response.status).toBe(401)
    expect(body).toEqual(problemDetails)
    expect(mockCookieSet).not.toHaveBeenCalled()
  })
})
