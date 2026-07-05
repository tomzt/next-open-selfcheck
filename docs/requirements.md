# next-open-selfcheck — Product Requirements

> Living document. Updated as design decisions are confirmed.  
> Reference deployer: RMUTI (Rajamangala University of Technology Isan, Thailand)  
> License: GPL-3.0-or-later

---

## 1. Project Purpose

Re-engineer [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) from PHP to Next.js/Node.js with:

- Modern, touch-friendly kiosk UI
- Pluggable auth (SIP2 barcode, OIDC SSO)
- Optional RFID support (ISO 15693)
- Zero vendor lock-in — works with any SIP2-compatible ILS (Koha, ALMA, Symphony, Evergreen, WALAI, ALIST, etc.)
- Deployable by any library worldwide via Docker + `.env` config, no code change required

---

## 2. MonoRepo Structure

```
next-open-selfcheck/
  apps/
    kiosk/          ← Patron self-check terminal (this phase)
    bookdrop/       ← Automated return station, RFID-only (future)
    workstation/    ← Staff RFID workstation (future)
  packages/
    rfid-adapter/   ← ISO 15693 hardware abstraction (shared)
    sip2-client/    ← SIP2 TCP client (shared)
```

Each app is **independently deployable**. A library can use Kiosk alone without Bookdrop or Workstation.

---

## 3. Apps

### 3.1 Kiosk (`apps/kiosk`)

Patron-facing self-check terminal. Runs full-screen in a browser, one Docker instance per physical kiosk.

#### Services offered (configurable)

| Service | Description | Configurable off |
|---|---|---|
| Borrow | Patron scans/taps items → SIP2 checkout | via `KIOSK_SERVICES` |
| Return | Patron scans/taps items → SIP2 checkin | via `KIOSK_SERVICES` |
| My Loans | Shows current charged items from ILS | via `KIOSK_SERVICES` |
| My Fines | Shows outstanding fine balance | via `KIOSK_SERVICES` |

**Rationale for service toggle:** Some kiosks are borrow-only (return goes to a physical bookdrop). Some libraries disable fine inquiry to avoid patron friction. `KIOSK_SERVICES=borrow,loans,fines` removes the Return button entirely.

---

### 3.2 Bookdrop (`apps/bookdrop`) — *Planned*

Automated return station with physical drop slot. No touch screen. No patron interaction.

**Hardware model (minimal):**
- Sloped channel / drop slot — patron drops book in, book slides through
- RFID sensor detects tag as book passes
- System auto-checks in via SIP2
- No shutter, no conveyor — reduces mechanical complexity and cost

**Key constraints:**
- **RFID-only** — books checked out via barcode cannot be returned via bookdrop (no tag to detect). This is a known limitation, not a bug.
- Status after auto-checkin = `returned` (ILS), **not** `on-shelf`. Staff must manually set `on-shelf` before reshelving.
- No patron-facing UI — staff web panel shows today's return log.
- Shares `rfid-adapter` and `sip2-client` packages with Kiosk.

---

### 3.3 Workstation (`apps/workstation`) — *Planned*

Staff RFID programming station. Used to:
- Program ISO 15693 tags with item barcode + library code
- Write AFI byte (`0x07` = in library, `0x02` = checked out)
- Batch-program new acquisitions before putting on shelf

Runs in Chrome/Chromium (Web Serial API). No Docker needed — LAN-hosted.

---

## 4. Patron Flow — Kiosk

```
Welcome Screen
  ↓ patron taps anywhere
Auth Screen
  ↓ authenticated (barcode scan or OIDC SSO)
Main Menu  (shows only configured services)
  ↓ patron selects Borrow or Return
Transaction Screen  ← SEE §5
  ↓ session done or timeout
Welcome Screen
```

---

## 5. Transaction Flow (Borrow / Return)

### 5.1 Design rationale

**Previous design (rejected):** Scan one item → success/fail → scan next → repeat.  
**Problem:** Patron cannot review what was processed. No confirmation before commit. Anxiety about whether items were actually checked out/in.

**Adopted design: Batch Scan → Review → Confirm**

```
Transaction Screen (Borrow or Return)
│
├─ SCAN MODE
│    Patron scans barcode (HID keyboard) or taps RFID tag
│    Each item is added to a pending list immediately
│    Patron can scan as many items as needed
│    "Done scanning" button visible at all times
│
├─ REVIEW MODE  (after "Done scanning")
│    List of all scanned items shown
│    Status per item: ✅ ready / ⚠️ warning / ❌ problem
│    Patron can remove items from list before confirming
│    "Confirm" and "Back to scanning" buttons
│
└─ PROCESSING MODE  (after Confirm)
     SIP2 transactions sent for all items
     Progress shown per item
     Final summary: success / partial / failed
     "Done" button → email sent → return to Welcome Screen
```

### 5.2 SIP2 messages used

