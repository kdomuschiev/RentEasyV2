import { describe, expect, it, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, fireEvent, cleanup } from '@testing-library/react'

const mockPush = vi.fn()
let mockLocale = 'bg'
let mockPathnameFn = () => '/bg/dashboard'

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: mockPush }),
  usePathname: () => mockPathnameFn(),
}))

vi.mock('next-intl', () => ({
  useLocale: () => mockLocale,
}))

describe('LanguageToggle', () => {
  beforeEach(() => {
    mockPush.mockClear()
    mockLocale = 'bg'
    mockPathnameFn = () => '/bg/dashboard'
  })

  afterEach(cleanup)

  it('renders BG and EN buttons', async () => {
    const { LanguageToggle } = await import('./LanguageToggle')
    render(<LanguageToggle />)
    expect(screen.getByRole('button', { name: 'BG' })).toBeTruthy()
    expect(screen.getByRole('button', { name: 'EN' })).toBeTruthy()
  })

  it('marks current locale as aria-pressed=true', async () => {
    const { LanguageToggle } = await import('./LanguageToggle')
    render(<LanguageToggle />)
    const bgButton = screen.getByRole('button', { name: 'BG' })
    const enButton = screen.getByRole('button', { name: 'EN' })
    expect(bgButton.getAttribute('aria-pressed')).toBe('true')
    expect(enButton.getAttribute('aria-pressed')).toBe('false')
  })

  it('calls router.push with new locale path when EN is clicked', async () => {
    const { LanguageToggle } = await import('./LanguageToggle')
    render(<LanguageToggle />)
    fireEvent.click(screen.getByRole('button', { name: 'EN' }))
    expect(mockPush).toHaveBeenCalledWith('/en/dashboard')
  })

  it('does not call router.push when current locale button is clicked', async () => {
    const { LanguageToggle } = await import('./LanguageToggle')
    render(<LanguageToggle />)
    fireEvent.click(screen.getByRole('button', { name: 'BG' }))
    expect(mockPush).not.toHaveBeenCalled()
  })
})
