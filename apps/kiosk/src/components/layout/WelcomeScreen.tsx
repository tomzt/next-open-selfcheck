'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// WelcomeScreen — tap-to-begin with theme-aware gradient background

import { useRouter } from 'next/navigation'
import { useLocale } from 'next-intl'
import { useEffect, useRef } from 'react'
import { Fingerprint } from 'lucide-react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

interface WelcomeScreenProps {
  ctaText: string
  ctaSubText: string
  libraryName: string
}

export default function WelcomeScreen({ ctaText, ctaSubText, libraryName }: WelcomeScreenProps) {
  const router = useRouter()
  const locale = useLocale()
  const videoRef = useRef<HTMLVideoElement>(null)

  useEffect(() => {
    videoRef.current?.play().catch(() => {})
  }, [])

  const handleTap = () => {
    router.push(`/${locale}/auth`)
  }

  return (
    <div
      className="relative w-screen h-screen overflow-hidden cursor-pointer select-none flex flex-col items-center justify-center"
      style={{ background: 'linear-gradient(135deg, rgb(var(--kt-welcome-bg-from)), rgb(var(--kt-welcome-bg-to)))' }}
      onClick={handleTap}
      role="button"
      aria-label={ctaText}
    >
      {/* Background video — hidden on error, gradient shows through */}
      <video
        ref={videoRef}
        className="absolute inset-0 w-full h-full object-cover opacity-20"
        autoPlay
        muted
        loop
        playsInline
        src="/api/config/welcome-video"
        onError={(e) => {
          ;(e.target as HTMLVideoElement).style.display = 'none'
        }}
      />

      {/* Logo */}
      <div className="absolute top-8 left-8 z-10">
        <img
          src="/api/config/logo"
          alt={`${libraryName} logo`}
          width={120}
          height={56}
          className="object-contain"
          onError={(e) => {
            ;(e.target as HTMLImageElement).style.display = 'none'
          }}
        />
      </div>

      {/* Main CTA */}
      <div className="relative z-10 flex flex-col items-center gap-8 text-center px-8">
        {/* Animated tap icon */}
        <div
          className="w-28 h-28 rounded-full flex items-center justify-center shadow-lg animate-pulse"
          style={{ backgroundColor: 'rgb(var(--kt-primary))' }}
        >
          <Fingerprint className="w-14 h-14" style={{ color: 'rgb(var(--kt-primary-fg))' }} />
        </div>

        <div>
          <p className="text-5xl font-bold tracking-tight mb-3" style={{ color: 'rgb(var(--kt-text))' }}>
            {ctaText}
          </p>
          <p className="text-2xl" style={{ color: 'rgb(var(--kt-text-muted))' }}>
            {ctaSubText}
          </p>
        </div>
      </div>

      {/* Accessibility bar */}
      <div className="absolute bottom-8 right-8 z-20">
        <AccessibilityBar />
      </div>
    </div>
  )
}
