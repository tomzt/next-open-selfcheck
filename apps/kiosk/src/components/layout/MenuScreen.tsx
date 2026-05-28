'use client'
// SPDX-License-Identifier: GPL-3.0-or-later
// MenuScreen — main service selection: Borrow / Return / Loans / Fines

import { signOut, useSession } from 'next-auth/react'
import { useTranslations } from 'next-intl'
import { useLocale } from 'next-intl'
import { useRouter } from 'next/navigation'
import AccessibilityBar from '@/components/ui/AccessibilityBar'

const MENU_ITEMS = [
  { key: 'borrow', icon: '📖', color: 'bg-blue-600 hover:bg-blue-500', route: 'borrow' },
  { key: 'return', icon: '↩️', color: 'bg-emerald-600 hover:bg-emerald-500', route: 'return' },
  { key: 'check_status', icon: '📋', color: 'bg-violet-600 hover:bg-violet-500', route: 'loans' },
  { key: 'check_fines', icon: '💰', color: 'bg-amber-600 hover:bg-amber-500', route: 'fines' },
] as const

export default function MenuScreen() {
  const t = useTranslations('menu')
  const tc = useTranslations('auth')
  const locale = useLocale()
  const router = useRouter()
  const { data: session } = useSession()

  const handleSignOut = async () => {
    await signOut({ redirect: false })
    router.replace(`/${locale}`)
  }

  return (
    <div className="relative w-screen h-screen overflow-hidden flex flex-col bg-slate-50 dark:bg-slate-900">

      {/* Header */}
      <div className="flex items-center justify-between px-8 pt-8 pb-4">
        <div>
          <img
            src="/api/config/logo"
            alt="Library logo"
            width={100}
            height={50}
            className="object-contain"
            onError={(e) => {
              ;(e.target as HTMLImageElement).style.display = 'none'
            }}
          />
        </div>
        <div className="text-right">
          {session?.patronName && (
            <p className="text-lg font-semibold text-slate-700 dark:text-slate-200">
              {session.patronName}
            </p>
          )}
          <button
            onClick={handleSignOut}
            className="text-sm text-slate-500 dark:text-slate-400 hover:text-red-500 dark:hover:text-red-400 transition-colors mt-1"
          >
            {tc('logout')}
          </button>
        </div>
      </div>

      {/* Title */}
      <p className="text-center text-2xl font-bold text-slate-800 dark:text-white mb-6">
        {t('title')}
      </p>

      {/* Service buttons */}
      <div className="flex-1 grid grid-cols-2 gap-6 px-8 pb-8 content-start">
        {MENU_ITEMS.map(({ key, icon, color, route }) => (
          <button
            key={key}
            onClick={() => router.push(`/${locale}/menu/${route}`)}
            className={`${color} active:scale-95 text-white rounded-2xl shadow-lg transition-all duration-150 flex flex-col items-center justify-center gap-4 p-8 min-h-[160px]`}
          >
            <span className="text-5xl">{icon}</span>
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
