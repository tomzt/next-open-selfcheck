'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// SessionGuard — idle timeout with countdown warning

import { signOut } from 'next-auth/react'
import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useState, useCallback } from 'react'
import { Clock } from 'lucide-react'
import { useSessionTimeout } from '@/hooks/useSessionTimeout'

interface Props { children: React.ReactNode; timeoutSeconds: number }

export default function SessionGuard({ children, timeoutSeconds }: Props) {
  const t = useTranslations('session')
  const locale = useLocale()
  const router = useRouter()
  const [warningRemaining, setWarningRemaining] = useState<number | null>(null)

  const handleWarning = useCallback((remaining: number) => { setWarningRemaining(remaining) }, [])

  const handleExpired = useCallback(async () => {
    setWarningRemaining(null)
    await signOut({ redirect: false })
    router.replace(`/${locale}`)
  }, [locale, router])

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
        <div
          className="fixed inset-0 z-50 flex items-center justify-center animate-fade-in"
          style={{ backgroundColor: 'rgb(0 0 0 / 0.6)', backdropFilter: 'blur(4px)' }}
        >
          <div className="kt-card p-10 text-center shadow-2xl max-w-sm w-full mx-4">
            <div
              className="w-20 h-20 rounded-full flex items-center justify-center mx-auto mb-4"
              style={{ backgroundColor: 'rgb(var(--kt-error) / 0.1)' }}
            >
              <Clock className="w-10 h-10" style={{ color: 'rgb(var(--kt-error))' }} />
            </div>
            <p className="text-6xl font-bold mb-2" style={{ color: 'rgb(var(--kt-error))' }}>
              {warningRemaining}
            </p>
            <p className="text-lg font-medium mb-6" style={{ color: 'rgb(var(--kt-text))' }}>
              {t('timeout_warning', { seconds: warningRemaining })}
            </p>
            <button
              onClick={() => setWarningRemaining(null)}
              className="kt-btn-primary w-full"
            >
              {t('extend')}
            </button>
          </div>
        </div>
      )}
    </>
  )
}
