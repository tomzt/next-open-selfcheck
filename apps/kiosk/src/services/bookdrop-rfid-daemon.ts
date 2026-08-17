// SPDX-License-Identifier: GPL-3.0-or-later
// Bookdrop RFID Daemon — Reads Feig RD5200 via serial/RS485
// Broadcasts tag events to connected tablets via WebSocket/polling
// Runs on central server, handles all bookdrop locations

import { EventEmitter } from 'events'

/**
 * Bookdrop RFID Daemon
 *
 * Responsibilities:
 * 1. Connect to RD5200 reader via serial port (or RS485 interface)
 * 2. Listen for ISO15693 tag detections
 * 3. Extract barcode from tag memory
 * 4. Broadcast events to tablets via WebSocket or polling endpoint
 * 5. Maintain connection, auto-reconnect on failure
 */
export class BookdropRfidDaemon extends EventEmitter {
  private serialPort: any // SerialPort instance (use serialport npm package)
  private isConnected = false
  private buffer = ''
  private readonly deviceId: string
  private readonly serialPortPath: string

  constructor(options: {
    deviceId: string // e.g., 'bookdrop-01', 'bookdrop-02'
    serialPortPath: string // e.g., '/dev/ttyUSB0', 'COM3'
    baudRate?: number // default: 38400
  }) {
    super()
    this.deviceId = options.deviceId
    this.serialPortPath = options.serialPortPath
  }

  /**
   * Connect to RD5200 reader via serial port
   * Uses existing Feig DLL wrapper (ported to Node.js via FFI or serial protocol)
   */
  async connect(): Promise<void> {
    try {
      // TODO: Import SerialPort library
      // const { SerialPort } = require('serialport');
      // this.serialPort = new SerialPort({
      //   path: this.serialPortPath,
      //   baudRate: 38400,
      //   dataBits: 8,
      //   parity: 'even',
      //   stopBits: 1,
      // });

      // TODO: Handle serial data events
      // this.serialPort.on('data', (data) => this._onSerialData(data));
      // this.serialPort.on('error', (err) => this.emit('error', err));

      // For now, simulate connection
      console.log(`[Bookdrop Daemon] Connected to RD5200 on ${this.serialPortPath}`)
      this.isConnected = true
      this.emit('connected', { deviceId: this.deviceId })
    } catch (err) {
      console.error('[Bookdrop Daemon] Connection failed:', err)
      this.emit('error', err)
      throw err
    }
  }

  /**
   * Disconnect from RD5200 reader
   */
  async disconnect(): Promise<void> {
    if (this.serialPort) {
      this.serialPort.close()
    }
    this.isConnected = false
    this.emit('disconnected', { deviceId: this.deviceId })
  }

  /**
   * Handle incoming serial data from RD5200
   * Parses ISO15693 tag detection messages
   *
   * Message format (example):
   * - Inventory response: 0x66 [length] [tag data...] \r
   * - Tag UID + barcode from memory
   */
  private _onSerialData(data: Buffer): void {
    this.buffer += data.toString('hex')

    // Look for complete message (ends with \r = 0x0D)
    while (this.buffer.includes('0d')) {
      const idx = this.buffer.indexOf('0d')
      const msgHex = this.buffer.substring(0, idx)
      this.buffer = this.buffer.substring(idx + 2)

      try {
        this._parseRd5200Message(msgHex)
      } catch (err) {
        console.warn('[Bookdrop Daemon] Parse error:', err)
      }
    }
  }

  /**
   * Parse Feig RD5200 ISO15693 inventory response
   * Extracts:
   * - Tag UID
   * - Item barcode (from tag memory)
   * - AFI byte (application family)
   */
  private _parseRd5200Message(msgHex: string): void {
    // TODO: Implement actual Feig protocol parsing
    // For now, mock a tag detection

    // Example: Feig response format (simplified)
    // [Header] [UID: 8 bytes] [Barcode: variable] [AFI: 1 byte] [Checksum]

    // Mock tag for testing
    const tagId = this._extractTagUid(msgHex)
    const barcode = this._extractBarcode(msgHex)

    if (tagId && barcode) {
      this._emitTagDetected(tagId, barcode)
    }
  }

  private _extractTagUid(msgHex: string): string | null {
    // TODO: Parse tag UID from Feig message
    // Placeholder: return mock UID
    return msgHex.substring(0, 16) || null
  }

  private _extractBarcode(msgHex: string): string | null {
    // TODO: Parse barcode from tag memory
    // Feig stores barcode in tag user data area
    // Placeholder: return mock barcode
    return null
  }

  /**
   * Emit tag detected event
   * This is picked up by polling endpoint or WebSocket broadcast
   */
  private _emitTagDetected(tagId: string, barcode: string): void {
    const event = {
      deviceId: this.deviceId,
      tagId,
      barcode,
      timestamp: new Date().toISOString(),
    }

    console.log('[Bookdrop Daemon] Tag detected:', event)
    this.emit('tag-detected', event)

    // Also store in ephemeral cache for polling clients
    this._cacheTagEvent(event)
  }

  /**
   * Cache tag event for polling clients
   * (Alternative to WebSocket: clients poll /api/bookdrop/events)
   */
  private _cacheTagEvent(event: any): void {
    // TODO: Store in Redis or in-memory Map
    // Key: `bookdrop:${deviceId}:pending-tags`
    // Value: [event, ...]
    // Expire after 5 seconds (assume client polled by then)
  }

  /**
   * Get pending tag events for a bookdrop device
   * Used by polling endpoint: GET /api/bookdrop/events?deviceId=bookdrop-01
   */
  getPendingTags(deviceId: string): any[] {
    // TODO: Retrieve from cache
    return []
  }

  /**
   * Health check
   */
  isHealthy(): boolean {
    return this.isConnected
  }
}

/**
 * Global daemon instance
 * Typically one daemon per RD5200 reader (or one central daemon managing all readers)
 */
let globalDaemon: BookdropRfidDaemon | null = null

export function getBookdropDaemon(): BookdropRfidDaemon {
  if (!globalDaemon) {
    const serialPort = process.env.BOOKDROP_SERIAL_PORT || '/dev/ttyUSB0'
    globalDaemon = new BookdropRfidDaemon({
      deviceId: 'bookdrop-01',
      serialPortPath: serialPort,
    })
  }
  return globalDaemon
}

export async function startBookdropDaemon(): Promise<void> {
  const daemon = getBookdropDaemon()
  await daemon.connect()

  // Log all tag detections
  daemon.on('tag-detected', (event) => {
    console.log('[Bookdrop] Tag event cached for polling:', event)
  })

  daemon.on('error', (err) => {
    console.error('[Bookdrop Daemon] Error:', err)
  })

  daemon.on('disconnected', () => {
    console.warn('[Bookdrop Daemon] Disconnected, will reconnect...')
    // Auto-reconnect after 5 seconds
    setTimeout(() => startBookdropDaemon(), 5000)
  })
}
