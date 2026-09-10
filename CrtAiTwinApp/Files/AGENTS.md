# AGENTS -- `packages/CrtAiTwinApp/Files/`

## Metadata
- title: "package files root"
- short_summary: "Build, materialization, and packaged runtime assets for the package."
- Entrypoints:
  - packages/CrtAiTwinApp/Files/AGENTS.md
  - packages/CrtAiTwinApp/Files/app-descriptor.json
  - packages/CrtAiTwinApp/Files/ui

## Purpose
`Files/` contains package build inputs and packaged runtime assets.

## Local Guidance
- Keep build roots and packaged UI assets here.
- Maintain `app-descriptor.json` here as the source of truth for the
  Creatio application listing metadata used by UI management.
- Do not treat this directory as runtime-source authoring space.

## Scope
- Applies to this directory and descendants unless overridden by a nested `AGENTS.md`.
