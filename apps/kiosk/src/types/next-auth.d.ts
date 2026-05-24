// SPDX-License-Identifier: GPL-3.0-or-later
// Extend NextAuth Session and JWT with library patron fields

import type { DefaultSession } from 'next-auth'

declare module 'next-auth' {
  interface Session extends DefaultSession {
    patronId: string
    patronName: string | null
  }
}

declare module 'next-auth/jwt' {
  interface JWT {
    patronId: string
    patronName: string | null
  }
}
