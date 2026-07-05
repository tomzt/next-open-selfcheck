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
import { isLockedOut, recordFailure, recordSuccess, maskId } from './rate-limit'
import { nextAuthSecret } from './auth-secret'

export type AuthMode = 'barcode' | 'oidc' | 'both'
export const authMode = (process.env.AUTH_MODE ?? 'barcode') as AuthMode

// Generic OIDC provider — no vendor lock-in.
//
// patron ID mapping: the claim that carries the library patron identifier
// varies by institution. Keycloak often exposes it as `preferred_username`
// (the login username = student/staff ID), but some IdPs use `student_id`,
// `library_barcode`, `employee_number`, etc. The claim is configurable via
// OIDC_PATRON_ID_CLAIM (default: preferred_username). It must resolve to the
// same identifier the ILS expects in SIP2 message 63's AA field.
//
// eslint-disable-next-line @typescript-eslint/no-explicit-any
function buildOidcProvider(): any | null {
  const issuer = process.env.OIDC_ISSUER
  const clientId = process.env.OIDC_CLIENT_ID
  const clientSecret = process.env.OIDC_CLIENT_SECRET
  if (!issuer || !clientId || !clientSecret) return null

  try {
    const parsed = new URL(issuer)
    if (parsed.protocol !== 'https:' && parsed.protocol !== 'http:') {
      throw new Error('unsupported protocol')
    }
  } catch {
    console.error(`[auth] OIDC_ISSUER is not a valid http(s) URL: ${issuer}`)
    return null
  }

  const patronIdClaim =
    process.env.OIDC_PATRON_ID_CLAIM?.trim() || 'preferred_username'

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
      // The patron identifier — falls back to sub only if the configured
      // claim is entirely absent (degraded mode; deployer likely misconfigured).
      const patronId = profile[patronIdClaim] ?? profile['sub']

      return {
        id: patronId,
        name: profile['name'] ?? profile['preferred_username'] ?? '',
        email: profile['email'] ?? null,
      }
    },
  }
}

export const authOptions: NextAuthOptions = {
  secret: nextAuthSecret,

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
            async authorize(credentials, req) {
              if (!credentials?.patronId?.trim()) return null

              const patronId = credentials.patronId.trim()
              const ip =
                (req?.headers as Record<string, string> | undefined)?.['x-forwarded-for']
                  ?.split(',')[0]
                  ?.trim() ?? 'unknown'
              const rateLimitKey = `${ip}:${patronId}`

              if (isLockedOut(rateLimitKey) || isLockedOut(ip)) {
                console.warn(
                  `[auth] Blocked login attempt (rate limited) for patronId=${maskId(patronId)} from ${ip}`,
                )
                return null
              }

              try {
                const result = await authenticatePatron(patronId, credentials.pin ?? '')
                if (!result.valid) {
                  recordFailure(rateLimitKey)
                  recordFailure(ip)
                  console.warn(
                    `[auth] Failed login attempt for patronId=${maskId(patronId)} from ${ip}`,
                  )
                  return null
                }
                recordSuccess(rateLimitKey)
                recordSuccess(ip)
                return {
                  id: result.patronId ?? patronId,
                  name: result.patronName ?? undefined,
                  email: null,
                }
              } catch (err) {
                recordFailure(rateLimitKey)
                recordFailure(ip)
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
