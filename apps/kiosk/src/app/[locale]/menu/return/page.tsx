// SPDX-License-Identifier: GPL-3.0-or-later
import { redirect } from 'next/navigation'
import ReturnScreen from '@/components/transaction/ReturnScreen'
import { isServiceEnabled } from '@/lib/site-config'

interface Props {
  params: Promise<{ locale: string }>
}

export default async function ReturnPage({ params }: Props) {
  const { locale } = await params
  if (!isServiceEnabled('return')) redirect(`/${locale}/menu`)

  return <ReturnScreen />
}
