// SPDX-License-Identifier: GPL-3.0-or-later
// Auth page — reads environment config server-side and passes to client AuthScreen.
// Already-authenticated patrons are redirected immediately to the menu.

import { getServerSession } from 'next-auth'
import { redirect } from 'next/navigation'
import { authOptions, authMode } from '@/lib/auth'
import AuthScreen from '@/components/auth/AuthScreen'

interface Props {
  params: Promise<{ locale: string }>
}

export default async function AuthPage({ params }: Props) {
  const { locale } = await params

  const session = await getServerSession(authOptions)
  if (session) redirect(`/${locale}/menu`)

  const pinRequired = process.env.AUTH_PIN_REQUIRED === 'true'
  const oidcProviderName = process.env.OIDC_PROVIDER_NAME ?? 'SSO'

  return (
    <AuthScreen
      authMode={authMode}
      pinRequired={pinRequired}
      oidcProviderName={oidcProviderName}
      locale={locale}
    />
  )
}
