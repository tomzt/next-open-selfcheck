'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// MenuScreen — patron service selection with theme-aware design

import { signOut, useSession } from 'next-auth/react'
import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import { BookOpen, CornerDownLeft, BookCopy, Receipt, LogOut, User } from 'lucide-react'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

interface MenuItem {
  key: 'borrow' | 'return' | 'check_status' | 'check_fines'
  icon: React.ReactNode
  route: string
  accentClass: string
  bgVar: string
}

const MENU_ITEMS: MenuItem[] = [
  {
    key: 'borrow',
    icon: <BookOpen className="w-10 h-10" />,
    route: 'borrow',
    accentClass: 'from-blue-500 to-indigo-600',
    bgVar: 'rgb(59 130 246)',
  },
  {
    key: 'return',
    icon: <CornerDownLeft className="w-10 h-10" />,
    route: 'return',
    accentClass: 'from-emerald-500 to-teal-600',
    bgVar: 'rgb(16 185 129)',
  },
  {
    key: 'check_status',
    icon: <BookCopy className="w-10 h-10" />,
    route: 'loans',
    accentClass: 'from-violet-500 to-purple-600',
    bgVar: 'rgb(139 92 246)',
  },
  {
    key: 'check_fines',
    icon: <Receipt className="w-10 h-10" />,
    route: 'fines',
    accentClass: 'from-amber-500 to-orange-600',
    bgVar: 'rgb(245 158 11)',
  },
]

interface MenuScreenProps {
  libraryName: string
}

export default function MenuScreen({ libraryName }: MenuScreenProps) {
  const t = useTranslations('menu')
  const tAuth = useTranslations('auth')
  const locale = useLocale()
  const router = useRouter()
  const { data: session } = useSession()

  const handleSignOut = async () => {
    await signOut({ redirect: false })
    router.replace(`/${locale}`)
  }

  return (
    <div className="kt-screen relative w-screen h-screen overflow-hidden flex flex-col">
      {/* Top navigation bar */}
      <header className="kt-nav flex items-center justify-between px-8 py-5 shrink-0">
        {/* Logo */}
        <img
          src="/api/config/logo"
          alt="Library logo"
          width={100}
          height={48}
          className="object-contain"
          onError={(e) => {
            ;(e.target as HTMLImageElement).style.display = 'none'
          }}
        />

        {/* Patron info + sign out */}
        <div className="flex items-center gap-4">
          {session?.patronName && (
            <div className="flex items-center gap-2">
              <div
                className="w-9 h-9 rounded-full flex items-center justify-center"
                style={{ backgroundColor: 'rgb(var(--kt-primary) / 0.1)' }}
              >
                <User className="w-4.5 h-4.5" style={{ color: 'rgb(var(--kt-primary))' }} />
              </div>
              <span className="font-semibold text-lg" style={{ color: 'rgb(var(--kt-text))' }}>
                {session.patronName}
              </span>
            </div>
          )}
          <button
            onClick={handleSignOut}
            className="flex items-center gap-2 px-4 py-2 rounded-xl text-base font-medium transition-all active:scale-95"
            style={{
              color: 'rgb(var(--kt-text-muted))',
              backgroundColor: 'rgb(var(--kt-surface-2))',
            }}
          >
            <LogOut className="w-4 h-4" />
            {useTranslations('auth')('logout')}
          </button>
        </div>
      </header>

      {/* Service title */}
      <div className="px-8 pt-8 pb-4 shrink-0">
        <h1 className="text-3xl font-bold" style={{ color: 'rgb(var(--kt-text))' }}>
          {t('title')}
        </h1>
      </div>

      {/* 2×2 service grid */}
      <div className="flex-1 grid grid-cols-2 gap-5 px-8 pb-8 overflow-hidden">
        {MENU_ITEMS.map(({ key, icon, route, accentClass }) => (
          <button
            key={key}
            onClick={() => router.push(`/${locale}/menu/${route}`)}
            className={`bg-gradient-to-br ${accentClass} text-white rounded-2xl shadow-md active:scale-95 transition-all duration-150 flex flex-col items-center justify-center gap-4 p-8`}
          >
            {icon}
            <span className="text-2xl font-bold">{t(key)}</span>
          </button>
        ))}
      </div>

      {/* Accessibility bar */}
      <div className="absolute bottom-6 right-6 z-20">
        <AccessibilityBar />
      </div>
    </div>
  )
}
