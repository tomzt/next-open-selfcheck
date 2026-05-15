'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// WelcomeScreen — video autoplay loop, logo, tap-to-begin CTA

import { useRouter } from 'next/navigation'
import { useLocale } from 'next-intl'
import { useEffect, useRef } from 'react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

interface WelcomeScreenProps {
  ctaText: string
  ctaSubText: string
}

export default function WelcomeScreen({ ctaText, ctaSubText }: WelcomeScreenProps) {
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
      className="relative w-screen h-screen overflow-hidden cursor-pointer select-none"
      onClick={handleTap}
      role="button"
      aria-label={ctaText}
    >
      {/* Background video */}
      <video
        ref={videoRef}
        className="absolute inset-0 w-full h-full object-cover"
        autoPlay
        muted
        loop
        playsInline
        src="/api/config/welcome-video"
        onError={(e) => {
          ;(e.target as HTMLVideoElement).style.display = 'none'
        }}
      />

      {/* Dark overlay */}
      <div className="absolute inset-0 bg-black/40" />

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

      {/* CTA — center */}
      <div className="absolute inset-0 flex flex-col items-center justify-center z-10">
        <div className="text-center text-white drop-shadow-lg">
          <p className="text-5xl font-bold tracking-wide mb-3">
            {ctaText}
          </p>
          <p className="text-2xl opacity-80">
            {ctaSubText}
          </p>
        </div>
        <div className="mt-10 w-16 h-16 rounded-full border-4 border-white/60 animate-pulse" />
      </div>

      {/* Accessibility buttons — bottom right */}
      <div className="absolute bottom-6 right-6 z-20">
        <AccessibilityBar />
      </div>
    </div>
  )
}
