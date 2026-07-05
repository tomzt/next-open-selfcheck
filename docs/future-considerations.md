# Future Considerations — Alternative Kiosk Hardware

> Not a confirmed design decision — a feasibility record from an exploratory
> discussion, kept so the reasoning doesn't have to be redone later. Update
> or promote sections into `requirements.md`/`architecture.md` once (if)
> any of this is actually decided.
>
> **This file is the source of truth.** [`future-considerations-th.md`](./future-considerations-th.md)
> is a translation — if they ever disagree, this English version wins.
> Edit this one first, then sync the Thai version.

---

## 1. Problem this addresses

Current kiosk hardware model (`docs/architecture.md`) is one Mini PC +
touchscreen monitor per site, running the full Docker stack locally. This
is more hardware than strictly necessary for many deployments, and the
idea below explores whether that cost can come down.

## 2. Option: Android tablet as thin client

Replaces the Mini PC + separate touchscreen monitor with a single Android
tablet running a browser in kiosk mode.

**Feasible today, zero code changes, for Phase 1–3b (barcode-only):**
- UI is already tablet-ready — `apps/kiosk/src/app/globals.css` uses
  `100vw/100vh` + flex layouts, no fixed desktop pixel widths; touch
  targets already ≥48px.
- `NEXT_PUBLIC_INPUT_MODE=touch` (`src/lib/theme.ts`, `globals.css`)
  already hides the cursor and disables `:hover` — built for this
  reason without knowing it'd be used here.
- Barcode scanning is pure HID-keyboard-emulation (`onKeyDown`/Enter in
  `AuthScreen.tsx`, `BorrowScreen.tsx`, `ReturnScreen.tsx`) — any
  USB-OTG or Bluetooth HID scanner is transparent to this code.
