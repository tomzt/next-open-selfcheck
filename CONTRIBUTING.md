# Contributing to open-selfcheck

Thank you for your interest in contributing! This project is built for **any library worldwide** — contributions that benefit the broader library community are especially welcome.

---

## Open Source First

Before writing any code, ask yourself:

> *"Would a librarian at another university, in another country, be able to use this without knowing anything about RMUTI?"*

**Hard rules — never violate:**

- No org-specific values in core code — all config belongs in `.env`
- No ALIST-only assumptions — SIP2 layer must work with Koha, Evergreen, WALAI, etc.
- No Keycloak-only auth — it's the reference deployer's default, not a requirement
- No Thai-only UI — Thai and English are the baseline; both must be complete
- No campus codes or org identifiers hardcoded in source

---

## Getting Started

### Prerequisites

- Node.js 20+
- Docker + Docker Compose
- Git

### Fork and clone

```bash
git clone https://github.com/YOUR_ORG/open-selfcheck.git
cd open-selfcheck
npm install
```

### Start development

```bash
# Start mock SIP2 server (no real ILS needed)
cd packages/sip2-client && npm run mock-server

# Start kiosk in dev mode
cd apps/kiosk && cp .env.example .env && npm run dev

# Start workstation in dev mode (optional)
cd apps/workstation && cp .env.example .env && npm run dev
```

---

## Branch Strategy

| Branch | Purpose |
|---|---|
| `main` | Stable, released code |
| `develop` | Integration branch |
| `feat/your-feature` | New features |
| `fix/your-fix` | Bug fixes |
| `docs/your-docs` | Documentation only |

```bash
git checkout develop
git checkout -b feat/your-feature
# make changes
git push origin feat/your-feature
# open PR → develop
```

---

## Pull Request Guidelines

- PRs target `develop`, not `main`
- One feature or fix per PR
- Include tests where applicable
- Update documentation if adding new config options
- Add changelog entry in `CHANGELOG.md`

---

## Adding a New Auth Provider

The auth layer uses NextAuth.js. To add a new provider:

1. Create `apps/kiosk/src/lib/auth/providers/your-provider.ts`
2. Implement the NextAuth.js provider interface
3. Register it in `apps/kiosk/src/lib/auth/index.ts`
4. Add config variables to `.env.example`
5. Document in `docs/auth-providers.md`

---

## Adding a New RFID Driver

The `rfid-adapter` package uses a common interface. To add a new driver:

1. Create `packages/rfid-adapter/src/your-driver/index.ts`
2. Implement the `RFIDAdapter` interface (see `packages/rfid-adapter/src/interface.ts`)
3. Register it in `packages/rfid-adapter/src/index.ts`
4. Add `RFID_DRIVER=your-driver` to `.env.example`
5. Document hardware requirements in `docs/rfid-adapter.md`

---

## Code Style

- TypeScript throughout
- ESLint + Prettier (run `npm run lint`)
- Meaningful commit messages: `feat:`, `fix:`, `docs:`, `chore:`

---

## License

By contributing, you agree that your contributions will be licensed under GPL-3.0, consistent with this project's license.

This project is a derivative work of [GallusMax/open-source-self-check](https://github.com/GallusMax/open-source-self-check) and must remain GPL-3.0.
