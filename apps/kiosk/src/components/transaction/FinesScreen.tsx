'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// FinesScreen — displays patron's outstanding fine balance from /api/sip2/fines

import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useEffect, useState } from 'react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

interface FinesData {
  totalAmount: number
  currency: string
}

type Status = 'loading' | 'loaded' | 'error'

export default function FinesScreen() {
  const t = useTranslations('transaction.fines')
  const tc = useTranslations('common')
  const locale = useLocale()
  const router = useRouter()

  const [status, setStatus] = useState<Status>('loading')
  const [fines, setFines] = useState<FinesData | null>(null)

  useEffect(() => {
    fetch('/api/sip2/fines')
      .then((res) => {
        if (res.status === 401) {
          router.replace(`/${locale}/auth`)
          return null
        }
        return res.json()
      })
      .then((data) => {
        if (!data) return
        setFines(data)
        setStatus('loaded')
      })
      .catch(() => setStatus('error'))
  }, [locale, router])

  const hasFines = fines && fines.totalAmount > 0

  return (
    <div className="relative w-screen h-screen overflow-hidden flex flex-col bg-slate-50 dark:bg-slate-900">

      {/* Header */}
      <div className="flex items-center gap-4 px-6 pt-6 pb-4 border-b border-slate-200 dark:border-slate-700">
        <button
          onClick={() => router.push(`/${locale}/menu`)}
          className="text-slate-500 dark:text-slate-400 hover:text-slate-800 dark:hover:text-white text-lg transition-colors"
        >
          ←
        </button>
        <h1 className="text-2xl font-bold text-slate-800 dark:text-white">{t('title')}</h1>
      </div>

      {/* Content */}
      <div className="flex-1 flex flex-col items-center justify-center px-8">
        {status === 'loading' && (
          <p className="text-xl text-slate-500 dark:text-slate-400 animate-pulse">
            {tc('loading')}
          </p>
        )}

        {status === 'error' && (
          <div className="text-center">
            <p className="text-xl text-red-500 mb-4">{tc('retry')}</p>
            <button
              onClick={() => { setStatus('loading'); window.location.reload() }}
              className="px-6 py-3 rounded-xl bg-blue-600 text-white font-bold"
            >
              {tc('retry')}
            </button>
          </div>
        )}

        {status === 'loaded' && !hasFines && (
          <div className="text-center">
            <div className="text-8xl mb-6">🎉</div>
            <p className="text-2xl font-bold text-emerald-600 dark:text-emerald-400">
              {t('no_fines')}
            </p>
          </div>
        )}

        {status === 'loaded' && hasFines && fines && (
          <div className="text-center">
            <div className="text-8xl mb-6">💰</div>
            <p className="text-2xl font-bold text-red-500 mb-2">
              {t('total', { amount: fines.totalAmount.toFixed(2) })}
            </p>
            <p className="text-base text-slate-500 dark:text-slate-400 mt-4">
              {tc('contact_staff')}
            </p>
          </div>
        )}
      </div>

      <div className="absolute bottom-6 right-6 z-20">
        <AccessibilityBar />
      </div>
    </div>
  )
}
