define("FileKnwSourceProviderSettingsPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "insert",
				"name": "FilesExpansionPanel",
				"values": {
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"title": "#ResourceString(FilesExpansionPanel_title)#",
					"toggleType": "default",
					"togglePosition": "before",
					"expanded": true,
					"labelColor": "auto",
					"fullWidthHeader": false,
					"titleWidth": 20,
					"padding": {
						"top": "small",
						"bottom": "small",
						"left": "none",
						"right": "none"
					},
					"fitContent": true,
					"visible": "$IsLoaded",
					"alignItems": "stretch"
				},
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FileActionsGridContainer",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(max-content, 24px)",
					"columns": [
						"minmax(32px, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": "none"
					},
					"styles": {
						"overflow-x": "hidden"
					},
					"items": [],
					"visible": true,
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"alignItems": "stretch"
				},
				"parentName": "FilesExpansionPanel",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FileActionsFlexContainer",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"gap": "none",
					"alignItems": "center",
					"items": [],
					"layoutConfig": {
						"colSpan": 1,
						"column": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "FileActionsGridContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "UploadFileBtn",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(UploadFileBtn_caption)#",
					"icon": "upload-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.UploadFileRequest",
						"params": {
							"recordId": "$Id",
							"viewElementName": "FilesDataGrid",
							"allowedFileTypes": ".pdf, .txt, .md, .json, .docx, .pptx"
						}
					},
					"visible": true,
					"clickMode": "default"
				},
				"parentName": "FileActionsFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RefreshFilesBtn",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(RefreshFilesBtn_caption)#",
					"icon": "reload-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.LoadDataRequest",
						"params": {
							"config": {
								"loadType": "reload"
							},
							"dataSourceName": "KnwSourceFilesDS"
						}
					}
				},
				"parentName": "FileActionsFlexContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "FilesSearchFilter",
				"values": {
					"type": "crt.SearchFilter",
					"placeholder": "#ResourceString(FilesSearchFilter_placeholder)#",
					"_filterOptions": {
						"expose": [
							{
								"attribute": "FilesSearchFilter_KnwSourceFiles",
								"converters": [
									{
										"converter": "crt.SearchFilterAttributeConverter",
										"args": [
											"KnwSourceFiles"
										]
									}
								]
							}
						],
						"from": [
							"FilesSearchFilter_SearchValue",
							"FilesSearchFilter_FilteredColumnsGroups"
						]
					}
				},
				"parentName": "FileActionsFlexContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "SupportFileTypesLabel",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(SupportFileTypesLabel_caption)#)#",
					"labelType": "caption",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "FilesExpansionPanel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FilesGridContainer",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(max-content, 32px)",
					"columns": [
						"minmax(32px, 1fr)",
						"minmax(32px, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": "none"
					},
					"styles": {
						"overflow-x": "hidden"
					},
					"items": [],
					"visible": true,
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"alignItems": "stretch"
				},
				"parentName": "FilesExpansionPanel",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "FilesDataGrid",
				"values": {
					"type": "crt.FileList",
					"masterRecordColumnValue": "$Id",
					"recordColumnName": "KnwSource",
					"layoutConfig": {
						"colSpan": 2,
						"column": 1,
						"row": 1,
						"rowSpan": 10
					},
					"items": "$KnwSourceFiles",
					"primaryColumnName": "KnwSourceFilesDS_Id",
					"columns": [
						{
							"id": "a79bc4d0-cae0-611e-01c4-ba0751ae8aab",
							"code": "KnwSourceFilesDS_Name",
							"caption": "#ResourceString(KnwSourceFilesDS_Name)#",
							"dataValueType": 28
						},
						{
							"id": "7ffffb3d-fd5b-7f29-578c-992566cfc276",
							"code": "KnwSourceFilesDS_CreatedOn",
							"caption": "#ResourceString(KnwSourceFilesDS_CreatedOn)#",
							"dataValueType": 7
						},
						{
							"id": "0c66a323-8275-2e17-5541-3623eb4ad1a5",
							"code": "KnwSourceFilesDS_CreatedBy",
							"caption": "#ResourceString(KnwSourceFilesDS_CreatedBy)#",
							"dataValueType": 10
						},
						{
							"id": "16ac0075-a393-b3e8-07a4-950c3ef995a9",
							"code": "KnwSourceFilesDS_Size",
							"caption": "#ResourceString(KnwSourceFilesDS_Size)#",
							"dataValueType": 4
						}
					],
					"visible": true,
					"viewType": "list",
					"tileSize": "medium"
				},
				"parentName": "FilesGridContainer",
				"propertyName": "items",
				"index": 0
			}
		]/**SCHEMA_VIEW_CONFIG_DIFF*/,
		viewModelConfigDiff: /**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"attributes"
				],
				"values": {
					"KnwSourceFiles": {
						"isCollection": true,
						"modelConfig": {
							"path": "KnwSourceFilesDS",
							"sortingConfig": {
								"default": [
									{
										"direction": "asc",
										"columnName": "CreatedOn"
									}
								]
							},
							"filterAttributes": [
								{
									"name": "FilesSearchFilter_KnwSourceFiles",
									"loadOnChange": true
								}
							]
						},
						"viewModelConfig": {
							"attributes": {
								"KnwSourceFilesDS_Name": {
									"modelConfig": {
										"path": "KnwSourceFilesDS.Name"
									}
								},
								"KnwSourceFilesDS_CreatedOn": {
									"modelConfig": {
										"path": "KnwSourceFilesDS.CreatedOn"
									}
								},
								"KnwSourceFilesDS_CreatedBy": {
									"modelConfig": {
										"path": "KnwSourceFilesDS.CreatedBy"
									}
								},
								"KnwSourceFilesDS_Size": {
									"modelConfig": {
										"path": "KnwSourceFilesDS.Size"
									}
								},
								"KnwSourceFilesDS_Id": {
									"modelConfig": {
										"path": "KnwSourceFilesDS.Id"
									}
								}
							}
						}
					},
					"Id": {
						"modelConfig": {
							"path": "PDS.Id"
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
					"dependencies": {
						"KnwSourceFilesDS": [
							{
								"attributePath": "KnwSource",
								"relationPath": "PDS.Id"
							}
						],
						"FilesDataGridDS": [
							{
								"attributePath": "KnwSource",
								"relationPath": "PDS.Id"
							}
						]
					}
				}
			},
			{
				"operation": "merge",
				"path": [
					"dataSources"
				],
				"values": {
					"KnwSourceFilesDS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "KnwSourceFile",
							"attributes": {
								"Name": {
									"path": "Name"
								},
								"CreatedOn": {
									"path": "CreatedOn"
								},
								"CreatedBy": {
									"path": "CreatedBy"
								},
								"Size": {
									"path": "Size"
								}
							}
						}
					},
					"FilesDataGridDS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "SysFile"
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