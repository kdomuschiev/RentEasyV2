import createMiddleware from 'next-intl/middleware'
import { defineRouting } from 'next-intl/routing'
import type { NextRequest } from 'next/server'

const routing = defineRouting({
  locales: ['bg', 'en'],
  defaultLocale: 'bg',
  localePrefix: 'always',
})

const intlProxy = createMiddleware(routing)

export function proxy(request: NextRequest) {
  return intlProxy(request)
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)'],
}
