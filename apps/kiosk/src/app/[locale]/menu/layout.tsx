// SPDX-License-Identifier: GPL-3.0-or-later
// Menu layout — wraps all /menu/** pages with session timeout guard

import SessionGuard from '@/components/layout/SessionGuard'

interface Props {
  children: React.ReactNode
}

export default function MenuLayout({ children }: Props) {
  const timeoutSeconds =
    parseInt(process.env.SESSION_TIMEOUT_MINUTES ?? '3', 10) * 60

  return (
    <SessionGuard timeoutSeconds={timeoutSeconds}>
      {children}
    </SessionGuard>
  )
}
