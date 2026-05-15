'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// WelcomeScreen — video autoplay loop, logo, tap-to-begin CTA
// Patron taps anywhere → navigate to /auth

import { useRouter } from 'next/navigation'
import { useLocale } from 'next-intl'
import Image from 'next/image'
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

  // Ensure video plays (browser autoplay policy)
  useEffect(() => {
    videoRef.current?.play().catch(() => {
      // Autoplay blocked — video will show first frame as fallback
    })
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
        // Video uploaded by admin via Setup Wizard
        // Falls back to solid background if no video configured
        src="/api/config/welcome-video"
        onError={(e) => {
          // Hide video element if not configured
          ;(e.target as HTMLVideoElement).style.display = 'none'
        }}
      />

      {/* Dark overlay for readability */}
      <div className="absolute inset-0 bg-black/40" />

      {/* Logo — top left */}
      <div className="absolute top-6 left-6 z-10">
        <Image
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
      <div className="absolute inset-0 flex flex-col items-center justify-center z-10 animate-fade-in">
        <div className="text-center text-white drop-shadow-lg">
          <p className="text-kiosk-xl font-bold tracking-wide mb-3">
            {ctaText}
          </p>
          <p className="text-kiosk-md opacity-80">
            {ctaSubText}
          </p>
        </div>

        {/* Pulsing tap indicator */}
        <div className="mt-10 w-16 h-16 rounded-full border-4 border-white/60 animate-pulse" />
      </div>

      {/* Accessibility buttons — bottom right */}
      <div className="absolute bottom-6 right-6 z-20">
        <AccessibilityBar />
      </div>
    </div>
  )
}
