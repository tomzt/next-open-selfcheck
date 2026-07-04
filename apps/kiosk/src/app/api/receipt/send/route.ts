// SPDX-License-Identifier: GPL-3.0-or-later
// POST /api/receipt/send — email receipt after a borrow/return batch (§6)

import { getServerSession } from 'next-auth'
import { NextResponse } from 'next/server'
import { z } from 'zod'
import { authOptions } from '@/lib/auth'
import { getPatronEmail, getPatronLoans } from '@/lib/sip2-client'
import { sendReceiptEmail } from '@/lib/email'

const ReceiptItem = z.object({
  itemBarcode: z.string(),
  title: z.string().nullable(),
  dueDate: z.string().nullable().optional(),
})

const Body = z.object({
  locale: z.string(),
  borrowed: z.array(ReceiptItem).default([]),
  returned: z.array(ReceiptItem).default([]),
})

export async function POST(req: Request) {
  const session = await getServerSession(authOptions)
  if (!session?.patronId) {
    return NextResponse.json({ error: 'Unauthorized' }, { status: 401 })
  }

  const parsed = Body.safeParse(await req.json())
  if (!parsed.success) {
    return NextResponse.json({ error: 'Invalid request' }, { status: 400 })
  }

  try {
    const [email, loans] = await Promise.all([
      getPatronEmail(session.patronId),
      getPatronLoans(session.patronId),
    ])

    const status = await sendReceiptEmail({
      to: email,
      locale: parsed.data.locale,
      borrowed: parsed.data.borrowed,
      returned: parsed.data.returned,
      outstandingLoanCount: loans.length,
    })

    return NextResponse.json({ status })
  } catch (err) {
    console.error('[receipt] SIP2 error:', err)
    // Receipt failures must never block the patron's completed transaction.
    return NextResponse.json({ status: 'failed' })
  }
}
