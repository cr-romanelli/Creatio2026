define("McpSourceActionSelectionPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"name": "DataGridMain",
				"values": {
					"columns": [
						{
							"id": "c3d4e5f6-0001-4030-8a30-000000000081",
							"code": "MainDS_Name",
							"caption": "#ResourceString(MainDS_Name)#",
							"dataValueType": 28,
							"width": 320
						},
						{
							"id": "c3d4e5f6-0002-4030-8a30-000000000082",
							"code": "MainDS_Description",
							"caption": "#ResourceString(MainDS_Description)#",
							"dataValueType": 28
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
					"MainDS_Name": {
						"modelConfig": {
							"path": "MainDS.Name"
						}
					},
					"MainDS_Description": {
						"modelConfig": {
							"path": "MainDS.Description"
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
					"entitySchemaName": "McpSourceCodeAction"
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
					"Name": {
						"path": "Name"
					},
					"Description": {
						"path": "Description"
					}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});
