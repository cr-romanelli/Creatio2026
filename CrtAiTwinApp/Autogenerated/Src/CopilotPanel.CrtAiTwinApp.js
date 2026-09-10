 define("CopilotPanel", /**SCHEMA_DEPS*/["css!CrtAiTwinStyles"]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"name": "Main",
				"values": {
					"classes": "$UseNewAITwin | crt.IfElse: $MainClassesTwin: $MainClassesDefault"
				}
			},
			{
				"operation": "merge",
				"name": "MainHeader",
				"values": {
					"visible": "$UseNewAITwin | crt.InvertBooleanValue"
				}
			},
			{
				"operation": "remove",
				"name": "AILogoImage",
			},
			{
				"operation": "merge",
				"name": "MainContainer",
				"values": {
					"padding": "$UseNewAITwin | crt.IfElse: $NewAIPadding: $ClassicAIPadding",
					"classes": "$UseNewAITwin | crt.IfElse: $MainContainerClassesTwin: $MainContainerClassesDefault"
				}
			},
			{
				"operation": "merge",
				"name": "NewChatButton",
				"values": {
					"visible": "$UseNewAITwin | crt.InvertBooleanValue"
				}
			},
			{
				"operation": "merge",
				"name": "CombinedModeChatButton",
				"values": {
					"visible": "$CombinedModeChatButtonVisibleTwin"
				}
			},
			{
				"operation": "merge",
				"name": "CopilotActionButtonsContainer",
				"values": {
					"visible": "$CopilotActionButtonsVisibleTwin"
				}
			},
			{
				"operation": "merge",
				"name": "Chat",
				"values": {
					"visible": "$UseNewAITwin | crt.InvertBooleanValue"
				}
			},
			{
				"operation": "insert",
				"name": "AiTwinShellHost",
				"values": {
					"type": "crt.CrtAiTwinShell",
					"visible": "$UseNewAITwin",
					"sidebarCode": "$SidebarCode",
					"containerName": "$ContainerName",
					"stretch": true,
					"classes": [
						"crt-ai-twin-shell-host"
					]
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 0
			}
		]/**SCHEMA_VIEW_CONFIG_DIFF*/,
		viewModelConfigDiff: /**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/[{
			"operation": "merge",
			"path": [
				"attributes"
			],
			"values": {
				"UseNewAITwin": {
					"converter": 'crt.AiChatStateConverter'
				},
				"NewAIPadding": {
					"value": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					}
				},
				"ClassicAIPadding": {
					"value": {
						"top": "medium",
						"right": "medium",
						"bottom": "medium",
						"left": "medium"
					}
				},
				"MainClassesDefault": {
					"value": []
				},
				"MainClassesTwin": {
					"value": [
						"crt-ai-twin-shell-main"
					]
				},
				"MainContainerClassesDefault": {
					"value": []
				},
				"MainContainerClassesTwin": {
					"value": [
						"crt-ai-twin-shell-main-container"
					]
				},
				"CopilotActionButtonsVisibleValue": {
					"value": false
				},
				"CopilotActionButtonsVisibleTwin": {
					"from": "UseNewAITwin",
					"converter": "crt.IfElse: $CopilotActionButtonsVisibleValue: $CopilotActionButtonsVisible",
					"triggers": [{
						"attribute": "CopilotActionButtonsVisible"
					}]
				},
				"CombinedModeChatButtonVisibleTwin": {
					"from": "UseNewAITwin",
					"converter": "crt.IfElse: $CombinedModeChatButtonVisibleValue: $CombinedModeButtonVisible",
					"triggers": [{
						"attribute": "CombinedModeButtonVisible"
					}]
				},
				"CombinedModeChatButtonVisibleValue": {
					"value": false
				}
			}
		}]/**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/,
		modelConfigDiff: /**SCHEMA_MODEL_CONFIG_DIFF*/[]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});
