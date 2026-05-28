'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// AuthScreen — patron sign-in via barcode/QR scan or OIDC SSO

import { signIn } from 'next-auth/react'
import { useTranslations } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useEffect, useRef, useState } from 'react'
import { CreditCard, Lock, LogIn } from 'lucide-react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'
import type { AuthMode } from '@/lib/auth'

interface AuthScreenProps {
  authMode: AuthMode
  pinRequired: boolean
  oidcProviderName: string
  locale: string
}

type Status = 'idle' | 'loading' | 'error'

export default function AuthScreen({
  authMode,
  pinRequired,
  oidcProviderName,
  locale,
}: AuthScreenProps) {
  const t = useTranslations('auth')
  const router = useRouter()

  const [patronId, setPatronId] = useState('')
  const [pin, setPin] = useState('')
  const [status, setStatus] = useState<Status>('idle')

  const patronIdRef = useRef<HTMLInputElement>(null)
  const pinRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    patronIdRef.current?.focus()
  }, [])

  const handleBarcodeSubmit = async (e?: React.FormEvent) => {
    e?.preventDefault()
    if (!patronId.trim() || status === 'loading') return
    setStatus('loading')

    const result = await signIn('barcode', {
      patronId: patronId.trim(),
      pin,
      redirect: false,
    })

    if (result?.ok) {
      router.replace(`/${locale}/menu`)
    } else {
      setStatus('error')
      setPatronId('')
      setPin('')
      setTimeout(() => {
        setStatus('idle')
        patronIdRef.current?.focus()
      }, 2500)
    }
  }

  const handlePatronIdKey = (e: React.KeyboardEvent) => {
    if (e.key !== 'Enter') return
    e.preventDefault()
    if (pinRequired) pinRef.current?.focus()
    else handleBarcodeSubmit()
  }

  const handleOidcSignIn = () => {
    setStatus('loading')
    signIn('oidc', { callbackUrl: `/${locale}/menu` })
  }

  const showBarcode = authMode !== 'oidc'
  const showOidc = authMode !== 'barcode'
  const isLoading = status === 'loading'

  return (
    <div className="kt-screen relative w-screen h-screen overflow-hidden flex items-center justify-center">
      {/* Logo — top left */}
      <div className="absolute top-8 left-8 z-10">
        <img
          src="/api/config/logo"
          alt="Library logo"
          width={110}
          height={52}
          className="object-contain"
          onError={(e) => {
            ;(e.target as HTMLImageElement).style.display = 'none'
          }}
        />
      </div>

      {/* Auth card */}
      <div className="w-full max-w-md mx-6">
        <div className="kt-card p-10 shadow-xl">
          {/* Header */}
          <div className="flex flex-col items-center mb-8">
            <div
              className="w-16 h-16 rounded-2xl flex items-center justify-center mb-4"
              style={{ backgroundColor: 'rgb(var(--kt-primary) / 0.1)' }}
            >
              <CreditCard className="w-8 h-8" style={{ color: 'rgb(var(--kt-primary))' }} />
            </div>
            <h1 className="text-3xl font-bold text-center" style={{ color: 'rgb(var(--kt-text))' }}>
              {t('title')}
            </h1>
            <p className="text-base text-center mt-1" style={{ color: 'rgb(var(--kt-text-muted))' }}>
              {t('subtitle')}
            </p>
          </div>

          {showBarcode && (
            <form onSubmit={handleBarcodeSubmit} className="space-y-4">
              <div>
                <label
                  htmlFor="patronId"
                  className="block text-sm font-medium mb-2"
                  style={{ color: 'rgb(var(--kt-text-muted))' }}
                >
                  {t('card_number')}
                </label>
                <input
                  id="patronId"
                  ref={patronIdRef}
                  type="text"
                  value={patronId}
                  onChange={(e) => setPatronId(e.target.value)}
                  onKeyDown={handlePatronIdKey}
                  disabled={isLoading}
                  autoComplete="off"
                  autoCorrect="off"
                  spellCheck={false}
                  inputMode="numeric"
                  className="kt-input disabled:opacity-50"
                  placeholder={t('scan_or_type')}
                />
              </div>

              {pinRequired && (
                <div>
                  <label
                    htmlFor="pin"
                    className="flex items-center gap-1.5 text-sm font-medium mb-2"
                    style={{ color: 'rgb(var(--kt-text-muted))' }}
                  >
                    <Lock className="w-3.5 h-3.5" />
                    {t('pin')}
                  </label>
                  <input
                    id="pin"
                    ref={pinRef}
                    type="password"
                    value={pin}
                    onChange={(e) => setPin(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && handleBarcodeSubmit()}
                    disabled={isLoading}
                    autoComplete="off"
                    inputMode="numeric"
                    className="kt-input disabled:opacity-50"
                  />
                </div>
              )}

              {status === 'error' && (
                <div
                  role="alert"
                  className="rounded-xl px-4 py-3 text-center text-sm font-medium"
                  style={{ backgroundColor: 'rgb(var(--kt-error) / 0.08)', color: 'rgb(var(--kt-error))' }}
                >
                  {t('invalid_credentials')}
                </div>
              )}

              <button
                type="submit"
                disabled={isLoading || !patronId.trim()}
                className="kt-btn-primary w-full flex items-center justify-center gap-2 mt-2"
              >
                {isLoading ? (
                  <span className="animate-pulse">{t('logging_in')}</span>
                ) : (
                  <>
                    <LogIn className="w-5 h-5" />
                    {t('sign_in')}
                  </>
                )}
              </button>
            </form>
          )}

          {showBarcode && showOidc && (
            <div className="flex items-center gap-3 my-6">
              <div className="flex-1 h-px" style={{ backgroundColor: 'rgb(var(--kt-border))' }} />
              <span className="text-sm" style={{ color: 'rgb(var(--kt-text-muted))' }}>{t('or')}</span>
              <div className="flex-1 h-px" style={{ backgroundColor: 'rgb(var(--kt-border))' }} />
            </div>
          )}

          {showOidc && (
            <button
              onClick={handleOidcSignIn}
              disabled={isLoading}
              className="kt-btn-ghost w-full flex items-center justify-center gap-2"
            >
              {isLoading ? t('logging_in') : t('login_with', { provider: oidcProviderName })}
            </button>
          )}
        </div>
      </div>

      <div className="absolute bottom-8 right-8 z-20">
        <AccessibilityBar />
      </div>
    </div>
  )
}
