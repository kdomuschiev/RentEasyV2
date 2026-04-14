import { describe, expect, it, vi, afterEach } from 'vitest'
import { render, screen, fireEvent, cleanup } from '@testing-library/react'
import { PasswordInput } from './PasswordInput'

afterEach(cleanup)

describe('PasswordInput', () => {
  it('renders as password type by default', () => {
    render(
      <PasswordInput id="pwd" name="pwd" value="" onChange={vi.fn()} />
    )
    const input = document.getElementById('pwd') as HTMLInputElement
    expect(input.type).toBe('password')
  })

  it('toggles to text type when show button is clicked', () => {
    render(
      <PasswordInput id="pwd" name="pwd" value="" onChange={vi.fn()} />
    )
    const toggle = screen.getByRole('button', { name: /show password/i })
    fireEvent.click(toggle)

    const input = document.getElementById('pwd') as HTMLInputElement
    expect(input.type).toBe('text')
    expect(screen.getByRole('button', { name: /hide password/i })).toBeTruthy()
  })

  it('toggles back to password type on second click', () => {
    render(
      <PasswordInput id="pwd" name="pwd" value="" onChange={vi.fn()} />
    )
    const toggle = screen.getByRole('button', { name: /show password/i })
    fireEvent.click(toggle)
    fireEvent.click(screen.getByRole('button', { name: /hide password/i }))

    const input = document.getElementById('pwd') as HTMLInputElement
    expect(input.type).toBe('password')
  })

  it('renders error message when error prop is set', () => {
    render(
      <PasswordInput id="pwd" name="pwd" value="" onChange={vi.fn()} error="Password is required" />
    )
    expect(screen.getByText('Password is required')).toBeTruthy()
  })

  it('sets aria-invalid and aria-describedby when error is present', () => {
    render(
      <PasswordInput id="pwd" name="pwd" value="" onChange={vi.fn()} error="Too short" />
    )
    const input = document.getElementById('pwd') as HTMLInputElement
    expect(input.getAttribute('aria-invalid')).toBe('true')
    expect(input.getAttribute('aria-describedby')).toBe('pwd-error')
  })

  it('does not render error element when error prop is absent', () => {
    render(
      <PasswordInput id="pwd" name="pwd" value="" onChange={vi.fn()} />
    )
    const input = document.getElementById('pwd') as HTMLInputElement
    expect(input.getAttribute('aria-invalid')).toBe('false')
    expect(input.getAttribute('aria-describedby')).toBeNull()
  })

  it('calls onChange with new value when input changes', () => {
    const onChange = vi.fn()
    render(
      <PasswordInput id="pwd" name="pwd" value="" onChange={onChange} />
    )
    const input = document.getElementById('pwd') as HTMLInputElement
    fireEvent.change(input, { target: { value: 'secret' } })
    expect(onChange).toHaveBeenCalledWith('secret')
  })

  it('calls onBlur when input loses focus', () => {
    const onBlur = vi.fn()
    render(
      <PasswordInput id="pwd" name="pwd" value="" onChange={vi.fn()} onBlur={onBlur} />
    )
    const input = document.getElementById('pwd') as HTMLInputElement
    fireEvent.blur(input)
    expect(onBlur).toHaveBeenCalled()
  })
})
