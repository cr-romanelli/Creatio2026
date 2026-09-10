define("McpProcessSelectionPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"name": "DataGridMain",
				"values": {
					"columns": [
						{
							"id": "a1b2c3d4-0001-4030-8a30-000000000041",
							"code": "MainDS_Caption",
							"caption": "#ResourceString(MainDS_Caption)#",
							"dataValueType": 28
						},
						{
							"id": "a1b2c3d4-0002-4030-8a30-000000000042",
							"code": "MainDS_Name",
							"caption": "#ResourceString(MainDS_Name)#",
							"dataValueType": 28,
							"width": 240
						},
						{
							"id": "a1b2c3d4-0003-4030-8a30-000000000043",
							"code": "MainDS_ModifiedOn",
							"caption": "#ResourceString(MainDS_ModifiedOn)#",
							"dataValueType": 7,
							"width": 200
						}
					],
					"features": {
						"rows": {
							"toolbar": false
						},
						"editable": false
					}
				}
			}
		]/**SCHEMA_VIEW_CONFIG_DIFF*/,
		viewModelConfigDiff: /**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"attributes",
					"DataGridMain",
					"viewModelConfig",
					"attributes"
				],
				"values": {
					"MainDS_Caption": {
						"modelConfig": {
							"path": "MainDS.Caption"
						}
					},
					"MainDS_Name": {
						"modelConfig": {
							"path": "MainDS.Name"
						}
					},
					"MainDS_ModifiedOn": {
						"modelConfig": {
							"path": "MainDS.ModifiedOn"
						}
					},
					"MainDS_Id": {
						"modelConfig": {
							"path": "MainDS.Id"
						}
					}
				}
			}
		]/**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/,
		modelConfigDiff: /**SCHEMA_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"dataSources",
					"MainDS",
					"config"
				],
				"values": {
					"entitySchemaName": "VwProcessLib"
				}
			},
			{
				"operation": "merge",
				"path": [
					"dataSources",
					"MainDS",
					"config",
					"attributes"
				],
				"values": {
					"Caption": {
						"path": "Caption"
					},
					"Name": {
						"path": "Name"
					},
					"ModifiedOn": {
						"path": "ModifiedOn"
					}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});
