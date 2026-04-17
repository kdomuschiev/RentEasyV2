import { cookies } from 'next/headers'
import { redirect } from 'next/navigation'
import { getTranslations } from 'next-intl/server'
import { PasswordChangeForm } from '@/components/ui/PasswordChangeForm'
import { AppHeader } from '@/components/ui/AppHeader'

export default async function SettingsPage({
  params,
}: {
  params: Promise<{ locale: string }>
}) {
  const { locale } = await params
  const cookieStore = await cookies()
  const token = cookieStore.get('jwt')?.value

  if (!token) redirect(`/${locale}/login`)

  let accountState: string | undefined
  try {
    const payload = JSON.parse(Buffer.from(token.split('.')[1], 'base64url').toString('utf-8')) as {
      account_state?: string
    }
    accountState = payload.account_state
  } catch {
    redirect(`/${locale}/login`)
  }

  if (accountState === 'RequiresPasswordChange') redirect(`/${locale}/change-password`)
  if (accountState === 'Expired') redirect(`/${locale}/expired`)

  const t = await getTranslations({ locale, namespace: 'settings' })

  return (
    <>
      <AppHeader />
      <main id="main-content" className="max-w-md mx-auto p-6">
        <h1 className="text-2xl font-semibold mb-6">{t('title')}</h1>
        <PasswordChangeForm />
      </main>
    </>
  )
}
