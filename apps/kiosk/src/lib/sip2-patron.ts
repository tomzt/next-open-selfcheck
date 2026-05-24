// SPDX-License-Identifier: GPL-3.0-or-later
// SIP2 patron authentication — validates patron identity via SIP2 message 63.
// ILS-agnostic: works with Koha, ALMA, Symphony, Evergreen, and any SIP2-compatible system.
//
// When AUTH_PIN_REQUIRED=false (default), only the BL (valid patron) flag is checked.
// This supports QR-code and barcode-only flows where the token is already trusted.

import * as net from 'net'

export interface PatronAuthResult {
  valid: boolean
  patronName: string | null
  patronId: string | null
}

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

export async function authenticatePatron(
  patronId: string,
  pin: string,
): Promise<PatronAuthResult> {
  const host = process.env.SIP2_HOST ?? 'localhost'
  const port = parseInt(process.env.SIP2_PORT ?? '6002', 10)
  const timeoutMs = parseInt(process.env.SIP2_TIMEOUT_MS ?? '5000', 10)
  const institution = process.env.SIP2_INSTITUTION ?? 'LIBRARY'
  const loginUser = process.env.SIP2_LOGIN_USER ?? ''
  const loginPassword = process.env.SIP2_LOGIN_PASSWORD ?? ''
  const pinRequired = process.env.AUTH_PIN_REQUIRED === 'true'

  return new Promise((resolve) => {
    const socket = new net.Socket()
    let buffer = ''

    const fail = () => {
      socket.destroy()
      resolve({ valid: false, patronName: null, patronId: null })
    }

    const timer = setTimeout(fail, timeoutMs)

    socket.connect(port, host, () => {
      // SIP2 Login (93): CN = automation user, CO = automation password
      socket.write(`9300CN${loginUser}|CO${loginPassword}|\r`)
    })

    socket.on('data', (chunk) => {
      buffer += chunk.toString()
      const messages = buffer.split('\r')
      buffer = messages.pop() ?? ''

      for (const raw of messages) {
        const line = raw.trim()
        if (!line) continue
        const type = line.substring(0, 2)

        if (type === '94') {
          // Login Response: third char '1' = OK, '0' = failed
          if (line[2] !== '1') {
            clearTimeout(timer)
            return fail()
          }
          // SIP2 Patron Information (63)
          const patronMsg =
            `63000${sipDate()}          ` +
            `AO${institution}|AA${patronId}|AD${pin}|\r`
          socket.write(patronMsg)
        } else if (type === '64') {
          // Patron Information Response
          const validPatron = parseField(line, 'BL')
          const validPassword = parseField(line, 'CQ')
          const name = parseField(line, 'AE')

          clearTimeout(timer)
          socket.destroy()

          const valid =
            validPatron === 'Y' && (!pinRequired || validPassword === 'Y')

          resolve({
            valid,
            patronName: name ?? null,
            patronId: parseField(line, 'AA') ?? patronId,
          })
        }
      }
    })

    socket.on('error', () => {
      clearTimeout(timer)
      fail()
    })
  })
}
