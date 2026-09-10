# AGENTS -- `packages/CrtAiTwinApp/Files/ui/`

## Metadata
- title: "package ui assets"
- short_summary: "Package-hosted UI and shell assets for the Creatio host lane."
- Entrypoints:
  - packages/CrtAiTwinApp/Files/ui/AGENTS.md
  - packages/CrtAiTwinApp/Files/ui/assistant-runtime-shell
  - packages/CrtAiTwinApp/Files/ui/assistant-ui

## Purpose
`Files/ui/` holds packaged shell and runtime asset bundles consumed by the host.

## Local Guidance
- Keep shell wrapper assets separate from packaged runtime artifacts.
- Do not reintroduce host-agnostic source here.

## Scope
- Applies to this directory and descendants unless overridden by a nested `AGENTS.md`.
