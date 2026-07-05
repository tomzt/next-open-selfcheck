// SPDX-License-Identifier: GPL-3.0-or-later
// Shared input guard for anything interpolated into a raw SIP2 message.
//
// SIP2 fields are pipe-delimited and messages are \r-terminated — a value
// containing either character lets an attacker inject extra fields or
// split the message. Reject rather than strip, since a legitimate barcode
// or patron ID should never contain these characters in the first place.

const FORBIDDEN = /[|\r\n]/

export function assertSipSafe(value: string, fieldName: string): string {
  if (FORBIDDEN.test(value)) {
    throw new Error(`Invalid ${fieldName}: contains a reserved SIP2 character`)
  }
  return value
}
