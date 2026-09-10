import { assistantRuntimeControlContract } from "./assistant-runtime-control.contract.js";

const shellState = new WeakMap();

async function loadRuntimeModule() {
  return import(assistantRuntimeControlContract.runtimeEntrypoint);
}

function resolveRuntimeApi(runtimeModule) {
  if (
    runtimeModule?.mountAssistantRuntime &&
    runtimeModule?.unmountAssistantRuntime
  ) {
    return runtimeModule;
  }

  if (
    runtimeModule?.default?.mountAssistantRuntime &&
    runtimeModule?.default?.unmountAssistantRuntime
  ) {
    return runtimeModule.default;
  }

  throw new Error(
    "Assistant runtime bundle does not expose mountAssistantRuntime/unmountAssistantRuntime."
  );
}

function createMountHost(element) {
  const mountHost = document.createElement("div");
  mountHost.className = assistantRuntimeControlContract.mountHostClassName;
  mountHost.dataset.assistantRuntimeHost = "true";
  applyStretchContract(element, mountHost);
  element.replaceChildren(mountHost);
  return mountHost;
}

function applyStretchContract(element, mountHost) {
  element.style.display = "block";
  element.style.width = "100%";
  element.style.height = "100%";
  element.style.minHeight = "100%";
  element.style.boxSizing = "border-box";

  mountHost.style.display = "block";
  mountHost.style.width = "100%";
  mountHost.style.height = "100%";
  mountHost.style.minHeight = "100%";
  mountHost.style.boxSizing = "border-box";
}

export class AssistantRuntimeShellElement extends HTMLElement {
  async connectedCallback() {
    console.info("[assistant-runtime-shell] connected", {
      selector: assistantRuntimeControlContract.selector,
      runtimeEntrypoint: assistantRuntimeControlContract.runtimeEntrypoint
    });
    const currentState = shellState.get(this);
    if (currentState?.mounted) {
      console.info("[assistant-runtime-shell] skip-remount");
      return;
    }

    const mountHost = currentState?.mountHost ?? createMountHost(this);
    applyStretchContract(this, mountHost);
    console.info("[assistant-runtime-shell] mount-host-ready", {
      childCount: mountHost.children.length
    });
    const runtimeModule = resolveRuntimeApi(await loadRuntimeModule());
    console.info("[assistant-runtime-shell] runtime-module-loaded", {
      exportedKeys: Object.keys(runtimeModule ?? {})
    });

    runtimeModule.mountAssistantRuntime(mountHost);
    console.info("[assistant-runtime-shell] runtime-mounted", {
      childCount: mountHost.children.length,
      htmlSnippet: mountHost.innerHTML.slice(0, 300)
    });
    shellState.set(this, {
      mountHost,
      mounted: true,
      runtimeModule,
    });
  }

  disconnectedCallback() {
    console.info("[assistant-runtime-shell] disconnected");
    const currentState = shellState.get(this);
    if (!currentState?.mounted) {
      return;
    }

    currentState.runtimeModule.unmountAssistantRuntime(currentState.mountHost);
    console.info("[assistant-runtime-shell] runtime-unmounted");
    shellState.set(this, {
      ...currentState,
      mounted: false,
    });
  }
}

export function defineAssistantRuntimeShellElement() {
  if (!customElements.get(assistantRuntimeControlContract.selector)) {
    customElements.define(
      assistantRuntimeControlContract.selector,
      AssistantRuntimeShellElement
    );
  }

  return assistantRuntimeControlContract.selector;
}
