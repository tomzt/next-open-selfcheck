// SPDX-License-Identifier: GPL-3.0-or-later
import { redirect } from 'next/navigation'
import LoansScreen from '@/components/transaction/LoansScreen'
import { isServiceEnabled } from '@/lib/site-config'

interface Props {
  params: Promise<{ locale: string }>
}

export default async function LoansPage({ params }: Props) {
  const { locale } = await params
  if (!isServiceEnabled('loans')) redirect(`/${locale}/menu`)

  return <LoansScreen />
}
