import { cookies } from 'next/headers'
import { NextResponse } from 'next/server'

const cookieOptions = {
  httpOnly: true,
  secure: process.env.NODE_ENV === 'production',
  sameSite: 'strict' as const,
  path: '/',
  maxAge: 60 * 60 * 24 * 7,
}

export async function POST(request: Request): Promise<NextResponse> {
  const apiUrl = process.env.RENTEASY_API_URL
  if (!apiUrl) return NextResponse.json({ error: 'Server misconfiguration' }, { status: 500 })

  const cookieStore = await cookies()
  const token = cookieStore.get('jwt')?.value
  if (!token) return NextResponse.json({ error: 'Unauthenticated' }, { status: 401 })

  let body: unknown
  try {
    body = await request.json()
  } catch {
    return NextResponse.json({ error: 'Invalid JSON' }, { status: 400 })
  }

  const apiRes = await fetch(`${apiUrl}/api/auth/change-password`, {
    method: 'POST',
    headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })

  if (!apiRes.ok) {
    const errorBody = await apiRes.json().catch(() => ({}))
    return NextResponse.json(errorBody, { status: apiRes.status })
  }

  const data = (await apiRes.json()) as { token: string }
  const response = NextResponse.json({ ok: true })
  response.cookies.set('jwt', data.token, cookieOptions)
  return response
}
