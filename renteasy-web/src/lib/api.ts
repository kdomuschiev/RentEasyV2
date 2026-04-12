import { cookies } from 'next/headers'

export async function apiRequest(path: string, init?: RequestInit): Promise<Response> {
  const cookieStore = await cookies()
  const token = cookieStore.get('jwt')?.value

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(init?.headers as Record<string, string>),
  }

  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }

  return fetch(`${process.env.RENTEASY_API_URL}${path}`, {
    ...init,
    headers,
  })
}
