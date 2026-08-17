// SPDX-License-Identifier: GPL-3.0-or-later
// GET /api/bookdrop/events — Poll for pending RFID tag events from RD5200 daemon
// Used by tablet clients to receive tag detections in near-real-time
//
// Alternative: Use WebSocket for true push (lower latency, but more complex)
// This polling approach is simpler and works well for ~100ms latency

import { NextRequest, NextResponse } from 'next/server'
import { getBookdropDaemon } from '@/services/bookdrop-rfid-daemon'

/**
 * GET /api/bookdrop/events?deviceId=bookdrop-01
 *
 * Poll endpoint for tablet clients to receive tag detections.
 * Returns: [{ tagId, barcode, timestamp }, ...]
 * or empty array if no events pending
 *
 * Client polling pattern:
 * - Poll every 100ms
 * - When tag received: submit /api/bookdrop/checkin
 * - Resume polling for next tag
 */
export async function GET(request: NextRequest) {
  const deviceId = request.nextUrl.searchParams.get('deviceId')

  if (!deviceId) {
    return NextResponse.json(
      { error: 'deviceId query parameter required' },
      { status: 400 },
    )
  }

  try {
    const daemon = getBookdropDaemon()

    // Check if daemon is healthy
    if (!daemon.isHealthy()) {
      console.warn('[bookdrop-events] Daemon unhealthy, attempting reconnect...')
      // Optionally: start reconnection attempt
    }

    // Get pending tag events from daemon cache
    const events = daemon.getPendingTags(deviceId)

    // Response: array of pending events
    // Empty array = no events yet, client should poll again
    return NextResponse.json({
      deviceId,
      events,
      timestamp: new Date().toISOString(),
    })
  } catch (err) {
    console.error('[bookdrop-events] Error:', err)
    return NextResponse.json(
      { error: 'Failed to fetch events' },
      { status: 503 },
    )
  }
}
