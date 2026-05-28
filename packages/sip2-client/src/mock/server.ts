// SPDX-License-Identifier: GPL-3.0-or-later
// Mock SIP2 Server — Phase 1 development
// Simulates SIP2 v2.0 responses for checkout, checkin, patron info, fee inquiry
// Run: npm run mock-server (TCP port 6002)

import * as net from 'net'

const PORT = parseInt(process.env.SIP2_MOCK_PORT ?? '6002', 10)

// ── SIP2 Message Builders ─────────────────────────────────────

function checkoutResponse(itemBarcode: string): string {
  const ok = '1'
  const renewalOk = 'N'
  const magneticMedia = 'U'
  const desensitize = 'Y'
  const transactionDate = sipDate()
  const dueDate = sipDueDate()

  return `120${ok}${renewalOk}${magneticMedia}${desensitize}${transactionDate}` +
    `AO|AA123456|AB${itemBarcode}|AJMock Book Title|AH${dueDate}|`
}

function checkinResponse(itemBarcode: string): string {
  const ok = '1'
  const resensitize = 'Y'
  const magneticMedia = 'U'
  const alert = 'N'
  const transactionDate = sipDate()

  return `100${ok}${resensitize}${magneticMedia}${alert}${transactionDate}` +
    `AO|AB${itemBarcode}|AQMain Library|AJMock Book Title|`
}

const MOCK_LOANS = [
  { barcode: 'BOOK-001', title: 'Library Self-Check Systems', dueDate: 'Sat Jun 13 2026' },
  { barcode: 'BOOK-002', title: 'Digital Library Management', dueDate: 'Sat Jun 20 2026' },
]
const MOCK_FINE_AMOUNT = '0.00'

function patronInfoResponse(patronId: string, summary: string): string {
  const transactionDate = sipDate()
  const wantCharged = (summary[2] ?? ' ') === 'Y'
  const wantFines = (summary[3] ?? ' ') === 'Y'

  const chargedCount = wantCharged
    ? String(MOCK_LOANS.length).padStart(4, '0')
    : '0000'

  let resp =
    `64              ` +  // patron status (14 chars)
    `000` +               // language
    transactionDate +
    `0000` +              // hold items count
    `0000` +              // overdue items count
    chargedCount +        // charged items count
    `0000` +              // fine items count
    `0000` +              // recall items count
    `0000` +              // unavail holds count
    `AO|AA${patronId}|AETest Patron|BLY|CQY|`

  if (wantCharged) {
    for (const loan of MOCK_LOANS) {
      resp += `AU${loan.barcode}|AJ${loan.title}|AH${loan.dueDate}|`
    }
  }

  if (wantFines) {
    resp += `BV${MOCK_FINE_AMOUNT}|BHTHU|`
  }

  return resp
}

function feeResponse(patronId: string): string {
  const transactionDate = sipDate()
  return `38${transactionDate}AO|AA${patronId}|BV${MOCK_FINE_AMOUNT}|`
}

function sipDate(): string {
  const d = new Date()
  return d.getFullYear().toString() +
    String(d.getMonth() + 1).padStart(2, '0') +
    String(d.getDate()).padStart(2, '0') +
    String(d.getHours()).padStart(2, '0') +
    String(d.getMinutes()).padStart(2, '0') +
    String(d.getSeconds()).padStart(2, '0')
}

function sipDueDate(): string {
  const d = new Date()
  d.setDate(d.getDate() + 14) // 2 weeks loan period
  return d.toDateString()
}

// ── SIP2 Message Router ───────────────────────────────────────

function handleMessage(message: string): string {
  const msgType = message.substring(0, 2)
  console.log(`[SIP2 Mock] ← ${msgType}: ${message.trim()}`)

  switch (msgType) {
    case '93': // Login
      return '941'
    case '11': // Checkout
      const checkoutItem = message.match(/AB([^|]+)/)?.[1] ?? 'UNKNOWN'
      return checkoutResponse(checkoutItem)
    case '09': // Checkin
      const checkinItem = message.match(/AB([^|]+)/)?.[1] ?? 'UNKNOWN'
      return checkinResponse(checkinItem)
    case '63': { // Patron info
      const patron = message.match(/AA([^|]+)/)?.[1] ?? 'UNKNOWN'
      // Summary field: 63 (2) + language (3) + date (18) = offset 23, length 10
      const summary = message.substring(23, 33)
      return patronInfoResponse(patron, summary)
    }
    case '37': // Fee paid / inquiry
      const feePatron = message.match(/AA([^|]+)/)?.[1] ?? 'UNKNOWN'
      return feeResponse(feePatron)
    default:
      console.warn(`[SIP2 Mock] Unknown message type: ${msgType}`)
      return `96` // Request SC resend
  }
}

// ── TCP Server ────────────────────────────────────────────────

const server = net.createServer((socket) => {
  const addr = `${socket.remoteAddress}:${socket.remotePort}`
  console.log(`[SIP2 Mock] Client connected: ${addr}`)

  let buffer = ''

  socket.on('data', (data) => {
    buffer += data.toString()

    // SIP2 messages end with \r
    const messages = buffer.split('\r')
    buffer = messages.pop() ?? ''

    for (const message of messages) {
      if (!message.trim()) continue
      const response = handleMessage(message)
      console.log(`[SIP2 Mock] → ${response.substring(0, 2)}: ${response.trim()}`)
      socket.write(response + '\r')
    }
  })

  socket.on('end', () => console.log(`[SIP2 Mock] Client disconnected: ${addr}`))
  socket.on('error', (err) => console.error(`[SIP2 Mock] Socket error: ${err.message}`))
})

server.listen(PORT, () => {
  console.log(`\n✅ Mock SIP2 server running on TCP port ${PORT}`)
  console.log(`   SIP2 version: 2.0`)
  console.log(`   Supported: Login (93), Checkout (11), Checkin (09), Patron Info (63), Fee (37)`)
  console.log(`   Press Ctrl+C to stop\n`)
})

server.on('error', (err) => {
  console.error(`[SIP2 Mock] Server error: ${err.message}`)
  process.exit(1)
})
