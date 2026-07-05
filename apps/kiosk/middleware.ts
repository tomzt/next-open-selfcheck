// SPDX-License-Identifier: GPL-3.0-or-later
// Middleware chain: JWT auth check → next-intl locale routing
//
// Public routes (no auth required): / (welcome) and /[locale]/auth
// All other routes require a valid NextAuth JWT session.

import createMiddleware from 'next-intl/middleware'
import { getToken } from 'next-auth/jwt'
import { type NextRequest, NextResponse } from 'next/server'
import { locales, defaultLocale } from './i18n'
import { nextAuthSecret } from '@/lib/auth-secret'

const intlMiddleware = createMiddleware({
  locales,
  defaultLocale,
  localePrefix: 'as-needed',
})

// Route segments that are accessible without authentication
const PUBLIC_SEGMENTS = new Set(['auth'])

function detectLocale(req: NextRequest): string {
  const first = req.nextUrl.pathname.split('/')[1]
  return locales.includes(first as (typeof locales)[number]) ? first : defaultLocale
}

export async function middleware(req: NextRequest) {
  const segments = req.nextUrl.pathname.split('/').filter(Boolean)
  const hasLocale = locales.includes(segments[0] as (typeof locales)[number])
  const routeSegment = hasLocale ? segments[1] : segments[0]

  // Welcome (root) and /auth are always public
  const isPublic = !routeSegment || PUBLIC_SEGMENTS.has(routeSegment)

  if (!isPublic) {
    const token = await getToken({ req, secret: nextAuthSecret })

    if (!token) {
      const locale = detectLocale(req)
      const url = req.nextUrl.clone()
      url.pathname = `/${locale}/auth`
      return NextResponse.redirect(url)
    }
  }

  return intlMiddleware(req)
}

export const config = {
  matcher: ['/((?!api|_next|_vercel|.*\\..*).*)'],
}
