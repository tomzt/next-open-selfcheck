// SPDX-License-Identifier: GPL-3.0-or-later
import { redirect } from 'next/navigation'
import { defaultLocale } from '../../i18n'

export default function RootPage() {
  redirect(`/${defaultLocale}`)
}
