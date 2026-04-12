import { describe, expect, it, vi, beforeEach } from 'vitest'

const mockFetch = vi.fn()
global.fetch = mockFetch

const mockCookiesGet = vi.fn()
vi.mock('next/headers', () => ({
  cookies: vi.fn().mockResolvedValue({ get: mockCookiesGet }),
}))

describe('apiRequest', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    process.env.RENTEASY_API_URL = 'http://localhost:5164'
  })

  it('adds Bearer token from jwt cookie to the Authorization header', async () => {
    mockCookiesGet.mockReturnValue({ value: 'test-jwt-token' })
    mockFetch.mockResolvedValueOnce(new Response('{"id":"1"}', { status: 200 }))

    const { apiRequest } = await import('./api')
    const response = await apiRequest('/api/properties')

    expect(mockFetch).toHaveBeenCalledWith(
      'http://localhost:5164/api/properties',
      expect.objectContaining({
        headers: expect.objectContaining({
          Authorization: 'Bearer test-jwt-token',
          'Content-Type': 'application/json',
        }),
      })
    )
    expect(response.status).toBe(200)
  })

  it('omits Authorization header when jwt cookie is absent', async () => {
    mockCookiesGet.mockReturnValue(undefined)
    mockFetch.mockResolvedValueOnce(new Response('{}', { status: 401 }))

    const { apiRequest } = await import('./api')
    await apiRequest('/api/properties')

    const callArgs = mockFetch.mock.calls[0][1] as RequestInit & { headers: Record<string, string> }
    expect(callArgs.headers.Authorization).toBeUndefined()
  })

  it('returns raw Response for callers to parse', async () => {
    mockCookiesGet.mockReturnValue({ value: 'tok' })
    const mockResponse = new Response('{"data":"value"}', { status: 200 })
    mockFetch.mockResolvedValueOnce(mockResponse)

    const { apiRequest } = await import('./api')
    const result = await apiRequest('/api/tenancies/uuid')

    expect(result).toBe(mockResponse)
  })

  it('merges caller-provided init options with auth headers', async () => {
    mockCookiesGet.mockReturnValue({ value: 'tok' })
    mockFetch.mockResolvedValueOnce(new Response('{}', { status: 201 }))

    const { apiRequest } = await import('./api')
    await apiRequest('/api/properties', { method: 'POST', body: JSON.stringify({ name: 'Test' }) })

    expect(mockFetch).toHaveBeenCalledWith(
      'http://localhost:5164/api/properties',
      expect.objectContaining({
        method: 'POST',
        body: JSON.stringify({ name: 'Test' }),
      })
    )
  })
})
