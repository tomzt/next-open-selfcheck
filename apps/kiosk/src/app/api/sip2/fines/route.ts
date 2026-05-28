// SPDX-License-Identifier: GPL-3.0-or-later
// GET /api/sip2/fines — patron fine balance via SIP2 patron info (msg 63)

import { getServerSession } from 'next-auth'
import { NextResponse } from 'next/server'
import { authOptions } from '@/lib/auth'
import { getPatronFines } from '@/lib/sip2-client'

export async function GET() {
  const session = await getServerSession(authOptions)
  if (!session?.patronId) {
    return NextResponse.json({ error: 'Unauthorized' }, { status: 401 })
  }

  try {
    const fines = await getPatronFines(session.patronId)
    return NextResponse.json(fines)
  } catch (err) {
    console.error('[fines] SIP2 error:', err)
    return NextResponse.json({ error: 'SIP2 connection failed' }, { status: 503 })
  }
}
