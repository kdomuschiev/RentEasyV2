import { cookies } from 'next/headers'
import { redirect } from 'next/navigation'
import { AppHeader } from '@/components/ui/AppHeader'

export default async function LandlordLayout({
  children,
  params,
}: {
  children: React.ReactNode
  params: Promise<{ locale: string }>
}) {
  const { locale } = await params
  const cookieStore = await cookies()
  const token = cookieStore.get('jwt')?.value

  if (!token) redirect(`/${locale}/login`)

  try {
    const payload = JSON.parse(
      Buffer.from(token.split('.')[1], 'base64url').toString('utf-8')
    ) as { role: string; account_state: string }

    if (payload.role !== 'Landlord') redirect(`/${locale}/login`)

    if (payload.account_state === 'RequiresPasswordChange') redirect(`/${locale}/change-password`)
  } catch {
    redirect(`/${locale}/login`)
  }

  return (
    <>
      <AppHeader />
      <main id="main-content">{children}</main>
    </>
  )
}
