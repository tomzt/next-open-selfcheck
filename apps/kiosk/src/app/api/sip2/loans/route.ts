// SPDX-License-Identifier: GPL-3.0-or-later
// GET /api/sip2/loans — current patron loans via SIP2 patron info (msg 63)

import { getServerSession } from 'next-auth'
import { NextResponse } from 'next/server'
import { authOptions } from '@/lib/auth'
import { getPatronLoans } from '@/lib/sip2-client'

export async function GET() {
  const session = await getServerSession(authOptions)
  if (!session?.patronId) {
    return NextResponse.json({ error: 'Unauthorized' }, { status: 401 })
  }

  try {
    const loans = await getPatronLoans(session.patronId)
    return NextResponse.json({ loans })
  } catch (err) {
    console.error('[loans] SIP2 error:', err)
    return NextResponse.json({ error: 'SIP2 connection failed' }, { status: 503 })
  }
}
