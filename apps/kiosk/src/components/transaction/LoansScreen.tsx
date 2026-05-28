'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// LoansScreen — displays patron's current loans fetched from /api/sip2/loans

import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useEffect, useState } from 'react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

interface LoanItem {
  itemBarcode: string
  title: string | null
  dueDate: string | null
}

type Status = 'loading' | 'loaded' | 'error'

function isOverdue(dueDate: string | null): boolean {
  if (!dueDate) return false
  const due = new Date(dueDate)
  return !isNaN(due.getTime()) && due < new Date()
}

export default function LoansScreen() {
  const t = useTranslations('transaction.status')
  const tc = useTranslations('common')
  const locale = useLocale()
  const router = useRouter()

  const [status, setStatus] = useState<Status>('loading')
  const [loans, setLoans] = useState<LoanItem[]>([])

  useEffect(() => {
    fetch('/api/sip2/loans')
      .then((res) => {
        if (res.status === 401) {
          router.replace(`/${locale}/auth`)
          return null
        }
        return res.json()
      })
      .then((data) => {
        if (!data) return
        setLoans(data.loans ?? [])
        setStatus('loaded')
      })
      .catch(() => setStatus('error'))
  }, [locale, router])

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
      <div className="flex-1 overflow-y-auto px-6 py-4">
        {status === 'loading' && (
          <div className="flex items-center justify-center h-full">
            <p className="text-xl text-slate-500 dark:text-slate-400 animate-pulse">
              {tc('loading')}
            </p>
          </div>
        )}

        {status === 'error' && (
          <div className="flex flex-col items-center justify-center h-full gap-4">
            <p className="text-xl text-red-500">{tc('retry')}</p>
            <button
              onClick={() => { setStatus('loading'); window.location.reload() }}
              className="px-6 py-3 rounded-xl bg-blue-600 text-white font-bold"
            >
              {tc('retry')}
            </button>
          </div>
        )}

        {status === 'loaded' && loans.length === 0 && (
          <div className="flex flex-col items-center justify-center h-full">
            <div className="text-6xl mb-4">📚</div>
            <p className="text-xl text-slate-500 dark:text-slate-400">{t('no_items')}</p>
          </div>
        )}

        {status === 'loaded' && loans.length > 0 && (
          <>
            <p className="text-base text-slate-500 dark:text-slate-400 mb-4">
              {t('item_count', { count: loans.length })}
            </p>
            <ul className="space-y-3">
              {loans.map((loan) => {
                const overdue = isOverdue(loan.dueDate)
                return (
                  <li
                    key={loan.itemBarcode}
                    className="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-sm border border-slate-200 dark:border-slate-700"
                  >
                    <p className="font-semibold text-slate-800 dark:text-white text-lg leading-tight">
                      {loan.title ?? loan.itemBarcode}
                    </p>
                    {loan.dueDate && (
                      <p className={`text-sm mt-1 ${overdue ? 'text-red-500 font-semibold' : 'text-slate-500 dark:text-slate-400'}`}>
                        {t('due_date')}: {loan.dueDate}
                        {overdue && ` — ${t('overdue')}`}
                      </p>
                    )}
                  </li>
                )
              })}
            </ul>
          </>
        )}
      </div>

      <div className="absolute bottom-6 right-6 z-20">
        <AccessibilityBar />
      </div>
    </div>
  )
}