| Action | SIP2 Message | Response |
|---|---|---|
| Borrow | 11 (Checkout Request) | 12 (Checkout Response) — ok flag at index 2 |
| Return | 09 (Checkin Request) | 10 (Checkin Response) — ok at index 2, alert at index 5 |
| Auth | 63 (Patron Information) | 64 — BL field = valid patron |
| Loans | 63 + summary `  Y       ` | 64 — AU fields = charged items |
| Fines | 63 + summary `   Y      ` | 64 — BV field = total fine amount |

### 5.3 RFID in transactions

When `RFID_ENABLED=true`, RFID scan supplements barcode:
- On borrow: after SIP2 checkout, write AFI `0x02` (checked out) to tag
- On return: after SIP2 checkin, write AFI `0x07` (in library) to tag
- If AFI write fails, transaction still succeeds — log the failure

---

## 6. Email Receipt

**Decision: No paper slip. Email only (eco-friendly).**

### 6.1 When sent

- After patron confirms transaction in REVIEW MODE (§5.1)
- One email per session (covers all items borrowed and/or returned)
- Sent asynchronously — patron does not wait for email delivery

### 6.2 Content

```
Subject: ห้องสมุด — สรุปการยืม/คืน [date]

ยืม (2 รายการ):
  • Introduction to Library Science — กำหนดคืน 13 มิ.ย. 2569
  • Digital Cataloging Methods — กำหนดคืน 20 มิ.ย. 2569

คืน (1 รายการ):
  • Network Infrastructure — คืนแล้ว ✓

รายการค้างยืมทั้งหมด: 3 รายการ
```

### 6.3 Patron email source

Fetched from SIP2 Patron Information Response (64), field `BE` (email address). If `BE` is empty, email is skipped silently (not an error).

### 6.4 Configuration

```env
EMAIL_PROVIDER=smtp            # smtp | resend | sendgrid | disabled
SMTP_HOST=mail.example.com
SMTP_PORT=587
SMTP_USER=
SMTP_PASSWORD=
SMTP_FROM=library@example.com
EMAIL_SUBJECT_TH=ห้องสมุด — สรุปการยืม/คืน
EMAIL_SUBJECT_EN=Library — Transaction Receipt
```

---

## 7. Authentication

### 7.1 Modes

Controlled by `AUTH_MODE` env var:

| Mode | Description |
|---|---|
| `barcode` (default) | Patron card / QR scan validated via SIP2 msg 63 |
| `oidc` | Institutional SSO only (Keycloak, Azure AD, Auth0, any OIDC) |
| `both` | Patron chooses either method |

### 7.2 Design decisions

- **No PIN by default** (`AUTH_PIN_REQUIRED=false`). QR code from the library's mobile app is already a trusted token — requiring PIN is redundant and slows the flow. Libraries that need PIN can enable it via ENV.
- **OIDC is provider-agnostic** — configured via `OIDC_ISSUER / CLIENT_ID / CLIENT_SECRET`. No Keycloak-specific code.
- **SIP2 message 63** used for barcode auth — ILS-agnostic, works with any compliant system.
- Session: JWT, 30-minute maxAge, resets on each transaction.

### 7.3 Login rate limiting & abuse protection

Because `AUTH_PIN_REQUIRED=false` is the default (§7.2), a correct patron ID
alone is enough to sign in. Without a limit, that ID space could be
enumerated freely from a single kiosk. Barcode login is therefore rate
limited in-memory (no shared/central state, per the no-central-server
design in `docs/architecture.md`):

| Parameter | Value |
|---|---|
| Failed attempts before lockout | 5 |
| Window | 60 seconds |
| Lockout duration | 5 minutes |
| Keyed by | client IP, and IP+patronId |

Every failed attempt is logged with the patron ID masked (first/last 2
characters only — never the full ID, per §13). This resets on process
restart, which is an accepted trade-off for a single-instance kiosk.

---

## 8. Session & Idle Timeout

| ENV | Default | Description |
|---|---|---|
| `SESSION_TIMEOUT_MINUTES` | 3 | Idle time before auto sign-out |
| `SCREENSAVER_TIMEOUT_MINUTES` | 5 | Time on Welcome Screen before screensaver starts |

**Warning dialog** appears at `min(30s, timeout/3)` before expiry. Patron can tap "ใช้งานต่อ" to reset the timer.

On expiry: `signOut()` → redirect to Welcome Screen. Accessibility settings (font, theme, language) reset to library defaults.

---

## 9. User Interface

### 9.1 Theme System

Set via `NEXT_PUBLIC_KIOSK_THEME`. Libraries pick once; no code change needed.

| Theme | Style | Best for |
|---|---|---|
| `light` | White + Indigo accent | General use, bright rooms |
| `dark` | Slate-950 + Cyan accent | Dim rooms, media centres |
| `colorful` | White + Violet accent | Schools, youth libraries |
| `glass` | Dark gradient + frosted glass blur | Modern lobbies |
| `material` | M3 tonal surface + pill buttons | Google-familiar users |

