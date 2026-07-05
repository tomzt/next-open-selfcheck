// SPDX-License-Identifier: GPL-3.0-or-later
// SIP2 client — checkout, checkin, patron loans, patron fines
// Server-side only. Never import in client components.

import * as net from 'net'

// ── Types ──────────────────────────────────────────────────────────────────

export interface CheckoutResult {
  ok: boolean
  itemBarcode: string
  title: string | null
  dueDate: string | null
}

export interface CheckinResult {
  ok: boolean
  itemBarcode: string
  title: string | null
  alert: boolean
}

export interface LoanItem {
  itemBarcode: string
  title: string | null
  dueDate: string | null
}

export interface PatronFines {
  totalAmount: number
  currency: string
}

// ── Helpers ────────────────────────────────────────────────────────────────

function sipDate(): string {
  const d = new Date()
  return (
    d.getFullYear().toString() +
    String(d.getMonth() + 1).padStart(2, '0') +
    String(d.getDate()).padStart(2, '0') +
    String(d.getHours()).padStart(2, '0') +
    String(d.getMinutes()).padStart(2, '0') +
    String(d.getSeconds()).padStart(2, '0') +
    '0000'
  )
}

function parseField(message: string, code: string): string | null {
  const m = message.match(new RegExp(`${code}([^|]*)\\|`))
  return m?.[1] ?? null
}

function parseAllFields(message: string, code: string): string[] {
  const results: string[] = []
  const re = new RegExp(`${code}([^|]*)\\|`, 'g')
  let m: RegExpExecArray | null
  while ((m = re.exec(message)) !== null) {
    results.push(m[1])
  }
  return results
}

// ── TCP Session ────────────────────────────────────────────────────────────

class Sip2Session {
  private socket: net.Socket | null = null
  private buffer = ''
  private queue: string[] = []
  private waiters: Array<(line: string) => void> = []

  async connect(): Promise<void> {
    const host = process.env.SIP2_HOST ?? 'localhost'
    const port = parseInt(process.env.SIP2_PORT ?? '6002', 10)
    const timeoutMs = parseInt(process.env.SIP2_TIMEOUT_MS ?? '5000', 10)

    return new Promise((resolve, reject) => {
      this.socket = new net.Socket()

      const timer = setTimeout(() => {
        this.socket?.destroy()
        reject(new Error('SIP2 connection timeout'))
      }, timeoutMs)

      this.socket.connect(port, host, () => {
        clearTimeout(timer)
        resolve()
      })

      this.socket.on('data', (chunk) => {
        this.buffer += chunk.toString()
        const msgs = this.buffer.split('\r')
        this.buffer = msgs.pop() ?? ''
        for (const raw of msgs) {
          const line = raw.trim()
          if (!line) continue
          if (this.waiters.length > 0) {
            this.waiters.shift()!(line)
          } else {
            this.queue.push(line)
          }
        }
      })

      this.socket.on('error', (err) => {
        clearTimeout(timer)
        reject(err)
      })
    })
  }

  send(msg: string): void {
    this.socket?.write(msg + '\r')
  }

  receive(): Promise<string> {
    return new Promise((resolve) => {
      if (this.queue.length > 0) {
        resolve(this.queue.shift()!)
      } else {
        this.waiters.push(resolve)
      }
    })
  }

  disconnect(): void {
    this.socket?.destroy()
    this.socket = null
  }
}

// ── Connection wrapper ─────────────────────────────────────────────────────

async function withSession<T>(fn: (s: Sip2Session) => Promise<T>): Promise<T> {
  const s = new Sip2Session()
  await s.connect()

  const loginUser = process.env.SIP2_LOGIN_USER ?? ''
  const loginPassword = process.env.SIP2_LOGIN_PASSWORD ?? ''
  s.send(`9300CN${loginUser}|CO${loginPassword}|`)

  const loginResp = await s.receive()
  if (loginResp[2] !== '1') {
    s.disconnect()
    throw new Error('SIP2 login failed')
  }

  try {
    return await fn(s)
  } finally {
    s.disconnect()
  }
}

// ── Public API ─────────────────────────────────────────────────────────────

export async function checkoutItem(
  patronId: string,
  itemBarcode: string,
  pin = '',
): Promise<CheckoutResult> {
  return withSession(async (s) => {
    const institution = process.env.SIP2_INSTITUTION ?? 'LIBRARY'
    s.send(`11YN${sipDate()}AO${institution}|AA${patronId}|AB${itemBarcode}|AC|AD${pin}|`)

    const resp = await s.receive()
    return {
      ok: resp[2] === '1',
      itemBarcode: parseField(resp, 'AB') ?? itemBarcode,
      title: parseField(resp, 'AJ'),
      dueDate: parseField(resp, 'AH'),
    }
  })
}

export async function checkinItem(itemBarcode: string): Promise<CheckinResult> {
  return withSession(async (s) => {
    const institution = process.env.SIP2_INSTITUTION ?? 'LIBRARY'
    s.send(`09N${sipDate()}AO${institution}|AB${itemBarcode}|AC|`)

    const resp = await s.receive()
    return {
      ok: resp[2] === '1',
      itemBarcode: parseField(resp, 'AB') ?? itemBarcode,
      title: parseField(resp, 'AJ'),
      // Position 5 in the fixed fields: alert flag
      alert: resp[5] === 'Y',
    }
  })
}

export async function getPatronLoans(patronId: string): Promise<LoanItem[]> {
  return withSession(async (s) => {
    const institution = process.env.SIP2_INSTITUTION ?? 'LIBRARY'
    // Summary field (10 chars): Y at position 2 = request charged items
    s.send(`63000${sipDate()}  Y       AO${institution}|AA${patronId}|AC|`)

    const resp = await s.receive()
    const barcodes = parseAllFields(resp, 'AU')
    const titles = parseAllFields(resp, 'AJ')
    const dueDates = parseAllFields(resp, 'AH')

    return barcodes.map((barcode, i) => ({
      itemBarcode: barcode,
      title: titles[i] ?? null,
      dueDate: dueDates[i] ?? null,
    }))
  })
}

/**
 * Patron email for receipt delivery (§6.3). Returns null if the ILS has no
 * email on file for this patron — callers must skip sending silently, not
 * treat it as an error.
 */
export async function getPatronEmail(patronId: string): Promise<string | null> {
  return withSession(async (s) => {
    const institution = process.env.SIP2_INSTITUTION ?? 'LIBRARY'
    s.send(`63000${sipDate()}          AO${institution}|AA${patronId}|AC|`)

    const resp = await s.receive()
    return parseField(resp, 'BE')
  })
}

export async function getPatronFines(patronId: string): Promise<PatronFines> {
  return withSession(async (s) => {
    const institution = process.env.SIP2_INSTITUTION ?? 'LIBRARY'
    // Summary field (10 chars): Y at position 3 = request fine items
    s.send(`63000${sipDate()}   Y      AO${institution}|AA${patronId}|AC|`)

    const resp = await s.receive()
    const bv = parseField(resp, 'BV')
    const currency = parseField(resp, 'BH') ?? process.env.SIP2_DEFAULT_CURRENCY ?? 'THB'
    return {
      totalAmount: parseFloat(bv ?? '0') || 0,
      currency,
    }
  })
}
