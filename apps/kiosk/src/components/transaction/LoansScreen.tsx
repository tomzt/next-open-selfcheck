'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// LoansScreen — patron current loans list

import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useEffect, useState } from 'react'
import { BookCopy, ArrowLeft, Calendar, AlertCircle, BookOpen } from 'lucide-react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

interface LoanItem { itemBarcode: string; title: string | null; dueDate: string | null }

function isOverdue(dueDate: string | null) {
  if (!dueDate) return false
  const d = new Date(dueDate)
  return !isNaN(d.getTime()) && d < new Date()
}

export default function LoansScreen() {
  const t = useTranslations('transaction.status')
  const tc = useTranslations('common')
  const locale = useLocale()
  const router = useRouter()
  const [status, setStatus] = useState<'loading' | 'loaded' | 'error'>('loading')
  const [loans, setLoans] = useState<LoanItem[]>([])

  useEffect(() => {
    fetch('/api/sip2/loans')
      .then((r) => { if (r.status === 401) { router.replace(`/${locale}/auth`); return null }; return r.json() })
      .then((d) => { if (!d) return; setLoans(d.loans ?? []); setStatus('loaded') })
      .catch(() => setStatus('error'))
  }, [locale, router])

  const accentColor = 'rgb(139 92 246)'
  const accentBg = 'rgb(139 92 246 / 0.1)'

  return (
    <div className="relative w-screen h-screen overflow-hidden flex flex-col" style={{ backgroundColor: 'rgb(var(--kt-bg))' }}>
      <header className="flex items-center gap-4 px-8 py-5 border-b shrink-0"
        style={{ backgroundColor: 'rgb(var(--kt-nav-bg))', borderColor: 'rgb(var(--kt-border))' }}>
        <button onClick={() => router.push(`/${locale}/menu`)}
          className="w-10 h-10 rounded-xl flex items-center justify-center transition-all active:scale-90"
          style={{ backgroundColor: 'rgb(var(--kt-surface-2))', color: 'rgb(var(--kt-text))' }}>
          <ArrowLeft className="w-5 h-5" />
        </button>
        <div className="flex items-center gap-3">
          <BookCopy className="w-6 h-6" style={{ color: accentColor }} />
          <h1 className="text-2xl font-bold" style={{ color: 'rgb(var(--kt-text))' }}>{t('title')}</h1>
        </div>
      </header>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        {status === 'loading' && (
          <div className="flex items-center justify-center h-full">
            <p className="text-lg animate-pulse" style={{ color: 'rgb(var(--kt-text-muted))' }}>{tc('loading')}</p>
          </div>
        )}

        {status === 'error' && (
          <div className="flex flex-col items-center justify-center h-full gap-4">
            <AlertCircle className="w-12 h-12" style={{ color: 'rgb(var(--kt-error))' }} />
            <button onClick={() => { setStatus('loading'); window.location.reload() }}
              className="kt-btn-primary">{tc('retry')}</button>
          </div>
        )}

        {status === 'loaded' && loans.length === 0 && (
          <div className="flex flex-col items-center justify-center h-full gap-4">
            <div className="w-20 h-20 rounded-full flex items-center justify-center" style={{ backgroundColor: accentBg }}>
              <BookOpen className="w-10 h-10" style={{ color: accentColor }} />
            </div>
            <p className="text-xl" style={{ color: 'rgb(var(--kt-text-muted))' }}>{t('no_items')}</p>
          </div>
        )}

        {status === 'loaded' && loans.length > 0 && (
          <>
            <p className="text-sm font-medium mb-4" style={{ color: 'rgb(var(--kt-text-muted))' }}>
              {t('item_count', { count: loans.length })}
            </p>
            <ul className="space-y-3">
              {loans.map((loan) => {
                const overdue = isOverdue(loan.dueDate)
                return (
                  <li key={loan.itemBarcode} className="kt-card p-4 shadow-sm">
                    <p className="font-semibold text-lg leading-tight" style={{ color: 'rgb(var(--kt-text))' }}>
                      {loan.title ?? loan.itemBarcode}
                    </p>
                    {loan.dueDate && (
                      <div className={`flex items-center gap-1.5 mt-2 text-sm font-medium`}
                        style={{ color: overdue ? 'rgb(var(--kt-error))' : 'rgb(var(--kt-text-muted))' }}>
                        {overdue
                          ? <AlertCircle className="w-3.5 h-3.5 shrink-0" />
                          : <Calendar className="w-3.5 h-3.5 shrink-0" />
                        }
                        {t('due_date')}: {loan.dueDate}
                        {overdue && <span className="ml-1 font-bold">— {t('overdue')}</span>}
                      </div>
                    )}
                  </li>
                )
              })}
            </ul>
          </>
        )}
      </div>

      <div className="absolute bottom-6 right-6 z-20"><AccessibilityBar /></div>
    </div>
  )
}
