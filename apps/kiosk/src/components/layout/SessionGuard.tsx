'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// SessionGuard — idle timeout with countdown warning.
// Wraps all /menu/** pages. Signs out and returns to welcome on expiry.

import { signOut } from 'next-auth/react'
import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useState, useCallback } from 'react'
import { useSessionTimeout } from '@/hooks/useSessionTimeout'

interface Props {
  children: React.ReactNode
  timeoutSeconds: number
}

export default function SessionGuard({ children, timeoutSeconds }: Props) {
  const t = useTranslations('session')
  const locale = useLocale()
  const router = useRouter()
  const [warningRemaining, setWarningRemaining] = useState<number | null>(null)

  const handleWarning = useCallback((remaining: number) => {
    setWarningRemaining(remaining)
  }, [])

  const handleExpired = useCallback(async () => {
    setWarningRemaining(null)
    await signOut({ redirect: false })
    router.replace(`/${locale}`)
  }, [locale, router])

  const handleExtend = () => {
    setWarningRemaining(null)
  }

  useSessionTimeout({
    timeoutSeconds,
    warningSeconds: Math.min(30, Math.floor(timeoutSeconds / 3)),
    onWarning: handleWarning,
    onExpired: handleExpired,
  })

  return (
    <>
      {children}

      {warningRemaining !== null && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 animate-fade-in">
          <div className="bg-white dark:bg-slate-800 rounded-2xl p-10 text-center shadow-2xl max-w-sm w-full mx-4">
            <p className="text-5xl font-bold text-red-500 mb-4">{warningRemaining}</p>
            <p className="text-xl font-semibold text-slate-800 dark:text-white mb-2">
              {t('timeout_warning', { seconds: warningRemaining })}
            </p>
            <button
              onClick={handleExtend}
              className="mt-6 w-full py-4 rounded-xl bg-primary text-primary-foreground text-xl font-bold transition-colors hover:opacity-90 active:opacity-80"
            >
              {t('extend')}
            </button>
          </div>
        </div>
      )}
    </>
  )
}
