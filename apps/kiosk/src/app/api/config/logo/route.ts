// SPDX-License-Identifier: GPL-3.0-or-later
// GET /api/config/logo — serve the deployer's library logo.
//
// Resolved from site-config.ts:
//   1. KIOSK_LOGO_URL env (absolute or root-relative URL) → 302 redirect
//   2. public/branding/{logo.svg|png|jpg|webp}            → stream file
//   3. nothing configured                                  → 404 (UI hides)
//
// UI components reference this route in <img src="/api/config/logo">; their
// onError handler hides the element on 404, so no logo = no broken image.

import { NextResponse } from 'next/server'
import { getSiteBranding } from '@/lib/site-config'
import { streamBrandingAsset } from '../_lib/stream'

const CONTENT_TYPES: Record<string, string> = {
  svg: 'image/svg+xml',
  png: 'image/png',
  jpg: 'image/jpeg',
  jpeg: 'image/jpeg',
  webp: 'image/webp',
}

export async function GET() {
  const { logo } = getSiteBranding()

  if (logo.source === 'remote') {
    return NextResponse.redirect(logo.url, { status: 302 })
  }

  if (logo.source === 'local') {
    const ext = logo.filename.split('.').pop() ?? ''
    return streamBrandingAsset(logo.filename, CONTENT_TYPES[ext] ?? 'application/octet-stream')
  }

  return new NextResponse(null, { status: 404 })
}
