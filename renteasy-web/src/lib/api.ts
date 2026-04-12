import { cookies } from 'next/headers'

const API_URL = process.env.RENTEASY_API_URL
if (!API_URL) throw new Error('RENTEASY_API_URL environment variable is not set')

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

  return fetch(`${API_URL}${path}`, {
    ...init,
    headers,
  })
}
