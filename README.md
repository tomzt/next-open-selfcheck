# next-open-selfcheck

> Next-generation library self-check system — Next.js re-engineering of [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check)

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Next.js](https://img.shields.io/badge/Next.js-15-black)](https://nextjs.org)
[![Node.js](https://img.shields.io/badge/Node.js-20+-green)](https://nodejs.org)
[![Based on](https://img.shields.io/badge/based%20on-GallusMax%2Fopen--source--self--check-orange)](https://github.com/GallusMax/open-source-self-check)

A next-generation re-engineering of [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) — rebuilt from PHP to **Node.js/Next.js** with a modern stack, pluggable architecture, and optional RFID workstation.

Built for **any library worldwide** — RMUTI (Rajamangala University of Technology Isan, Thailand) is the reference deployer. Any library using a SIP2-compatible ILS (Koha, Evergreen, WALAI, ALIST, etc.) can deploy this system.

## What's new vs upstream

| | [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) | **next-open-selfcheck** |
|---|---|---|
| **Stack** | PHP | Next.js + Node.js |
| **SIP2** | PHP socket | Node.js `net` module (server-side only) |
| **Auth** | Basic | NextAuth.js (pluggable providers) |
| **RFID** | — | ISO 15693 via Web Serial API (optional) |
| **Workstation** | — | Staff RFID workstation (optional) |
| **Deploy** | PHP server | Docker per site |
| **Config** | Code edit | `.env` + First-Run Setup Wizard |

---

## Apps

| App | Description | Deploy |
|---|---|---|
| [`apps/kiosk`](./apps/kiosk) | Patron self-check terminal — borrow, return, check status, fee inquiry | Docker per site |
| [`apps/workstation`](./apps/workstation) | Staff RFID workstation — program tags, write AFI, batch operations | LAN web app (optional) |

Both apps are **independent** — deploy Kiosk without Workstation if you don't use RFID.

---

## Quick Start

### Prerequisites

- Node.js 20+
- Docker + Docker Compose (Kiosk only)
- Chrome/Chromium (Workstation — Web Serial API)
- A SIP2-compatible ILS (or use the built-in mock server for development)

### Kiosk

```bash
cd apps/kiosk
cp .env.example .env
# Edit .env with your SIP2 host, auth provider, etc.
docker compose up -d
```

Open `http://localhost:3000` — the First-Run Setup Wizard will guide you through configuration.

### Workstation (optional — RFID only)

```bash
cd apps/workstation
cp .env.example .env
# Edit .env with your library code, ILS lookup URL, etc.
npm install
npm run dev
```

Open `http://localhost:3001` in Chrome/Chromium. Plug in your ISO 15693 USB RFID reader (e.g. ACS ACR1552U).

---

## Architecture

```
Patron Kiosk (touch screen)
  └─ apps/kiosk (Next.js + PostgreSQL, Docker)
        └─ SIP2 over TCP ──LAN──► ILS (any SIP2-compatible)
        └─ RFID (Phase 3, optional) via packages/rfid-adapter

Staff Workstation (library PC)
  └─ apps/workstation (Next.js, LAN-hosted)
        └─ USB ──► ISO 15693 RFID Reader (e.g. ACR1552U)
        └─ RFID via packages/rfid-adapter (same package as kiosk)
```

### Monorepo

```
next-open-selfcheck/
  apps/
    kiosk/              ← Patron self-check (Next.js, Docker)
    workstation/        ← Staff RFID workstation (Next.js, LAN)
  packages/
    rfid-adapter/       ← Hardware abstraction layer (shared)
      src/
        interface.ts    ← RFIDAdapter interface
        web-serial/     ← ISO 15693 driver via Web Serial API
    sip2-client/        ← SIP2 TCP client (used by kiosk)
    ui-components/      ← Shared UI primitives (optional)
  docs/
```

---

## Patron Flow (Kiosk)

```
Welcome Screen (video loop)
  → patron taps
Auth Screen (pluggable provider)
  → authenticated
Main Menu: Borrow / Return / Check Status / Fee Inquiry
  → scan barcode or RFID tag
Transaction → SIP2 → result displayed
  → timeout or complete → Welcome Screen
```

---

## Configuration

All configuration via `.env` files and First-Run Setup Wizard. **No source code modification required** to deploy for any organization.

### Kiosk — key `.env` variables

```env
# SIP2
SIP2_HOST=192.168.1.100
SIP2_PORT=6002
SIP2_CHECKSUM_ENABLED=false

# Auth (pluggable — any NextAuth.js provider)
AUTH_PROVIDER=keycloak
KEYCLOAK_ISSUER=https://your-keycloak/realms/your-realm
KEYCLOAK_CLIENT_ID=
KEYCLOAK_CLIENT_SECRET=

# RFID (optional)
RFID_ENABLED=false
RFID_DRIVER=webserial

# Session
SESSION_TIMEOUT_MINUTES=3
SCREENSAVER_TIMEOUT_MINUTES=5
```

### Workstation — key `.env` variables

```env
RFID_ENABLED=true
RFID_DRIVER=webserial
LIBRARY_CODE=your-library-code
STAFF_AUTH_SECRET=change-this-secret
STAFF_AUTH_USER=admin
STAFF_AUTH_PASSWORD=change-this
ILS_LOOKUP_ENABLED=false
DEFAULT_LANGUAGE=en
```

See [`apps/kiosk/.env.example`](./apps/kiosk/.env.example) and [`apps/workstation/.env.example`](./apps/workstation/.env.example) for full reference.

---

## RFID Support

RFID is **optional** — set `RFID_ENABLED=false` for barcode-only mode.

Reference hardware: **ACS ACR1552U** (ISO 15693, USB Type-C, plug & play on Windows/Linux/macOS).

| AFI Value | Meaning | Written by |
|---|---|---|
| `0x07` | In library / available | Workstation (program) or Kiosk (check-in) |
| `0x02` | Checked out | Kiosk (check-out) |

---

## Phases

| Phase | Scope | Status |
|---|---|---|
| **1 — Core MVP** | Kiosk: Auth + Barcode + Mock SIP2 | 🔄 In progress |
| **2 — Open Source Release** | Docs, pluggable arch, i18n, fork | ⏳ Next |
| **3 — Hardware** | Kiosk RFID + Workstation (together) | ⏳ Future |
| **4 — Contribution Round 2** | Hardware docs upstream | ⏳ Future |

---

## ILS Compatibility

Works with any SIP2 v2.0 compatible ILS:

- ✅ ALIST (PSU)
- ✅ Koha
- ✅ Evergreen
- ✅ WALAI AutoLib
- ✅ Any SIP2 v2.0 compatible system

---

## Development

### Mock SIP2 Server

No real ILS needed during development:

```bash
cd packages/sip2-client
npm run mock-server   # TCP server on port 6002
```

### Run all apps

```bash
npm install
npm run dev
```

---

## Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md).

**Open Source First** — every contribution must work for any library, any country, without org-specific assumptions in core code.

---

## Credits

- Based on [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) (PHP, GPL-3.0)
- Thai localization originally developed by Tom (ต้อม)
- Reference deployer: RMUTI (มทร.อีสาน), Thailand

## License

[GNU General Public License v3.0](./LICENSE)

This project is a derivative work of [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) and must remain GPL-3.0.
