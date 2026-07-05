// SPDX-License-Identifier: GPL-3.0-or-later
// POST /api/sip2/checkin — return an item via SIP2 message 09

import { getServerSession } from 'next-auth'
import { NextResponse } from 'next/server'
import { z } from 'zod'
import { authOptions } from '@/lib/auth'
import { checkinItem } from '@/lib/sip2-client'

const Body = z.object({
  itemBarcode: z.string().min(1).regex(/^[^|\r\n]+$/, 'contains invalid characters'),
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
    const result = await checkinItem(parsed.data.itemBarcode)
    return NextResponse.json(result)
  } catch (err) {
    console.error('[checkin] SIP2 error:', err)
    return NextResponse.json({ error: 'SIP2 connection failed' }, { status: 503 })
  }
}
