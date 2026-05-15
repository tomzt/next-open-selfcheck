# Setup Guide — Push to GitHub + Gitea Mirror

## Step 1 — Create GitHub repo

ไปที่ https://github.com/new แล้วสร้าง repo:

- **Repository name:** `next-open-selfcheck`
- **Description:** `Next-generation library self-check system — Next.js re-engineering of GallusMax/open-source-self-check`
- **Visibility:** Public
- **Initialize:** ❌ ไม่ต้อง (อย่า tick อะไรเลย)

---

## Step 2 — Push โค้ด

```bash
# แตก zip แล้วเข้าไปใน folder
unzip next-open-selfcheck-init.zip
cd next-open-selfcheck

# Init git
git init
git add .
git commit -m "chore: initial project scaffold

Next-generation re-engineering of GallusMax/open-source-self-check
- Next.js + Node.js stack (replaces PHP)
- Pluggable auth via NextAuth.js
- Optional RFID workstation (ISO 15693)
- Docker per-site deployment
- All config via .env + Setup Wizard"

# Push to GitHub
git remote add origin https://github.com/tomzt/next-open-selfcheck.git
git branch -M main
git push -u origin main
```

---

## Step 3 — Setup Gitea Mirror (gitnet.rmuti.ac.th)

หลัง push GitHub แล้ว ทำ mirror ที่ Gitea:

1. Login ที่ `gitnet.rmuti.ac.th`
2. ไปที่ **[+] → New Migration → GitHub**
3. กรอก:
   - **Clone URL:** `https://github.com/tomzt/next-open-selfcheck`
   - **Owner:** เลือก org หรือ personal
   - **Repository name:** `next-open-selfcheck`
   - **Mirror:** ✅ เปิด
   - **Mirror interval:** `8h` (sync ทุก 8 ชั่วโมง)
4. กด **Migrate**

หลังจากนี้ — แก้โค้ดที่ GitHub เท่านั้น Gitea จะ sync ตามอัตโนมัติ

---

## Step 4 — ตรวจสอบ

```
✅ https://github.com/tomzt/next-open-selfcheck  — main repo
✅ gitnet.rmuti.ac.th/.../next-open-selfcheck     — mirror (read-only)
```

---

## Workflow ต่อไป

```
แก้โค้ด → commit → push origin main (GitHub)
                         ↓ auto sync (ทุก 8h)
                    Gitea mirror
```
