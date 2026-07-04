// SPDX-License-Identifier: GPL-3.0-or-later
import { redirect } from 'next/navigation'
import FinesScreen from '@/components/transaction/FinesScreen'
import { isServiceEnabled } from '@/lib/site-config'

interface Props {
  params: Promise<{ locale: string }>
}

export default async function FinesPage({ params }: Props) {
  const { locale } = await params
  if (!isServiceEnabled('fines')) redirect(`/${locale}/menu`)

  return <FinesScreen />
}
