# Architecture

## Overview (Updated: Centralized + Flutter Native, 2026-07-25)

**next-open-selfcheck is a monorepo with 1 centralized backend + 3 native client apps.**

Centralized server (Docker) handles all business logic (SIP2, auth, email receipts). Native Flutter apps on tablets/staff PCs provide the UI for patrons and staff. Bookdrop runs as a background daemon on the server.

```
next-open-selfcheck/
  apps/
    kiosk/            ← Next.js backend API (all 3 clients use this)
  packages/
    rfid-adapter/     ← RFID driver interfaces (deprecated: Web Serial; new: Flutter USB Host)
    sip2-client/      ← SIP2 TCP client (backend only)
  clients/
    (Flutter mobile apps — not yet in repo; referenced in plan)
    kiosk-mobile/     ← Flutter Kiosk app (patron self-check, tablet)
    workstation-mobile/ ← Flutter Workstation app (staff tool)
    bookdrop-daemon/  ← Node.js service (Feig RFID loop)
```

---

## Backend Architecture

### Centralized Docker (1 server for all libraries)

**Server (University central or cloud-hosted):**

```
[Central Server: Docker Compose]
  ├─ Container 1: Next.js API (port 3000, behind nginx reverse proxy)
  │    ├─ NextAuth.js (token-based for mobile clients)
  │    ├─ SIP2 Client (TCP to ILS)
  │    ├─ Email service (nodemailer)
  │    ├─ Rate limiter (auth protection)
  │    └─ API endpoints:
  │         /api/auth/login → token
  │         /api/sip2/{checkout,checkin,loans,fines}
  │         /api/receipt/send
  │         /api/health
  │
  ├─ Container 2: Bookdrop Daemon (Node.js + Feig SDK)
  │    ├─ USB hub connection (multiple Feig reader modules)
  │    ├─ RFID loop (detect tag → auto-checkin)
  │    └─ Handles all bookdrops (per-location multiplexing)
  │
  ├─ Container 3: PostgreSQL
  │    ├─ System config (from First-Run Setup Wizard)
  │    ├─ Transaction log (audit trail)
  │    └─ Error log (SIP2 failures)
  │
  └─ Container 4: Nginx (reverse proxy)
       ├─ SSL/TLS termination
       └─ Route all clients → backend

[Hardware connected to server]
  ├─ USB hub (4–8 ports)
  │    └─ Feig RFID reader modules (1 per bookdrop location)
  │         ├─ Feig LRM5400 or M02-M8 (50cm read range)
  │         └─ ISO 15693, ferrite-backed antenna pair
  │
  └─ Network
       └─ LAN/WAN to all client locations
```

**Key decision:** No per-site Docker. All libraries share the same central deployment. Cost & maintenance << 3 separate stacks.

---

## Client Architecture

### 1. Kiosk (Patron Self-Check) — Flutter on Tablet

```
[Patron Tablet (Android)]
  └─ Flutter App (native)
       ├─ Screens:
       │    ├─ Welcome (video loop)
       │    ├─ Auth (scan barcode / QR)
       │    ├─ Menu (Borrow / Return / Loans / Fines)
       │    ├─ Batch-Scan (multiple items)
       │    ├─ Review (confirm items)
       │    └─ Done (email receipt sent)
       │
       ├─ RFID Reader: USB Host via Android API
       │    └─ ACR1552U (ISO 15693, 7cm range)
       │
       ├─ Barcode Scanner: External HID (USB-OTG or Bluetooth)
       │
       └─ API Client (HTTP to central backend)
            ├─ POST /api/auth/login → receive JWT token
            ├─ POST /api/sip2/checkout (per item)
            ├─ POST /api/sip2/checkin (per item)
            ├─ GET  /api/sip2/loans
            ├─ GET  /api/sip2/fines
            └─ POST /api/receipt/send (batch)
```

**Deployment:** Browser or Flutter app in kiosk-lockdown mode (Fully Kiosk Browser or Flutter native).

---

### 2. Workstation (Staff Tool) — Flutter on Desktop/Tablet

