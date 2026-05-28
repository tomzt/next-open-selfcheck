'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// Idle-based session timeout hook.
// Resets on any user activity (click, touch, keydown).

import { useEffect, useRef } from 'react'

interface Options {
  timeoutSeconds: number
  warningSeconds?: number
  onWarning: (remainingSeconds: number) => void
  onExpired: () => void
}

export function useSessionTimeout({
  timeoutSeconds,
  warningSeconds = 30,
  onWarning,
  onExpired,
}: Options) {
  const lastActivity = useRef(Date.now())
  const onWarningRef = useRef(onWarning)
  const onExpiredRef = useRef(onExpired)

  // Keep callbacks current without re-triggering the effect
  useEffect(() => {
    onWarningRef.current = onWarning
    onExpiredRef.current = onExpired
  })

  useEffect(() => {
    const resetActivity = () => {
      lastActivity.current = Date.now()
    }

    window.addEventListener('click', resetActivity)
    window.addEventListener('touchstart', resetActivity)
    window.addEventListener('keydown', resetActivity)

    const interval = setInterval(() => {
      const elapsed = (Date.now() - lastActivity.current) / 1000
      const remaining = timeoutSeconds - elapsed

      if (remaining <= 0) {
        onExpiredRef.current()
      } else if (remaining <= warningSeconds) {
        onWarningRef.current(Math.ceil(remaining))
      }
    }, 1000)

    return () => {
      window.removeEventListener('click', resetActivity)
      window.removeEventListener('touchstart', resetActivity)
      window.removeEventListener('keydown', resetActivity)
      clearInterval(interval)
    }
  }, [timeoutSeconds, warningSeconds])
}
