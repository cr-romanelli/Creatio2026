define("SystemDesigner", ["NavigationHelper", "SystemDesignerResources"], function () {
	return {
		methods: {
			/**
			 * Opens Creatio.ai Studio in a new browser tab.
			 * The URL is taken from the "AIStudioServiceUrl" system setting when it is filled in;
			 * otherwise the default https://ai-studio.creatio.com is used.
			 * @public
			 */
			navigateToAIStudio: function () {
				Terrasoft.SysSettings.querySysSettingsItem("AIStudioServiceUrl", (url) => {
					const targetUrl = url || "https://ai-studio.creatio.com";
					const navigationHelper = this.Ext.create("Terrasoft.NavigationHelper", {
						Ext: this.Ext,
						sandbox: this.sandbox
					});
					navigationHelper.navigateTo({
						target: "Url",
						options: {
							newTab: true,
							url: targetUrl
						}
					});
				}, this);
			},

			/**
			 * Checks whether the legacy "Creatio.ai sub-agents" and "Creatio.ai agents"
			 * links should be shown in the System setup tile. Controlled by the
			 * "ShowAISkillsAndAgentsInSystemDesigner" feature flag, which is disabled by default
			 * (an absent feature evaluates to false), so the legacy links are hidden out of the box.
			 * @return {Boolean} True when the legacy links should be visible.
			 * @public
			 */
			isLegacyAiLinksVisible: function () {
				return this.getIsFeatureEnabled("ShowAISkillsAndAgentsInSystemDesigner");
			}
		},
		diff: [
			{
				"operation": "merge",
				"name": "AISkills",
				"values": {
					"visible": {"bindTo": "isLegacyAiLinksVisible"},
					"isNew": false
				}
			},
			{
				"operation": "merge",
				"name": "AIAgents",
				"values": {
					"visible": {"bindTo": "isLegacyAiLinksVisible"},
					"isNew": false
				}
			},
			{
				"operation": "insert",
				"propertyName": "items",
				"parentName": "SystemSettingsTile",
				"name": "AIStudio",
				"values": {
					"itemType": Terrasoft.ViewItemType.LINK,
					"caption": {"bindTo": "Resources.Strings.AIStudioCaption"},
					"tag": "navigateToAIStudio",
					"click": {"bindTo": "invokeOperation"},
					"isNew": true
				}
			}
		]
	};
});