import { describe, expect, it, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, fireEvent, waitFor, cleanup } from '@testing-library/react'

vi.mock('next-intl', () => ({
  useTranslations: () => (key: string) => key,
}))

const mockFetch = vi.fn()
global.fetch = mockFetch

describe('PasswordChangeForm', () => {
  beforeEach(() => {
    mockFetch.mockReset()
  })

  afterEach(cleanup)

  it('renders current password and new password fields', async () => {
    const { PasswordChangeForm } = await import('./PasswordChangeForm')
    render(<PasswordChangeForm />)
    expect(document.getElementById('currentPassword')).toBeTruthy()
    expect(document.getElementById('newPassword')).toBeTruthy()
  })

  it('shows required error on blur when current password is empty', async () => {
    const { PasswordChangeForm } = await import('./PasswordChangeForm')
    render(<PasswordChangeForm />)
    const input = document.getElementById('currentPassword') as HTMLInputElement
    fireEvent.blur(input)
    await waitFor(() => {
      expect(screen.getByText('validation.required')).toBeTruthy()
    })
  })

  it('shows complexity error when new password has no digit', async () => {
    const { PasswordChangeForm } = await import('./PasswordChangeForm')
    render(<PasswordChangeForm />)
    const input = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.change(input, { target: { value: 'NoDigitsHere' } })
    fireEvent.blur(input)
    await waitFor(() => {
      expect(screen.getByText('validation.passwordComplexity')).toBeTruthy()
    })
  })

  it('calls POST /api/auth/change-password with correct body', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ ok: true }),
    })

    const { PasswordChangeForm } = await import('./PasswordChangeForm')
    render(<PasswordChangeForm />)

    const currentInput = document.getElementById('currentPassword') as HTMLInputElement
    const newInput = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.change(currentInput, { target: { value: 'OldPassword1' } })
    fireEvent.change(newInput, { target: { value: 'NewPassword1' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(mockFetch).toHaveBeenCalledWith(
        '/api/auth/change-password',
        expect.objectContaining({
          method: 'POST',
          body: JSON.stringify({ currentPassword: 'OldPassword1', newPassword: 'NewPassword1' }),
        })
      )
    })
  })

  it('shows success message and clears form on success', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ ok: true }),
    })

    const { PasswordChangeForm } = await import('./PasswordChangeForm')
    render(<PasswordChangeForm />)

    const currentInput = document.getElementById('currentPassword') as HTMLInputElement
    const newInput = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.change(currentInput, { target: { value: 'OldPassword1' } })
    fireEvent.change(newInput, { target: { value: 'NewPassword1' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(screen.getByRole('status')).toBeTruthy()
      expect(screen.getByText('changePassword.success')).toBeTruthy()
    })

    expect((document.getElementById('currentPassword') as HTMLInputElement).value).toBe('')
    expect((document.getElementById('newPassword') as HTMLInputElement).value).toBe('')
  })

  it('shows inline API error on failure', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: false,
      status: 400,
      json: async () => ({ detail: 'Incorrect password.' }),
    })

    const { PasswordChangeForm } = await import('./PasswordChangeForm')
    render(<PasswordChangeForm />)

    const currentInput = document.getElementById('currentPassword') as HTMLInputElement
    const newInput = document.getElementById('newPassword') as HTMLInputElement
    fireEvent.change(currentInput, { target: { value: 'WrongOld1' } })
    fireEvent.change(newInput, { target: { value: 'NewPassword1' } })
    fireEvent.submit(document.querySelector('form')!)

    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeTruthy()
      expect(screen.getByText('Incorrect password.')).toBeTruthy()
    })
  })
})
