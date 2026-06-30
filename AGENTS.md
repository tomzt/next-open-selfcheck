# AGENTS.md — Operating context for AI coding agents

> ไฟล์นี้ให้ AI agent (Claude, ZCode, ฯลฯ) อ่านทุกครั้งตอนเริ่ม session
> มันฝัง context ที่ agent ไม่รู้จากโค้ดอย่างเดียว และบังคับนิสัยการทำงานที่รอบคอบ
> ถ้า context สำคัญเปลี่ยน — อัปเดตไฟล์นี้ก่อนเริ่มงาน

---

## 1. ธรรมชาติของ project (สำคัญที่สุด — อ่านก่อนเขียนอะไร)

**next-open-selfcheck เป็น open-source ที่ห้องสมุดทุกแห่งทั่วโลก clone ไป deploy เอง** — ไม่ใช่ app ใช้เองที่เดียวในโลก

Reference deployer คือ มทร.อีสาน (RMUTI) แต่โค้ดหลัก **ห้ามมี assumption เฉพาะของสถาบันใดเลย**

จากนี้สืบเนื่องกฎเหล็ก 3 ข้อ:

- **🚫 ห้าม hardcode** brand, ชื่อห้องสมุด, โลโก้, สี, URL, ข้อความเฉพาะองค์กร ใน source code
- **⚙️ ทุก config ต้องตั้งผ่าน `.env`** (หรือ First-Run Setup Wizard ในอนาคต) — ไม่ใช่แก้โค้ด
- **🌍 ทุก string ที่ผู้ใช้เห็นต้องผ่าน i18n** (`apps/kiosk/messages/{th,en}.json`) — ห้ามใส่ข้อความตรง ๆ ใน component

> กฎตัดสินใจ: ถ้าคำถามคือ "จะใส่ค่า X ที่ไหน?" → คำตอบเกือบทุกครั้งคือ **ENV var ใหม่ใน `.env.example` + resolver ใน `src/lib/`** ไม่ใช่ hardcode

---

## 2. กฎการทำงาน (ทำตามทุกครั้ง)

### 2.1 อ่านก่อนสรุป และรายงานสิ่งที่อ่าน
- **อ่านให้ครบก่อนสรุปอะไร** — ถ้าสรุปแล้วยังมีไฟล์สำคัญที่ไม่ได้อ่าน ถือว่าสรุปก่อนเวลา
- เมื่อสรุป ให้บอกด้วยว่า **อ่านไฟล์อะไรไปแล้วบ้าง และอะไรที่ยังไม่ได้อ่าน/เหตุผล**
- ถ้าไม่แน่ใจเรื่องใด → พูดว่า "ยังไม่ได้ตรวจสอบ" ห้ามเดา

### 2.2 ไฟล์ที่ต้องอ่านตอนเริ่ม session ทุกครั้ง
อ่านทั้งหมดนี้ก่อนลงมือ:
- `README.md` — **ต้นฉบับ (source of truth)** อ่านก่อนเสมอ
- `README.th.md` — ฉบับแปล อ่านควบ เพื่อเปรียบเทียบความต่าง/ล้าสมัย
- `docs/requirements.md` — spec ครบ + design decisions (พูดถึง feature ก่อนที่จะมีในโค้ด)
- `docs/architecture.md` — โครงสร้าง + สัญญาที่วางไว้ (เช่น retry policy)
- `apps/kiosk/.env.example` — config ทั้งหมดที่รองรับจริง
- `apps/kiosk/package.json` — scripts + dependencies

### 2.3 แยกประเภทปัญหาออกจากกัน
ห้ามเรียกทุกอย่างว่า "bug" แยกออกเป็น 3 กลุ่มเสมอ:

| ประเภท | ความหมาย | ตัวอย่าง |
|---|---|---|
| **🐛 bug** | โค้ดที่มีอยู่ทำงานผิด | parse SIP2 ผิดตำแหน่ง, hook ใช้ผิดกฎ |
| **🧩 missing feature** | doc/spec กำหนดไว้ แต่ยังไม่ถูกสร้าง | batch scan flow, email receipt |
| **🔧 config gap** | feature มีแต่ deployer ยัง config ไม่ได้ | ไม่มี ENV สำหรับโลโก้/ชื่อห้องสมุด |