- No PWA/manifest needed — a kiosk-lockdown browser app (e.g. "Fully
  Kiosk Browser") pointed at the existing URL works today.

**Cost:** ~$100–200 (budget-mid kiosk-grade Android tablet) vs.
~$450–1,000 (NUC + kiosk-grade touchscreen monitor) — roughly 2–4×
cheaper at comparable tiers. Commercial-grade (24/7 duty-cycle rated)
Android tablets run $300–500+, narrowing the gap.

**Hard blocker for Phase 4 RFID:** `docs/architecture.md`'s shared-package
section has `rfid-adapter/web-serial` used by **both Kiosk and
Workstation** — RFID is planned on the kiosk itself, not just the staff
tool. **Web Serial API does not exist in any mobile browser** (Chrome for
Android included) — a hard platform limitation. Not a blocker today
(RFID isn't built yet), but caps a tablet-based kiosk fleet at
barcode-only unless a native app is built later (see §5).

**Also feasible in the web app (no native rewrite):** front-camera
QR/barcode reading via `getUserMedia` + a JS decode library (e.g. the
browser's `BarcodeDetector` or `zxing-js`). Recommended for **patron
login QR only** — front cameras are optimized for video calls (wide FOV,
fixed focus), not close-up decoding, so repeated book-barcode scanning in
the batch-scan flow would likely be slower/less reliable than a real
scanner. Keep an external HID scanner (~$20–80) for the borrow/return flow.

## 3. Option: iPad instead of Android

**Cost:** Higher than Android — cheapest current iPad ~$329–449 vs.
Android tablets at $100–200 — cuts into the original cost-saving
motivation. Trade-off: Apple's OS support lifecycle is typically
5–7 years vs. 2–3 years for a typical Android tablet, which may offset
the higher upfront cost over a multi-year deployment.

**Browser story is identical to Android for this app's needs:** Apple
requires all iOS browsers to use WebKit (even "Chrome for iOS" is WebKit
under the hood) — Safari/WebKit does not support Web Serial either, so
RFID-via-browser is equally blocked on both platforms; no difference.
`getUserMedia` (camera scanning) and external HID barcode scanners
(USB-C/Lightning adapter or Bluetooth) both work fine on iPad — this is
an established retail/POS pattern already.

**Where iPad differs — native RFID path is gated by MFi:** a native app
talking to a USB/Lightning accessory needs Apple's MFi (Made for
iPhone/iPad) certification via the External Accessory framework — a
closed process the manufacturer must opt into, unlike Android's more open
USB Host API. Checked directly: ACS's own product page for the
**ACR1552U** (the RFID reader referenced in this project's docs) states
it supports **iOS/iPadOS 16+** — a positive signal, but no public source
confirms a third-party developer SDK exists (vs. just "the device works
with iOS" at a consumer level). **Needs direct confirmation from ACS**
before relying on iPad for a native RFID path.

**Kiosk lockdown:** iOS's built-in "Guided Access" is comparable to
Android's kiosk/screen-pinning options — no meaningful difference.

## 4. Option: centralize the backend server (independent of tablet vs. Mini PC)

Originally considered, then set aside — worth revisiting with the
distinction below made explicit.

**The real constraint from RFID is narrower than "no shared server":**
Web Serial requires the *browser* to run on the same machine as the USB
reader — it does not require the full Docker/Postgres/SIP2 stack to run
locally. `docs/architecture.md:21` / `docs/requirements.md:322`'s "no
central server, no WAN dependency" is a **separate, deliberately chosen
resilience decision** (a site keeps working if the central server or WAN
link goes down) — it is not written anywhere as a consequence of RFID.

**A third topology is possible:** one shared Docker/Postgres/SIP2 backend
+ a cheap local box per kiosk that runs only the browser (for local RFID
USB access) pointed at the shared backend. This keeps RFID working and
reduces backend hardware duplication, but gives up the "each site survives
independently" guarantee — a real trade-off to decide deliberately, not a
free win.

**Tablets don't change this calculus for RFID specifically** — whether
centralized or not, Web Serial is unavailable on any mobile browser, so
tablet + RFID always needs the native-app path from §2/§3 regardless of
backend topology.

## 5. Option: native app (Flutter) instead of the web app

Only relevant if RFID-on-tablet is actually wanted.

- Unlocks direct hardware access: Android's USB Host API (no Web Serial
  limitation); iPad gated by MFi (see §3).
- Backend/API layer (`apps/kiosk/src/app/api/*`, SIP2 client, auth, email
  receipt, rate limiting) does **not** need to change — a Flutter app
  would just be a new client calling the same existing HTTP endpoints.
- Everything else does need rebuilding: every screen/component, the
  5-theme system, i18n, accessibility bar, session-timeout UI — a project
  of comparable size to the original web app build, not a small patch.
  Auth would also need to move from cookie-based sessions to token-based
  (JWT in secure storage + Authorization header).

## 6. Recommended sequencing

| Track | Timing | Why |
|---|---|---|
| Pilot: 1 Android tablet as thin client (barcode-only, existing backend) | Parallel, any time | Pure hardware/ops work, zero dev time, de-risks Wi-Fi/scanner/mount before buying a fleet |
| Camera-based QR scanning for login | Parallel, whenever convenient | Small, isolated web feature, doesn't touch RFID-related code |
| iPad vs. Android decision per site | Parallel with the pilot | Budget vs. longevity trade-off; confirm ACR1552U's iOS SDK story with ACS if RFID matters to that site |
| Native (Flutter) rebuild + RFID-on-tablet | **After** Phase 4 RFID is built and proven on the existing Mini PC path | Avoids debugging new hardware (RFID protocol/AFI writes) and a new framework at the same time |

## 7. Open questions before committing budget

- [ ] Confirm with ACS whether ACR1552U has a public/licensable iOS SDK for third-party apps, or only MFi-gated first-party support.
- [ ] Site survey: is Wi-Fi reliable enough at each candidate kiosk location, or does it need wired backhaul to an access point near the stand?
- [ ] Decide per-site: independent Docker stack (current model, resilient) vs. shared central backend (cheaper, single point of failure) — see §4.
- [ ] Confirm chosen tablet model actually supports USB-OTG (not universal on cheap Android tablets) if a wired barcode scanner is planned instead of Bluetooth.
- [ ] Budget a lockable kiosk stand/enclosure regardless of platform choice.

---

[🇹🇭 ภาษาไทย](./future-considerations-th.md)
