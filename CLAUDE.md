# RentEasyV2 — Claude Instructions

## Branching Strategy

- `main` is the stable branch — never commit directly to it
- Before starting any piece of work, create a branch from `main`:
  - `feat-[brief-description]` for new functionality (e.g. `feat-property-setup`)
  - `fix-[brief-description]` for bug fixes (e.g. `fix-jwt-expiry`)

## Pull Requests

- After every commit, create a PR from the current branch into `main` using `gh pr create` — only if a PR for that branch does not already exist
