// SPDX-License-Identifier: GPL-3.0-or-later
import type { Metadata } from 'next'
import { NextIntlClientProvider } from 'next-intl'
import { getMessages } from 'next-intl/server'
import { notFound } from 'next/navigation'
import { locales } from '../../i18n'
import './globals.css'

export const metadata: Metadata = {
  title: 'Library Self-Check',
  description: 'Library self-check kiosk',
  // Prevent indexing — kiosk is LAN-only
  robots: 'noindex, nofollow',
}

interface RootLayoutProps {
  children: React.ReactNode
  params: { locale: string }
}

export default async function RootLayout({ children, params }: RootLayoutProps) {
  const { locale } = await params

  // Validate locale
  if (!locales.includes(locale as any)) notFound()

  // Load messages for this locale
  const messages = await getMessages()

  return (
    <html
      lang={locale}
      // data-theme set by ThemeProvider at runtime
      suppressHydrationWarning
    >
      <body className="min-h-screen bg-background text-foreground antialiased">
        <NextIntlClientProvider messages={messages}>
          {children}
        </NextIntlClientProvider>
      </body>
    </html>
  )
}

// Generate static params for all supported locales
export function generateStaticParams() {
  return locales.map((locale) => ({ locale }))
}
