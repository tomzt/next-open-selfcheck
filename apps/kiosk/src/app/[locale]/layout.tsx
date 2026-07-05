// SPDX-License-Identifier: GPL-3.0-or-later
import type { Metadata } from 'next'
import { NextIntlClientProvider } from 'next-intl'
import { getMessages } from 'next-intl/server'
import { notFound } from 'next/navigation'
import { locales } from '../../../i18n'
import Providers from '@/components/Providers'
import { kioskTheme, inputMode } from '@/lib/theme'
import { libraryNameFor } from '@/lib/site-config'
import '../globals.css'

// Build per-locale metadata with the deployer's library name.
// Title format: "{Library Name} — Self-Check"
export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>
}): Promise<Metadata> {
  const { locale } = await params
  const libraryName = libraryNameFor(locale)
  return {
    title: `${libraryName} — Self-Check`,
    description: `${libraryName} self-check kiosk`,
    robots: 'noindex, nofollow',
  }
}

interface RootLayoutProps {
  children: React.ReactNode
  params: Promise<{ locale: string }>
}

export default async function RootLayout({ children, params }: RootLayoutProps) {
  const { locale } = await params

  if (!locales.includes(locale as any)) notFound()

  const messages = await getMessages()

  return (
    <html
      lang={locale}
      data-kiosk-theme={kioskTheme}
      data-input-mode={inputMode}
      suppressHydrationWarning
    >
      <body className="antialiased">
        <Providers>
          <NextIntlClientProvider messages={messages}>
            {children}
          </NextIntlClientProvider>
        </Providers>
      </body>
    </html>
  )
}

export function generateStaticParams() {
  return locales.map((locale) => ({ locale }))
}
