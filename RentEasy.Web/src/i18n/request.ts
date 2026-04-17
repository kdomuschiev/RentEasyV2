import { getRequestConfig } from 'next-intl/server'

export default getRequestConfig(async ({ requestLocale }) => {
  let locale = await requestLocale
  if (!locale || !['bg', 'en'].includes(locale)) locale = 'bg'
  return {
    locale,
    messages: (await import(`../messages/${locale}.json`)).default,
  }
})
