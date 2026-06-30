// SPDX-License-Identifier: GPL-3.0-or-later
// Shared helper — stream a bundled branding asset from public/branding/.
// Used by /api/config/logo and /api/config/welcome-video.

import * as fs from 'fs'
import * as path from 'path'
import { NextResponse } from 'next/server'

const BRANDING_DIR = path.join(process.cwd(), 'public', 'branding')

const ALLOWED = new Set(['logo.svg', 'logo.png', 'logo.jpg', 'logo.webp',
  'welcome.mp4', 'welcome.webm', 'welcome.mov'])

/**
 * Stream a file from public/branding/ by filename.
 * Defends against path traversal: filename must be in the allow-list.
 */
export function streamBrandingAsset(filename: string, contentType: string): NextResponse {
  if (!ALLOWED.has(filename)) {
    return new NextResponse(null, { status: 404 })
  }

  const fullPath = path.join(BRANDING_DIR, filename)

  try {
    const stat = fs.statSync(fullPath)
    const body = fs.readFileSync(fullPath)

    return new NextResponse(body, {
      status: 200,
      headers: {
        'Content-Type': contentType,
        'Content-Length': String(stat.size),
        'Cache-Control': 'public, max-age=3600',
      },
    })
  } catch {
    return new NextResponse(null, { status: 404 })
  }
}
