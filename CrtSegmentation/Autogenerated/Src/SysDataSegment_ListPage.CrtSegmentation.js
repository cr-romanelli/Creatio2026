define("SysDataSegment_ListPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"name": "AddButton",
				"values": {
					"size": "large"
				}
			},
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
				"operation": "merge",
				"name": "FolderTree",
				"values": {
					"rootSchemaName": "SysDataSegment"
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
							"width": 273
						},
						{
							"id": "c0e01e53-afb3-83a3-2a8e-21adf2954c45",
							"code": "PDS_SysRefreshMode",
							"caption": "#ResourceString(PDS_SysRefreshMode)#",
							"dataValueType": 10
						},
						{
							"id": "7bc56db6-fa9b-d441-fb8a-e9fd84eae03f",
							"code": "PDS_SysUserStatus",
							"caption": "#ResourceString(PDS_SysUserStatus)#",
							"dataValueType": 10,
							"width": 170.99305725097656
						},
						{
							"id": "e335f989-29ee-e215-8ef1-100f7fb4ef6b",
							"code": "PDS_RecordCount",
							"caption": "#ResourceString(PDS_RecordCount)#",
							"dataValueType": 4,
							"width": 167.97222900390625
						},
						{
							"id": "49447907-d5e0-6fc4-f22b-1da5ec188b18",
							"code": "PDS_EntitySchema",
							"caption": "#ResourceString(PDS_EntitySchema)#",
							"dataValueType": 10,
							"width": 130.99305725097656
						},
						{
							"id": "91d1f4a4-bb3a-5299-5fcc-d2452a6810f5",
							"code": "PDS_Description",
							"caption": "#ResourceString(PDS_Description)#",
							"dataValueType": 29,
							"width": 313
						},
						{
							"id": "c8689d78-80ba-4e71-8cf2-fa478e3be5bc",
							"code": "PDS_CreatedOn",
							"caption": "#ResourceString(PDS_CreatedOn)#",
							"dataValueType": 7,
							"width": 144
						},
						{
							"id": "113e0eaf-18f7-86b1-68dc-c896d0cafc17",
							"code": "PDS_ModifiedOn",
							"caption": "#ResourceString(PDS_ModifiedOn)#",
							"dataValueType": 7,
							"width": 136
						}
					],
					"features": {
						"rows": {
							"selection": false,
							"numeration": false
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
						"entitySchemaName": "SysDataSegment",
						"dependencies": [
							{
								"attributePath": "Id",
								"relationPath": "PDS.Id"
							}
						],
						"filters": []
					}
				}
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
					"PDS_SysRefreshMode": {
						"modelConfig": {
							"path": "PDS.SysRefreshMode"
						}
					},
					"PDS_SysUserStatus": {
						"modelConfig": {
							"path": "PDS.SysUserStatus"
						}
					},
					"PDS_RecordCount": {
						"modelConfig": {
							"path": "PDS.RecordCount"
						}
					},
					"PDS_EntitySchema": {
						"modelConfig": {
							"path": "PDS.EntitySchema"
						}
					},
					"PDS_Description": {
						"modelConfig": {
							"path": "PDS.Description"
						}
					},
					"PDS_CreatedOn": {
						"modelConfig": {
							"path": "PDS.CreatedOn"
						}
					},
					"PDS_ModifiedOn": {
						"modelConfig": {
							"path": "PDS.ModifiedOn"
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
							"columnName": "Status"
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
					"entitySchemaName": "SysDataSegment",
					"attributes": {
						"Name": {
							"path": "Name"
						},
						"SysRefreshMode": {
							"path": "SysRefreshMode"
						},
						"SysUserStatus": {
							"path": "SysUserStatus"
						},
						"RecordCount": {
							"path": "RecordCount"
						},
						"EntitySchema": {
							"path": "EntitySchema"
						},
						"Description": {
							"path": "Description"
						},
						"CreatedOn": {
							"path": "CreatedOn"
						},
						"ModifiedOn": {
							"path": "ModifiedOn"
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