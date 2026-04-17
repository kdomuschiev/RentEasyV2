# renteasy-web

Next.js 16 frontend for the RentEasy rental management platform.

## Local development

```bash
npm install
npm run dev     # dev server at http://localhost:3000
npm run build   # production build (outputs to .next/standalone)
npm run lint    # ESLint
npm run test    # Vitest
```

Requires the API running locally. Set `RENTEASY_API_URL` in a `.env.local` file:

```
RENTEASY_API_URL=http://localhost:5000
```

## Deployment

Deployed to Azure App Service (Node.js 22 LTS) via `web-deploy.yml`. The build uses `output: 'standalone'` — the standalone output is deployed and started with `node server.js`.

See the root `README.md` for the full Azure setup guide.