All themes use CSS custom properties (`--kt-*`). Switching theme = change one ENV var + redeploy.

### 9.2 Accessibility Bar

Always visible in corner. Patron-adjustable, per-session only (resets on sign-out):

- **Font size**: sm → md → lg → xl
- **Theme**: light → dark → high-contrast
- **Language**: TH → EN (→ future languages)

### 9.3 Touch Targets

All interactive elements ≥ 48px (WCAG 2.5.5). Kiosk-mode: no scrollbars, no text selection.

### 9.4 Kiosk Browser Lock-down

- `X-Frame-Options: DENY`
- `X-Content-Type-Options: nosniff`
- `robots: noindex, nofollow`
- Run in kiosk mode (`--kiosk --app=http://localhost:3000`)

---

## 10. Internationalisation

- Default locale: `NEXT_PUBLIC_DEFAULT_LANGUAGE` (default: `th`)
- Supported: `th`, `en`
- Message files: `apps/kiosk/messages/{locale}.json`
- Framework: `next-intl` (server + client)
- Community can add languages by adding a new messages file

---

## 11. Configuration Philosophy

**Zero code change to deploy.** Every library-specific setting lives in `.env`:

- ILS connection (SIP2 host/port/institution)
- Auth method and provider
- Enabled services
- Theme and default language
- Email provider
- Session timeouts
- RFID on/off

First-Run Setup Wizard (planned) will generate `.env` via a web UI on first boot.

---

## 12. Deployment

Each physical kiosk = one Docker Compose stack:

```yaml
services:
  kiosk:   next-open-selfcheck/kiosk
  db:      postgres:16
```

No central server. No WAN dependency after initial Docker pull. All data (transaction log, config) stays on the local machine.

---

## 13. Data Retention

| Data | Stored where | Retention |
|---|---|---|
| Transaction log | Local PostgreSQL | Configurable (default: 90 days) |
| Error log | Local PostgreSQL | 30 days |
| Session tokens | JWT (stateless) | 30 minutes |
| Patron data | Never persisted | — |

Patron name and card number are **never written to disk**. Only transaction metadata (timestamp, item barcode, action) is logged.

---

## 14. Out of Scope

| Feature | Decision |
|---|---|
| Print slip / receipt printer | ❌ Not implemented — email replaces printing |
| Payment / fine payment | ❌ Patron directed to staff desk |
| Item renewal | ❌ Not in Phase 3; possible future addition |
| Reservations / holds | ❌ Patron directed to OPAC |
| Central analytics dashboard | ❌ Each site keeps its own logs |

---

## 15. Phase Roadmap

| Phase | Status | Scope |
|---|---|---|
| 1 | ✅ Done | Scaffold, Welcome Screen, i18n, Docker, Mock SIP2 |
| 2 | ✅ Done | Auth system (SIP2 barcode, OIDC, middleware guard) |
| 3 | ✅ Done | Main Menu, transaction screens, session timeout |
| 3b | ✅ Done | Batch scan flow, email receipt, `KIOSK_SERVICES` toggle |
| 4 | 🔄 Next | RFID integration (rfid-adapter, ISO 15693, AFI write) |
| 5 | 📋 Planned | Bookdrop app, staff return log |
| 6 | 📋 Planned | Workstation app (RFID tag programming) |
| 7 | 📋 Planned | First-Run Setup Wizard |

---

## 16. Security Hardening

Confirmed via an OWASP Top 10 pass and independently verified against
a running instance (mock SIP2 + a real `docker build`/`docker run`
cycle), not just read from source.

### 16.1 SIP2 protocol injection

SIP2 messages are pipe-delimited and `\r`-terminated. Any patron ID,
item barcode, or PIN reaching the wire is rejected if it contains `|`,
`\r`, or `\n` — both at the API boundary (clean `400`) and again at the
SIP2 client itself as defense-in-depth, so a value can never reach an
ILS with forged fields spliced in.

### 16.2 Auth secret — fail closed

`NEXTAUTH_SECRET` has no usable fallback in production. A deployment
started with `NODE_ENV=production` and no real secret throws rather
than silently signing sessions with a well-known default — there is no
scenario where the app serves a patron with a forgeable session.

### 16.3 HTTP security headers

Baseline headers on every response: `Content-Security-Policy`,
`X-Frame-Options: DENY`, `X-Content-Type-Options: nosniff`,
`Referrer-Policy`, `Permissions-Policy`, `Strict-Transport-Security`.
CSP allows `'unsafe-eval'` only when `NODE_ENV !== 'production'`
(required by `next dev`'s bundler) — dropped in the deployed image.

### 16.4 Patron data in logs

Per §13, patron identifiers never reach a log line unmasked. Failed
login attempts log a masked patron ID (first/last 2 characters); the
email receipt path logs delivery status only, never the address.

### 16.5 Docker image hygiene

The image build never carries `.env` (or any local secret file) into a
layer — enforced with an explicit delete step right after the source
copy, not solely relying on `.dockerignore`.
