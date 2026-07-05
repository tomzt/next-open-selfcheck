# Security Policy

## Supported Versions

This project is pre-1.0 and under active development. Security fixes are
only made against the `main` branch — there are no maintained release
branches yet.

## Reporting a Vulnerability

**Please do not open a public issue for security vulnerabilities.**

Use GitHub's private vulnerability reporting instead:

1. Go to the [Security tab](https://github.com/tomzt/next-open-selfcheck/security) of this repository
2. Click **"Report a vulnerability"**
3. Describe the issue, affected component (e.g. SIP2 client, auth, email
   receipt), and steps to reproduce if possible

If private reporting isn't available for any reason, open a regular issue
with as few technical details as possible and ask a maintainer to follow up
privately.

## Scope Notes for This Project

Given what this app actually handles, these are the areas most worth a
careful look before reporting:

- **Patron data handling** — patron name/card number must never be written
  to disk (see `docs/requirements.md` §13). If you find a code path that
  persists this, that's a real finding.
- **SIP2 client** (`packages/sip2-client`, `apps/kiosk/src/lib/sip2-*.ts`) —
  runs against a raw TCP socket; injection or parsing issues here are
  high-impact since it talks directly to a library's ILS.
- **Auth** (`apps/kiosk/src/lib/auth.ts`) — barcode/QR and OIDC flows.
- **Kiosk browser lock-down** — this app is meant to run full-screen on a
  public terminal; anything that lets a patron escape the kiosk UI or reach
  another origin is worth reporting.

Thank you for helping keep libraries running this project safe.
