'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// BorrowScreen — patron scans item barcode → SIP2 checkout → result

import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useEffect, useRef, useState } from 'react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

type Phase = 'idle' | 'loading' | 'success' | 'error'

interface SuccessData {
  title: string | null
  dueDate: string | null
  itemBarcode: string
}

export default function BorrowScreen() {
  const t = useTranslations('transaction.borrow')
  const tc = useTranslations('common')
  const te = useTranslations('error')
  const locale = useLocale()
  const router = useRouter()

  const [phase, setPhase] = useState<Phase>('idle')
  const [barcode, setBarcode] = useState('')
  const [success, setSuccess] = useState<SuccessData | null>(null)
  const [errorMsg, setErrorMsg] = useState<string | null>(null)

  const inputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    if (phase === 'idle') inputRef.current?.focus()
  }, [phase])

  const handleScan = async (scanned: string) => {
    if (!scanned.trim() || phase === 'loading') return
    setPhase('loading')

    try {
      const res = await fetch('/api/sip2/checkout', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ itemBarcode: scanned.trim() }),
      })

      if (res.status === 401) {
        router.replace(`/${locale}/auth`)
        return
      }

      const data = await res.json()

      if (data.ok) {
        setSuccess({ title: data.title, dueDate: data.dueDate, itemBarcode: data.itemBarcode })
        setPhase('success')
      } else {
        setErrorMsg(te('item_not_available'))
        setPhase('error')
      }
    } catch {
      setErrorMsg(te('generic'))
      setPhase('error')
    }
  }

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault()
      handleScan(barcode)
    }
  }

  const handleBorrowAnother = () => {
    setBarcode('')
    setSuccess(null)
    setErrorMsg(null)
    setPhase('idle')
  }

  const handleDone = () => {
    router.push(`/${locale}/menu`)
  }

  return (
    <div className="relative w-screen h-screen overflow-hidden flex flex-col items-center justify-center bg-slate-50 dark:bg-slate-900 px-8">

      {/* Back button */}
      <button
        onClick={handleDone}
        className="absolute top-6 left-6 text-slate-500 dark:text-slate-400 hover:text-slate-800 dark:hover:text-white text-lg transition-colors"
      >
        ← {tc('back')}
      </button>

      <div className="w-full max-w-lg animate-fade-in">
        {/* Idle — scan input */}
        {phase === 'idle' && (
          <div className="text-center">
            <div className="text-7xl mb-6">📖</div>
            <h1 className="text-3xl font-bold text-slate-800 dark:text-white mb-2">{t('title')}</h1>
            <p className="text-lg text-slate-500 dark:text-slate-400 mb-8">{t('instruction')}</p>
            <input
              ref={inputRef}
              type="text"
              value={barcode}
              onChange={(e) => setBarcode(e.target.value)}
              onKeyDown={handleKeyDown}
              autoComplete="off"
              autoCorrect="off"
              spellCheck={false}
              className="w-full px-5 py-5 rounded-xl border-2 border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-slate-900 dark:text-white text-2xl text-center focus:outline-none focus:border-blue-500 transition-colors"
              placeholder="_ _ _ _ _ _ _ _"
            />
          </div>
        )}

        {/* Loading */}
        {phase === 'loading' && (
          <div className="text-center">
            <div className="text-7xl mb-6 animate-pulse">⏳</div>
            <p className="text-2xl text-slate-600 dark:text-slate-300">{t('scanning')}</p>
          </div>
        )}

        {/* Success */}
        {phase === 'success' && success && (
          <div className="text-center">
            <div className="text-7xl mb-6">✅</div>
            <h2 className="text-3xl font-bold text-emerald-600 mb-3">{t('success')}</h2>
            {success.title && (
              <p className="text-xl text-slate-700 dark:text-slate-200 font-medium mb-2">
                {success.title}
              </p>
            )}
            {success.dueDate && (
              <p className="text-lg text-slate-500 dark:text-slate-400 mb-8">
                {t('success_detail', { dueDate: success.dueDate })}
              </p>
            )}
            <div className="flex gap-4">
              <button
                onClick={handleBorrowAnother}
                className="flex-1 py-4 rounded-xl bg-blue-600 text-white text-xl font-bold hover:bg-blue-500 active:scale-95 transition-all"
              >
                {t('add_more')}
              </button>
              <button
                onClick={handleDone}
                className="flex-1 py-4 rounded-xl bg-slate-200 dark:bg-slate-700 text-slate-800 dark:text-white text-xl font-bold hover:opacity-90 active:scale-95 transition-all"
              >
                {t('done')}
              </button>
            </div>
          </div>
        )}

        {/* Error */}
        {phase === 'error' && (
          <div className="text-center">
            <div className="text-7xl mb-6">❌</div>
            <h2 className="text-3xl font-bold text-red-500 mb-3">{t('failed')}</h2>
            {errorMsg && (
              <p className="text-lg text-slate-500 dark:text-slate-400 mb-8">{errorMsg}</p>
            )}
            <div className="flex gap-4">
              <button
                onClick={handleBorrowAnother}
                className="flex-1 py-4 rounded-xl bg-blue-600 text-white text-xl font-bold hover:bg-blue-500 active:scale-95 transition-all"
              >
                {tc('retry')}
              </button>
              <button
                onClick={handleDone}
                className="flex-1 py-4 rounded-xl bg-slate-200 dark:bg-slate-700 text-slate-800 dark:text-white text-xl font-bold hover:opacity-90 active:scale-95 transition-all"
              >
                {t('done')}
              </button>
            </div>
          </div>
        )}
      </div>

      <div className="absolute bottom-6 right-6 z-20">
        <AccessibilityBar />
      </div>
    </div>
  )
}
