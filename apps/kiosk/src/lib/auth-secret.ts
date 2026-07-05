// SPDX-License-Identifier: GPL-3.0-or-later
// Shared NEXTAUTH_SECRET resolution — used by both auth.ts and middleware.ts.
// Kept dependency-free (no `net`, no other lib imports) so it stays safe to
// import from Next.js middleware, which may run on the Edge runtime.
//
// Fail closed: a production deployment with no NEXTAUTH_SECRET must refuse
// to boot rather than silently sign/verify JWTs with a well-known dev secret.
//
// Skipped during `next build` (NEXT_PHASE=phase-production-build): the build
// step imports route modules for static analysis with NODE_ENV=production
// already set but before the real runtime .env has been supplied, so this
// module gets evaluated long before there's a secret to check.
const isBuildPhase = process.env.NEXT_PHASE === 'phase-production-build'

if (process.env.NODE_ENV === 'production' && !process.env.NEXTAUTH_SECRET && !isBuildPhase) {
  throw new Error(
    'NEXTAUTH_SECRET is required in production — refusing to start with an insecure default. ' +
      'Set it in apps/kiosk/.env (see .env.example).',
  )
}

export const nextAuthSecret: string =
  process.env.NEXTAUTH_SECRET ??
  // Only reachable in non-production (dev/test) — the check above already
  // guarantees a real secret is set whenever NODE_ENV is 'production'.
  'dev-secret-change-in-production'
