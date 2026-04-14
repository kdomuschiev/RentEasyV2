'use client'
import { useRouter } from 'next/navigation'
import { useLocale, useTranslations } from 'next-intl'
import { LanguageToggle } from './LanguageToggle'

export function AppHeader() {
  const locale = useLocale()
  const router = useRouter()
  const t = useTranslations('auth.header')

  const handleLogout = async () => {
    try {
      await fetch('/api/auth/logout', { method: 'POST' })
    } catch {
      // Best-effort logout — redirect to login regardless so the user is not stuck
    }
    router.push(`/${locale}/login`)
  }

  return (
    <header className="flex items-center justify-between px-4 py-3 border-b">
      <LanguageToggle />
      <button
        type="button"
        onClick={handleLogout}
        className="text-sm text-gray-700"
      >
        {t('logout')}
      </button>
    </header>
  )
}
