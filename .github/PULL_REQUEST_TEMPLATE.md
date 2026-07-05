## What this changes

<!-- Describe the change and why. Link an issue if there is one. -->

## Checklist

Per `AGENTS.md` §5 — before this is considered done:

- [ ] Any new config is added to `apps/kiosk/.env.example` with a comment
- [ ] No brand/institution-specific value is hardcoded in source
- [ ] Any new user-facing string is in both `messages/th.json` **and**
      `messages/en.json`
- [ ] New source files have the SPDX header
      (`// SPDX-License-Identifier: GPL-3.0-or-later`)
- [ ] Patron name/card number is never written to disk (see
      `docs/requirements.md` §13)
- [ ] `README.md` / `README.th.md` updated if this changes something a
      deployer or patron would notice (new ENV var, changed flow, phase
      status) — English first, then Thai

## How was this tested?

<!-- e.g. "ran against the mock SIP2 server, verified X/Y/Z screens" -->
