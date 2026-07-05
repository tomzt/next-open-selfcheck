// SPDX-License-Identifier: GPL-3.0-or-later
// Kiosk visual theme — set via NEXT_PUBLIC_KIOSK_THEME env var.
// Libraries choose a theme during First-Run Setup Wizard.

export type KioskTheme = 'light' | 'dark' | 'colorful' | 'glass' | 'material'

export const kioskTheme: KioskTheme =
  (process.env.NEXT_PUBLIC_KIOSK_THEME as KioskTheme) ?? 'light'

// Input mode — set via NEXT_PUBLIC_INPUT_MODE. 'touch' hides the cursor and
// disables hover effects (a stuck :hover after tapping looks like a bug).
export type InputMode = 'mouse' | 'touch'

export const inputMode: InputMode =
  process.env.NEXT_PUBLIC_INPUT_MODE === 'touch' ? 'touch' : 'mouse'
