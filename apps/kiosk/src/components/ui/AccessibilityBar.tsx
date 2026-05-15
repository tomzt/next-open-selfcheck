'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// AccessibilityBar — 3 separate buttons: Font (A) / Theme (☀️🌙) / Language (🌐)
// Patron settings reset to admin defaults after session timeout

import { useLocale, useTranslations } from 'next-intl'
import { useRouter, usePathname } from 'next/navigation'
import { useState, useEffect } from 'react'
import { locales, type Locale } from '../../../i18n'

type FontSize = 'sm' | 'md' | 'lg' | 'xl'
type Theme = 'light' | 'dark' | 'high-contrast'

const FONT_SIZES: FontSize[] = ['sm', 'md', 'lg', 'xl']
const THEMES: Theme[] = ['light', 'dark', 'high-contrast']

// Admin defaults — loaded from system_config in production
// Phase 1: use env defaults
const DEFAULT_FONT: FontSize = 'md'
const DEFAULT_THEME: Theme = 'light'

export default function AccessibilityBar() {
  const t = useTranslations('accessibility')
  const locale = useLocale()
  const router = useRouter()
  const pathname = usePathname()

  const [fontSize, setFontSize] = useState<FontSize>(DEFAULT_FONT)
  const [theme, setTheme] = useState<Theme>(DEFAULT_THEME)

  // Apply font size to root
  useEffect(() => {
    const sizeMap: Record<FontSize, string> = {
      sm: 'kiosk-sm', md: 'kiosk-md', lg: 'kiosk-lg', xl: 'kiosk-xl'
    }
    document.documentElement.dataset.fontSize = sizeMap[fontSize]
  }, [fontSize])

  // Apply theme to root
  useEffect(() => {
    document.documentElement.dataset.theme = theme
    document.documentElement.classList.remove('light', 'dark', 'high-contrast')
    document.documentElement.classList.add(theme)
  }, [theme])

  // Cycle font size
  const cycleFont = (e: React.MouseEvent) => {
    e.stopPropagation()
    const idx = FONT_SIZES.indexOf(fontSize)
    setFontSize(FONT_SIZES[(idx + 1) % FONT_SIZES.length])
  }

  // Cycle theme
  const cycleTheme = (e: React.MouseEvent) => {
    e.stopPropagation()
    const idx = THEMES.indexOf(theme)
    setTheme(THEMES[(idx + 1) % THEMES.length])
  }

  // Switch language
  const cycleLocale = (e: React.MouseEvent) => {
    e.stopPropagation()
    const idx = locales.indexOf(locale as Locale)
    const nextLocale = locales[(idx + 1) % locales.length]
    // Replace locale segment in path
    const newPath = pathname.replace(`/${locale}`, `/${nextLocale}`)
    router.replace(newPath)
  }

  const themeIcon = theme === 'dark' ? '🌙' : theme === 'high-contrast' ? '◑' : '☀️'
  const fontChanged = fontSize !== DEFAULT_FONT
  const themeChanged = theme !== DEFAULT_THEME

  return (
    <div className="flex gap-2" role="toolbar" aria-label={t('language')}>

      {/* Font size button */}
      <button
        onClick={cycleFont}
        className="relative accessibility-btn"
        aria-label={t('font_size')}
        title={t('font_size')}
      >
        <span className="font-bold">A</span>
        {fontChanged && <span className="changed-dot" />}
      </button>

      {/* Theme button */}
      <button
        onClick={cycleTheme}
        className="relative accessibility-btn"
        aria-label={t('theme')}
        title={t('theme')}
      >
        <span>{themeIcon}</span>
        {themeChanged && <span className="changed-dot" />}
      </button>

      {/* Language button */}
      <button
        onClick={cycleLocale}
        className="relative accessibility-btn"
        aria-label={t('language')}
        title={t('language')}
      >
        <span>🌐</span>
        <span className="text-xs ml-1 uppercase">{locale}</span>
      </button>

    </div>
  )
}
