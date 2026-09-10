define("LocalizationChangeLogModalPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"name": "MainHeader",
				"values": {
					"wrap": "wrap"
				}
			},
			{
				"operation": "merge",
				"name": "PageTitle",
				"values": {
					"labelType": "headline-3",
					"visible": true
				}
			},
			{
				"operation": "remove",
				"name": "ActionButtonsContainer"
			},
			{
				"operation": "remove",
				"name": "ContinueInOtherPageButton"
			},
			{
				"operation": "merge",
				"name": "CloseButton",
				"values": {
					"color": "primary",
					"caption": "#ResourceString(CloseButton_caption)#",
					"iconPosition": "only-text",
					"size": "large",
					"disabled": false
				}
			},
			{
				"operation": "remove",
				"name": "CloseButton",
				"properties": [
					"visible",
					"icon"
				]
			},
			{
				"operation": "move",
				"name": "CloseButton",
				"parentName": "FooterContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "merge",
				"name": "MainContainer",
				"values": {
					"alignItems": "stretch"
				}
			},
			{
				"operation": "remove",
				"name": "CancelButton"
			},
			{
				"operation": "remove",
				"name": "SaveButton"
			},
			{
				"operation": "insert",
				"name": "KeyInput",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.LocalizationChangeLogDS_Key_0c0ryer",
					"control": "$LocalizationChangeLogDS_Key_0c0ryer",
					"placeholder": "",
					"tooltip": "",
					"readonly": true,
					"multiline": false,
					"labelPosition": "above",
					"visible": true
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "OldValueInput",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.LocalizationChangeLogDS_OldValue_mcc8c4z",
					"control": "$LocalizationChangeLogDS_OldValue_mcc8c4z",
					"placeholder": "",
					"tooltip": "",
					"readonly": false,
					"multiline": false,
					"labelPosition": "above"
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "NewValueInput",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.LocalizationChangeLogDS_NewValue_kazn68e",
					"control": "$LocalizationChangeLogDS_NewValue_kazn68e",
					"placeholder": "",
					"tooltip": "",
					"readonly": false,
					"multiline": false,
					"labelPosition": "above"
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 2
			}
		]/**SCHEMA_VIEW_CONFIG_DIFF*/,
		viewModelConfigDiff: /**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"attributes"
				],
				"values": {
					"LocalizationChangeLogDS_Key_0c0ryer": {
						"modelConfig": {
							"path": "LocalizationChangeLogDS.Key"
						}
					},
					"LocalizationChangeLogDS_OldValue_mcc8c4z": {
						"modelConfig": {
							"path": "LocalizationChangeLogDS.OldValue"
						}
					},
					"LocalizationChangeLogDS_NewValue_kazn68e": {
						"modelConfig": {
							"path": "LocalizationChangeLogDS.NewValue"
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
					"dataSources": {
						"LocalizationChangeLogDS": {
							"type": "crt.EntityDataSource",
							"scope": "page",
							"config": {
								"entitySchemaName": "LocalizationChangeLog",
								"loadParameters": {
									"options": {
										"pagingConfig": {
											"rowCount": 1,
											"rowsOffset": -1
										},
										"sortingConfig": {
											"columns": []
										}
									}
								},
								"allowCopyingRecords": false
							}
						}
					},
					"primaryDataSourceName": "LocalizationChangeLogDS"
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});