define("ConfActivityLogDiffBuilder", [
	"ConfActivityLogDiffMapper",
	"ConfActivityLogDiffTemplate",
	"ConfActivityLogDiffRuntime"
], function(mapper, template, runtime) {
	function serializeForInlineScript(value) {
		return JSON.stringify(value || {})
			.replace(/</g, "\\u003c")
			.replace(/>/g, "\\u003e")
			.replace(/&/g, "\\u0026")
			.replace(/\u2028/g, "\\u2028")
			.replace(/\u2029/g, "\\u2029")
			.replace(/<\/script/gi, "<\\/script");
	}

	function escapeInlineScript(value) {
		return String(value || "").replace(/<\/script/gi, "<\\/script");
	}

	function buildHtmlForModel(model) {
		const runtimeTextsJson = serializeForInlineScript(runtime.getTexts());
		return template.buildDiffHtml(
			serializeForInlineScript(model),
			escapeInlineScript(runtime.getScript(runtimeTextsJson))
		);
	}

	function buildErrorHtml(message) {
		return template.buildErrorHtml(message);
	}

	function buildHtml(response, options) {
		if (!mapper.isSuccessful(response)) {
			return buildErrorHtml(mapper.getErrorMessage(response));
		}
		return buildHtmlForModel(mapper.buildDiffModel(response, options));
	}

	return {
		buildHtml: buildHtml,
		buildErrorHtml: buildErrorHtml
	};
});
