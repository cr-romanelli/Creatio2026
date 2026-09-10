# Assistant Runtime Shell

This directory holds the package-owned runtime shell helpers for the first-wave
assistant custom control.

Current boundary:

- keep selector, type, and runtime-entrypoint ownership inside the package;
- keep the package-owned helper files that mount the materialized React runtime;
- leave the actual runtime registration/load-before-resolve seam to the
  package schema loader.

Current registration path:

- `Schemas/CrtAiTwinShellRuntimeLoader/CrtAiTwinShellRuntimeLoader.js`
  defines the custom element and registers `crt.CrtAiTwinShell`;
- `Schemas/CopilotPanel/CopilotPanel.js` preloads that loader through
  `SCHEMA_DEPS` before it inserts the custom control into the sidebar subtree.
