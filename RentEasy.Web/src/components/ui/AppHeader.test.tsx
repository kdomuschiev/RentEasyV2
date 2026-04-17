import { describe, expect, it, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, fireEvent, waitFor, cleanup } from '@testing-library/react'

const mockPush = vi.fn()

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: mockPush }),
  usePathname: () => '/bg/dashboard',
}))

vi.mock('next-intl', () => ({
  useLocale: () => 'bg',
  useTranslations: () => (key: string) => key,
}))

const mockFetch = vi.fn()
global.fetch = mockFetch

describe('AppHeader', () => {
  beforeEach(() => {
    mockPush.mockReset()
    mockFetch.mockReset()
  })

  afterEach(cleanup)

  it('renders logout button and language toggle', async () => {
    const { AppHeader } = await import('./AppHeader')
    render(<AppHeader />)
    expect(document.querySelector('header')).toBeTruthy()
    expect(screen.getByRole('button', { name: /logout/i })).toBeTruthy()
    // LanguageToggle renders BG and EN buttons
    expect(screen.getByRole('button', { name: 'BG' })).toBeTruthy()
    expect(screen.getByRole('button', { name: 'EN' })).toBeTruthy()
  })

  it('calls POST /api/auth/logout and redirects to login on logout click', async () => {
    mockFetch.mockResolvedValueOnce({ ok: true })

    const { AppHeader } = await import('./AppHeader')
    render(<AppHeader />)

    fireEvent.click(screen.getByRole('button', { name: /logout/i }))

    await waitFor(() => {
      expect(mockFetch).toHaveBeenCalledWith('/api/auth/logout', { method: 'POST' })
      expect(mockPush).toHaveBeenCalledWith('/bg/login')
    })
  })

  it('still redirects to login when logout API call fails', async () => {
    mockFetch.mockRejectedValueOnce(new Error('Network error'))

    const { AppHeader } = await import('./AppHeader')
    render(<AppHeader />)

    fireEvent.click(screen.getByRole('button', { name: /logout/i }))

    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith('/bg/login')
    })
  })
})
