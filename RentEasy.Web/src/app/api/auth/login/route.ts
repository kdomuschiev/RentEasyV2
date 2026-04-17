import { cookies } from 'next/headers'
import { NextRequest, NextResponse } from 'next/server'

export async function POST(request: NextRequest): Promise<NextResponse> {
  const apiUrl = process.env.RENTEASY_API_URL
  if (!apiUrl) throw new Error('RENTEASY_API_URL environment variable is not set')

  let body: unknown
  try {
    body = await request.json()
  } catch {
    return NextResponse.json({ error: 'Invalid request body' }, { status: 400 })
  }

  let apiResponse: Response
  try {
    apiResponse = await fetch(`${apiUrl}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    })
  } catch {
    return NextResponse.json({ error: 'Could not reach API' }, { status: 502 })
  }

  let data: unknown
  try {
    data = await apiResponse.json()
  } catch {
    return NextResponse.json({ error: 'Invalid API response' }, { status: 502 })
  }

  if (!apiResponse.ok) {
    return NextResponse.json(data, { status: apiResponse.status })
  }

  const { token, role, accountState } = data as { token: string; role: string; accountState: string }

  const cookieStore = await cookies()
  cookieStore.set('jwt', token, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'strict',
    path: '/',
    maxAge: 60 * 60 * 24 * 7,
  })

  return NextResponse.json({ role, accountState })
}
