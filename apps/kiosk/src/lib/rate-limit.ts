// SPDX-License-Identifier: GPL-3.0-or-later
// Simple in-memory rate limiter for the barcode login endpoint.
//
// Each kiosk is its own single-instance Docker deployment (no central
// server, per docs/architecture.md), so an in-memory limiter is enough —
// no Redis/shared-state needed. This only protects against rapid patron-ID
// guessing from one kiosk; it resets on restart, which is an acceptable
// trade-off for a physical terminal with a single well-known IP.

const WINDOW_MS = 60_000
const MAX_ATTEMPTS = 5
const LOCKOUT_MS = 5 * 60_000

interface Entry {
  failures: number[]
  lockedUntil: number | null
}

const attempts = new Map<string, Entry>()

/** Returns true if this key is currently locked out from further attempts. */
export function isLockedOut(key: string): boolean {
  const entry = attempts.get(key)
  if (!entry?.lockedUntil) return false
  if (Date.now() >= entry.lockedUntil) {
    attempts.delete(key)
    return false
  }
  return true
}

/** Record a failed login attempt; locks the key out once MAX_ATTEMPTS is hit. */
export function recordFailure(key: string): void {
  const now = Date.now()
  const entry = attempts.get(key) ?? { failures: [], lockedUntil: null }
  entry.failures = entry.failures.filter((t) => now - t < WINDOW_MS)
  entry.failures.push(now)
  if (entry.failures.length >= MAX_ATTEMPTS) {
    entry.lockedUntil = now + LOCKOUT_MS
  }
  attempts.set(key, entry)
}

/** Clear failure history on a successful login. */
export function recordSuccess(key: string): void {
  attempts.delete(key)
}

/** Mask a patron ID for logging — never write the full ID to logs/disk. */
export function maskId(id: string): string {
  if (id.length <= 4) return '*'.repeat(id.length)
  return id.slice(0, 2) + '*'.repeat(id.length - 4) + id.slice(-2)
}
