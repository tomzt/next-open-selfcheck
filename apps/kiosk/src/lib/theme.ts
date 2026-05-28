// SPDX-License-Identifier: GPL-3.0-or-later
// Kiosk visual theme — set via NEXT_PUBLIC_KIOSK_THEME env var.
// Libraries choose a theme during First-Run Setup Wizard.

export type KioskTheme = 'light' | 'dark' | 'colorful'

export const kioskTheme: KioskTheme =
  (process.env.NEXT_PUBLIC_KIOSK_THEME as KioskTheme) ?? 'light'
