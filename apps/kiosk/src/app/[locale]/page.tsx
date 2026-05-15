// SPDX-License-Identifier: GPL-3.0-or-later
// Phase 1 — Welcome Screen
// Video autoplay loop + logo + "Tap to begin" CTA
// Patron taps → redirect to /auth

import { useTranslations } from 'next-intl'
import { getTranslations } from 'next-intl/server'
import WelcomeScreen from '@/components/layout/WelcomeScreen'

export default async function HomePage() {
  const t = await getTranslations('welcome')

  return (
    <WelcomeScreen
      ctaText={t('cta')}
      ctaSubText={t('cta_sub')}
    />
  )
}
