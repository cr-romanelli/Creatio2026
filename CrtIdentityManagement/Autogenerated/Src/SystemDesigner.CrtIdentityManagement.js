define("SystemDesigner", ["RightUtilities"], function(RightUtilities) {
	return {
		methods: {
			openIdentityProvidersManagementSection: function() {
				var canUseIdentityProvidersManagement = this.get("CanUseIdentityProvidersManagement");
				if (!this.Ext.isEmpty(canUseIdentityProvidersManagement)) {
					this.navigateToIdentityProvidersManagementSection();
				} else {
					RightUtilities.checkCanExecuteOperation({
						operation: "CanUseIdentityProvidersManagement"
					}, function(result) {
						this.set("CanUseIdentityProvidersManagement", result);
						this.navigateToIdentityProvidersManagementSection();
					}, this);
				}
				return false;
			},

			/**
			 * Opens identity providers management page when user has rights.
			 * Otherwise shows a permissions error message.
			 * @private
			 */
			navigateToIdentityProvidersManagementSection: function() {
				if (this.getIsFeatureEnabled("IdentityProvidersManagement")) {
					if (this.get("CanUseIdentityProvidersManagement") === true) {
						this.sandbox.publish("PushHistoryState", {
							hash: "Page/IdentityProvidersManagement_Page"
						});
					} else {
						this.showPermissionsErrorMessage("CanUseIdentityProvidersManagement");
					}
				}
			},

			/**
			 * @return {Boolean} True if IdentityProvidersManagement integration enabled.
			 * @private
			 */
			getIsIdentityProvidersManagementEnabled: function() {
				return this.getIsFeatureEnabled("IdentityProvidersManagement");
			}
		},
		diff: [
			{
				"operation": "insert",
				"index": 1,
				"propertyName": "items",
				"parentName": "IntegrationTile",
				"name": "IdentityProvidersManagementSection",
				"values": {
					"itemType": this.Terrasoft.ViewItemType.LINK,
					"caption": {"bindTo": "Resources.Strings.IdentityProvidersManagementSectionCaption"},
					"click": {"bindTo": "openIdentityProvidersManagementSection"},
					"visible": {"bindTo": "getIsIdentityProvidersManagementEnabled"}
				}
			}
		]
	};
});
