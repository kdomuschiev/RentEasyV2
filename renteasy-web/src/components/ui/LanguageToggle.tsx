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
        type="button"
        onClick={() => switchLocale('bg')}
        aria-pressed={locale === 'bg'}
        className={`min-h-[44px] px-3 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#4A6172] focus-visible:ring-offset-2 rounded ${locale === 'bg' ? 'font-semibold text-white' : 'text-white/70'}`}
      >
        BG
      </button>
      <button
        type="button"
        onClick={() => switchLocale('en')}
        aria-pressed={locale === 'en'}
        className={`min-h-[44px] px-3 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#4A6172] focus-visible:ring-offset-2 rounded ${locale === 'en' ? 'font-semibold text-white' : 'text-white/70'}`}
      >
        EN
      </button>
    </div>
  )
}
