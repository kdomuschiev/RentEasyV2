import { cookies } from 'next/headers'
import { NextRequest, NextResponse } from 'next/server'

const API_URL = process.env.RENTEASY_API_URL
if (!API_URL) throw new Error('RENTEASY_API_URL environment variable is not set')

export async function POST(request: NextRequest): Promise<NextResponse> {
  let body: unknown
  try {
    body = await request.json()
  } catch {
    return NextResponse.json({ error: 'Invalid request body' }, { status: 400 })
  }

  const apiResponse = await fetch(`${API_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })

  const data = await apiResponse.json()

  if (!apiResponse.ok) {
    return NextResponse.json(data, { status: apiResponse.status })
  }

  const cookieStore = await cookies()
  cookieStore.set('jwt', data.token, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'strict',
    path: '/',
    maxAge: 60 * 60 * 24 * 7,
  })

  return NextResponse.json({ role: data.role, accountState: data.accountState })
}
