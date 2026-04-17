'use client'
import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { useLocale, useTranslations } from 'next-intl'
import { PasswordInput } from '@/components/ui/PasswordInput'

export default function ChangePasswordPage() {
  const locale = useLocale()
  const router = useRouter()
  const t = useTranslations('auth')
  const tCommon = useTranslations('common')
  const [newPassword, setNewPassword] = useState('')
  const [error, setError] = useState<string | undefined>()
  const [apiError, setApiError] = useState<string | undefined>()
  const [loading, setLoading] = useState(false)

  const validateNewPassword = (v: string): string => {
    if (!v) return t('validation.required')
    if (v.length < 8) return t('validation.passwordTooShort')
    if (!/[a-zA-Z]/.test(v) || !/[0-9]/.test(v)) return t('validation.passwordComplexity')
    return ''
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const err = validateNewPassword(newPassword)
    if (err) {
      setError(err)
      return
    }

    setLoading(true)
    setApiError(undefined)

    try {
      const res = await fetch('/api/auth/forced-change-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ newPassword }),
      })

      if (!res.ok) {
        const body = await res.json().catch(() => ({}))
        setApiError((body as { detail?: string }).detail ?? tCommon('error'))
        setLoading(false)
        return
      }

      const { role } = (await res.json()) as { role: string; accountState: string }
      setLoading(false)
      router.push(role === 'Landlord' ? `/${locale}/dashboard` : `/${locale}/billing`)
    } catch {
      setApiError(tCommon('error'))
      setLoading(false)
    }
  }

  return (
    <main id="main-content" className="min-h-screen flex items-center justify-center p-4">
      <div className="w-full max-w-sm">
        <h1 className="text-2xl font-semibold mb-6">{t('changePassword.title')}</h1>
        <p className="text-sm text-[--color-muted] mb-4">{t('changePassword.requirements')}</p>
        <form onSubmit={handleSubmit} noValidate>
          <div className="mb-4">
            <label htmlFor="newPassword" className="block text-sm font-medium mb-1">
              {t('changePassword.newPassword')}
            </label>
            <PasswordInput
              id="newPassword"
              name="newPassword"
              value={newPassword}
              onChange={setNewPassword}
              onBlur={() => {
                const err = validateNewPassword(newPassword)
                setError(err || undefined)
              }}
              error={error}
              showPasswordLabel={t('login.showPassword')}
              hidePasswordLabel={t('login.hidePassword')}
            />
          </div>

          {apiError && (
            <p role="alert" className="text-[--color-error] text-sm mb-4">
              {apiError}
            </p>
          )}

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-[--color-accent] text-white rounded px-4 py-2 min-h-[44px] disabled:opacity-50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#4A6172] focus-visible:ring-offset-2"
          >
            {loading ? tCommon('loading') : t('changePassword.submit')}
          </button>
        </form>
      </div>
    </main>
  )
}
