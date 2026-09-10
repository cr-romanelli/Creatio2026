define("ConfActivityLogDiffTemplate", ["ConfActivityLogDiffTemplateResources"], function(resources) {
	const ModelMarker = "__DIFF_MODEL_JSON__";
	const RuntimeMarker = "__RUNTIME_SCRIPT__";
	const Texts = {
		pageTitle: resources.localizableStrings.PageTitle,
		compareContent: resources.localizableStrings.CompareContent,
		noFilesTitle: resources.localizableStrings.NoFilesTitle,
		noFilesMeta: resources.localizableStrings.NoFilesMeta,
		diffViewMode: resources.localizableStrings.DiffViewMode,
		sideBySide: resources.localizableStrings.SideBySide,
		unified: resources.localizableStrings.Unified,
		compactUnchangedBlocks: resources.localizableStrings.CompactUnchangedBlocks
	};
	const ShellFontFamily = `"Open Sans", "Bpmonline Open Sans", Arial, sans-serif`;
	const FontFaceStyles = `
		@font-face {
			font-family: "Open Sans";
			font-style: normal;
			font-weight: 400;
			src: local("Open Sans"),
				url("../Resources/ui/Fonts/Open%20Sans/OpenSans-Regular.woff2") format("woff2"),
				url("../Resources/ui/Fonts/Open%20Sans/OpenSans-Regular.woff") format("woff");
		}
	`;
	const DiffStyles = `
${FontFaceStyles}

		:root {
			color-scheme: light;
			--conf-activity-log-diff-font-family: ${ShellFontFamily};
			font-family: var(--conf-activity-log-diff-font-family);
		}

		html,
		body {
			height: 100%;
			overflow: hidden;
		}

		body {
			margin: 0;
			background: #edf3f8;
			color: #15324a;
			font-family: var(--conf-activity-log-diff-font-family);
		}

		.gidiff {
			display: grid;
			grid-template-columns: 280px minmax(0, 1fr);
			height: 100%;
			min-height: 0;
			overflow: hidden;
		}

		.sidebar {
			display: flex;
			flex-direction: column;
			background: #fff;
			border-right: 1px solid #d7e3ef;
			min-height: 0;
			overflow: hidden;
		}

		.sidebar-head {
			padding: 18px 16px 0;
		}

		.sidebar-head h1 {
			margin: 0;
			font-size: 14px;
			letter-spacing: .04em;
			text-transform: uppercase;
			color: #61788f;
		}

		.sidebar-title-row {
			display: flex;
			align-items: center;
		}

		.compare-menu-button {
			display: none;
			align-items: center;
			justify-content: center;
			box-sizing: border-box;
			width: 32px;
			height: 32px;
			border: 0;
			border-radius: 8px;
			background: transparent;
			cursor: pointer;
		}

		.compare-menu-button:hover,
		.compare-menu-button.active {
			background: #f2f5ff;
		}

		.compare-menu-button:focus {
			outline: none;
		}

		.compare-menu-icon,
		.compare-menu-icon::before,
		.compare-menu-icon::after {
			display: block;
			width: 4px;
			height: 4px;
			border-radius: 50%;
			background: #15324a;
		}

		.compare-menu-icon {
			position: relative;
		}

		.compare-menu-icon::before,
		.compare-menu-icon::after {
			content: "";
			position: absolute;
			left: 0;
		}

		.compare-menu-icon::before {
			top: -7px;
		}

		.compare-menu-icon::after {
			top: 7px;
		}

		.compare-menu {
			display: flex;
			flex: 1 1 auto;
			flex-direction: column;
			min-height: 0;
			overflow: hidden;
		}

		.compare-modes {
			display: flex;
			flex-direction: column;
			gap: 4px;
			padding: 12px;
			border-bottom: 1px solid #d7e3ef;
		}

		.compare-mode {
			display: flex;
			align-items: center;
			justify-content: space-between;
			gap: 8px;
			min-height: 32px;
			padding: 6px 10px;
			border: 1px solid transparent;
			border-radius: 8px;
			background: transparent;
			color: #15324a;
			cursor: pointer;
			font: 13px var(--conf-activity-log-diff-font-family);
			text-align: left;
		}

		.compare-mode.active {
			background: #f2f5ff;
			border-color: transparent;
			font-weight: 600;
		}

		.compare-mode:not(:disabled):hover {
			background: #f7f9ff;
		}

		.compare-mode:disabled {
			color: #9daabb;
			cursor: default;
		}

		.compare-mode-count {
			color: #61788f;
			font-size: 12px;
			font-weight: 400;
		}

		.tabs {
			display: flex;
			flex-direction: column;
			flex: 1 1 auto;
			gap: 8px;
			min-height: 0;
			padding: 12px;
			overflow: auto;
		}

		.toggle {
			display: inline-flex;
			align-items: center;
			gap: 8px;
			min-height: 32px;
			font-size: 13px;
			line-height: 1.2;
			color: #15324a;
			cursor: pointer;
			user-select: none;
		}

		.toggle.is-hidden {
			display: none;
		}

		.toggle input {
			appearance: none;
			-webkit-appearance: none;
			box-sizing: border-box;
			width: 16px;
			height: 16px;
			margin: 0;
			flex: 0 0 auto;
			align-self: center;
			border: 1px solid #004fd6;
			border-radius: 4px;
			background: #fff;
			cursor: pointer;
		}

		.toggle input:checked {
			background-color: #004fd6;
			background-image: url("data:image/svg+xml,%3Csvg width='12' height='10' viewBox='0 0 12 10' fill='none' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M1 5L4.2 8L11 1' stroke='white' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'/%3E%3C/svg%3E");
			background-position: center;
			background-repeat: no-repeat;
			background-size: 12px 10px;
			border-color: #004fd6;
		}

		.toggle input:focus {
			outline: none;
		}

		.toggle span {
			display: inline-flex;
			align-items: center;
			color: #15324a;
			font-family: var(--conf-activity-log-diff-font-family);
			font-size: 13px;
			font-weight: 600;
			line-height: 18px;
		}

		.tab {
			padding: 10px 12px;
			border: 1px solid transparent;
			border-radius: 8px;
			background: transparent;
			cursor: pointer;
			text-align: left;
			font: inherit;
			color: inherit;
		}

		.tab.active {
			background: #f2f5ff;
			border-color: transparent;
		}

		.tab:hover {
			background: #f7f9ff;
		}

		.tab-title {
			display: block;
			font-size: 13px;
			font-weight: 600;
			white-space: nowrap;
			overflow: hidden;
			text-overflow: ellipsis;
		}

		.tab-meta {
			display: block;
			margin-top: 5px;
			font-size: 12px;
			color: #61788f;
		}

		.empty {
			padding: 24px 18px;
			color: #61788f;
			font-size: 13px;
		}

		.main {
			position: relative;
			display: flex;
			flex-direction: column;
			min-width: 0;
			min-height: 0;
			overflow: hidden;
		}

		.main-head {
			display: flex;
			align-items: center;
			justify-content: space-between;
			gap: 16px;
			padding: 16px 42px 16px 18px;
			border-bottom: 1px solid #d7e3ef;
			background: rgba(255, 255, 255, .88);
		}

		.main-head-main {
			min-width: 0;
		}

		.main-title-row {
			display: flex;
			align-items: flex-start;
			gap: 8px;
			min-width: 0;
		}

		.main-title-text {
			min-width: 0;
		}

		.main-head h2 {
			margin: 0;
			font-size: 16px;
			white-space: nowrap;
			overflow: hidden;
			text-overflow: ellipsis;
		}

		.main-head p {
			margin: 5px 0 0;
			color: #61788f;
			font-size: 12px;
		}

		.main-actions {
			display: flex;
			align-items: center;
			justify-content: flex-end;
			gap: 12px;
			flex-wrap: wrap;
			min-height: 32px;
		}

		.change-navigation {
			display: inline-flex;
			align-items: center;
			gap: 2px;
			min-height: 32px;
		}

		.change-nav-button {
			display: inline-flex;
			align-items: center;
			justify-content: center;
			box-sizing: border-box;
			width: 32px;
			height: 32px;
			padding: 0;
			border: 0;
			border-radius: 8px;
			background: transparent;
			color: #15324a;
			cursor: pointer;
		}

		.change-nav-button:not(:disabled):hover {
			background: #f7f9ff;
		}

		.change-nav-button:disabled {
			color: #9daabb;
			cursor: default;
		}

		.change-nav-button:focus {
			outline: none;
		}

		.change-nav-button svg {
			width: 16px;
			height: 16px;
			fill: none;
			stroke: currentColor;
			stroke-linecap: round;
			stroke-linejoin: round;
			stroke-width: 2;
		}

		.view-mode {
			display: inline-flex;
			align-items: center;
			gap: 4px;
			min-height: 32px;
		}

		.view-mode.is-hidden {
			display: none;
		}

		.mode-button {
			display: inline-flex;
			align-items: center;
			justify-content: center;
			box-sizing: border-box;
			min-height: 32px;
			padding: 6px 10px;
			border: 0;
			border-radius: 8px;
			background: transparent;
			color: #15324a;
			cursor: pointer;
			font: 13px var(--conf-activity-log-diff-font-family);
			line-height: 1.2;
		}

		.mode-button.active {
			background: #f2f5ff;
			color: #15324a;
			font-weight: 600;
		}

		.mode-button:hover {
			background: #f7f9ff;
		}

		.mode-button:focus {
			outline: none;
		}

		.fullscreen-button {
			position: absolute;
			top: 4px;
			right: 4px;
			z-index: 2;
			display: inline-flex;
			align-items: center;
			justify-content: center;
			box-sizing: border-box;
			width: 24px;
			height: 24px;
			padding: 0;
			border: 0;
			border-radius: 4px;
			background: transparent;
			color: #15324a;
			cursor: pointer;
			flex: 0 0 auto;
		}

		.fullscreen-button:hover,
		.fullscreen-button.active {
			background: transparent;
		}

		.fullscreen-button:focus {
			outline: none;
		}

		.fullscreen-button svg {
			width: 12px;
			height: 12px;
			fill: currentColor;
		}

		.fullscreen-button .is-hidden {
			display: none;
		}

		.gidiff.is-fullscreen {
			grid-template-columns: minmax(0, 1fr);
		}

		.gidiff.is-fullscreen .sidebar {
			position: absolute;
			top: 50px;
			left: 14px;
			z-index: 6;
			display: block;
			width: 280px;
			min-height: 0;
			border-right: 0;
			background: transparent;
			overflow: visible;
			pointer-events: none;
		}

		.gidiff.is-fullscreen .sidebar-head {
			display: none;
		}

		.gidiff.is-fullscreen .compare-menu-button {
			display: inline-flex;
		}

		.gidiff.is-fullscreen .compare-menu {
			display: none;
			max-height: 420px;
			padding: 8px;
			border: 1px solid #d7e3ef;
			border-radius: 8px;
			background: #fff;
			box-shadow: 0 8px 20px rgba(21, 50, 74, .14);
			overflow: auto;
			pointer-events: auto;
		}

		.gidiff.is-fullscreen .compare-menu.is-open {
			display: flex;
		}

		.gidiff.is-fullscreen .compare-modes {
			padding: 0;
			border-bottom: 0;
		}

		.gidiff.is-fullscreen .tabs {
			flex: 0 0 auto;
			padding: 8px 0 0;
			overflow: visible;
		}

		.viewer {
			display: grid;
			grid-template-columns: 1fr 1fr;
			min-height: 0;
			flex: 1;
			overflow: hidden;
		}

		.gidiff.no-previous-version .viewer {
			grid-template-columns: 1fr;
		}

		.gidiff.no-previous-version .main-actions {
			display: none;
		}

		.unified-viewer {
			display: none;
			min-height: 0;
			flex: 1;
			overflow: hidden;
		}

		.gidiff.is-unified .viewer {
			display: none;
		}

		.gidiff.is-unified .unified-viewer {
			display: flex;
		}

		.pane {
			display: flex;
			flex-direction: column;
			min-height: 0;
			min-width: 0;
		}

		.pane + .pane {
			border-left: 1px solid #d7e3ef;
		}

		.pane.is-hidden {
			display: none;
		}

		.gidiff.no-previous-version .pane + .pane {
			border-left: 0;
		}

		.unified-viewer .pane {
			flex: 1 1 auto;
			border-left: 0;
		}

		.unified-version-head {
			display: flex;
			align-items: center;
			gap: 24px;
			min-width: 0;
			min-height: 26px;
		}

		.pane-head {
			box-sizing: border-box;
			display: flex;
			align-items: center;
			height: 46px;
			padding: 4px 14px;
			border-bottom: 1px solid #d7e3ef;
			background: #fff;
			font-size: 12px;
			font-weight: 600;
			line-height: 18px;
			color: #61788f;
		}

		.version-head {
			display: flex;
			align-items: center;
			gap: 6px;
			min-height: 26px;
			overflow: hidden;
			font-family: var(--conf-activity-log-diff-font-family);
			font-size: 13px;
			font-weight: 600;
			line-height: 18px;
			color: #15324a;
		}

		.version-head.is-stacked {
			flex-direction: column;
			align-items: flex-start;
			gap: 0;
		}

		.version-title {
			display: inline-flex;
			align-items: center;
			color: #15324a;
			font-family: var(--conf-activity-log-diff-font-family);
			font-size: 13px;
			font-weight: 600;
			line-height: 18px;
		}

		.version-meta {
			display: inline-flex;
			align-items: center;
			min-width: 0;
			overflow: hidden;
			text-overflow: ellipsis;
			color: #15324a;
			font-family: var(--conf-activity-log-diff-font-family);
			font-size: 13px;
			font-weight: 600;
			line-height: 18px;
		}

		.content {
			margin: 0;
			padding: 8px 0;
			flex: 1 1 auto;
			height: auto;
			min-height: 0;
			overflow-x: scroll;
			overflow-y: auto;
			scrollbar-gutter: stable;
			background: #fbfdff;
			font: 12px/1.45 Consolas, "Courier New", monospace;
		}

		.lines {
			min-width: 100%;
		}

		.line {
			box-sizing: border-box;
			display: grid;
			align-items: center;
			grid-template-columns: 52px 18px 1fr;
			gap: 8px;
			min-height: 22px;
			width: 100%;
			min-width: 100%;
			padding: 1px 12px;
		}

		.num,
		.mark {
			color: #8a98a8;
			user-select: none;
		}

		.num {
			text-align: right;
		}

		.mark {
			text-align: center;
			font-weight: 700;
		}

		.text {
			display: block;
			min-width: 0;
			white-space: pre;
		}

		.added {
			background: #e8f7ec;
		}

		.removed {
			background: #fdeceb;
		}

		.changed {
			background: #fff4d8;
		}

		.emptyline {
			background: repeating-linear-gradient(
				-45deg,
				rgba(128, 146, 168, .12) 0,
				rgba(128, 146, 168, .12) 4px,
				rgba(128, 146, 168, .04) 4px,
				rgba(128, 146, 168, .04) 8px
			);
			color: #a3aebb;
		}

		.inline-added {
			background: #c7ecd2;
			border-radius: 2px;
			display: inline-block;
			padding: 1px 0;
		}

		.inline-removed {
			background: #f6cfc9;
			border-radius: 2px;
			display: inline-block;
			padding: 1px 0;
		}

		@media (max-width: 900px) {
			.gidiff {
				grid-template-columns: 1fr;
			}

			.sidebar {
				position: absolute;
				top: 50px;
				left: 14px;
				z-index: 6;
				display: block;
				width: min(280px, calc(100% - 28px));
				border-right: 0;
				background: transparent;
				overflow: visible;
				pointer-events: none;
			}

			.sidebar-head {
				display: none;
			}

			.compare-menu-button {
				display: inline-flex;
			}

			.compare-menu {
				display: none;
				max-height: 360px;
				padding: 8px;
				border: 1px solid #d7e3ef;
				border-radius: 8px;
				background: #fff;
				box-shadow: 0 8px 20px rgba(21, 50, 74, .14);
				overflow: auto;
				pointer-events: auto;
			}

			.compare-menu.is-open {
				display: flex;
			}

			.compare-modes {
				padding: 0;
				border-bottom: 0;
			}

			.tabs {
				flex: 0 0 auto;
				padding: 8px 0 0;
				overflow: visible;
			}

			.main-head {
				flex-direction: column;
				align-items: stretch;
				padding-right: 42px;
			}

			.main-actions {
				justify-content: flex-start;
			}

			.viewer {
				grid-template-columns: 1fr;
				grid-template-rows: repeat(2, minmax(0, 1fr));
			}

			.gidiff.no-previous-version .viewer {
				grid-template-rows: minmax(0, 1fr);
			}

			.pane + .pane {
				border-top: 1px solid #d7e3ef;
				border-left: 0;
			}

			.gidiff.no-previous-version .pane + .pane {
				border-top: 0;
			}

		}
	`;
	const DiffBody = `
	<div class="gidiff">
		<aside class="sidebar">
			<div class="sidebar-head">
				<div class="sidebar-title-row">
					<h1>${Texts.compareContent}</h1>
				</div>
			</div>
			<div class="compare-menu" data-compare-menu>
				<div class="compare-modes" data-compare-modes></div>
				<div class="tabs" data-tabs></div>
			</div>
		</aside>
		<main class="main">
			<div class="main-head">
				<div class="main-head-main">
					<div class="main-title-row">
						<button class="compare-menu-button" type="button" data-compare-menu-toggle
								aria-label="${Texts.compareContent}" aria-expanded="false">
							<span class="compare-menu-icon"></span>
						</button>
						<div class="main-title-text">
							<h2 data-title>${Texts.noFilesTitle}</h2>
							<p data-meta>${Texts.noFilesMeta}</p>
						</div>
					</div>
				</div>
				<div class="main-actions">
					<div class="change-navigation" role="group" aria-label="Change navigation">
						<button class="change-nav-button" type="button" data-change-nav="previous"
								aria-label="Previous change" title="Previous change">
							<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"
									aria-hidden="true" focusable="false">
								<path d="m18 15-6-6-6 6" />
							</svg>
						</button>
						<button class="change-nav-button" type="button" data-change-nav="next"
								aria-label="Next change" title="Next change">
							<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"
									aria-hidden="true" focusable="false">
								<path d="m6 9 6 6 6-6" />
							</svg>
						</button>
					</div>
					<div class="view-mode is-hidden" role="group" aria-label="${Texts.diffViewMode}">
						<button class="mode-button active" type="button" data-view-mode="side" aria-pressed="true">${Texts.sideBySide}</button>
						<button class="mode-button" type="button" data-view-mode="unified" aria-pressed="false">${Texts.unified}</button>
					</div>
					<label class="toggle">
						<input type="checkbox" data-compact-toggle checked />
						<span>${Texts.compactUnchangedBlocks}</span>
					</label>
				</div>
			</div>
			<button class="fullscreen-button" type="button" data-fullscreen-toggle>
				<svg xmlns="http://www.w3.org/2000/svg" width="100%" height="100%" viewBox="0 0 12 12"
						aria-hidden="true" focusable="false">
					<path data-fullscreen-enter-icon fill="currentColor" d="M6.824 2.465a.59.59 0 0 1 .59-.59h2.12a.59.59 0 0 1 .591.59v2.121a.59.59 0 1 1-1.18 0V3.89L7.477 5.357a.59.59 0 1 1-.835-.835L8.11 3.056h-.696a.59.59 0 0 1-.59-.59ZM5.357 6.643a.59.59 0 0 1 0 .835L3.89 8.944h.696a.59.59 0 1 1 0 1.181h-2.12a.59.59 0 0 1-.591-.59V7.414a.59.59 0 0 1 1.18 0v.696l1.467-1.467a.59.59 0 0 1 .835 0Z" />
					<path data-fullscreen-exit-icon class="is-hidden" fill="currentColor" d="M10.311 1.689a.644.644 0 0 1 0 .91l-1.6 1.6h.76a.644.644 0 0 1 0 1.288H7.156a.644.644 0 0 1-.644-.644V2.53a.644.644 0 1 1 1.288 0v.759l1.6-1.6a.644.644 0 0 1 .91 0ZM1.886 7.157c0-.356.288-.644.644-.644h2.313c.356 0 .644.288.644.644V9.47a.644.644 0 1 1-1.288 0v-.759l-1.6 1.6a.644.644 0 1 1-.91-.91l1.6-1.6h-.76a.644.644 0 0 1-.643-.644Z" />
				</svg>
			</button>
			<div class="viewer">
				<section class="pane">
					<div class="pane-head">
						<div class="version-head">
							<span class="version-title" data-left-version-title></span>
							<span class="version-meta" data-left-version-meta></span>
						</div>
					</div>
					<div class="content" data-left></div>
				</section>
				<section class="pane">
					<div class="pane-head">
						<div class="version-head">
							<span class="version-title" data-right-version-title></span>
							<span class="version-meta" data-right-version-meta></span>
						</div>
					</div>
					<div class="content" data-right></div>
				</section>
			</div>
			<div class="unified-viewer">
				<section class="pane">
					<div class="pane-head">
						<div class="unified-version-head">
							<div class="version-head">
								<span class="version-title" data-unified-left-version-title></span>
								<span class="version-meta" data-unified-left-version-meta></span>
							</div>
							<div class="version-head">
								<span class="version-title" data-unified-right-version-title></span>
								<span class="version-meta" data-unified-right-version-meta></span>
							</div>
						</div>
					</div>
					<div class="content" data-unified></div>
				</section>
			</div>
		</main>
	</div>
	`;
	const ErrorStyles = `
${FontFaceStyles}

		body {
			margin: 0;
			padding: 24px;
			background: #edf3f8;
			color: #15324a;
			font: 13px ${ShellFontFamily};
		}

		pre {
			margin: 0;
			padding: 16px;
			border: 1px solid #d7e3ef;
			border-radius: 8px;
			background: #fff;
			white-space: pre-wrap;
		}
	`;
	const DiffHtmlTemplate = `<!doctype html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1" />
	<title>${Texts.pageTitle}</title>
	<style>
${DiffStyles}
	</style>
</head>
<body>
${DiffBody}
	<script>
		var diffModel = ${ModelMarker};
${RuntimeMarker}
	</script>
</body>
</html>`;
	const ErrorHtmlTemplate = `<!doctype html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1" />
	<title>${Texts.pageTitle}</title>
	<style>
${ErrorStyles}
	</style>
</head>
<body><pre>__ERROR_MESSAGE__</pre></body>
</html>`;

	function normalizeText(value) {
		return value === null || value === undefined ? "" : String(value);
	}

	function escapeHtml(value) {
		return normalizeText(value)
			.replace(/&/g, "&amp;")
			.replace(/</g, "&lt;")
			.replace(/>/g, "&gt;")
			.replace(/"/g, "&quot;")
			.replace(/'/g, "&#39;");
	}

	function buildDiffHtml(filesJson, runtimeScript) {
		return DiffHtmlTemplate
			.replace(ModelMarker, filesJson)
			.replace(RuntimeMarker, runtimeScript);
	}

	function buildErrorHtml(message) {
		return ErrorHtmlTemplate.replace("__ERROR_MESSAGE__", escapeHtml(message));
	}

	return {
		buildDiffHtml: buildDiffHtml,
		buildErrorHtml: buildErrorHtml
	};
});
