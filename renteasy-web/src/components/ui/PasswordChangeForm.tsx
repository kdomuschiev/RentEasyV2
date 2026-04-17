'use client'
import { useState } from 'react'
import { useTranslations } from 'next-intl'
import { PasswordInput } from './PasswordInput'

export function PasswordChangeForm() {
  const t = useTranslations('auth')
  const tCommon = useTranslations('common')
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [errors, setErrors] = useState<{ currentPassword?: string; newPassword?: string }>({})
  const [apiError, setApiError] = useState<string | undefined>()
  const [success, setSuccess] = useState(false)
  const [loading, setLoading] = useState(false)

  const validateCurrentPassword = (v: string): string => (!v ? t('validation.required') : '')

  const validateNewPassword = (v: string): string => {
    if (!v) return t('validation.required')
    if (v.length < 8) return t('validation.passwordTooShort')
    if (!/[a-zA-Z]/.test(v) || !/[0-9]/.test(v)) return t('validation.passwordComplexity')
    return ''
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const currentErr = validateCurrentPassword(currentPassword)
    const newErr = validateNewPassword(newPassword)
    if (currentErr || newErr) {
      setErrors({ currentPassword: currentErr || undefined, newPassword: newErr || undefined })
      return
    }

    setLoading(true)
    setApiError(undefined)
    setSuccess(false)

    try {
      const res = await fetch('/api/auth/change-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ currentPassword, newPassword }),
      })

      if (!res.ok) {
        const body = await res.json().catch(() => ({}))
        setApiError((body as { detail?: string }).detail ?? tCommon('error'))
        setLoading(false)
        return
      }

      setCurrentPassword('')
      setNewPassword('')
      setErrors({})
      setSuccess(true)
      setLoading(false)
    } catch {
      setApiError(tCommon('error'))
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} noValidate className="space-y-4">
      <div>
        <label htmlFor="currentPassword" className="block text-sm font-medium mb-1">
          {t('changePassword.currentPassword')}
        </label>
        <PasswordInput
          id="currentPassword"
          name="currentPassword"
          value={currentPassword}
          onChange={setCurrentPassword}
          onBlur={() => {
            const err = validateCurrentPassword(currentPassword)
            setErrors((prev) => ({ ...prev, currentPassword: err || undefined }))
          }}
          error={errors.currentPassword}
          showPasswordLabel={t('login.showPassword')}
          hidePasswordLabel={t('login.hidePassword')}
        />
      </div>

      <div>
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
            setErrors((prev) => ({ ...prev, newPassword: err || undefined }))
          }}
          error={errors.newPassword}
          showPasswordLabel={t('login.showPassword')}
          hidePasswordLabel={t('login.hidePassword')}
        />
      </div>

      {apiError && (
        <p role="alert" className="text-[var(--color-error)] text-sm">
          {apiError}
        </p>
      )}

      {success && (
        <p role="status" className="text-[var(--color-success)] text-sm">
          {t('changePassword.success')}
        </p>
      )}

      <button
        type="submit"
        disabled={loading}
        className="w-full bg-[var(--color-accent)] text-white rounded px-4 py-2 min-h-[44px] disabled:opacity-50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#4A6172] focus-visible:ring-offset-2"
      >
        {loading ? tCommon('loading') : t('changePassword.submit')}
      </button>
    </form>
  )
}
