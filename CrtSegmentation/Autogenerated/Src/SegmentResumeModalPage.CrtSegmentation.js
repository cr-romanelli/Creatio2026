define("SegmentResumeModalPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"name": "PageTitle",
				"values": {
					"caption": "#MacrosTemplateString(#ResourceString(PageTitle_caption)#)#",
					"visible": true
				}
			},
			{
				"operation": "merge",
				"name": "ContinueInOtherPageButton",
				"values": {
					"color": "default",
					"clickMode": "default"
				}
			},
			{
				"operation": "remove",
				"name": "CloseButton"
			},
			{
				"operation": "merge",
				"name": "MainContainer",
				"values": {
					"gap": {
						"columnGap": "large",
						"rowGap": "small"
					},
					"alignItems": "stretch"
				}
			},
			{
				"operation": "merge",
				"name": "SaveButton",
				"values": {
					"clicked": {
						"request": "crt.SaveRecordRequest",
						"params": {
							"showSuccessMessage": false
						}
					},
					"caption": "#ResourceString(SaveButton_caption)#"
				}
			},
			{
				"operation": "insert",
				"name": "DescriptionLabel",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(DescriptionLabel_caption)#)#",
					"labelType": "body",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "#757575",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "SegmentResumeModeComboBox",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PageParameters_LookupParameter1_dqx8ru2",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": false,
					"labelPosition": "above",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PageParameters_LookupParameter1_dqx8ru2",
					"mode": "List",
					"visible": true,
					"readonly": false,
					"placeholder": "",
					"valueDetails": null,
					"secondaryDisplayValue": "Description"
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 1
			}
		]/**SCHEMA_VIEW_CONFIG_DIFF*/,
		viewModelConfigDiff: /**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"attributes"
				],
				"values": {
					"PageParameters_LookupParameter1_dqx8ru2": {
						"modelConfig": {
							"path": "PageParameters.SegmentResumeMode"
						}
					},
					"PageParameters_LookupParameter1_dqx8ru2_List": {
						"isCollection": true,
						"modelConfig": {
							"sortingConfig": {
								"default": [
									{
										"columnName": "Name",
										"direction": "asc"
									}
								]
							}
						}
					}
				}
			}
		]/**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/,
		modelConfigDiff: /**SCHEMA_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [],
				"values": {
					"dataSources": {}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});