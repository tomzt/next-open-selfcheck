// SPDX-License-Identifier: GPL-3.0-or-later
// next-open-selfcheck — i18n middleware
// Handles locale detection and routing for Next.js App Router

import createMiddleware from 'next-intl/middleware'
import { locales, defaultLocale } from './i18n'

export default createMiddleware({
  locales,
  defaultLocale,
  // Use path-based locale: /th/... or /en/...
  localePrefix: 'as-needed',
})

export const config = {
  // Match all paths except static files and API routes
  matcher: ['/((?!api|_next|_vercel|.*\\..*).*)'],
}
