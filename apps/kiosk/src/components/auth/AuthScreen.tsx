'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// AuthScreen — patron authentication UI
// Supports barcode/QR scan (HID keyboard), manual entry, and OIDC SSO.

import { signIn } from 'next-auth/react'
import { useTranslations } from 'next-intl'
import { useRouter } from 'next/navigation'
import { useEffect, useRef, useState } from 'react'
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

  // Auto-focus: barcode scanners send keystrokes here
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

  // HID barcode scanner sends Enter after the card number
  const handlePatronIdKey = (e: React.KeyboardEvent) => {
    if (e.key !== 'Enter') return
    e.preventDefault()
    if (pinRequired) {
      pinRef.current?.focus()
    } else {
      handleBarcodeSubmit()
    }
  }

  const handleOidcSignIn = () => {
    setStatus('loading')
    signIn('oidc', { callbackUrl: `/${locale}/menu` })
  }

  const showBarcode = authMode !== 'oidc'
  const showOidc = authMode !== 'barcode'
  const isLoading = status === 'loading'

  return (
    <div className="relative w-screen h-screen overflow-hidden bg-slate-900 flex items-center justify-center">
      {/* Subtle background gradient */}
      <div className="absolute inset-0 bg-gradient-to-br from-slate-800 to-slate-950" />

      {/* Logo — top left */}
      <div className="absolute top-6 left-6 z-10">
        <img
          src="/api/config/logo"
          alt="Library logo"
          width={120}
          height={60}
          className="object-contain"
          onError={(e) => {
            ;(e.target as HTMLImageElement).style.display = 'none'
          }}
        />
      </div>

      {/* Auth card */}
      <div className="relative z-10 w-full max-w-md mx-4">
        <div className="bg-white/10 backdrop-blur-md rounded-2xl p-10 shadow-2xl border border-white/20">
          <h1 className="text-white text-3xl font-bold text-center mb-2">
            {t('title')}
          </h1>
          <p className="text-white/60 text-center mb-8 text-lg">
            {t('subtitle')}
          </p>

          {showBarcode && (
            <form onSubmit={handleBarcodeSubmit} className="space-y-4">
              <div>
                <label
                  htmlFor="patronId"
                  className="block text-white/70 text-sm mb-2"
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
                  className="w-full px-4 py-4 rounded-xl bg-white/15 border border-white/25 text-white text-xl placeholder-white/30 focus:outline-none focus:ring-2 focus:ring-white/40 focus:border-white/40 disabled:opacity-50 transition-colors"
                  placeholder={t('scan_or_type')}
                />
              </div>

              {pinRequired && (
                <div>
                  <label
                    htmlFor="pin"
                    className="block text-white/70 text-sm mb-2"
                  >
                    {t('pin')}
                  </label>
                  <input
                    id="pin"
                    ref={pinRef}
                    type="password"
                    value={pin}
                    onChange={(e) => setPin(e.target.value)}
                    onKeyDown={(e) =>
                      e.key === 'Enter' && handleBarcodeSubmit()
                    }
                    disabled={isLoading}
                    autoComplete="off"
                    inputMode="numeric"
                    className="w-full px-4 py-4 rounded-xl bg-white/15 border border-white/25 text-white text-xl placeholder-white/30 focus:outline-none focus:ring-2 focus:ring-white/40 disabled:opacity-50 transition-colors"
                  />
                </div>
              )}

              {status === 'error' && (
                <p
                  role="alert"
                  className="text-red-300 text-center text-sm py-1"
                >
                  {t('invalid_credentials')}
                </p>
              )}

              <button
                type="submit"
                disabled={isLoading || !patronId.trim()}
                className="w-full py-4 rounded-xl bg-white text-slate-900 text-xl font-bold hover:bg-white/90 active:bg-white/80 disabled:opacity-40 disabled:cursor-not-allowed transition-colors mt-2"
              >
                {isLoading ? t('logging_in') : t('sign_in')}
              </button>
            </form>
          )}

          {showBarcode && showOidc && (
            <div className="flex items-center gap-3 my-6">
              <div className="flex-1 h-px bg-white/20" />
              <span className="text-white/40 text-sm">{t('or')}</span>
              <div className="flex-1 h-px bg-white/20" />
            </div>
          )}

          {showOidc && (
            <button
              onClick={handleOidcSignIn}
              disabled={isLoading}
              className="w-full py-4 rounded-xl bg-indigo-600 text-white text-xl font-bold hover:bg-indigo-500 active:bg-indigo-700 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
            >
              {isLoading
                ? t('logging_in')
                : t('login_with', { provider: oidcProviderName })}
            </button>
          )}
        </div>
      </div>

      {/* Accessibility bar — bottom right */}
      <div className="absolute bottom-6 right-6 z-20">
        <AccessibilityBar />
      </div>
    </div>
  )
}
