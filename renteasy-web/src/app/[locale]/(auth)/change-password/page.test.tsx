import { describe, expect, it, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, fireEvent, waitFor, cleanup } from '@testing-library/react'

const mockPush = vi.fn()

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: mockPush }),
}))

vi.mock('next-intl', () => ({
  useLocale: () => 'bg',
  useTranslations: () => (key: string) => key,
}))

const mockFetch = vi.fn()
global.fetch = mockFetch

describe('ChangePasswordPage', () => {
  beforeEach(() => {
    mockPush.mockReset()
    mockFetch.mockReset()
  })

  afterEach(cleanup)

  it('renders a single new password field', async () => {
    const { default: ChangePasswordPage } = await import('./page')
    render(<ChangePasswordPage />)
    expect(document.getElementById('newPassword')).toBeTruthy()
    expect(document.getElementById('currentPassword')).toBeNull()
    expect(document.getElementById('confirmPassword')).toBeNull()
  })

  it('shows required error on blur when field is empty', async () => {
    const { default: ChangePasswordPage } = await import('./page')
    render(<ChangePasswordPage />)
    const input = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.blur(input)
    await waitFor(() => {
      expect(screen.getByText('validation.required')).toBeTruthy()
    })
  })

  it('shows password too short error when less than 8 chars', async () => {
    const { default: ChangePasswordPage } = await import('./page')
    render(<ChangePasswordPage />)
    const input = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.change(input, { target: { value: 'short1' } })
    fireEvent.blur(input)
    await waitFor(() => {
      expect(screen.getByText('validation.passwordTooShort')).toBeTruthy()
    })
  })

  it('shows complexity error when no digit present', async () => {
    const { default: ChangePasswordPage } = await import('./page')
    render(<ChangePasswordPage />)
    const input = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.change(input, { target: { value: 'NoDigitsHere' } })
    fireEvent.blur(input)
    await waitFor(() => {
      expect(screen.getByText('validation.passwordComplexity')).toBeTruthy()
    })
  })

  it('calls POST /api/auth/forced-change-password on valid submit', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ role: 'Tenant', accountState: 'Active' }),
    })

    const { default: ChangePasswordPage } = await import('./page')
    render(<ChangePasswordPage />)
    const input = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.change(input, { target: { value: 'ValidPass1' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(mockFetch).toHaveBeenCalledWith(
        '/api/auth/forced-change-password',
        expect.objectContaining({
          method: 'POST',
          body: JSON.stringify({ newPassword: 'ValidPass1' }),
        })
      )
    })
  })

  it('redirects to /bg/dashboard when role is Landlord', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ role: 'Landlord', accountState: 'Active' }),
    })

    const { default: ChangePasswordPage } = await import('./page')
    render(<ChangePasswordPage />)
    const input = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.change(input, { target: { value: 'ValidPass1' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith('/bg/dashboard')
    })
  })

  it('redirects to /bg/billing when role is Tenant', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ role: 'Tenant', accountState: 'Active' }),
    })

    const { default: ChangePasswordPage } = await import('./page')
    render(<ChangePasswordPage />)
    const input = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.change(input, { target: { value: 'ValidPass1' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith('/bg/billing')
    })
  })

  it('shows inline API error on non-200 response', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: false,
      status: 400,
      json: async () => ({ detail: 'Password too weak.' }),
    })

    const { default: ChangePasswordPage } = await import('./page')
    render(<ChangePasswordPage />)
    const input = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.change(input, { target: { value: 'ValidPass1' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeTruthy()
      expect(screen.getByText('Password too weak.')).toBeTruthy()
    })
  })
})
