'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// AccessibilityBar — Font / Theme / Language toggles

import { useLocale, useTranslations } from 'next-intl'
import { useRouter, usePathname } from 'next/navigation'
import { useState, useEffect } from 'react'
import { Type, Sun, Globe } from 'lucide-react'
import { locales, type Locale } from '../../../i18n'

type FontSize = 'kiosk-sm' | 'kiosk-md' | 'kiosk-lg' | 'kiosk-xl'
type Theme = 'light' | 'dark' | 'high-contrast'

const FONT_SIZES: FontSize[] = ['kiosk-sm', 'kiosk-md', 'kiosk-lg', 'kiosk-xl']
const THEMES: Theme[] = ['light', 'dark', 'high-contrast']

// NEXT_PUBLIC_DEFAULT_FONT_SIZE uses friendly names — map to the internal
// CSS data-attribute values. Unknown/unset falls back to medium.
const FONT_SIZE_MAP: Record<string, FontSize> = {
  small: 'kiosk-sm',
  medium: 'kiosk-md',
  large: 'kiosk-lg',
  xlarge: 'kiosk-xl',
}
const DEFAULT_FONT: FontSize =
  FONT_SIZE_MAP[process.env.NEXT_PUBLIC_DEFAULT_FONT_SIZE ?? 'medium'] ?? 'kiosk-md'
const DEFAULT_THEME: Theme = 'light'

export default function AccessibilityBar() {
  const t = useTranslations('accessibility')
  const locale = useLocale()
  const router = useRouter()
  const pathname = usePathname()

  const [fontSize, setFontSize] = useState<FontSize>(DEFAULT_FONT)
  const [theme, setTheme] = useState<Theme>(DEFAULT_THEME)

  useEffect(() => {
    document.documentElement.dataset.fontSize = fontSize
  }, [fontSize])

  useEffect(() => {
    document.documentElement.dataset.theme = theme
  }, [theme])

  const cycleFont = (e: React.MouseEvent) => {
    e.stopPropagation()
    const idx = FONT_SIZES.indexOf(fontSize)
    setFontSize(FONT_SIZES[(idx + 1) % FONT_SIZES.length])
  }

  const cycleTheme = (e: React.MouseEvent) => {
    e.stopPropagation()
    const idx = THEMES.indexOf(theme)
    setTheme(THEMES[(idx + 1) % THEMES.length])
  }

  const cycleLocale = (e: React.MouseEvent) => {
    e.stopPropagation()
    const idx = locales.indexOf(locale as Locale)
    const nextLocale = locales[(idx + 1) % locales.length]
    router.replace(pathname.replace(`/${locale}`, `/${nextLocale}`))
  }

  return (
    <div className="flex gap-2" role="toolbar" aria-label={t('language')}>
      <button onClick={cycleFont} className="relative accessibility-btn" title={t('font_size')}>
        <Type className="w-4 h-4" />
        {fontSize !== DEFAULT_FONT && <span className="changed-dot" />}
      </button>
      <button onClick={cycleTheme} className="relative accessibility-btn" title={t('theme')}>
        <Sun className="w-4 h-4" />
        {theme !== DEFAULT_THEME && <span className="changed-dot" />}
      </button>
      <button onClick={cycleLocale} className="relative accessibility-btn" title={t('language')}>
        <Globe className="w-4 h-4" />
        <span className="text-xs ml-0.5 font-medium uppercase">{locale}</span>
      </button>
    </div>
  )
}
