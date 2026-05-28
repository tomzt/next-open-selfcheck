'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// ReturnScreen — scan barcode → SIP2 checkin → result

import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useEffect, useRef, useState } from 'react'
import { CornerDownLeft, ScanLine, CheckCircle2, XCircle, AlertTriangle, ArrowLeft } from 'lucide-react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

type Phase = 'idle' | 'loading' | 'success' | 'error'
interface SuccessData { title: string | null; itemBarcode: string; alert: boolean }

export default function ReturnScreen() {
  const t = useTranslations('transaction.return')
  const tc = useTranslations('common')
  const te = useTranslations('error')
  const locale = useLocale()
  const router = useRouter()

  const [phase, setPhase] = useState<Phase>('idle')
  const [barcode, setBarcode] = useState('')
  const [success, setSuccess] = useState<SuccessData | null>(null)
  const [errorMsg, setErrorMsg] = useState<string | null>(null)
  const inputRef = useRef<HTMLInputElement>(null)

  useEffect(() => { if (phase === 'idle') inputRef.current?.focus() }, [phase])

  const handleScan = async (scanned: string) => {
    if (!scanned.trim() || phase === 'loading') return
    setPhase('loading')
    try {
      const res = await fetch('/api/sip2/checkin', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ itemBarcode: scanned.trim() }),
      })
      if (res.status === 401) { router.replace(`/${locale}/auth`); return }
      const data = await res.json()
      if (data.ok) {
        setSuccess({ title: data.title, itemBarcode: data.itemBarcode, alert: data.alert })
        setPhase('success')
      } else {
        setErrorMsg(te('generic'))
        setPhase('error')
      }
    } catch {
      setErrorMsg(te('generic'))
      setPhase('error')
    }
  }

  const reset = () => { setBarcode(''); setSuccess(null); setErrorMsg(null); setPhase('idle') }

  // Emerald for return
  const accentColor = 'rgb(16 185 129)'
  const accentBg = 'rgb(16 185 129 / 0.1)'

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
          <CornerDownLeft className="w-6 h-6" style={{ color: accentColor }} />
          <h1 className="text-2xl font-bold" style={{ color: 'rgb(var(--kt-text))' }}>{t('title')}</h1>
        </div>
      </header>

      <div className="flex-1 flex items-center justify-center px-8 pb-8">
        <div className="w-full max-w-md animate-fade-in">

          {phase === 'idle' && (
            <div className="text-center">
              <div className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6" style={{ backgroundColor: accentBg }}>
                <ScanLine className="w-12 h-12" style={{ color: accentColor }} />
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
              <div className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6 animate-pulse" style={{ backgroundColor: accentBg }}>
                <ScanLine className="w-12 h-12" style={{ color: accentColor }} />
              </div>
              <p className="text-xl" style={{ color: 'rgb(var(--kt-text-muted))' }}>{t('scanning')}</p>
            </div>
          )}

          {phase === 'success' && success && (
            <div className="text-center">
              <div className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6" style={{ backgroundColor: 'rgb(var(--kt-success) / 0.1)' }}>
                <CheckCircle2 className="w-12 h-12" style={{ color: 'rgb(var(--kt-success))' }} />
              </div>
              <h2 className="text-2xl font-bold mb-2" style={{ color: 'rgb(var(--kt-success))' }}>{t('success')}</h2>
              {success.title && <p className="text-lg font-medium mb-1" style={{ color: 'rgb(var(--kt-text))' }}>{success.title}</p>}
              {success.alert && (
                <div className="flex items-center justify-center gap-2 mt-3 mb-2 px-4 py-3 rounded-xl"
                  style={{ backgroundColor: 'rgb(var(--kt-warning) / 0.1)', color: 'rgb(var(--kt-warning))' }}>
                  <AlertTriangle className="w-4 h-4 shrink-0" />
                  <span className="text-sm font-medium">{tc('contact_staff')}</span>
                </div>
              )}
              <div className="flex gap-3 mt-6">
                <button onClick={reset} className="kt-btn-primary flex-1" style={{ backgroundColor: accentColor }}>{t('add_more')}</button>
                <button onClick={() => router.push(`/${locale}/menu`)} className="kt-btn-ghost flex-1">{t('done')}</button>
              </div>
            </div>
          )}

          {phase === 'error' && (
            <div className="text-center">
              <div className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6" style={{ backgroundColor: 'rgb(var(--kt-error) / 0.1)' }}>
                <XCircle className="w-12 h-12" style={{ color: 'rgb(var(--kt-error))' }} />
              </div>
              <h2 className="text-2xl font-bold mb-2" style={{ color: 'rgb(var(--kt-error))' }}>{t('failed')}</h2>
              {errorMsg && <p className="text-base mb-8" style={{ color: 'rgb(var(--kt-text-muted))' }}>{errorMsg}</p>}
              <div className="flex gap-3">
                <button onClick={reset} className="kt-btn-primary flex-1" style={{ backgroundColor: accentColor }}>{tc('retry')}</button>
                <button onClick={() => router.push(`/${locale}/menu`)} className="kt-btn-ghost flex-1">{t('done')}</button>
              </div>
            </div>
          )}

        </div>
      </div>

      <div className="absolute bottom-6 right-6 z-20"><AccessibilityBar /></div>
    </div>
  )
}
