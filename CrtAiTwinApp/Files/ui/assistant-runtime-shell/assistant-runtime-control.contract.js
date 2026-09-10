export const assistantRuntimeControlContract = Object.freeze({
  selector: "crt-ai-twin-shell",
  type: "crt.CrtAiTwinShell",
  moduleCode: "crt_ai_assistant_chat_app",
  replacementSchema: "CopilotPanel",
  targetSubtree: "MainContainer -> Chat",
  runtimeEntrypoint: "../assistant-ui/assistant-runtime.js",
  mountHostClassName: "assistant-runtime-host",
});
