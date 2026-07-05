# next-open-selfcheck

> Next-generation library self-check system — Next.js re-engineering of [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check)

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Next.js](https://img.shields.io/badge/Next.js-15-black)](https://nextjs.org)
[![Node.js](https://img.shields.io/badge/Node.js-20+-green)](https://nodejs.org)
[![Based on](https://img.shields.io/badge/based%20on-GallusMax%2Fopen--source--self--check-orange)](https://github.com/GallusMax/open-source-self-check)

A next-generation re-engineering of [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) — rebuilt from PHP to **Node.js/Next.js** with a modern stack, 5-theme UI, pluggable auth, and optional RFID support.

Built for **any library worldwide** — RMUTI (Rajamangala University of Technology Isan, Thailand) is the reference deployer. Any library using a SIP2-compatible ILS (Koha, Evergreen, WALAI, ALIST, etc.) can deploy with zero code changes.

---

## What's new vs upstream

| | [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) | **next-open-selfcheck** |
|---|---|---|
| **Stack** | PHP | Next.js 15 + Node.js |
| **SIP2** | PHP socket | Node.js `net` (server-side, ILS-agnostic) |
| **Auth** | Basic | NextAuth.js — barcode/QR via SIP2, or any OIDC provider |
| **UI** | Basic HTML | 5 themes: light / dark / colorful / glass / material |
| **Transaction** | One item at a time | Batch scan → review list → confirm → email receipt |
| **Receipt** | Thermal printer | Email to patron (eco-friendly, no hardware) |
| **RFID** | — | ISO 15693 via Web Serial API (optional) |
| **Bookdrop** | — | Automated return station, RFID-only (planned) |
| **Workstation** | — | Staff RFID tag programming (planned) |
| **Deploy** | PHP server | Docker per site |
| **Config** | Code edit | `.env` only — no code change to deploy |
| **i18n** | — | Thai + English built-in, community-extensible |

---

## Apps

| App | Description | Status |
|---|---|---|
| [`apps/kiosk`](./apps/kiosk) | Patron self-check — borrow, return, loans, fines | ✅ Phase 3 done |
| [`apps/bookdrop`](./apps/bookdrop) | Automated return station — RFID-only, no touch screen | 📋 Planned (Phase 5) |
| [`apps/workstation`](./apps/workstation) | Staff RFID workstation — program tags, write AFI | 📋 Planned (Phase 6) |

All apps are **independent** — deploy Kiosk alone without Bookdrop or Workstation.

---

## Quick Start

### Prerequisites

- Node.js 20+
- Docker + Docker Compose
- A SIP2-compatible ILS — or use the built-in **mock server** for development

### Development (mock SIP2, no ILS needed)

```bash
# 1. Install dependencies (monorepo)
npm install

# 2. Start the mock SIP2 server (TCP :6002)
cd packages/sip2-client
npm run mock-server

# 3. In a new terminal, start the kiosk dev server
cd apps/kiosk
cp .env.example .env.local
# Set SIP2_HOST=localhost in .env.local
npm run dev
```

Open `http://localhost:3000` — scan any barcode to sign in against the mock server.

### Production (Docker)

```bash
cd apps/kiosk
cp .env.example .env
# Edit .env: SIP2_HOST, NEXTAUTH_SECRET, email config, etc.
docker compose up -d
```

---

## Patron Flow

```
Welcome Screen
  ↓ tap anywhere
Auth Screen  (barcode/QR scan, or OIDC SSO)
  ↓ authenticated
Main Menu  (services configured by KIOSK_SERVICES env var)
  ↓ select Borrow or Return
Scan Mode  — scan / tap as many items as needed, list builds up
  ↓ "Done scanning"
Review Mode  — see all items, remove if needed
  ↓ Confirm
Processing  — SIP2 transactions sent for all items
  ↓ Done
Email receipt sent to patron  →  Welcome Screen
```

My Loans and My Fines show read-only data from the ILS and require no confirmation.

---

## UI Themes

Set once per kiosk via `NEXT_PUBLIC_KIOSK_THEME`. No code change needed.

| Theme | Style | Best for |
|---|---|---|
| `light` *(default)* | White + Indigo accent | General use, bright rooms |
| `dark` | Slate-950 + Cyan neon | Dim rooms, media centres |
| `colorful` | White + Violet accent | Schools, youth libraries |
| `glass` | Dark gradient + frosted blur | Modern lobbies |
| `material` | M3 tonal surface + pill buttons | Google-familiar users |

All themes share the same component tree — switching is one ENV var + redeploy.

---

## Configuration

**Zero code change to deploy.** Every setting lives in `.env`.

### Key variables — Kiosk

```env
# Branding & identity — set for YOUR library
KIOSK_LIBRARY_NAME=                          # shown in <title>, header, receipts
                                             # (also: KIOSK_LIBRARY_NAME_TH / _EN to
                                             #  localize; blank falls back to "Library")
KIOSK_LOGO_URL=                              # logo image — see "Branding" below
KIOSK_WELCOME_VIDEO_URL=                     # welcome-screen background video

# SIP2 connection
SIP2_HOST=192.168.1.100
SIP2_PORT=6002
SIP2_INSTITUTION=LIBRARY
SIP2_LOGIN_USER=
SIP2_LOGIN_PASSWORD=
SIP2_DEFAULT_CURRENCY=THB   # shown for fines only if the ILS omits the BH field

# Auth mode: barcode | oidc | both
AUTH_MODE=barcode
AUTH_PIN_REQUIRED=false   # false = QR/barcode only, no PIN prompt

# OIDC (required only when AUTH_MODE=oidc or both)
OIDC_ISSUER=https://keycloak.example.com/realms/library
OIDC_CLIENT_ID=
OIDC_CLIENT_SECRET=
OIDC_PROVIDER_NAME=Institutional SSO
# Which OIDC claim carries the patron ID the ILS expects (SIP2 AA field)?
# default: preferred_username (Keycloak login username = student/staff ID)
# also try: student_id, library_barcode, employee_number — whatever your IdP exposes
OIDC_PATRON_ID_CLAIM=preferred_username

# UI
NEXT_PUBLIC_KIOSK_THEME=light           # light | dark | colorful | glass | material
NEXT_PUBLIC_DEFAULT_LANGUAGE=th         # th | en
NEXT_PUBLIC_DEFAULT_FONT_SIZE=medium    # small | medium | large | xlarge (first-load only —
                                        #  patron can still change it via the accessibility bar)
NEXT_PUBLIC_INPUT_MODE=mouse            # mouse | touch — touch hides the cursor + disables hover
KIOSK_SERVICES=borrow,return,loans,fines   # remove any to hide from menu

# Session
SESSION_TIMEOUT_MINUTES=3
SCREENSAVER_TIMEOUT_MINUTES=5

# Email receipt (replaces thermal printer)
EMAIL_PROVIDER=smtp          # smtp | resend | sendgrid | disabled
SMTP_HOST=mail.example.com
SMTP_PORT=587
SMTP_USER=
SMTP_PASSWORD=
SMTP_FROM=library@example.com

# RFID (optional — Phase 4)
RFID_ENABLED=false
RFID_DRIVER=webserial
```

Full reference: [`apps/kiosk/.env.example`](./apps/kiosk/.env.example)

### Branding

The kiosk shows **no built-in logo or library name** — every deployment supplies its own. There are three ways to provide the logo and welcome video, tried in order:

1. **URL** — set `KIOSK_LOGO_URL` / `KIOSK_WELCOME_VIDEO_URL` to an absolute `https://…` URL or a root-relative `/path` (served via redirect)
2. **Bundled file** — drop a file in `apps/kiosk/public/branding/`:
   - logo: `logo.svg` | `logo.png` | `logo.jpg` | `logo.webp`
   - video: `welcome.mp4` | `welcome.webm` | `welcome.mov`
3. **Omitted** — leave both unset → the UI hides the element (no broken image)

The library name (`KIOSK_LIBRARY_NAME`) appears in the page `<title>` and the header on every screen. Set `KIOSK_LIBRARY_NAME_TH` / `KIOSK_LIBRARY_NAME_EN` to localize.

---

## Architecture

```
Patron Kiosk (touch screen, full-screen browser)
  └─ apps/kiosk  (Next.js 15 + PostgreSQL, Docker)
        ├─ NextAuth.js ──► SIP2 (barcode) or OIDC provider
        ├─ SIP2 Client ──LAN──► ILS (Koha / ALMA / WALAI / any SIP2 v2.0)
        └─ rfid-adapter ──USB──► ISO 15693 Reader (optional, Phase 4)

Bookdrop Station (drop slot, no screen)
  └─ apps/bookdrop  (Node.js service, planned Phase 5)
        ├─ rfid-adapter ──USB──► RFID sensor (detect tag as book slides through)
        └─ SIP2 Client ──LAN──► ILS (auto-checkin)

Staff Workstation (library PC, Chrome)
  └─ apps/workstation  (Next.js, LAN-hosted, planned Phase 6)
        └─ rfid-adapter ──USB──► ISO 15693 Reader (tag programming)
```

### Monorepo

```
next-open-selfcheck/
  apps/
    kiosk/          ← Patron self-check (Next.js, Docker)  ✅
    bookdrop/       ← Automated return (Node.js)           📋 planned
    workstation/    ← Staff RFID workstation (Next.js)     📋 planned
  packages/
    rfid-adapter/   ← ISO 15693 hardware abstraction (shared)
    sip2-client/    ← SIP2 TCP client + mock server (shared)
  docs/
    requirements.md ← Full product spec and design decisions
    architecture.md
    i18n.md
```

---

## SIP2 Messages Used

| Operation | Message | Response |
|---|---|---|
| Login | 93 → 94 | ok flag at char[2] |
| Patron auth | 63 (Patron Information) | 64 — BL=valid patron |
| Borrow | 11 (Checkout Request) | 12 — ok at [2] |
| Return | 09 (Checkin Request) | 10 — ok at [2], alert at [5] |
| My Loans | 63 + summary `  Y       ` | 64 — AU fields (charged items) |
| My Fines | 63 + summary `   Y      ` | 64 — BV field (total amount) |
| Patron email | 63 | 64 — BE field (for email receipt) |

---

## RFID

Optional. Set `RFID_ENABLED=false` for barcode-only mode.

Reference hardware: **ACS ACR1552U** (ISO 15693, USB, plug & play).

| AFI | Meaning | Written by |
|---|---|---|
| `0x07` | In library / available | Workstation (program) or Kiosk (check-in) |
| `0x02` | Checked out | Kiosk (check-out) |

Bookdrop is **RFID-only** — items checked out via barcode cannot use the drop slot.

---

## ILS Compatibility

Works with any **SIP2 v2.0** compatible system:

| ILS | Tested |
|---|---|
| ALIST (PSU Thailand) | ✅ |
| WALAI AutoLib | ✅ |
| Koha | ✅ (community) |
| Evergreen | ✅ (community) |
| ALMA, Symphony, Polaris | SIP2 v2.0 compliant — should work |

---

## Security

Hardened against an OWASP Top 10 pass, verified against a running instance:

- **SIP2 injection blocked** — patron ID / item barcode / PIN are rejected if they contain `|` or `\r`/`\n` (SIP2's own field delimiter and message terminator), both at the API and again at the SIP2 client.
- **Login rate limiting** — 5 failed attempts / 60s locks out for 5 minutes (by IP and by IP+patronId), since `AUTH_PIN_REQUIRED=false` means a correct ID alone signs a patron in.
- **Fail-closed auth secret** — a production deploy with no `NEXTAUTH_SECRET` refuses to serve authenticated routes rather than falling back to a default.
- **Security headers** — CSP, `X-Frame-Options`, `Referrer-Policy`, `Permissions-Policy`, HSTS on every response.
- **No patron PII in logs** — failed logins log a masked ID only; email receipts log delivery status, never the address.
- **No secrets in the Docker image** — `.env` is excluded from every build, not just via `.dockerignore`.

Full details: [`docs/requirements.md §16`](./docs/requirements.md#16-security-hardening).

---

## Development

### Mock SIP2 server

Simulates Checkout, Checkin, Patron Information, and Fee responses. No real ILS needed.

```bash
cd packages/sip2-client
npm install
npm run mock-server   # listens on TCP :6002
```

Mock data includes 2 sample loans and 0 fines. Edit `src/mock/server.ts` to add more scenarios.

### Running a single app

```bash
# from repo root
npm run dev --workspace=apps/kiosk
```

---

## Phase Roadmap

| Phase | Scope | Status |
|---|---|---|
| **1** | Scaffold, Welcome Screen, i18n (TH/EN), Docker, Mock SIP2 | ✅ Done |
| **2** | Auth — SIP2 barcode, OIDC-generic, middleware guard | ✅ Done |
| **3** | Main Menu, transaction screens, session timeout, 5-theme UI | ✅ Done |
| **3b** | Batch scan flow, email receipt, `KIOSK_SERVICES` toggle | ✅ Done |
| **4** | RFID integration — rfid-adapter, ISO 15693, AFI write on borrow/return | 🔄 Next |
| **5** | Bookdrop app — RFID-only auto-return, staff return log | 📋 Planned |
| **6** | Workstation app — staff RFID tag programming | 📋 Planned |
| **7** | First-Run Setup Wizard — web UI to generate `.env` on first boot | 📋 Planned |

Full requirements and design decisions: [`docs/requirements.md`](./docs/requirements.md)

---

## Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md).

**Open Source First** — every feature must work for any library worldwide without org-specific assumptions in core code. Configuration belongs in `.env`, not in source.

---

## Credits

- Based on [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) (PHP, GPL-3.0)
- Reference deployer: RMUTI (มทร.อีสาน), Thailand

## License

[GNU General Public License v3.0](./LICENSE)

This project is a derivative work of [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) and must remain GPL-3.0.
