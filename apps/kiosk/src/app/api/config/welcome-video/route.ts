// SPDX-License-Identifier: GPL-3.0-or-later
// GET /api/config/welcome-video — serve the welcome-screen background video.
//
// Resolved from site-config.ts:
//   1. KIOSK_WELCOME_VIDEO_URL env (absolute or root-relative URL) → 302 redirect
//   2. public/branding/{welcome.mp4|webm|mov}                     → stream file
//   3. nothing configured                                         → 404 (UI hides)
//
// WelcomeScreen references this in <video src="/api/config/welcome-video">;
// onError hides the element on 404, so no video = gradient background only.

import { NextResponse } from 'next/server'
import { getSiteBranding } from '@/lib/site-config'
import { streamBrandingAsset } from '../_lib/stream'

const CONTENT_TYPES: Record<string, string> = {
  mp4: 'video/mp4',
  webm: 'video/webm',
  mov: 'video/quicktime',
}

export async function GET() {
  const { welcomeVideo } = getSiteBranding()

  if (welcomeVideo.source === 'remote') {
    return NextResponse.redirect(welcomeVideo.url, { status: 302 })
  }

  if (welcomeVideo.source === 'local') {
    const ext = welcomeVideo.filename.split('.').pop() ?? ''
    return streamBrandingAsset(welcomeVideo.filename, CONTENT_TYPES[ext] ?? 'application/octet-stream')
  }

  return new NextResponse(null, { status: 404 })
}
