// SPDX-License-Identifier: GPL-3.0-or-later
import type { NextConfig } from 'next'
import createNextIntlPlugin from 'next-intl/plugin'

const withNextIntl = createNextIntlPlugin('./i18n.ts')

const nextConfig: NextConfig = {
  // Kiosk mode — no need for image optimization from external sources
  images: {
    localPatterns: [{ pathname: '/uploads/**' }],
  },
  // Security headers for kiosk deployment
  async headers() {
    return [
      {
        source: '/(.*)',
        headers: [
          { key: 'X-Frame-Options', value: 'DENY' },
          { key: 'X-Content-Type-Options', value: 'nosniff' },
        ],
      },
    ]
  },
}

export default withNextIntl(nextConfig)
