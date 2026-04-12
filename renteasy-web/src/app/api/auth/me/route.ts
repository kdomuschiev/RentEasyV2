import { cookies } from 'next/headers'
import { NextResponse } from 'next/server'

function decodeJwtPayload(token: string): Record<string, unknown> {
  const base64 = token.split('.')[1]
  return JSON.parse(Buffer.from(base64, 'base64url').toString('utf-8'))
}

export async function GET(): Promise<NextResponse> {
  const cookieStore = await cookies()
  const token = cookieStore.get('jwt')?.value

  if (!token) {
    return NextResponse.json({ error: 'Unauthenticated' }, { status: 401 })
  }

  try {
    const payload = decodeJwtPayload(token)
    return NextResponse.json({
      role: payload.role,
      accountState: payload.account_state,
    })
  } catch {
    return NextResponse.json({ error: 'Unauthenticated' }, { status: 401 })
  }
}
