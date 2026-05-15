# Architecture

## Overview

open-selfcheck is a monorepo containing two independent apps that share RFID and SIP2 packages.

```
open-selfcheck/
  apps/
    kiosk/          ← Patron self-check terminal
    workstation/    ← Staff RFID workstation (optional)
  packages/
    rfid-adapter/   ← Shared RFID hardware layer
    sip2-client/    ← Shared SIP2 TCP client
```

---

## Kiosk Architecture

Each deployment site runs its own **independent Docker instance**. No central server, no WAN dependency.

```
[Kiosk PC / Mini PC]
  └─ Browser (full-screen kiosk mode)
        └─ Next.js App (Docker)
              ├─ NextAuth.js ──► Auth Provider (Keycloak / any OIDC)
              ├─ SIP2 Client ──LAN──► ILS SIP2 Server (port 6002)
              ├─ PostgreSQL (local, Docker Compose)
              └─ RFID (Phase 3, optional) ──USB──► ISO 15693 Reader
```

### Kiosk Patron Flow

```
Welcome Screen (video loop + logo)
  ↓ patron taps
Auth Screen (provider from config)
  ↓ authenticated
Main Menu: Borrow / Return / Check Status / Fee Inquiry
  ↓ scan barcode or RFID
Transaction → SIP2 → ILS → result
  ↓ complete or timeout
Welcome Screen
```

---

## Workstation Architecture (Optional)

Deployed only when the library uses RFID. Runs directly in the browser — no Docker needed.

```
[Staff PC]
  └─ Chrome/Chromium
        └─ Workstation App (Next.js, LAN-hosted)
              ├─ Web Serial API ──USB──► ISO 15693 RFID Reader (ACR1552U)
              └─ ILS Lookup (optional) ──LAN──► Z39.50 / ALIST API
```

### When to deploy Workstation

| Scenario | Deploy? |
|---|---|
| Barcode-only library | ❌ Not needed |
| Library adding RFID | ✅ Required — program tags before using Kiosk RFID |
| Library with legacy RFID (e.g. old 3M system) | ✅ Required — reprogram existing tags |

---

## Shared Packages

### rfid-adapter

Hardware abstraction layer — both Kiosk and Workstation use the same driver.

```
kiosk ──────┐
            ├──► rfid-adapter/web-serial ──► Web Serial API ──USB──► ACR1552U
workstation ┘
```

See `packages/rfid-adapter/src/interface.ts` for the driver interface.

### sip2-client

SIP2 TCP client — used only by Kiosk. ILS-agnostic.

```
kiosk ──► sip2-client ──TCP/LAN──► Any SIP2-compatible ILS
```

---

## SIP2 Retry Policy

All traffic is campus LAN — timeouts are kept tight.

| Parameter | Default | Configurable |
|---|---|---|
| Timeout per attempt | 3s | ✅ |
| Retry count | 3 | ✅ |
| Delay between retries | 2s | ✅ |
| Worst-case total | ~15s | — |

---

## Database (Kiosk only)

PostgreSQL runs locally inside Docker Compose — no central database.

| Table | Contents |
|---|---|
| `system_config` | All setup wizard settings |
| `transaction_log` | Borrow / return records |
| `error_log` | SIP2 connection failures |

---

## RFID AFI Values

| AFI | Meaning | Written by |
|---|---|---|
| `0x07` | In library / available | Workstation (program) or Kiosk (check-in) |
| `0x02` | Checked out | Kiosk (check-out) |
