import { describe, expect, it, vi, beforeEach } from 'vitest'

const mockCookieDelete = vi.fn()
vi.mock('next/headers', () => ({
  cookies: vi.fn().mockResolvedValue({ delete: mockCookieDelete }),
}))

describe('POST /api/auth/logout', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('deletes the jwt cookie and returns ok', async () => {
    const { POST } = await import('./route')
    const response = await POST()

    expect(response.status).toBe(200)
    const body = await response.json()
    expect(body).toEqual({ ok: true })
    expect(mockCookieDelete).toHaveBeenCalledWith('jwt')
  })
})
