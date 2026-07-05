# SPDX-License-Identifier: GPL-3.0-or-later
# next-open-selfcheck — Kiosk Dockerfile
#
# Build context is the monorepo root (see docker-compose.yml: context: ../..)
# — this is an npm workspaces project, so `npm ci` needs the root lockfile
# plus every workspace's package.json to resolve reproducibly.

FROM node:20-alpine AS base
WORKDIR /app

# ── Dependencies ──────────────────────────────────────────────
FROM base AS deps
RUN apk add --no-cache libc6-compat
COPY package.json package-lock.json ./
COPY apps/kiosk/package.json ./apps/kiosk/package.json
COPY packages/rfid-adapter/package.json ./packages/rfid-adapter/package.json
COPY packages/sip2-client/package.json ./packages/sip2-client/package.json
RUN npm ci --workspace=apps/kiosk --omit=dev

# ── Builder ───────────────────────────────────────────────────
FROM base AS builder
RUN apk add --no-cache libc6-compat
COPY package.json package-lock.json ./
COPY apps/kiosk/package.json ./apps/kiosk/package.json
COPY packages/rfid-adapter/package.json ./packages/rfid-adapter/package.json
COPY packages/sip2-client/package.json ./packages/sip2-client/package.json
RUN npm ci --workspace=apps/kiosk
COPY apps/kiosk ./apps/kiosk
COPY packages ./packages

# Belt-and-suspenders: .dockerignore should already keep local .env files out
# of the build context, but explicitly delete any that slipped through
# before building — secrets must never end up baked into an image layer.
RUN find /app/apps/kiosk -maxdepth 1 -name ".env*" ! -name ".env.example" -delete

ENV NEXT_TELEMETRY_DISABLED=1

RUN npm run build --workspace=apps/kiosk

# ── Runner ────────────────────────────────────────────────────
FROM base AS runner
ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1

RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

# Standalone output mirrors the monorepo path (apps/kiosk/server.js, etc.)
# because the build context/tracing root is the repo root, not this app.
COPY --from=builder /app/apps/kiosk/public ./apps/kiosk/public
COPY --from=builder --chown=nextjs:nodejs /app/apps/kiosk/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/apps/kiosk/.next/static ./apps/kiosk/.next/static

USER nextjs

EXPOSE 3000
ENV PORT=3000
ENV HOSTNAME="0.0.0.0"

CMD ["node", "apps/kiosk/server.js"]
