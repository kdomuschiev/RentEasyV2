'use client'
import { useLocale } from 'next-intl'
import { useRouter, usePathname } from 'next/navigation'

export function LanguageToggle() {
  const locale = useLocale()
  const router = useRouter()
  const pathname = usePathname()

  const switchLocale = (newLocale: 'bg' | 'en') => {
    if (newLocale === locale) return
    const segments = pathname.split('/')
    segments[1] = newLocale
    router.push(segments.join('/'))
  }

  return (
    <div className="flex gap-2" role="group" aria-label="Language">
      <button
        onClick={() => switchLocale('bg')}
        aria-pressed={locale === 'bg'}
        className={locale === 'bg' ? 'font-semibold' : 'text-gray-500'}
      >
        BG
      </button>
      <button
        onClick={() => switchLocale('en')}
        aria-pressed={locale === 'en'}
        className={locale === 'en' ? 'font-semibold' : 'text-gray-500'}
      >
        EN
      </button>
    </div>
  )
}
