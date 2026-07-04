// SPDX-License-Identifier: GPL-3.0-or-later
// Site branding & identity config — read from ENV only.
// No hardcoded org-specific values. Server-side only.
//
// This is the foundation for the First-Run Setup Wizard (Phase 7):
// the wizard will write these ENV vars, this module reads them.

import * as fs from 'fs'
import * as path from 'path'

// ── Types ──────────────────────────────────────────────────────────────────

export interface SiteBranding {
  /** Library name shown in <title>, header, receipt — localized if provided */
  libraryNameTh: string
  libraryNameEn: string
  /** Resolved logo asset (route decides how to serve) */
  logo: AssetResolution
  /** Resolved welcome-screen background video asset */
  welcomeVideo: AssetResolution
}

export type AssetResolution =
  /** Serve a remote/absolute URL via 302 redirect (http(s):// or /path) */
  | { source: 'remote'; url: string }
  /** Serve a local file from public/branding/ — route streams it if it exists */
  | { source: 'local'; filename: string }
  /** Nothing configured — route returns 404, UI hides gracefully */
  | { source: 'none' }

// ── Defaults ───────────────────────────────────────────────────────────────
// Intentionally generic — deployer MUST set KIOSK_LIBRARY_NAME for their site.
// These are neutral fallbacks, never an org-specific brand.

const DEFAULT_LIBRARY_NAME_TH = 'ห้องสมุด'
const DEFAULT_LIBRARY_NAME_EN = 'Library'

// Filenames a deployer can drop into public/branding/ without touching ENV.
// Tried in order; first existing file wins.
const LOGO_CANDIDATES = ['logo.svg', 'logo.png', 'logo.jpg', 'logo.webp']
const VIDEO_CANDIDATES = ['welcome.mp4', 'welcome.webm', 'welcome.mov']

// ── Asset resolver ─────────────────────────────────────────────────────────

/**
 * Resolve a branding asset.
 *
 * Priority:
 *   1. ENV url var (KIOSK_*_URL) — if set, treat as remote/absolute URL
 *   2. Bundled file in public/branding/ (checked at runtime against disk)
 *   3. none → route returns 404, UI hides the element via onError
 */
function resolveAsset(
  urlEnvVar: string,
  candidates: string[],
): AssetResolution {
  // 1. Explicit URL override (absolute or root-relative path)
  const url = process.env[urlEnvVar]?.trim()
  if (url) return { source: 'remote', url }

  // 2. Look for a default file in public/branding/
  const brandingDir = path.join(process.cwd(), 'public', 'branding')
  for (const filename of candidates) {
    try {
      if (fs.existsSync(path.join(brandingDir, filename))) {
        return { source: 'local', filename }
      }
    } catch {
      // fs unavailable (edge/serverless) — fall through to none
    }
  }

  // 3. Nothing configured
  return { source: 'none' }
}

// ── Public API ─────────────────────────────────────────────────────────────

let cached: SiteBranding | null = null

/**
 * Read site branding from ENV. Result is cached for the process lifetime —
 * ENV does not change at runtime.
 */
export function getSiteBranding(): SiteBranding {
  if (cached) return cached

  // KIOSK_LIBRARY_NAME applies to every locale; TH/EN variants override.
  const anyName = process.env.KIOSK_LIBRARY_NAME?.trim()
  const nameTh = process.env.KIOSK_LIBRARY_NAME_TH?.trim() ?? anyName
  const nameEn = process.env.KIOSK_LIBRARY_NAME_EN?.trim() ?? anyName

  cached = {
    libraryNameTh: nameTh || DEFAULT_LIBRARY_NAME_TH,
    libraryNameEn: nameEn || DEFAULT_LIBRARY_NAME_EN,
    logo: resolveAsset('KIOSK_LOGO_URL', LOGO_CANDIDATES),
    welcomeVideo: resolveAsset('KIOSK_WELCOME_VIDEO_URL', VIDEO_CANDIDATES),
  }
  return cached
}

/**
 * Pick the localized library name for a given locale.
 * Falls back to whichever variant is set.
 */
export function libraryNameFor(locale: string): string {
  const b = getSiteBranding()
  return locale === 'th' ? b.libraryNameTh : b.libraryNameEn
}

// ── Service toggle ─────────────────────────────────────────────────────────
// KIOSK_SERVICES lets a deployer disable a menu item without touching code —
// e.g. a library whose return slot is Bookdrop-only sets
// KIOSK_SERVICES=borrow,loans,fines to hide "Return" from this kiosk.

export type ServiceRoute = 'borrow' | 'return' | 'loans' | 'fines'

const ALL_SERVICES: ServiceRoute[] = ['borrow', 'return', 'loans', 'fines']

/**
 * Read KIOSK_SERVICES from ENV. Unset → all services enabled.
 * Unknown entries are ignored; if nothing valid remains, falls back to all
 * services rather than locking patrons out entirely on a config typo.
 */
export function getEnabledServices(): ServiceRoute[] {
  const raw = process.env.KIOSK_SERVICES?.trim()
  if (!raw) return ALL_SERVICES

  const requested = raw.split(',').map((s) => s.trim().toLowerCase())
  const enabled = ALL_SERVICES.filter((s) => requested.includes(s))
  return enabled.length > 0 ? enabled : ALL_SERVICES
}

export function isServiceEnabled(route: ServiceRoute): boolean {
  return getEnabledServices().includes(route)
}
