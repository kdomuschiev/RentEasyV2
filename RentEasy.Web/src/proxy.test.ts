import { describe, expect, it, vi, beforeAll } from 'vitest'
import { unstable_doesMiddlewareMatch } from 'next/experimental/testing/server'

// Mock next-intl and next/server to avoid ESM resolution issues in test environment
vi.mock('next-intl/middleware', () => ({
  default: () => () => new Response(),
}))
vi.mock('next-intl/routing', () => ({
  defineRouting: (config: unknown) => config,
}))
vi.mock('next/server', () => ({
  NextResponse: { next: () => new Response() },
}))

describe('proxy matcher config', () => {
  let config: { matcher: string[] }

  beforeAll(async () => {
    const proxyModule = await import('./proxy')
    config = proxyModule.config
  })

  it('matches locale page routes', () => {
    expect(unstable_doesMiddlewareMatch({ config, url: '/bg/dashboard' })).toBe(true)
  })

  it('matches root path for locale redirect', () => {
    expect(unstable_doesMiddlewareMatch({ config, url: '/' })).toBe(true)
  })

  it('does not match /api routes', () => {
    expect(unstable_doesMiddlewareMatch({ config, url: '/api/auth/login' })).toBe(false)
  })

  it('does not match _next/static assets', () => {
    expect(unstable_doesMiddlewareMatch({ config, url: '/_next/static/chunks/main.js' })).toBe(false)
  })

  it('does not match _next/image routes', () => {
    expect(unstable_doesMiddlewareMatch({ config, url: '/_next/image?url=foo' })).toBe(false)
  })
})
