import { getRequestConfig } from 'next-intl/server'

// Supported locales — add more here for community contributions
export const locales = ['th', 'en'] as const
export type Locale = (typeof locales)[number]

// Default locale — overridden by NEXT_PUBLIC_DEFAULT_LANGUAGE env
export const defaultLocale: Locale = 
  (process.env.NEXT_PUBLIC_DEFAULT_LANGUAGE as Locale) ?? 'th'

export default getRequestConfig(async ({ locale }) => {
  // Validate locale — fall back to default if unknown
  const validLocale = locales.includes(locale as Locale) 
    ? locale 
    : defaultLocale

  return {
    locale: validLocale,
    messages: (await import(`../messages/${validLocale}.json`)).default,
  }
})
