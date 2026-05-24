'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// Client wrapper — provides NextAuth SessionProvider to the component tree

import { SessionProvider } from 'next-auth/react'

export default function Providers({ children }: { children: React.ReactNode }) {
  return <SessionProvider>{children}</SessionProvider>
}
