define("SysDataSegment_ModalPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "remove",
				"name": "ContinueInOtherPageButton"
			},
			{
				"operation": "merge",
				"name": "MainContainer",
				"values": {
					"alignItems": "stretch"
				}
			},
			{
				"operation": "insert",
				"name": "NameInput",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.SysDataSegmentDS_Name_95pjsu8",
					"control": "$SysDataSegmentDS_Name_95pjsu8",
					"placeholder": "",
					"tooltip": "",
					"readonly": false,
					"multiline": false,
					"labelPosition": "above"
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "DescriptionInput",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 2
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.SysDataSegmentDS_Description_r17sfex",
					"control": "$SysDataSegmentDS_Description_r17sfex",
					"placeholder": "",
					"tooltip": "",
					"readonly": false,
					"multiline": true,
					"labelPosition": "above"
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
					"SysDataSegmentDS_Name_95pjsu8": {
						"modelConfig": {
							"path": "SysDataSegmentDS.Name"
						}
					},
					"SysDataSegmentDS_Description_r17sfex": {
						"modelConfig": {
							"path": "SysDataSegmentDS.Description"
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
						"SysDataSegmentDS": {
							"type": "crt.EntityDataSource",
							"scope": "page",
							"config": {
								"entitySchemaName": "SysDataSegment",
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
					"primaryDataSourceName": "SysDataSegmentDS"
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});