ลำดับความสำคัญสำหรับ open-source: **config gap > missing feature > bug** เพราะ config gap คือสิ่งแรกที่คนอื่นที่เอาไป deploy จะเจอ

### 2.4 ถามก่อน assume
- ก่อนลงมือ ถ้ามีคำถามที่คำตอบเปลี่ยนสิ่งที่จะทำ → ถามผู้ใช้ก่อน
- อย่าเดาจากโค้ดอย่างเดียว เพราะ context มักอยู่นอกโค้ด (เช่น "เป็น open source")
- แต่ถ้าคำตอบหาได้จากไฟล์ใน repo → อ่านเอง อย่าถาม

### 2.5 คุ้มครองผู้ใช้ (privacy)
- ชื่อ + เลขบัตร patron **ห้ามเขียนลง disk** (requirements §13)
- log เก็บได้แค่ metadata (timestamp, item barcode, action)

### 2.6 README sync — ไทย ↔ อังกฤษ
**Source of truth = `README.md` (อังกฤษ)** เพราะเป็น open-source สากล, contributor นอกไทยจะแก้อังกฤษก่อนเป็นธรรมดา

เมื่อแก้สิ่งที่ผู้ใช้ deploy เจอ → แก้ `README.md` ก่อน แล้ว sync ไป `README.th.md`

**ต้อง sync ทั้งคู่ (user-facing):**
- เปลี่ยน/เพิ่ม/ลบ ENV var
- เปลี่ยน flow ผู้ใช้ หรือ feature list / status
- เปลี่ยนสถานะ roadmap (Phase)
- เปลี่ยน architecture หรือ ILS/SIP2 behavior

**ไม่ต้อง sync (internal only):**
- refactor, bug fix ภายใน, เปลี่ยนชื่อตัวแปร
- เปลี่ยน dependency ที่ผู้ใช้ไม่รับรู้
- typo ใน source code

**ถ้าแก้ได้แค่ข้างเดียว** (เช่น ขาดเวลา/ความรู้ภาษา):
ทิ้งบันทึกใน commit message ว่า `docs(th): pending sync` หรือ `docs(en): pending sync`
เพื่อให้คนถัดไปรู้ว่ามี drift รอจัดการ — ดีกว่าบังคับแปลเร่งแล้วผิด

### 2.6 README sync (ไทย ↔ อังกฤษ)

**Source of truth = `README.md` (อังกฤษ)**
- อังกฤษคือต้นฉบับ เพราะเป็น open-source สากล — contributor นอกไทยจะแก้ที่นี่ก่อนเสมอ
- เมื่อแก้ของที่ผู้ใช้ deploy เจอ → **แก้ `README.md` ก่อน แล้วจึง sync ไป `README.th.md`**

**ต้อง sync ทั้งคู่** (เมื่อเปลี่ยนสิ่งที่ deployer/patron เจอ):
- เพิ่ม/ลบ/เปลี่ยน ENV var
- เปลี่ยน flow ผู้ใช้ หรือรายการ feature
- เปลี่ยนสถานะ roadmap / phase
- เปลี่ยน architecture หรือโครงสร้าง repo

**ไม่ต้อง sync** (internal only, ผู้ใช้ไม่รับรู้):
- refactor, bug fix ภายใน, เปลี่ยนชื่อตัวแปร
- เปลี่ยน dependency ที่ผู้ใช้ไม่เห็น
- แก้ typo ในโค้ด

**เมื่อแก้ได้แค่ข้างเดียว** (sync ไม่ครบ):
- ทิ้งบันทึกใน commit message: `docs(th): pending sync` หรือ `docs(en): pending sync`
- เพื่อให้คนถัดไปเห็นว่ามี drift ค้าง แล้วยกเลิกบันทึกนั้นเมื่อ sync ครบ

---

## 3. สถาปัตยกรรม — สิ่งที่มีจริง vs สิ่งที่วางไว้

หลายอย่างใน README/doc **ยังไม่มีในโค้ด** อย่าถูกหลอก:

