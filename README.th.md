# next-open-selfcheck

> ระบบยืม-คืนหนังสือด้วยตนเอง รุ่นใหม่ — พัฒนาด้วย Next.js ต่อยอดจาก [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check)

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Next.js](https://img.shields.io/badge/Next.js-15-black)](https://nextjs.org)
[![Node.js](https://img.shields.io/badge/Node.js-20+-green)](https://nodejs.org)
[![Based on](https://img.shields.io/badge/based%20on-GallusMax%2Fopen--source--self--check-orange)](https://github.com/GallusMax/open-source-self-check)

ระบบ Self-Check ห้องสมุดรุ่นใหม่ที่พัฒนาต่อยอดจาก [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) โดยเปลี่ยน stack จาก PHP มาเป็น **Node.js/Next.js** พร้อม architecture แบบ pluggable และ Workstation สำหรับจัดการ RFID (optional)

ออกแบบมาสำหรับ **ห้องสมุดทุกแห่งทั่วโลก** — ผู้ deploy อ้างอิง (Reference Deployer) คือ มทร.อีสาน (RMUTI) ประเทศไทย ห้องสมุดที่ใช้ ILS ที่รองรับ SIP2 (Koha, Evergreen, WALAI, ALIST ฯลฯ) สามารถ deploy ระบบนี้ได้ทันที

---

## สิ่งที่เพิ่มขึ้นจากระบบเดิม

| | [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) | **next-open-selfcheck** |
|---|---|---|
| **Stack** | PHP | Next.js + Node.js |
| **SIP2** | PHP socket | Node.js `net` module (server-side only) |
| **Authentication** | พื้นฐาน | NextAuth.js (เปลี่ยน provider ได้) |
| **RFID** | — | ISO 15693 ผ่าน Web Serial API (optional) |
| **Workstation** | — | สำหรับเจ้าหน้าที่โปรแกรม RFID Tag (optional) |
| **Deploy** | PHP server | Docker per site |
| **Config** | แก้โค้ด | `.env` + First-Run Setup Wizard |

---

## แอปพลิเคชัน

| แอป | คำอธิบาย | Deploy |
|---|---|---|
| [`apps/kiosk`](./apps/kiosk) | ตู้ Self-Check สำหรับผู้ใช้ — ยืม, คืน, ตรวจสอบรายการ, ค้นหาค่าปรับ | Docker per site |
| [`apps/workstation`](./apps/workstation) | Workstation สำหรับเจ้าหน้าที่ — โปรแกรม Tag, เขียน AFI, Batch program | LAN web app (optional) |

ทั้งสองแอปเป็น **อิสระต่อกัน** — deploy Kiosk ได้โดยไม่ต้องมี Workstation ถ้าไม่ใช้ RFID

---

## เริ่มต้นใช้งาน

### ความต้องการของระบบ

- Node.js 20+
- Docker + Docker Compose (สำหรับ Kiosk)
- Chrome/Chromium (สำหรับ Workstation — Web Serial API)
- ILS ที่รองรับ SIP2 (หรือใช้ Mock SIP2 server สำหรับ development)

### Kiosk

```bash
cd apps/kiosk
cp .env.example .env
# แก้ไข .env ตามค่าของระบบ เช่น SIP2 host, auth provider
docker compose up -d
```

เปิด `http://localhost:3000` — First-Run Setup Wizard จะแนะนำการตั้งค่าทีละขั้นตอน

### Workstation (optional — สำหรับ RFID เท่านั้น)

```bash
cd apps/workstation
cp .env.example .env
# แก้ไข .env เช่น library code, ILS lookup URL
npm install
npm run dev
```

เปิด `http://localhost:3001` ด้วย Chrome/Chromium แล้วเสียบ RFID Reader ISO 15693 (เช่น ACS ACR1552U)

---

## สถาปัตยกรรมระบบ

```
ตู้ Kiosk (หน้าจอสัมผัส)
  └─ apps/kiosk (Next.js + PostgreSQL, Docker)
        └─ SIP2 over TCP ──LAN──► ILS (รองรับ SIP2 ทุกยี่ห้อ)
        └─ RFID (Phase 3, optional) ผ่าน packages/rfid-adapter

Workstation เจ้าหน้าที่ (PC ห้องสมุด)
  └─ apps/workstation (Next.js, LAN-hosted)
        └─ USB ──► RFID Reader ISO 15693 (เช่น ACR1552U)
        └─ RFID ผ่าน packages/rfid-adapter (package เดียวกับ Kiosk)
```

### โครงสร้าง Monorepo

```
next-open-selfcheck/
  apps/
    kiosk/              ← ตู้ Self-Check (Next.js, Docker)
    workstation/        ← Workstation เจ้าหน้าที่ (Next.js, LAN)
  packages/
    rfid-adapter/       ← Hardware abstraction layer (ใช้ร่วมกัน)
      src/
        interface.ts    ← RFIDAdapter interface
        web-serial/     ← Driver ISO 15693 ผ่าน Web Serial API
    sip2-client/        ← SIP2 TCP client (ใช้โดย Kiosk)
    ui-components/      ← UI components ร่วม (optional)
  docs/
```

---

## Flow การใช้งาน (Kiosk)

```
หน้าจอต้อนรับ (วิดีโอ loop)
  → ผู้ใช้แตะหน้าจอ
หน้า Login (provider ตาม config)
  → เข้าสู่ระบบสำเร็จ
เมนูหลัก: ยืม / คืน / ตรวจสอบรายการ / ค้นหาค่าปรับ
  → สแกน Barcode หรือ RFID Tag
ทำรายการ → SIP2 → แสดงผล
  → เสร็จสิ้นหรือหมดเวลา → หน้าต้อนรับ
```

---

## การตั้งค่า

ทุก config ผ่านไฟล์ `.env` และ First-Run Setup Wizard — **ไม่ต้องแก้ไข source code** เพื่อ deploy ในองค์กรของคุณ

### Kiosk — ตัวแปรหลักใน `.env`

```env
# SIP2
SIP2_HOST=192.168.1.100
SIP2_PORT=6002
SIP2_CHECKSUM_ENABLED=false

# Auth (เปลี่ยน provider ได้ — รองรับ NextAuth.js ทุก provider)
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

### Workstation — ตัวแปรหลักใน `.env`

```env
RFID_ENABLED=true
RFID_DRIVER=webserial
LIBRARY_CODE=รหัสห้องสมุดของคุณ
STAFF_AUTH_SECRET=เปลี่ยน-ค่านี้
STAFF_AUTH_USER=admin
STAFF_AUTH_PASSWORD=เปลี่ยน-รหัสผ่าน
ILS_LOOKUP_ENABLED=false
DEFAULT_LANGUAGE=th
```

ดูรายละเอียดครบได้ที่ [`apps/kiosk/.env.example`](./apps/kiosk/.env.example) และ [`apps/workstation/.env.example`](./apps/workstation/.env.example)

---

## RFID

RFID เป็น **optional** — ตั้ง `RFID_ENABLED=false` เพื่อใช้ Barcode เท่านั้น

Hardware อ้างอิง: **ACS ACR1552U** (ISO 15693, USB Type-C, plug & play บน Windows/Linux/macOS)

| ค่า AFI | ความหมาย | เขียนโดย |
|---|---|---|
| `0x07` | อยู่ในห้องสมุด / พร้อมให้ยืม | Workstation (program) หรือ Kiosk (check-in) |
| `0x02` | ถูกยืมออกไป | Kiosk (check-out) |

---

## แผนการพัฒนา

| Phase | ขอบเขต | สถานะ |
|---|---|---|
| **1 — Core MVP** | Kiosk: Auth + Barcode + Mock SIP2 | 🔄 กำลังพัฒนา |
| **2 — Open Source Release** | Docs, pluggable arch, i18n, fork | ⏳ ถัดไป |
| **3 — Hardware** | Kiosk RFID + Workstation (พร้อมกัน) | ⏳ อนาคต |
| **4 — Contribution Round 2** | Hardware docs upstream | ⏳ อนาคต |

---

## ILS ที่รองรับ

รองรับ ILS ที่ใช้ SIP2 v2.0 ทุกระบบ:

- ✅ ALIST (PSU)
- ✅ Koha
- ✅ Evergreen
- ✅ WALAI AutoLib
- ✅ ทุก ILS ที่รองรับ SIP2 v2.0

---

## การพัฒนา

### Mock SIP2 Server

ไม่จำเป็นต้องมี ILS จริงระหว่าง development:

```bash
cd packages/sip2-client
npm run mock-server   # เปิด TCP server ที่ port 6002
```

### เริ่มทุกแอปพร้อมกัน

```bash
npm install
npm run dev
```

---

## การมีส่วนร่วม

ดูรายละเอียดที่ [CONTRIBUTING.md](./CONTRIBUTING.md)

หลักการ **Open Source First** — ทุก contribution ต้องใช้งานได้กับห้องสมุดทุกแห่ง ทุกประเทศ โดยไม่มี assumption เฉพาะองค์กรใดในโค้ดหลัก

---

## เครดิต

- พัฒนาต่อยอดจาก [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) (PHP, GPL-3.0)
- Thai localization พัฒนาโดย ต้อม (Tom)
- Reference Deployer: มทร.อีสาน (RMUTI), ประเทศไทย

---

## License

[GNU General Public License v3.0](./LICENSE)

โปรเจกต์นี้เป็น derivative work ของ [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) และต้องใช้ license GPL-3.0 ต่อไป

---

[🇬🇧 English README](./README.md)
