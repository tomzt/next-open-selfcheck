// SPDX-License-Identifier: GPL-3.0-or-later
import type { NextConfig } from 'next'
import createNextIntlPlugin from 'next-intl/plugin'

const withNextIntl = createNextIntlPlugin('./i18n.ts')

const nextConfig: NextConfig = {
  output: 'standalone',
  images: {
    localPatterns: [{ pathname: '/uploads/**' }],
  },
  async headers() {
    // Logo/welcome-video are deployer-set to any https URL (see
    // KIOSK_LOGO_URL / KIOSK_WELCOME_VIDEO_URL), so img-src/media-src can't
    // be pinned to a single origin. Next.js needs 'unsafe-inline' for its
    // own hydration script and for the inline `style={{...}}` props used
    // throughout the kiosk UI. 'unsafe-eval' is dev-only — `next dev`'s
    // webpack bundling uses eval() for fast refresh; the production build
    // doesn't need it, so it's dropped there for a tighter CSP.
    const isDev = process.env.NODE_ENV !== 'production'
    const csp = [
      "default-src 'self'",
      `script-src 'self' 'unsafe-inline'${isDev ? " 'unsafe-eval'" : ''}`,
      "style-src 'self' 'unsafe-inline'",
      "img-src 'self' https: data:",
      "media-src 'self' https:",
      "connect-src 'self'",
      "font-src 'self'",
      "object-src 'none'",
      "frame-ancestors 'none'",
      "base-uri 'self'",
      "form-action 'self'",
    ].join('; ')

    return [
      {
        source: '/(.*)',
        headers: [
          { key: 'X-Frame-Options', value: 'DENY' },
          { key: 'X-Content-Type-Options', value: 'nosniff' },
          { key: 'Content-Security-Policy', value: csp },
          { key: 'Referrer-Policy', value: 'same-origin' },
          { key: 'Permissions-Policy', value: 'camera=(), microphone=(), geolocation=()' },
          // Only takes effect over HTTPS; harmless no-op on local http dev.
          { key: 'Strict-Transport-Security', value: 'max-age=63072000; includeSubDomains' },
        ],
      },
    ]
  },
}

export default withNextIntl(nextConfig)
