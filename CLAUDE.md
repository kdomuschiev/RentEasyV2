# RentEasyV2 — Claude Instructions

## Branching Strategy

- `main` is the stable branch — never commit directly to it
- Before starting any piece of work, create a branch from `main`:
  - `feat-[brief-description]` for new functionality (e.g. `feat-property-setup`)
  - `fix-[brief-description]` for bug fixes (e.g. `fix-jwt-expiry`)
  - `bmad-[brief-description]` when changes are only in `_bmad/` or `_bmad-output/` (e.g. `bmad-update-prd`)

## Merging

- When the user says a PR has been merged, run `git checkout main && git pull` to sync the local main branch
- If there are uncommitted changes when switching to main, stash them first with `git stash` before checking out

## Pull Requests

- After every commit, create a PR from the current branch into `main` using `gh pr create` — only if a PR for that branch does not already exist
- PR titles must follow the format: `[feat] Short description`, `[fix] Short description`, or `[bmad] Short description` (matching the branch type)
