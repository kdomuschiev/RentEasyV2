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

describe('LoginPage', () => {
  beforeEach(() => {
    mockPush.mockReset()
    mockFetch.mockReset()
  })

  afterEach(cleanup)

  it('renders email and password fields with submit button', async () => {
    const { default: LoginPage } = await import('./page')
    render(<LoginPage />)
    expect(screen.getByLabelText(/email/i)).toBeTruthy()
    expect(document.getElementById('password')).toBeTruthy()
    // Submit button text is "login.submit" (from i18n mock returning the key)
    expect(document.querySelector('button[type="submit"]')).toBeTruthy()
  })

  it('shows email validation error on blur when email is empty', async () => {
    const { default: LoginPage } = await import('./page')
    render(<LoginPage />)
    const emailInput = screen.getByLabelText(/email/i)
    fireEvent.blur(emailInput)
    await waitFor(() => {
      expect(screen.getByText('validation.required')).toBeTruthy()
    })
  })

  it('shows invalid email error on blur with malformed email', async () => {
    const { default: LoginPage } = await import('./page')
    render(<LoginPage />)
    const emailInput = screen.getByLabelText(/email/i)
    fireEvent.change(emailInput, { target: { value: 'notanemail' } })
    fireEvent.blur(emailInput)
    await waitFor(() => {
      expect(screen.getByText('validation.invalidEmail')).toBeTruthy()
    })
  })

  it('calls POST /api/auth/login with correct body on submit', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ role: 'Landlord', accountState: 'Active' }),
    })

    const { default: LoginPage } = await import('./page')
    render(<LoginPage />)

    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'user@test.com' } })
    const passwordInput = document.getElementById('password') as HTMLInputElement
    fireEvent.change(passwordInput, { target: { value: 'pass123' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(mockFetch).toHaveBeenCalledWith(
        '/api/auth/login',
        expect.objectContaining({
          method: 'POST',
          body: JSON.stringify({ email: 'user@test.com', password: 'pass123' }),
        })
      )
    })
  })

  it('redirects to /bg/dashboard when role is Landlord', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ role: 'Landlord', accountState: 'Active' }),
    })

    const { default: LoginPage } = await import('./page')
    render(<LoginPage />)

    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'landlord@test.com' } })
    const passwordInput = document.getElementById('password') as HTMLInputElement
    fireEvent.change(passwordInput, { target: { value: 'pass123' } })
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

    const { default: LoginPage } = await import('./page')
    render(<LoginPage />)

    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'tenant@test.com' } })
    const passwordInput = document.getElementById('password') as HTMLInputElement
    fireEvent.change(passwordInput, { target: { value: 'pass123' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith('/bg/billing')
    })
  })

  it('redirects to /bg/change-password when accountState is RequiresPasswordChange', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ role: 'Tenant', accountState: 'RequiresPasswordChange' }),
    })

    const { default: LoginPage } = await import('./page')
    render(<LoginPage />)

    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'tenant@test.com' } })
    const passwordInput = document.getElementById('password') as HTMLInputElement
    fireEvent.change(passwordInput, { target: { value: 'pass123' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith('/bg/change-password')
    })
  })

  it('shows inline API error on 401 response', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: false,
      status: 401,
      json: async () => ({ detail: 'Invalid email or password.' }),
    })

    const { default: LoginPage } = await import('./page')
    render(<LoginPage />)

    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'bad@test.com' } })
    const passwordInput = document.getElementById('password') as HTMLInputElement
    fireEvent.change(passwordInput, { target: { value: 'wrongpass' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeTruthy()
      expect(screen.getByText('Invalid email or password.')).toBeTruthy()
    })
  })
})