| เรื่อง | ใน doc | ในโค้ดจริง |
|---|---|---|
| 5 themes | ✅ | ✅ มีครบ (`globals.css`) |
| Auth barcode + OIDC | ✅ | ✅ (`src/lib/auth.ts`) |
| SIP2 client | ✅ | ✅ (`src/lib/sip2-client.ts`) |
| Mock SIP2 server | ✅ | ✅ (`packages/sip2-client/src/mock/server.ts`) |
| **Batch scan → review → confirm** | ✅ | ❌ ปัจจุบันทีละรายการ (Phase 3b) |
| **Email receipt** | ✅ | ❌ ยังไม่มี (Phase 3b) |
| **`KIOSK_SERVICES` toggle** | ✅ | ❌ menu แสดงครบเสมอ |
| **PostgreSQL transaction_log** | ✅ | ❌ dep ลงไว้แต่ไม่ได้ใช้ |
| **SIP2 retry logic** | ✅ | ❌ ENV มี แต่ `withSession` ไม่ retry |
| **`/api/config/logo` + `/welcome-video`** | (UI เรียก) | ❌ route ไม่มี |
| RFID driver | ✅ | ❌ มีแค่ interface (Phase 4) |
| Bookdrop / Workstation app | ✅ | ❌ stub (Phase 5/6) |
| First-Run Setup Wizard | ✅ | ❌ (Phase 7) |

---

## 4. โครงสร้างที่สำคัญ

```
apps/kiosk/
  src/
    lib/        ← ทุก config reader + business logic (server-side)
      auth.ts          # NextAuth config, AUTH_MODE
      sip2-client.ts   # TCP client — stateless, server-only
      sip2-patron.ts   # auth via SIP2 msg 63
      theme.ts         # NEXT_PUBLIC_KIOSK_THEME
    app/
      [locale]/        # i18n routing (next-intl)
        auth/ menu/    # pages → client components
      api/
        auth/[...nextauth]/   # NextAuth handler
        sip2/{checkout,checkin,loans,fines}/  # server actions → SIP2
    components/   # client UI, ใช้ --kt-* CSS vars
    hooks/        # useSessionTimeout (idle)
  middleware.ts    # JWT guard + locale routing
  i18n.ts          # locales = ['th','en']
  messages/{th,en}.json
packages/
  sip2-client/     # client lib + mock server
  rfid-adapter/    # interface only (Phase 4)
```

### สัญญาที่ต้องรักษา
- `src/lib/sip2-*.ts` = **server-side only** (ใช้ `net`) — ห้าม import ใน client component
- ทุก SIP2 call ผ่าน `withSession()` = เปิด TCP → login (msg 93) → ทำ → disconnect ทุกครั้ง (stateless)
- Theme = CSS custom property `--kt-*` เปลี่ยนที่ `data-kiosk-theme` บน `<html>`
- Patron data อยู่ใน NextAuth JWT (`patronId`, `patronName`) — ไม่มี DB session

---

## 5. เมื่อเพิ่ม feature ใหม่ — checklist

ก่อนถือว่าเสร็จ ต้องครบ:
- [ ] config ใหม่เพิ่มใน `apps/kiosk/.env.example` พร้อมคอมเมนต์
- [ ] ไม่มี brand/ค่าเฉพาะองค์กร hardcode ใน source
- [ ] string ที่ผู้ใช้เห็นเข้า `messages/th.json` + `messages/en.json` คู่กัน
- [ ] มี SPDX license header (`// SPDX-License-Identifier: GPL-3.0-or-later`)
- [ ] ไม่ violate กฎ privacy (§2.5)
- [ ] อัปเดต doc ที่เกี่ยวข้อง (README/docs) ให้ตรงโค้ด

---

## 6. การสื่อสารกับผู้ใช้

- คุยภาษาไทยเป็นหลัก (ตามที่ผู้ใช้กำหนด)
- ใส่ตัวเลข/ตาราง/หัวข้อ เพื่อให้สแกนได้เร็ว
- เมื่อแจ้งปัญหา ให้บอก **ประเภท (bug/missing/config-gap) + ไฟล์:บรรทัด + เหตุผล**
- ห้าม oversell: ถ้ายังไม่ได้ test ให้บอกว่ายังไม่ได้ test
