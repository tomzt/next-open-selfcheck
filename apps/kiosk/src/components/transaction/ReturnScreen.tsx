'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// ReturnScreen — batch scan → review → confirm → SIP2 checkin (docs/requirements.md §5.1)

import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useEffect, useRef, useState } from 'react'
import { CornerDownLeft, ScanLine, CheckCircle2, XCircle, AlertTriangle, ArrowLeft, X, Loader2, Mail } from 'lucide-react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

type Phase = 'scan' | 'review' | 'processing' | 'done'

interface ItemResult {
  itemBarcode: string
  status: 'pending' | 'success' | 'error'
  title: string | null
  alert: boolean
}

const accentColor = 'rgb(16 185 129)'
const accentBg = 'rgb(16 185 129 / 0.1)'

export default function ReturnScreen() {
  const t = useTranslations('transaction.return')
  const tc = useTranslations('common')
  const locale = useLocale()
  const router = useRouter()

  const [phase, setPhase] = useState<Phase>('scan')
  const [barcode, setBarcode] = useState('')
  const [items, setItems] = useState<string[]>([])
  const [results, setResults] = useState<ItemResult[]>([])
  const [emailStatus, setEmailStatus] = useState<'sent' | 'skipped' | 'failed' | null>(null)
  const inputRef = useRef<HTMLInputElement>(null)

  useEffect(() => { if (phase === 'scan') inputRef.current?.focus() }, [phase])

  const addItem = (scanned: string) => {
    const trimmed = scanned.trim()
    if (!trimmed) return
    setItems((prev) => [...prev, trimmed])
    setBarcode('')
  }

  const removeItem = (index: number) => {
    setItems((prev) => prev.filter((_, i) => i !== index))
  }

  const runBatch = async () => {
    setPhase('processing')
    setResults(items.map((itemBarcode) => ({ itemBarcode, status: 'pending', title: null, alert: false })))

    const settled: ItemResult[] = []
    for (const itemBarcode of items) {
      try {
        const res = await fetch('/api/sip2/checkin', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ itemBarcode }),
        })
        if (res.status === 401) { router.replace(`/${locale}/auth`); return }
        const data = await res.json()
        settled.push({
          itemBarcode,
          status: data.ok ? 'success' : 'error',
          title: data.title ?? null,
          alert: !!data.alert,
        })
      } catch {
        settled.push({ itemBarcode, status: 'error', title: null, alert: false })
      }
      setResults([...settled])
    }

    setPhase('done')

    const succeeded = settled.filter((r) => r.status === 'success')
    if (succeeded.length > 0) {
      fetch('/api/receipt/send', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          locale,
          borrowed: [],
          returned: succeeded.map((r) => ({ itemBarcode: r.itemBarcode, title: r.title })),
        }),
      })
        .then((res) => res.json())
        .then((data) => setEmailStatus(data.status))
        .catch(() => setEmailStatus('failed'))
    }
  }

  const reset = () => {
    setItems([])
    setResults([])
    setEmailStatus(null)
    setBarcode('')
    setPhase('scan')
  }

  const successCount = results.filter((r) => r.status === 'success').length
  const hasAlert = results.some((r) => r.status === 'success' && r.alert)

  return (
    <div className="relative w-screen h-screen overflow-hidden flex flex-col kt-screen">
      <header className="kt-nav flex items-center gap-4 px-8 py-5 shrink-0">
        <button onClick={() => router.push(`/${locale}/menu`)}
          className="w-10 h-10 rounded-xl flex items-center justify-center transition-all active:scale-90"
          style={{ backgroundColor: 'rgb(var(--kt-surface-2))', color: 'rgb(var(--kt-text))' }}>
          <ArrowLeft className="w-5 h-5" />
        </button>
        <div className="flex items-center gap-3">
          <CornerDownLeft className="w-6 h-6" style={{ color: accentColor }} />
          <h1 className="text-2xl font-bold" style={{ color: 'rgb(var(--kt-text))' }}>
            {phase === 'review' ? t('review_title') : t('title')}
          </h1>
        </div>
      </header>

      <div className="flex-1 flex flex-col items-center px-8 pb-8 overflow-y-auto">
        <div className="w-full max-w-md animate-fade-in py-8">

          {phase === 'scan' && (
            <>
              <div className="text-center mb-6">
                <div className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6" style={{ backgroundColor: accentBg }}>
                  <ScanLine className="w-12 h-12" style={{ color: accentColor }} />
                </div>
                <p className="text-lg mb-6" style={{ color: 'rgb(var(--kt-text-muted))' }}>{t('instruction')}</p>
                <input
                  ref={inputRef}
                  type="text"
                  value={barcode}
                  onChange={(e) => setBarcode(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && addItem(barcode)}
                  autoComplete="off"
                  autoCorrect="off"
                  spellCheck={false}
                  className="kt-input"
                  placeholder="_ _ _ _ _ _ _ _"
                />
              </div>

              <ItemList
                items={items}
                title={t('scan_list_title', { count: items.length })}
                empty={t('scan_empty')}
                removable
                onRemove={removeItem}
                removeLabel={tc('remove')}
              />

              <button
                onClick={() => setPhase('review')}
                disabled={items.length === 0}
                className="kt-btn-primary w-full mt-6 disabled:opacity-40"
                style={{ backgroundColor: accentColor }}
              >
                {tc('done_scanning')}
              </button>
            </>
          )}

          {phase === 'review' && (
            <>
              <ItemList
                items={items}
                title={t('scan_list_title', { count: items.length })}
                empty={t('scan_empty')}
                removable
                onRemove={removeItem}
                removeLabel={tc('remove')}
              />

              <div className="flex gap-3 mt-6">
                <button onClick={() => setPhase('scan')} className="kt-btn-ghost flex-1">{tc('back_to_scan')}</button>
                <button onClick={runBatch} disabled={items.length === 0} className="kt-btn-primary flex-1 disabled:opacity-40"
                  style={{ backgroundColor: accentColor }}>
                  {t('confirm_all')}
                </button>
              </div>
            </>
          )}

          {phase === 'processing' && (
            <div className="text-center">
              <div className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6 animate-pulse" style={{ backgroundColor: accentBg }}>
                <ScanLine className="w-12 h-12" style={{ color: accentColor }} />
              </div>
              <p className="text-xl mb-6" style={{ color: 'rgb(var(--kt-text-muted))' }}>{tc('processing')}</p>
              <ProgressList results={results} />
            </div>
          )}

          {phase === 'done' && (
            <div className="text-center">
              <div className="w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6"
                style={{
                  backgroundColor: successCount === results.length
                    ? 'rgb(var(--kt-success) / 0.1)'
                    : successCount > 0
                      ? 'rgb(var(--kt-warning) / 0.1)'
                      : 'rgb(var(--kt-error) / 0.1)',
                }}>
                {successCount > 0
                  ? <CheckCircle2 className="w-12 h-12" style={{ color: 'rgb(var(--kt-success))' }} />
                  : <XCircle className="w-12 h-12" style={{ color: 'rgb(var(--kt-error))' }} />}
              </div>
              <h2 className="text-2xl font-bold mb-4" style={{ color: 'rgb(var(--kt-text))' }}>
                {successCount === results.length
                  ? t('summary_success', { count: successCount })
                  : successCount > 0
                    ? t('summary_partial', { success: successCount, total: results.length })
                    : t('summary_failed')}
              </h2>

              {hasAlert && (
                <div className="flex items-center justify-center gap-2 mb-4 px-4 py-3 rounded-xl"
                  style={{ backgroundColor: 'rgb(var(--kt-warning) / 0.1)', color: 'rgb(var(--kt-warning))' }}>
                  <AlertTriangle className="w-4 h-4 shrink-0" />
                  <span className="text-sm font-medium">{tc('contact_staff')}</span>
                </div>
              )}

              <ProgressList results={results} />

              {emailStatus === 'sent' && (
                <div className="flex items-center justify-center gap-2 mt-4 mb-2 text-sm"
                  style={{ color: 'rgb(var(--kt-text-muted))' }}>
                  <Mail className="w-4 h-4" />
                  {t('email_sent')}
                </div>
              )}

              <div className="flex gap-3 mt-6">
                <button onClick={reset} className="kt-btn-primary flex-1" style={{ backgroundColor: accentColor }}>{t('add_more')}</button>
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

function ItemList({
  items, title, empty, removable, onRemove, removeLabel,
}: {
  items: string[]
  title: string
  empty: string
  removable?: boolean
  onRemove?: (index: number) => void
  removeLabel?: string
}) {
  return (
    <div className="kt-card p-4">
      <p className="text-sm font-semibold mb-3" style={{ color: 'rgb(var(--kt-text-muted))' }}>{title}</p>
      {items.length === 0 ? (
        <p className="text-sm text-center py-4" style={{ color: 'rgb(var(--kt-text-muted))' }}>{empty}</p>
      ) : (
        <ul className="space-y-2 max-h-64 overflow-y-auto">
          {items.map((barcode, i) => (
            <li key={`${barcode}-${i}`} className="flex items-center justify-between px-3 py-2 rounded-lg"
              style={{ backgroundColor: 'rgb(var(--kt-surface-2))' }}>
              <span className="font-mono text-sm" style={{ color: 'rgb(var(--kt-text))' }}>{barcode}</span>
              {removable && (
                <button
                  onClick={() => onRemove?.(i)}
                  aria-label={removeLabel}
                  className="w-7 h-7 rounded-full flex items-center justify-center transition-all active:scale-90"
                  style={{ color: 'rgb(var(--kt-error))' }}
                >
                  <X className="w-4 h-4" />
                </button>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

function ProgressList({ results }: { results: ItemResult[] }) {
  if (results.length === 0) return null
  return (
    <ul className="space-y-2 max-h-72 overflow-y-auto text-left">
      {results.map((r, i) => (
        <li key={`${r.itemBarcode}-${i}`} className="flex items-center gap-3 px-3 py-2 rounded-lg"
          style={{ backgroundColor: 'rgb(var(--kt-surface-2))' }}>
          {r.status === 'pending' && <Loader2 className="w-4 h-4 shrink-0 animate-spin" style={{ color: 'rgb(var(--kt-text-muted))' }} />}
          {r.status === 'success' && <CheckCircle2 className="w-4 h-4 shrink-0" style={{ color: 'rgb(var(--kt-success))' }} />}
          {r.status === 'error' && <XCircle className="w-4 h-4 shrink-0" style={{ color: 'rgb(var(--kt-error))' }} />}
          <span className="text-sm truncate" style={{ color: 'rgb(var(--kt-text))' }}>
            {r.title ?? r.itemBarcode}
          </span>
          {r.status === 'success' && r.alert && (
            <AlertTriangle className="w-4 h-4 shrink-0 ml-auto" style={{ color: 'rgb(var(--kt-warning))' }} />
          )}
        </li>
      ))}
    </ul>
  )
}
