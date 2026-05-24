// SPDX-License-Identifier: GPL-3.0-or-later
// NextAuth configuration — supports barcode/QR (via SIP2) and generic OIDC (Keycloak, Azure AD, Auth0…)
//
// Auth mode is controlled by the AUTH_MODE env var:
//   barcode (default) — patron ID from card scan / QR, validated via SIP2
//   oidc              — institutional SSO only
//   both              — patron can choose either method

import CredentialsProvider from 'next-auth/providers/credentials'
import type { NextAuthOptions } from 'next-auth'
import { authenticatePatron } from './sip2-patron'

export type AuthMode = 'barcode' | 'oidc' | 'both'
export const authMode = (process.env.AUTH_MODE ?? 'barcode') as AuthMode

// Generic OIDC provider — no vendor lock-in
// eslint-disable-next-line @typescript-eslint/no-explicit-any
function buildOidcProvider(): any | null {
  const issuer = process.env.OIDC_ISSUER
  const clientId = process.env.OIDC_CLIENT_ID
  const clientSecret = process.env.OIDC_CLIENT_SECRET
  if (!issuer || !clientId || !clientSecret) return null

  return {
    id: 'oidc',
    name: process.env.OIDC_PROVIDER_NAME ?? 'Institutional SSO',
    type: 'oauth',
    wellKnown: `${issuer}/.well-known/openid-configuration`,
    clientId,
    clientSecret,
    authorization: { params: { scope: 'openid email profile' } },
    idToken: true,
    checks: ['pkce', 'state'],
    profile(profile: Record<string, string>) {
      return {
        id: profile['sub'],
        name: profile['name'] ?? profile['preferred_username'] ?? '',
        email: profile['email'] ?? null,
      }
    },
  }
}

const devSecret =
  process.env.NODE_ENV !== 'production' ? 'dev-secret-change-in-production' : undefined

export const authOptions: NextAuthOptions = {
  secret: process.env.NEXTAUTH_SECRET ?? devSecret,

  session: {
    strategy: 'jwt',
    maxAge: 30 * 60, // 30 minutes — resets on each transaction
  },

  pages: {
    signIn: '/auth',
    error: '/auth',
  },

  providers: [
    ...(authMode !== 'oidc'
      ? [
          CredentialsProvider({
            id: 'barcode',
            name: 'Library Card',
            credentials: {
              patronId: { label: 'Card / QR', type: 'text' },
              pin: { label: 'PIN', type: 'password' },
            },
            async authorize(credentials) {
              if (!credentials?.patronId?.trim()) return null
              try {
                const result = await authenticatePatron(
                  credentials.patronId.trim(),
                  credentials.pin ?? '',
                )
                if (!result.valid) return null
                return {
                  id: result.patronId ?? credentials.patronId,
                  name: result.patronName ?? undefined,
                  email: null,
                }
              } catch (err) {
                console.error('[auth] SIP2 error:', err)
                return null
              }
            },
          }),
        ]
      : []),

    ...(authMode !== 'barcode'
      ? ([buildOidcProvider()].filter(Boolean) as NextAuthOptions['providers'])
      : []),
  ],

  callbacks: {
    jwt({ token, user }) {
      if (user) {
        token.patronId = user.id
        token.patronName = user.name ?? null
      }
      return token
    },
    session({ session, token }) {
      session.patronId = token.patronId as string
      session.patronName = token.patronName as string | null
      return session
    },
  },
}
