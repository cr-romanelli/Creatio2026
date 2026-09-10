define("KnwLib_ListPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
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
				"name": "AddButton",
				"values": {
					"clicked": {
						"request": "crt.CreateRecordRequest",
						"params": {
							"entityName": "KnwSource"
						}
					},
					"caption": "#ResourceString(AddButton_caption)#",
					"size": "large",
					"visible": true,
					"clickMode": "default"
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
				"operation": "remove",
				"name": "ActionButton"
			},
			{
				"operation": "remove",
				"name": "MenuItem_ExportToExcel"
			},
			{
				"operation": "merge",
				"name": "MainFilterContainer",
				"values": {
					"visible": true,
					"alignItems": "stretch"
				}
			},
			{
				"operation": "merge",
				"name": "LeftFilterContainer",
				"values": {
					"visible": true
				}
			},
			{
				"operation": "remove",
				"name": "FolderTreeActions"
			},
			{
				"operation": "remove",
				"name": "LookupQuickFilterByTag"
			},
			{
				"operation": "merge",
				"name": "RightFilterContainer",
				"values": {
					"padding": {
						"top": "small",
						"right": "large",
						"bottom": "none",
						"left": "none"
					},
					"gap": "small",
					"visible": true
				}
			},
			{
				"operation": "remove",
				"name": "DataTable_Summaries"
			},
			{
				"operation": "merge",
				"name": "RefreshButton",
				"values": {
					"size": "medium",
					"clicked": {
						"request": "crt.LoadDataRequest",
						"params": {
							"config": {
								"loadType": "reload",
								"useLastLoadParameters": true
							},
							"dataSourceName": "PDS"
						}
					},
					"visible": true
				}
			},
			{
				"operation": "merge",
				"name": "ContentContainer",
				"values": {
					"visible": true,
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"alignItems": "stretch",
					"justifyContent": "start"
				}
			},
			{
				"operation": "remove",
				"name": "FolderTree"
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
							"dataValueType": 1
						},
						{
							"id": "b2477af7-9f7d-a435-2ffc-64920e2ebe64",
							"code": "PDS_Description",
							"caption": "#ResourceString(PDS_Description)#",
							"dataValueType": 30
						},
						{
							"id": "2d120ae0-a342-3037-3e10-377b49dbbd07",
							"code": "PDS_KnwSourceStatus",
							"caption": "#ResourceString(PDS_KnwSourceStatus)#",
							"dataValueType": 10
						},
						{
							"id": "f2991ead-da14-eac1-838c-f9b073be8a69",
							"code": "PDS_KnwProvider",
							"caption": "#ResourceString(PDS_KnwProvider)#",
							"dataValueType": 10
						},
						{
							"id": "fe7dc283-ef40-4eff-37f0-1494cdb5cccf",
							"code": "PDS_KnwProviderConnection",
							"caption": "#ResourceString(PDS_KnwProviderConnection)#",
							"dataValueType": 10
						}
					],
					"placeholder": false,
					"features": {
						"rows": {
							"selection": false
						},
						"editable": {
							"enable": false,
							"itemsCreation": false
						},
						"columns": {
							"sorting": true,
							"adding": false
						}
					},
					"bulkActions": [],
					"visible": true,
					"fitContent": true
				}
			}
		]/**SCHEMA_VIEW_CONFIG_DIFF*/,
		viewModelConfigDiff: /**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"attributes"
				],
				"values": {
					"Items_PredefinedFilter": {
						"value": null
					}
				}
			},
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
					"PDS_Description": {
						"modelConfig": {
							"path": "PDS.Description"
						}
					},
					"PDS_KnwSourceStatus": {
						"modelConfig": {
							"path": "PDS.KnwSourceStatus"
						}
					},
					"PDS_KnwProvider": {
						"modelConfig": {
							"path": "PDS.KnwProvider"
						}
					},
					"PDS_KnwProviderConnection": {
						"modelConfig": {
							"path": "PDS.KnwProviderConnection"
						}
					}
				}
			},
			{
				"operation": "merge",
				"path": [
					"attributes",
					"Items",
					"modelConfig"
				],
				"values": {
					"filterAttributes": [
						{
							"name": "SearchFilter_Items",
							"loadOnChange": true
						},
						{
							"loadOnChange": true,
							"name": "Items_PredefinedFilter"
						}
					]
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
					"entitySchemaName": "KnwSource",
					"attributes": {
						"Name": {
							"path": "Name"
						},
						"Description": {
							"path": "Description"
						},
						"KnwSourceStatus": {
							"path": "KnwSourceStatus"
						},
						"KnwProvider": {
							"path": "KnwProvider"
						},
						"KnwProviderConnection": {
							"path": "KnwProviderConnection"
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
