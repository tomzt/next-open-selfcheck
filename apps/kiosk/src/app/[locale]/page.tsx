// SPDX-License-Identifier: GPL-3.0-or-later
// Phase 1 — Welcome Screen
// Video autoplay loop + logo + "Tap to begin" CTA
// Patron taps → redirect to /auth

import { useTranslations } from 'next-intl'
import { getTranslations } from 'next-intl/server'
import WelcomeScreen from '@/components/layout/WelcomeScreen'
import { libraryNameFor } from '@/lib/site-config'

interface Props {
  params: Promise<{ locale: string }>
}

export default async function HomePage({ params }: Props) {
  const { locale } = await params
  const t = await getTranslations('welcome')

  return (
    <WelcomeScreen
      ctaText={t('cta')}
      ctaSubText={t('cta_sub')}
      libraryName={libraryNameFor(locale)}
    />
  )
}