```
[Staff Device (Desktop/Tablet)]
  └─ Flutter App (native)
       ├─ Screens:
       │    ├─ Staff Login (PIN-based)
       │    ├─ Patron Search
       │    ├─ Patron Detail (loans, fines, history)
       │    ├─ Manual Checkout (type or scan barcode)
       │    ├─ Manual Checkin (process returns)
       │    ├─ Settings (admin panel: KIOSK_SERVICES toggle)
       │    └─ Audit Log (who did what, when)
       │
       ├─ Optional RFID Reader: USB Host
       │    └─ ACR1552U (to verify tags during checkin)
       │
       └─ API Client
            ├─ POST /api/auth/login → JWT token
            ├─ GET  /api/sip2/patron-info (search)
            ├─ POST /api/sip2/checkout (manual)
            ├─ POST /api/sip2/checkin (manual)
            ├─ GET  /api/sip2/loans (details)
            ├─ GET  /api/sip2/fines (details)
            └─ GET  /api/admin/config (audit log)
```

**Deployment:** Flutter app on staff Windows/Mac/Linux + optional external RFID reader.

---

### 3. Bookdrop (Automated Return) — Backend Daemon

```
[Central Server: Bookdrop Daemon (Node.js)]
  ├─ Hardware: Feig Reader Module (LRM5400 or M02-M8)
  │    ├─ Interface: USB or RS232 to server
  │    ├─ Read range: 50 cm (long-range)
  │    └─ Location: Inside metal chute with ferrite-backed dual antennas
  │
  ├─ Workflow:
  │    1. Book drops into chute (RFID tag detected)
  │    2. Daemon reads tag UID
  │    3. Daemon POST /api/sip2/checkin (auto-trigger)
  │    4. Backend sends to ILS via SIP2
  │    5. Item checked in, inventory updated
  │    6. Log transaction (audit trail)
  │
  ├─ Multiplexing: Supports multiple Feig readers
  │    (multiple bookdrops connected to same server via USB hub + multiplexer)
  │
  └─ No UI (headless service)
```

**Deployment:** Docker container on central server. Feig reader modules connected via USB hub on the server (not on each bookdrop).

---

## Network Topology

```
┌─────────────────────────────────────────────────────────────┐
│                  Central Server (Docker)                    │
│              (University datacenter or cloud)               │
│                                                              │
│  Next.js API + Bookdrop Daemon + PostgreSQL + Nginx         │
│                          ↑                                   │
│                    USB Hub (Feig readers)                    │
└────────────┬─────────────┬──────────────────────────────────┘
             │             │
       ┌─────┴─────┐   ┌───┴────┐
       │ Location 1│   │Location 2
       │           │   │
     ┌─┴─┐       ┌─┴─┐
     │   │       │   │
  [Tablet] [Desktop]  [Tablet] [Desktop]
   Kiosk   Workstation Kiosk   Workstation
  Flutter  Flutter     Flutter  Flutter
   App      App        App      App
       │           │   │           │
       └─────HTTP/HTTPS (WiFi/LAN)─┘
```

**Resilience:**
- ✅ Scaling: Add tablet → no hardware needed (uses shared backend)
- ✅ Maintenance: Update backend code once → all libraries benefit
- ❌ Single point of failure: If server/WAN down, all branches offline (mitigated with HA setup later)

---

## Client Authentication

**Token-based (mobile-friendly):**

```
1. POST /api/auth/login { patronId, pin }
   └─ Returns: { token, refreshToken, patronName, ... }

2. Client stores token (secure storage via Flutter)

3. All subsequent requests include:
   Authorization: Bearer <token>

4. Token expires after session timeout (30 min default)
   └─ Refresh via refreshToken
```

**Why tokens instead of cookies?** Mobile apps (Flutter) don't manage cookies reliably across HTTP redirects. Tokens are simpler for native clients.

---

## RFID Architecture (Phases 4–6)

### Phase 4: Kiosk RFID (USB Host on Android)

```
Tablet (Flutter) ──USB-OTG──→ ACR1552U Reader ──→ Patron holds tag (5-7cm)
                   └─ Platform channel (Dart → Kotlin)
                      └─ Android USB Host API
```

**Implementation:** Flutter platform channel bridges Dart ↔ Kotlin native code. Kotlin uses Android's USB Host API to communicate with ACR1552U.

### Phase 5: Bookdrop RFID (Feig OEM module)

```
Server ──USB Hub──→ Feig LRM5400 ──→ Chute antenna (dual panel)
  └─ Feig SDK driver (Node.js)
     └─ Detects ISO 15693 tags automatically
        └─ Auto-calls /api/sip2/checkin
```

**Why different hardware?** ACR1552U (7cm) is too close for a 15-20cm chute window. Feig LRM5400 (50cm long-range) works for books sliding through.

### Phase 6: Workstation RFID (optional)

