// SPDX-License-Identifier: GPL-3.0-or-later
// Email receipt — replaces paper slip (docs/requirements.md §6). Server-side only.
//
// EMAIL_PROVIDER=disabled (default-safe) or missing patron email → skip silently,
// never treated as an error (§6.3). Only "smtp" is implemented; resend/sendgrid
// are reserved names in .env.example for future contribution.

import nodemailer from 'nodemailer'

export interface ReceiptItem {
  itemBarcode: string
  title: string | null
  dueDate?: string | null
}

export interface ReceiptInput {
  to: string | null
  locale: string
  borrowed: ReceiptItem[]
  returned: ReceiptItem[]
  outstandingLoanCount: number
}

function buildBody(input: ReceiptInput): string {
  const isThai = input.locale === 'th'
  const lines: string[] = []

  if (input.borrowed.length > 0) {
    lines.push(isThai ? `ยืม (${input.borrowed.length} รายการ):` : `Borrowed (${input.borrowed.length} items):`)
    for (const item of input.borrowed) {
      const due = item.dueDate
        ? isThai
          ? ` — กำหนดคืน ${item.dueDate}`
          : ` — due ${item.dueDate}`
        : ''
      lines.push(`  • ${item.title ?? item.itemBarcode}${due}`)
    }
    lines.push('')
  }

  if (input.returned.length > 0) {
    lines.push(isThai ? `คืน (${input.returned.length} รายการ):` : `Returned (${input.returned.length} items):`)
    for (const item of input.returned) {
      lines.push(`  • ${item.title ?? item.itemBarcode}${isThai ? ' — คืนแล้ว ✓' : ' — returned ✓'}`)
    }
    lines.push('')
  }

  lines.push(
    isThai
      ? `รายการค้างยืมทั้งหมด: ${input.outstandingLoanCount} รายการ`
      : `Total items still on loan: ${input.outstandingLoanCount}`,
  )

  return lines.join('\n')
}

function buildSubject(locale: string): string {
  const isThai = locale === 'th'
  const configured = isThai ? process.env.EMAIL_SUBJECT_TH : process.env.EMAIL_SUBJECT_EN
  const fallback = isThai ? 'ห้องสมุด — สรุปการยืม/คืน' : 'Library — Transaction Receipt'
  const prefix = configured?.trim() || fallback
  const date = new Date().toLocaleDateString(isThai ? 'th-TH' : 'en-US')
  return `${prefix} [${date}]`
}

/**
 * Send the transaction receipt email. Never throws — logs and returns
 * a status instead, since email is best-effort and must not block or fail
 * the patron's transaction (§6.1: sent asynchronously).
 */
export async function sendReceiptEmail(
  input: ReceiptInput,
): Promise<'sent' | 'skipped' | 'failed'> {
  if (!input.to) {
    console.log('[email] Skipped — no patron email on file')
    return 'skipped'
  }

  const provider = process.env.EMAIL_PROVIDER ?? 'disabled'
  if (provider === 'disabled') {
    console.log('[email] Skipped — EMAIL_PROVIDER=disabled')
    return 'skipped'
  }

  if (provider !== 'smtp') {
    console.warn(`[email] EMAIL_PROVIDER=${provider} is not implemented yet — skipping send`)
    return 'skipped'
  }

  try {
    const transporter = nodemailer.createTransport({
      host: process.env.SMTP_HOST,
      port: parseInt(process.env.SMTP_PORT ?? '587', 10),
      secure: process.env.SMTP_PORT === '465',
      auth: process.env.SMTP_USER
        ? { user: process.env.SMTP_USER, pass: process.env.SMTP_PASSWORD }
        : undefined,
    })

    await transporter.sendMail({
      from: process.env.SMTP_FROM,
      to: input.to,
      subject: buildSubject(input.locale),
      text: buildBody(input),
    })

    console.log(`[email] Sent receipt to ${input.to}`)
    return 'sent'
  } catch (err) {
    console.error('[email] Send failed:', err)
    return 'failed'
  }
}
