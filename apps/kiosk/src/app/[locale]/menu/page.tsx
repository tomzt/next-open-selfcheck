// SPDX-License-Identifier: GPL-3.0-or-later
// Main menu page — service selection for authenticated patron

import { getServerSession } from 'next-auth'
import { redirect } from 'next/navigation'
import { authOptions } from '@/lib/auth'
import MenuScreen from '@/components/layout/MenuScreen'

interface Props {
  params: Promise<{ locale: string }>
}

export default async function MenuPage({ params }: Props) {
  const { locale } = await params
  const session = await getServerSession(authOptions)
  if (!session) redirect(`/${locale}/auth`)

  return <MenuScreen />
}
