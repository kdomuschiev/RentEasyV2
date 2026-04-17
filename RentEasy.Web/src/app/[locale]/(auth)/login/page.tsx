'use client'
import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { useLocale, useTranslations } from 'next-intl'
import { PasswordInput } from '@/components/ui/PasswordInput'

export default function LoginPage() {
  const locale = useLocale()
  const router = useRouter()
  const t = useTranslations('auth')
  const tCommon = useTranslations('common')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [errors, setErrors] = useState<{ email?: string; password?: string; api?: string }>({})
  const [loading, setLoading] = useState(false)

  const validateEmail = (v: string): string => {
    if (!v) return t('validation.required')
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v)) return t('validation.invalidEmail')
    return ''
  }

  const validatePassword = (v: string): string => (!v ? t('validation.required') : '')

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const emailErr = validateEmail(email)
    const passErr = validatePassword(password)
    if (emailErr || passErr) {
      setErrors({ email: emailErr || undefined, password: passErr || undefined })
      return
    }

    setLoading(true)
    setErrors({})

    try {
      const res = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
      })

      if (!res.ok) {
        const body = await res.json().catch(() => ({}))
        setErrors({ api: (body as { detail?: string }).detail ?? t('login.invalidCredentials') })
        setLoading(false)
        return
      }

      const { role, accountState } = (await res.json()) as { role: string; accountState: string }
      setLoading(false)
      if (accountState === 'RequiresPasswordChange') {
        router.push(`/${locale}/change-password`)
        return
      }
      router.push(role === 'Landlord' ? `/${locale}/dashboard` : `/${locale}/billing`)
    } catch {
      setErrors({ api: tCommon('error') })
      setLoading(false)
    }
  }

  return (
    <main id="main-content" className="min-h-screen flex items-center justify-center p-4">
      <div className="w-full max-w-sm">
        <h1 className="text-2xl font-semibold mb-6">{t('login.title')}</h1>
        <form onSubmit={handleSubmit} noValidate>
          <div className="mb-4">
            <label htmlFor="email" className="block text-sm font-medium mb-1">
              {t('login.email')}
            </label>
            <input
              id="email"
              name="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              onBlur={() => {
                const err = validateEmail(email)
                setErrors((prev) => ({ ...prev, email: err || undefined }))
              }}
              aria-invalid={!!errors.email}
              aria-describedby={errors.email ? 'email-error' : undefined}
              className="w-full border border-[#E5E0D8] rounded px-3 py-2 min-h-[44px] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#4A6172] focus-visible:ring-offset-2"
              autoComplete="email"
            />
            {errors.email && (
              <p id="email-error" className="text-[var(--color-error)] text-sm mt-1">
                {errors.email}
              </p>
            )}
          </div>

          <div className="mb-4">
            <label htmlFor="password" className="block text-sm font-medium mb-1">
              {t('login.password')}
            </label>
            <PasswordInput
              id="password"
              name="password"
              value={password}
              onChange={setPassword}
              onBlur={() => {
                const err = validatePassword(password)
                setErrors((prev) => ({ ...prev, password: err || undefined }))
              }}
              error={errors.password}
              showPasswordLabel={t('login.showPassword')}
              hidePasswordLabel={t('login.hidePassword')}
            />
          </div>

          {errors.api && (
            <p role="alert" className="text-[var(--color-error)] text-sm mb-4">
              {errors.api}
            </p>
          )}

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-[var(--color-accent)] text-white rounded px-4 py-2 min-h-[44px] disabled:opacity-50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#4A6172] focus-visible:ring-offset-2"
          >
            {loading ? tCommon('loading') : t('login.submit')}
          </button>
        </form>
      </div>
    </main>
  )
}
