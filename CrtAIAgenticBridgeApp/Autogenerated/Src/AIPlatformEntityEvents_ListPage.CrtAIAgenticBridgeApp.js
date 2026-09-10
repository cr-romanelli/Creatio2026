define("AIPlatformEntityEvents_ListPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "remove",
				"name": "DataImportButton"
			},
			{
				"operation": "remove",
				"name": "MenuItem_ImportFromExcel"
			},
			{
				"operation": "remove",
				"name": "OpenLandingDesigner"
			},
			{
				"operation": "remove",
				"name": "ActionButton"
			},
			{
				"operation": "remove",
				"name": "MenuItem_ExportToExcel"
			},
			{
				"operation": "merge",
				"name": "FolderTree",
				"values": {
					"rootSchemaName": "AIPlatformEntityEvents"
				}
			},
			{
				"operation": "merge",
				"name": "DataTable",
				"values": {
					"columns": [
						{
							"id": "f252f581-0ccf-44ac-b7c9-c00df2ad9919",
							"code": "PDS_Name",
							"caption": "#ResourceString(PDS_Name)#",
							"dataValueType": 1,
							"width": 336
						},
						{
							"id": "a146e5cf-ea90-3d1a-54bc-2215183a5d83",
							"code": "PDS_EntitySchema",
							"caption": "#ResourceString(PDS_EntitySchema)#",
							"dataValueType": 10
						},
						{
							"id": "d994b3f3-e376-333f-11dd-3a6c4d93bb69",
							"code": "PDS_EntitySchemaChangeType",
							"caption": "#ResourceString(PDS_EntitySchemaChangeType)#",
							"dataValueType": 10
						},
						{
							"id": "c8689d78-80ba-4e71-8cf2-fa478e3be5bc",
							"code": "PDS_CreatedOn",
							"caption": "#ResourceString(PDS_CreatedOn)#",
							"dataValueType": 7
						},
						{
							"id": "fd4b3485-a46e-4219-b775-adef1210fe51",
							"code": "PDS_CreatedBy",
							"caption": "#ResourceString(PDS_CreatedBy)#",
							"dataValueType": 10
						}
					],
					"features": {
						"rows": {
							"selection": false
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						}
					},
					"visible": true
				}
			},
			{
				"operation": "merge",
				"name": "Dashboards",
				"values": {
					"_designOptions": {
						"entitySchemaName": "AIPlatformEntityEvents",
						"dependencies": [
							{
								"attributePath": "Id",
								"relationPath": "PDS.Id"
							}
						],
						"filters": []
					}
				}
			},
			{
				"operation": "insert",
				"name": "PushTriggersButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(PushTriggersButton_caption)#",
					"clicked": {
						"request": "crt.PushTriggersRequest",
						"params": {
							"successMessage": "#ResourceString(PushTriggersSuccessMessage)#",
							"successMessageTemplate": "#ResourceString(PushTriggersSuccessMessageTemplate)#",
							"fallbackErrorMessage": "#ResourceString(PushTriggersFailedMessage)#"
						}
					},
					"color": "accent",
					"disabled": false,
					"size": "large",
					"iconPosition": "left-icon",
					"visible": true,
					"icon": "cloud-arrow-up-icon"
				},
				"parentName": "ActionButtonsContainer",
				"propertyName": "items",
				"index": 0
			}
		]/**SCHEMA_VIEW_CONFIG_DIFF*/,
		viewModelConfigDiff: /**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"attributes",
					"Items",
					"viewModelConfig",
					"attributes"
				],
				"values": {
					"PDS_Name": {
						"modelConfig": {
							"path": "PDS.Name"
						}
					},
					"PDS_EntitySchema": {
						"modelConfig": {
							"path": "PDS.EntitySchema"
						}
					},
					"PDS_EntitySchemaChangeType": {
						"modelConfig": {
							"path": "PDS.EntitySchemaChangeType"
						}
					},
					"PDS_CreatedOn": {
						"modelConfig": {
							"path": "PDS.CreatedOn"
						}
					},
					"PDS_CreatedBy": {
						"modelConfig": {
							"path": "PDS.CreatedBy"
						}
					}
				}
			},
			{
				"operation": "merge",
				"path": [
					"attributes",
					"Items",
					"modelConfig",
					"sortingConfig"
				],
				"values": {
					"default": [
						{
							"direction": "asc",
							"columnName": "Name"
						}
					]
				}
			}
		]/**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/,
		modelConfigDiff: /**SCHEMA_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"dataSources",
					"PDS",
					"config"
				],
				"values": {
					"entitySchemaName": "AIPlatformEntityEvents",
					"attributes": {
						"Name": {
							"path": "Name"
						},
						"EntitySchema": {
							"path": "EntitySchema"
						},
						"EntitySchemaChangeType": {
							"path": "EntitySchemaChangeType"
						},
						"CreatedOn": {
							"path": "CreatedOn"
						},
						"CreatedBy": {
							"path": "CreatedBy"
						}
					}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});
