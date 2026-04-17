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
    <header className="flex items-center justify-between px-4 py-3 border-b border-[#E5E0D8] bg-[#4A6172]">
      <LanguageToggle />
      <button
        type="button"
        onClick={handleLogout}
        className="text-sm text-white min-h-[44px] px-3 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#4A6172] focus-visible:ring-offset-2 rounded"
      >
        {t('logout')}
      </button>
    </header>
  )
}
