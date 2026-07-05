# next-open-selfcheck

> ระบบยืม-คืนหนังสือด้วยตนเอง รุ่นใหม่ — พัฒนาด้วย Next.js ต่อยอดจาก [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check)

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Next.js](https://img.shields.io/badge/Next.js-15-black)](https://nextjs.org)
[![Node.js](https://img.shields.io/badge/Node.js-20+-green)](https://nodejs.org)
[![Based on](https://img.shields.io/badge/based%20on-GallusMax%2Fopen--source--self--check-orange)](https://github.com/GallusMax/open-source-self-check)

ระบบ Self-Check ห้องสมุดรุ่นใหม่ที่พัฒนาต่อยอดจาก [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) โดยเปลี่ยน stack จาก PHP มาเป็น **Node.js/Next.js** พร้อม UI 5 ธีม, ระบบ Auth แบบ pluggable และรองรับ RFID (optional)

ออกแบบมาสำหรับ **ห้องสมุดทุกแห่งทั่วโลก** — ผู้ deploy อ้างอิง (Reference Deployer) คือ มทร.อีสาน (RMUTI) ประเทศไทย ห้องสมุดที่ใช้ ILS รองรับ SIP2 (Koha, Evergreen, WALAI, ALIST ฯลฯ) สามารถ deploy ได้ทันทีโดยไม่ต้องแก้โค้ด

---

## สิ่งที่เพิ่มขึ้นจากระบบเดิม

| | [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) | **next-open-selfcheck** |
|---|---|---|
| **Stack** | PHP | Next.js 15 + Node.js |
| **SIP2** | PHP socket | Node.js `net` (server-side, รองรับทุก ILS) |
| **Authentication** | พื้นฐาน | NextAuth.js — สแกนบัตร/QR ผ่าน SIP2 หรือ OIDC provider ใดก็ได้ |
| **UI** | HTML พื้นฐาน | 5 ธีม: light / dark / colorful / glass / material |
| **การทำรายการ** | ทีละรายการ | Batch scan → ดูรายการ → ยืนยัน → ส่ง email |
| **ใบเสร็จ** | พิมพ์ slip | ส่ง email (รักษ์โลก ไม่ต้องพิมพ์) |
| **RFID** | — | ISO 15693 ผ่าน Web Serial API (optional) |
| **Bookdrop** | — | ตู้รับคืนอัตโนมัติ รองรับ RFID (แผนในอนาคต) |
| **Workstation** | — | สำหรับเจ้าหน้าที่โปรแกรม RFID Tag (แผนในอนาคต) |
| **Deploy** | PHP server | Docker per site |
| **Config** | แก้โค้ด | `.env` เท่านั้น — ไม่ต้องแก้โค้ดเพื่อ deploy |
| **i18n** | — | ภาษาไทย + อังกฤษ พร้อม community เพิ่มภาษาได้ |

---

## แอปพลิเคชัน

| แอป | คำอธิบาย | สถานะ |
|---|---|---|
| [`apps/kiosk`](./apps/kiosk) | ตู้ Self-Check — ยืม, คืน, ตรวจสอบรายการ, ค้นหาค่าปรับ | ✅ Phase 3 เสร็จแล้ว |
| [`apps/bookdrop`](./apps/bookdrop) | ตู้รับคืนอัตโนมัติ — RFID-only ไม่มีหน้าจอสัมผัส | 📋 แผน (Phase 5) |
| [`apps/workstation`](./apps/workstation) | Workstation เจ้าหน้าที่ — โปรแกรม RFID Tag | 📋 แผน (Phase 6) |

แต่ละแอป **เป็นอิสระต่อกัน** — deploy Kiosk ได้โดยไม่ต้องมี Bookdrop หรือ Workstation

---

## เริ่มต้นใช้งาน

### ความต้องการของระบบ

- Node.js 20+
- Docker + Docker Compose
- ILS ที่รองรับ SIP2 — หรือใช้ **mock server** ที่มีในโปรเจกต์สำหรับ development

### Development (ใช้ mock SIP2 ไม่ต้องมี ILS จริง)

```bash
# 1. ติดตั้ง dependencies (monorepo)
npm install

# 2. เริ่ม mock SIP2 server (TCP :6002)
cd packages/sip2-client
npm run mock-server

# 3. เปิด terminal ใหม่ เริ่ม kiosk dev server
cd apps/kiosk
cp .env.example .env.local
# ตั้ง SIP2_HOST=localhost ใน .env.local
npm run dev
```

เปิด `http://localhost:3000` — สแกน barcode ใดก็ได้เพื่อ login กับ mock server

### Production (Docker)

```bash
cd apps/kiosk
cp .env.example .env
# แก้ไข .env: SIP2_HOST, NEXTAUTH_SECRET, email config ฯลฯ
docker compose up -d
```

---

## Flow การใช้งาน (Kiosk)

```
หน้าจอต้อนรับ
  ↓ ผู้ใช้แตะหน้าจอ
หน้า Login  (สแกนบัตร/QR หรือ OIDC SSO)
  ↓ เข้าสู่ระบบสำเร็จ
เมนูหลัก  (แสดงเฉพาะบริการที่เปิดใช้งาน)
  ↓ เลือก ยืม หรือ คืน
โหมดสแกน  — สแกน/วาง RFID ได้หลายรายการ รายการสะสมใน list
  ↓ "สแกนเสร็จแล้ว"
โหมด Review  — ดูรายการทั้งหมด ลบรายการที่ไม่ต้องการได้
  ↓ ยืนยัน
กำลังประมวลผล  — ส่ง SIP2 ทุกรายการ
  ↓ เสร็จสิ้น
ส่ง email สรุปรายการให้ผู้ใช้  →  หน้าจอต้อนรับ
```

ตรวจสอบรายการยืมและค้นหาค่าปรับ แสดงข้อมูลจาก ILS อย่างเดียว ไม่ต้องยืนยัน

---

## ธีม UI

ตั้งค่าครั้งเดียวต่อตู้ผ่าน `NEXT_PUBLIC_KIOSK_THEME` ไม่ต้องแก้โค้ด

| ธีม | สไตล์ | เหมาะกับ |
|---|---|---|
| `light` *(ค่าเริ่มต้น)* | ขาว + Indigo accent | ใช้ทั่วไป ห้องสว่าง |
| `dark` | Slate-950 + Cyan neon | ห้องมืด ห้องสื่อ |
| `colorful` | ขาว + Violet accent | โรงเรียน ห้องสมุดเยาวชน |
| `glass` | Dark gradient + Frosted blur | ล็อบบี้ทันสมัย |
| `material` | M3 tonal surface + pill button | ผู้ใช้คุ้นเคยกับ Google |

ทุกธีมใช้ component tree เดียวกัน — เปลี่ยนธีม = เปลี่ยน ENV var แล้ว redeploy

---

## การตั้งค่า

**ไม่ต้องแก้โค้ดเพื่อ deploy** ทุก config อยู่ใน `.env`

### ตัวแปรหลัก — Kiosk

```env
# Branding & identity — ตั้งค่าสำหรับห้องสมุดของคุณ
KIOSK_LIBRARY_NAME=                          # แสดงใน <title>, header, ใบเสร็จ
                                             # (หรือใช้ KIOSK_LIBRARY_NAME_TH / _EN
                                             #  เพื่อแยกภาษา; ว่างไว้จะใช้ "ห้องสมุด")
KIOSK_LOGO_URL=                              # ภาพโลโก้ — ดูรายละเอียดที่ "Branding" ด้านล่าง
KIOSK_WELCOME_VIDEO_URL=                     # วิดีโอพื้นหลังหน้าต้อนรับ

# SIP2 connection
SIP2_HOST=192.168.1.100
SIP2_PORT=6002
SIP2_INSTITUTION=LIBRARY
SIP2_LOGIN_USER=
SIP2_LOGIN_PASSWORD=
SIP2_DEFAULT_CURRENCY=THB   # ใช้แสดงค่าปรับเฉพาะกรณี ILS ไม่ส่งฟิลด์ BH มา

# Auth mode: barcode | oidc | both
AUTH_MODE=barcode
AUTH_PIN_REQUIRED=false   # false = สแกนบัตร/QR เท่านั้น ไม่ต้องใส่ PIN

# OIDC (ต้องการเมื่อ AUTH_MODE=oidc หรือ both)
OIDC_ISSUER=https://keycloak.example.com/realms/library
OIDC_CLIENT_ID=
OIDC_CLIENT_SECRET=
OIDC_PROVIDER_NAME=ระบบ SSO สถาบัน
# OIDC claim ใดที่เก็บเลขประจำตัวผู้ใช้ที่ ILS ต้องการ (ฟิลด์ AA ของ SIP2)?
# default: preferred_username (username ของ Keycloak = รหัสนักศึกษา/บุคลากร)
# อาจใช้: student_id, library_barcode, employee_number — แล้วแต่ IdP ของคุณ
OIDC_PATRON_ID_CLAIM=preferred_username

# UI
NEXT_PUBLIC_KIOSK_THEME=light            # light | dark | colorful | glass | material
NEXT_PUBLIC_DEFAULT_LANGUAGE=th          # th | en
NEXT_PUBLIC_DEFAULT_FONT_SIZE=medium     # small | medium | large | xlarge (แค่ตอนโหลดครั้งแรก —
                                         #  ผู้ใช้ยังปรับเองผ่าน accessibility bar ได้)
NEXT_PUBLIC_INPUT_MODE=mouse             # mouse | touch — touch จะซ่อน cursor และปิด hover effect
KIOSK_SERVICES=borrow,return,loans,fines   # ลบบริการที่ไม่ต้องการออก

# Session
SESSION_TIMEOUT_MINUTES=3
SCREENSAVER_TIMEOUT_MINUTES=5

# Email ใบเสร็จ (แทนการพิมพ์ slip)
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

ดูรายละเอียดครบได้ที่ [`apps/kiosk/.env.example`](./apps/kiosk/.env.example)

### Branding

ตู้ kiosk **ไม่มีโลโก้หรือชื่อห้องสมุดใดติดมาเลย** — ทุกการ deploy ต้องใส่ของตัวเอง มี 3 วิธีตั้งค่าโลโก้และวิดีโอต้อนรับ (เรียงตามลำดับความสำคัญ):

1. **URL** — ตั้ง `KIOSK_LOGO_URL` / `KIOSK_WELCOME_VIDEO_URL` เป็น URL แบบ `https://…` หรือพาธ `/path` (เสิร์ฟผ่าน redirect)
2. **ไฟล์ในแพ็กเกจ** — วางไฟล์ใน `apps/kiosk/public/branding/`:
   - โลโก้: `logo.svg` | `logo.png` | `logo.jpg` | `logo.webp`
   - วิดีโอ: `welcome.mp4` | `welcome.webm` | `welcome.mov`
3. **ไม่ใส่** — ปล่อยว่างทั้งคู่ → UI จะซ่อนไม่แสดง (ไม่มีรูปแตก)

ชื่อห้องสมุด (`KIOSK_LIBRARY_NAME`) จะแสดงใน `<title>` และ header ของทุกหน้าจอ หากต้องการแยกภาษา ให้ตั้ง `KIOSK_LIBRARY_NAME_TH` / `KIOSK_LIBRARY_NAME_EN`

---

## สถาปัตยกรรมระบบ

```
ตู้ Kiosk (หน้าจอสัมผัส, browser เต็มจอ)
  └─ apps/kiosk  (Next.js 15 + PostgreSQL, Docker)
        ├─ NextAuth.js ──► SIP2 (บัตร) หรือ OIDC provider
        ├─ SIP2 Client ──LAN──► ILS (Koha / ALMA / WALAI / ทุก SIP2 v2.0)
        └─ rfid-adapter ──USB──► เครื่องอ่าน ISO 15693 (optional, Phase 4)

ตู้ Bookdrop (ช่องคืนหนังสือ ไม่มีหน้าจอ)
  └─ apps/bookdrop  (Node.js service, แผน Phase 5)
        ├─ rfid-adapter ──USB──► sensor RFID (detect tag ขณะหนังสือไหลผ่าน)
        └─ SIP2 Client ──LAN──► ILS (auto-checkin)

Workstation เจ้าหน้าที่ (PC ห้องสมุด, Chrome)
  └─ apps/workstation  (Next.js, LAN-hosted, แผน Phase 6)
        └─ rfid-adapter ──USB──► เครื่องอ่าน ISO 15693 (โปรแกรม Tag)
```

### โครงสร้าง Monorepo

```
next-open-selfcheck/
  apps/
    kiosk/          ← ตู้ Self-Check (Next.js, Docker)         ✅
    bookdrop/       ← ตู้รับคืนอัตโนมัติ (Node.js)             📋 แผน
    workstation/    ← Workstation เจ้าหน้าที่ (Next.js)        📋 แผน
  packages/
    rfid-adapter/   ← Hardware abstraction ISO 15693 (ใช้ร่วมกัน)
    sip2-client/    ← SIP2 TCP client + mock server (ใช้ร่วมกัน)
  docs/
    requirements.md ← Spec ครบถ้วนและ design decisions ทั้งหมด
    architecture.md
    i18n.md
```

---

## SIP2 Messages ที่ใช้

| การทำงาน | Message | Response |
|---|---|---|
| Login | 93 → 94 | ok flag ที่ char[2] |
| ยืนยันตัวตนผู้ใช้ | 63 (Patron Information) | 64 — BL=valid patron |
| ยืม | 11 (Checkout Request) | 12 — ok ที่ [2] |
| คืน | 09 (Checkin Request) | 10 — ok ที่ [2], alert ที่ [5] |
| ตรวจสอบรายการยืม | 63 + summary `  Y       ` | 64 — AU fields (charged items) |
| ค้นหาค่าปรับ | 63 + summary `   Y      ` | 64 — BV field (ยอดรวม) |
| Email ผู้ใช้ | 63 | 64 — BE field (สำหรับส่ง email ใบเสร็จ) |

---

## RFID

เป็น optional — ตั้ง `RFID_ENABLED=false` เพื่อใช้ Barcode เท่านั้น

Hardware อ้างอิง: **ACS ACR1552U** (ISO 15693, USB, plug & play)

| ค่า AFI | ความหมาย | เขียนโดย |
|---|---|---|
| `0x07` | อยู่ในห้องสมุด / พร้อมให้ยืม | Workstation (program) หรือ Kiosk (check-in) |
| `0x02` | ถูกยืมออก | Kiosk (check-out) |

**Bookdrop รองรับเฉพาะ RFID เท่านั้น** — หนังสือที่ยืมผ่าน Barcode ไม่สามารถคืนผ่านตู้ Bookdrop ได้

---

## ILS ที่รองรับ

รองรับ ILS ที่ใช้ **SIP2 v2.0** ทุกระบบ:

| ILS | สถานะ |
|---|---|
| ALIST (มอ. / PSU Thailand) | ✅ |
| WALAI AutoLib | ✅ |
| Koha | ✅ (community) |
| Evergreen | ✅ (community) |
| ALMA, Symphony, Polaris | SIP2 v2.0 compliant — ควรใช้งานได้ |

---

## การพัฒนา

### Mock SIP2 Server

ไม่ต้องมี ILS จริงระหว่าง development:

```bash
cd packages/sip2-client
npm install
npm run mock-server   # เปิด TCP server ที่ port 6002
```

มีข้อมูล mock: รายการยืม 2 รายการ, ค่าปรับ 0 บาท แก้ไขได้ที่ `src/mock/server.ts`

### รันแอปเดียว

```bash
# จาก root ของ monorepo
npm run dev --workspace=apps/kiosk
```

---

## แผนการพัฒนา

| Phase | ขอบเขต | สถานะ |
|---|---|---|
| **1** | Scaffold, Welcome Screen, i18n (TH/EN), Docker, Mock SIP2 | ✅ เสร็จแล้ว |
| **2** | Auth — SIP2 barcode, OIDC-generic, middleware guard | ✅ เสร็จแล้ว |
| **3** | เมนูหลัก, transaction screens, session timeout, UI 5 ธีม | ✅ เสร็จแล้ว |
| **3b** | Batch scan flow, email ใบเสร็จ, `KIOSK_SERVICES` toggle | ✅ เสร็จแล้ว |
| **4** | RFID — rfid-adapter, ISO 15693, เขียน AFI ตอนยืม/คืน | 🔄 ถัดไป |
| **5** | Bookdrop app — auto-return RFID, log รายการคืน | 📋 แผน |
| **6** | Workstation app — โปรแกรม RFID Tag | 📋 แผน |
| **7** | First-Run Setup Wizard — web UI สร้าง `.env` ตอน boot ครั้งแรก | 📋 แผน |

ดู spec และ design decisions ครบถ้วนได้ที่ [`docs/requirements.md`](./docs/requirements.md)

---

## การมีส่วนร่วม

ดูรายละเอียดที่ [CONTRIBUTING.md](./CONTRIBUTING.md)

หลักการ **Open Source First** — ทุก contribution ต้องใช้งานได้กับห้องสมุดทุกแห่ง ทุกประเทศ โดยไม่มี assumption เฉพาะองค์กรใดในโค้ดหลัก config ทั้งหมดอยู่ใน `.env` ไม่ใช่ใน source code

---

## เครดิต

- พัฒนาต่อยอดจาก [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) (PHP, GPL-3.0)
- Reference Deployer: มทร.อีสาน (RMUTI), ประเทศไทย

## License

[GNU General Public License v3.0](./LICENSE)

โปรเจกต์นี้เป็น derivative work ของ [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) และต้องใช้ license GPL-3.0 ต่อไป

---

[🇬🇧 English README](./README.md)
