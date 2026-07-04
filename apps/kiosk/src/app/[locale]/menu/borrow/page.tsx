// SPDX-License-Identifier: GPL-3.0-or-later
import { redirect } from 'next/navigation'
import BorrowScreen from '@/components/transaction/BorrowScreen'
import { isServiceEnabled } from '@/lib/site-config'

interface Props {
  params: Promise<{ locale: string }>
}

export default async function BorrowPage({ params }: Props) {
  const { locale } = await params
  if (!isServiceEnabled('borrow')) redirect(`/${locale}/menu`)

  return <BorrowScreen />
}