```
Staff Desktop ──USB-OTG──→ ACR1552U Reader ──→ Staff verifies tag
                └─ Flutter USB Host (same as Kiosk)
```

Optional — staff can verify RFID tags during manual checkin. Barcode-only staff flows work fine without it.

---

## Shared Backend APIs

All 3 clients (Kiosk, Workstation, Bookdrop) use these endpoints:

| Endpoint | Method | Used By | Purpose |
|---|---|---|---|
| `/api/auth/login` | POST | All | Authenticate (barcode/QR + PIN) → token |
| `/api/sip2/checkout` | POST | Kiosk, Workstation | Borrow transaction |
| `/api/sip2/checkin` | POST | Workstation, Bookdrop daemon | Return transaction |
| `/api/sip2/patron-info` | GET | Workstation | Search patron by ID |
| `/api/sip2/loans` | GET | Kiosk, Workstation | List checked-out items |
| `/api/sip2/fines` | GET | Kiosk, Workstation | List fees owed |
| `/api/receipt/send` | POST | Kiosk | Email receipt to patron |
| `/api/admin/config` | GET/POST | Workstation | KIOSK_SERVICES toggle, etc. |
| `/api/health` | GET | Bookdrop daemon | Liveness check |

---

## Database (PostgreSQL, Centralized)

| Table | Contents | Owner |
|---|---|---|
| `system_config` | Setup wizard settings, KIOSK_SERVICES, theme, language | First-Run Setup (Phase 7) |
| `transaction_log` | Every checkout/checkin (audit trail) | Kiosk + Workstation + Bookdrop |
| `error_log` | SIP2 connection failures, auth rate-limit hits | Backend service |
| `session_*` | NextAuth session metadata (not used by mobile; tokens only) | NextAuth.js |

---

## SIP2 Retry Policy

All traffic is LAN/WAN. Timeouts are configurable:

| Parameter | Default | Env Var |
|---|---|---|
| Timeout per attempt | 3s | `SIP2_TIMEOUT_MS` |
| Retry count | 3 | `SIP2_RETRY_COUNT` |
| Delay between retries | 2s | `SIP2_RETRY_DELAY_MS` |
| Worst-case total | ~15s | — |

**Status:** Retry logic defined in env vars but not yet implemented in code (Phase 4 TODO).

---

## Security Hardening

(See `docs/requirements.md §16` for full details.)

- **SIP2 injection blocked** — patron ID, barcode, PIN rejected if contain `|`, `\r`, `\n`
- **Rate limiting** — 5 failed attempts / 60s → 5 min lockout
- **Token-based auth** — no session cookies (mobile-friendly)
- **HTTPS/TLS** — all client-to-server traffic encrypted
- **No PII in logs** — patron data masked
- **Feig reader security** — USB connection on secure server (not exposed to internet)

---

## Deployment Checklist

- [ ] Docker Compose setup on central server
- [ ] `.env` configured (SIP2_HOST, NEXTAUTH_SECRET, email, etc.)
- [ ] PostgreSQL initialized (migrations run)
- [ ] Reverse proxy (nginx) configured with SSL
- [ ] Feig reader modules connected to server USB hub
- [ ] Flutter apps built & signed (APK for Android, IPA for iOS)
- [ ] Tablets configured to kiosk mode
- [ ] Staff PCs with Workstation app installed
- [ ] Test end-to-end: login → scan → checkout → receipt email

---

## Migration from Phase 3b (Web) to Phase 4 (Flutter)

**Phase 3b (current):** Web app running in browser on Mini PC/Tablet.

**Phase 4 (planned):** Replace browser with Flutter native app. Backend API unchanged.

| Component | Phase 3b | Phase 4 | Change |
|---|---|---|---|
| **Client UI** | Next.js web (browser) | Flutter native | Rewrite UI |
| **Backend** | Next.js API | Next.js API | No change |
| **Auth** | Session cookies | JWT tokens | Tokens for mobile |
| **RFID** | Web Serial (desktop only) | USB Host (Android) | Native capability |
| **Deployment** | Local Docker per site | Centralized Docker | Major shift |
| **Hardware** | Mini PC + monitor | Tablet only | Cost reduction |

During migration, the backend (`apps/kiosk/`) continues to serve both web and mobile clients simultaneously (with content negotiation headers).

---

## References

- `docs/requirements.md` — Full feature list & security details
- `README.md` — Quick start & project overview
- `AGENTS.md` — Development guidelines
