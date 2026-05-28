'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// FinesScreen — patron outstanding fine balance

import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useEffect, useState } from 'react'
import { Receipt, ArrowLeft, CheckCircle2, AlertCircle } from 'lucide-react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

interface FinesData { totalAmount: number; currency: string }

export default function FinesScreen() {
  const t = useTranslations('transaction.fines')
  const tc = useTranslations('common')
  const locale = useLocale()
  const router = useRouter()
  const [status, setStatus] = useState<'loading' | 'loaded' | 'error'>('loading')
  const [fines, setFines] = useState<FinesData | null>(null)

  useEffect(() => {
    fetch('/api/sip2/fines')
      .then((r) => { if (r.status === 401) { router.replace(`/${locale}/auth`); return null }; return r.json() })
      .then((d) => { if (!d) return; setFines(d); setStatus('loaded') })
      .catch(() => setStatus('error'))
  }, [locale, router])

  const hasFines = fines && fines.totalAmount > 0
  const accentColor = 'rgb(245 158 11)'
  const accentBg = 'rgb(245 158 11 / 0.1)'

  return (
    <div className="relative w-screen h-screen overflow-hidden flex flex-col kt-screen">
      <header className="kt-nav flex items-center gap-4 px-8 py-5 shrink-0"
        >
        <button onClick={() => router.push(`/${locale}/menu`)}
          className="w-10 h-10 rounded-xl flex items-center justify-center transition-all active:scale-90"
          style={{ backgroundColor: 'rgb(var(--kt-surface-2))', color: 'rgb(var(--kt-text))' }}>
          <ArrowLeft className="w-5 h-5" />
        </button>
        <div className="flex items-center gap-3">
          <Receipt className="w-6 h-6" style={{ color: accentColor }} />
          <h1 className="text-2xl font-bold" style={{ color: 'rgb(var(--kt-text))' }}>{t('title')}</h1>
        </div>
      </header>

      <div className="flex-1 flex flex-col items-center justify-center px-8 gap-6">
        {status === 'loading' && (
          <p className="text-lg animate-pulse" style={{ color: 'rgb(var(--kt-text-muted))' }}>{tc('loading')}</p>
        )}
        {status === 'error' && (
          <div className="text-center flex flex-col items-center gap-4">
            <AlertCircle className="w-12 h-12" style={{ color: 'rgb(var(--kt-error))' }} />
            <button onClick={() => { setStatus('loading'); window.location.reload() }}
              className="kt-btn-primary">{tc('retry')}</button>
          </div>
        )}
        {status === 'loaded' && !hasFines && (
          <div className="text-center flex flex-col items-center gap-4">
            <div className="w-24 h-24 rounded-full flex items-center justify-center"
              style={{ backgroundColor: 'rgb(var(--kt-success) / 0.1)' }}>
              <CheckCircle2 className="w-12 h-12" style={{ color: 'rgb(var(--kt-success))' }} />
            </div>
            <p className="text-2xl font-bold" style={{ color: 'rgb(var(--kt-success))' }}>
              {t('no_fines')}
            </p>
          </div>
        )}
        {status === 'loaded' && hasFines && fines && (
          <div className="text-center flex flex-col items-center gap-4">
            <div className="w-24 h-24 rounded-full flex items-center justify-center" style={{ backgroundColor: accentBg }}>
              <Receipt className="w-12 h-12" style={{ color: accentColor }} />
            </div>
            <p className="text-3xl font-bold" style={{ color: 'rgb(var(--kt-error))' }}>
              {t('total', { amount: fines.totalAmount.toFixed(2) })}
            </p>
            <p className="text-base px-6 py-3 rounded-xl"
              style={{ backgroundColor: 'rgb(var(--kt-surface-2))', color: 'rgb(var(--kt-text-muted))' }}>
              {tc('contact_staff')}
            </p>
          </div>
        )}
      </div>

      <div className="absolute bottom-6 right-6 z-20"><AccessibilityBar /></div>
    </div>
  )
}
