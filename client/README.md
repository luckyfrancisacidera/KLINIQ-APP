# KLINIQ frontend

The React, TypeScript, Vite, Tailwind, TanStack Query, Google Maps, symptom-assisted physician search, clinic queue workflow, and PWA client for KLINIQ.

Use the repository-level [`README.md`](../README.md) for architecture, environment variables, setup, build, service-worker behavior, security notes, deployment instructions, and known limitations.

```bash
cp .env.example .env.local
corepack enable
pnpm install --frozen-lockfile
pnpm dev
```

Production checks:

```bash
pnpm lint
pnpm typecheck
pnpm build
pnpm preview
```
