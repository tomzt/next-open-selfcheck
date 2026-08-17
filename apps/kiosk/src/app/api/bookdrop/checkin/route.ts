// SPDX-License-Identifier: GPL-3.0-or-later
// POST /api/bookdrop/checkin — automated return station checkin
// Receives RFID tag UID or item barcode from Bookdrop hardware
// Returns: checkin result + localized book info for Tablet UI confirm screen

import { NextResponse } from 'next/server'
import { z } from 'zod'
import { checkinItem } from '@/lib/sip2-client'
import { i18n } from '@/i18n'

// Input validation: either tagId (RFID) or itemBarcode (direct barcode)
const Body = z.object({
  itemBarcode: z.string().min(1).regex(/^[^|\r\n]+$/, 'contains invalid characters'),
  deviceId: z.string().optional(), // for logging which physical bookdrop
})

interface BookdropResponse {
  success: boolean
  itemBarcode: string
  title?: string | null
  message: string // Thai
  message_en?: string // English
  alert?: boolean
}

export async function POST(req: Request) {
  try {
    const body = await req.json()
    const parsed = Body.safeParse(body)

    if (!parsed.success) {
      return NextResponse.json(
        { success: false, message: 'Invalid request format', message_en: 'Invalid request format' },
        { status: 400 },
      )
    }

    const { itemBarcode, deviceId } = parsed.data

    // Call SIP2 checkin (reuse existing backend logic)
    const result = await checkinItem(itemBarcode)

    const response: BookdropResponse = {
      success: result.ok,
      itemBarcode: result.itemBarcode,
      title: result.title,
      alert: result.alert,
      // Localized messages for Tablet UI
      message: result.ok
        ? `✓ "${result.title ?? itemBarcode}" คืนสำเร็จแล้ว` // Thai
        : `✗ ไม่สามารถคืนสินค้า โปรดติดต่อเจ้าหน้าที่`, // Thai error
      message_en: result.ok
        ? `✓ "${result.title ?? itemBarcode}" returned successfully` // English
        : `✗ Cannot process return. Please contact staff.`, // English error
    }

    // Log transaction (for audit trail)
    console.log('[bookdrop-checkin]', {
      success: result.ok,
      itemBarcode,
      deviceId,
      timestamp: new Date().toISOString(),
    })

    return NextResponse.json(response)
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : 'Unknown error'
    console.error('[bookdrop-checkin] Error:', errorMessage)

    return NextResponse.json(
      {
        success: false,
        message: 'เกิดข้อผิดพลาด โปรดลองใหม่', // Thai
        message_en: 'Error occurred. Please try again.', // English
      },
      { status: 503 },
    )
  }
}
