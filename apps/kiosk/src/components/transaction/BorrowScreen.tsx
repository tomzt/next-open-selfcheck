'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// BorrowScreen — scan barcode → SIP2 checkout → result

import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useEffect, useRef, useState } from 'react'
import { BookOpen, ScanLine, CheckCircle2, XCircle, ArrowLeft } from 'lucide-react'
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
      if (res.status === 401) { router.replace(`/${locale}/auth`); return }
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

  const reset = () => { setBarcode(''); setSuccess(null); setErrorMsg(null); setPhase('idle') }

  return (
    <div className="kt-screen relative w-screen h-screen overflow-hidden flex flex-col">
      {/* Header */}
      <header
        className="kt-nav flex items-center gap-4 px-8 py-5 shrink-0"
        
      >
        <button
          onClick={() => router.push(`/${locale}/menu`)}
          className="w-10 h-10 rounded-xl flex items-center justify-center transition-all active:scale-90"
          style={{ backgroundColor: 'rgb(var(--kt-surface-2))', color: 'rgb(var(--kt-text))' }}
        >
          <ArrowLeft className="w-5 h-5" />
        </button>
        <div className="flex items-center gap-3">
          <BookOpen className="w-6 h-6" style={{ color: 'rgb(var(--kt-primary))' }} />
          <h1 className="text-2xl font-bold" style={{ color: 'rgb(var(--kt-text))' }}>{t('title')}</h1>
        </div>
      </header>

      {/* Content */}
      <div className="flex-1 flex items-center justify-center px-8 pb-8">
        <div className="w-full max-w-md animate-fade-in">

          {phase === 'idle' && (
            <div className="text-center">
              <div
                className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6"
                style={{ backgroundColor: 'rgb(59 130 246 / 0.1)' }}
              >
                <ScanLine className="w-12 h-12" style={{ color: 'rgb(59 130 246)' }} />
              </div>
              <p className="text-lg mb-6" style={{ color: 'rgb(var(--kt-text-muted))' }}>{t('instruction')}</p>
              <input
                ref={inputRef}
                type="text"
                value={barcode}
                onChange={(e) => setBarcode(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleScan(barcode)}
                autoComplete="off"
                autoCorrect="off"
                spellCheck={false}
                className="kt-input"
                placeholder="_ _ _ _ _ _ _ _"
              />
            </div>
          )}

          {phase === 'loading' && (
            <div className="text-center">
              <div
                className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6 animate-pulse"
                style={{ backgroundColor: 'rgb(59 130 246 / 0.1)' }}
              >
                <ScanLine className="w-12 h-12" style={{ color: 'rgb(59 130 246)' }} />
              </div>
              <p className="text-xl" style={{ color: 'rgb(var(--kt-text-muted))' }}>{t('scanning')}</p>
            </div>
          )}

          {phase === 'success' && success && (
            <div className="text-center">
              <div className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6"
                style={{ backgroundColor: 'rgb(var(--kt-success) / 0.1)' }}>
                <CheckCircle2 className="w-12 h-12" style={{ color: 'rgb(var(--kt-success))' }} />
              </div>
              <h2 className="text-2xl font-bold mb-2" style={{ color: 'rgb(var(--kt-success))' }}>{t('success')}</h2>
              {success.title && (
                <p className="text-lg font-medium mb-1" style={{ color: 'rgb(var(--kt-text))' }}>{success.title}</p>
              )}
              {success.dueDate && (
                <p className="text-base mb-8" style={{ color: 'rgb(var(--kt-text-muted))' }}>
                  {t('success_detail', { dueDate: success.dueDate })}
                </p>
              )}
              <div className="flex gap-3">
                <button onClick={reset} className="kt-btn-primary flex-1">{t('add_more')}</button>
                <button onClick={() => router.push(`/${locale}/menu`)} className="kt-btn-ghost flex-1">{t('done')}</button>
              </div>
            </div>
          )}

          {phase === 'error' && (
            <div className="text-center">
              <div className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6"
                style={{ backgroundColor: 'rgb(var(--kt-error) / 0.1)' }}>
                <XCircle className="w-12 h-12" style={{ color: 'rgb(var(--kt-error))' }} />
              </div>
              <h2 className="text-2xl font-bold mb-2" style={{ color: 'rgb(var(--kt-error))' }}>{t('failed')}</h2>
              {errorMsg && <p className="text-base mb-8" style={{ color: 'rgb(var(--kt-text-muted))' }}>{errorMsg}</p>}
              <div className="flex gap-3">
                <button onClick={reset} className="kt-btn-primary flex-1">{tc('retry')}</button>
                <button onClick={() => router.push(`/${locale}/menu`)} className="kt-btn-ghost flex-1">{t('done')}</button>
              </div>
            </div>
          )}

        </div>
      </div>

      <div className="absolute bottom-6 right-6 z-20">
        <AccessibilityBar />
      </div>
    </div>
  )
}
