define("AIAgents_FormPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "insert",
				"name": "KnwSourcesTabContainer",
				"values": {
					"type": "crt.TabContainer",
					"items": [],
					"caption": "#ResourceString(KnwSourcesTabContainer_caption)#",
					"iconPosition": "only-text",
					"visible": true
				},
				"parentName": "Tabs",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "GridContainer_KnwSources",
				"values": {
					"type": "crt.GridContainer",
					"columns": [
						"minmax(32px, 1fr)",
						"minmax(32px, 1fr)"
					],
					"rows": "minmax(max-content, 32px)",
					"gap": {
						"columnGap": "large",
						"rowGap": "none"
					},
					"items": [],
					"fitContent": true,
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
				"parentName": "KnwSourcesTabContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwSourcesExpansionPanel",
				"values": {
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"title": "#ResourceString(KnwSourcesExpansionPanel_title)#",
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
					"visible": true,
					"alignItems": "stretch",
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "GridContainer_KnwSources",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridContainer_g9f677f",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(max-content, 24px)",
					"columns": [
						"minmax(32px, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": 0
					},
					"styles": {
						"overflow-x": "hidden"
					},
					"items": []
				},
				"parentName": "KnwSourcesExpansionPanel",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_qx03l6a",
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
				"parentName": "GridContainer_g9f677f",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailRefreshBtn_5lr6yjc",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailRefreshBtn_5lr6yjc_caption)#",
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
							"dataSourceName": "KnwSourcesDS"
						}
					}
				},
				"parentName": "FlexContainer_qx03l6a",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwSourcesAddButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(KnwSourcesAddButton_caption)#",
					"icon": "add-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.DataGridCreateItemRequest",
						"params": {
							"dataGridName": "KnwSourcesGridDetail",
							"defaultValues": []
						}
					}
				},
				"parentName": "FlexContainer_qx03l6a",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "KnwSourcesInnerGridContainer",
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
				"parentName": "KnwSourcesExpansionPanel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwSourcesGridDetail",
				"values": {
					"type": "crt.DataGrid",
					"layoutConfig": {
						"colSpan": 2,
						"column": 1,
						"row": 1,
						"rowSpan": 6
					},
					"features": {
						"rows": {
							"selection": {
								"enable": true,
								"multiple": true
							}
						},
						"editable": {
							"enable": true,
							"itemsCreation": false,
							"floatingEditPanel": true
						}
					},
					"items": "$KnwSources",
					"activeRow": "$KnwSources_ActiveRow",
					"selectionState": "$KnwSources_SelectionState",
					"_selectionOptions": {
						"attribute": "KnwSources_SelectionState"
					},
					"primaryColumnName": "KnwSourcesDS_Id",
					"columns": [
						{
							"id": "4bccc4cd-0c0e-a1e1-a05e-ceb610e2b290",
							"code": "KnwSourcesDS_KnwSource",
							"caption": "#ResourceString(KnwSourcesDS_KnwSource)#",
							"dataValueType": 10,
							"width": 327
						},
						{
							"id": "2d2c0d9d-3f27-d45e-9710-11a136e6bcfa",
							"code": "KnwSourcesDS_Status",
							"caption": "#ResourceString(KnwSourcesDS_Status)#",
							"dataValueType": 10,
							"width": 149
						},
						{
							"id": "880eaf03-2df5-4722-9876-5977e560fcf1",
							"code": "KnwSourcesDS_UseCitations",
							"caption": "#ResourceString(KnwSourcesDS_UseCitations)#",
							"dataValueType": 12,
							"width": 128
						},
						{
							"id": "02339be3-117c-45d9-6909-a3ef21c0521a",
							"code": "KnwSourcesDS_ProviderConnection",
							"caption": "#ResourceString(KnwSourcesDS_ProviderConnection)#",
							"dataValueType": 10,
							"width": 311
						}
					],
					"placeholder": false,
					"bulkActions": [],
					"visible": true,
					"fitContent": true
				},
				"parentName": "KnwSourcesInnerGridContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwSources_AddTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Add tag",
					"icon": "tag-icon",
					"clicked": {
						"request": "crt.AddTagsInRecordsRequest",
						"params": {
							"dataSourceName": "KnwSourcesDS",
							"filters": "$KnwSources | crt.ToCollectionFilters : 'KnwSources' : $KnwSources_SelectionState | crt.SkipIfSelectionEmpty : $KnwSources_SelectionState"
						}
					},
					"items": []
				},
				"parentName": "KnwSourcesGridDetail",
				"propertyName": "bulkActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwSources_RemoveTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Remove tag",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.RemoveTagsInRecordsRequest",
						"params": {
							"dataSourceName": "KnwSourcesDS",
							"filters": "$KnwSources | crt.ToCollectionFilters : 'KnwSources' : $KnwSources_SelectionState | crt.SkipIfSelectionEmpty : $KnwSources_SelectionState"
						}
					}
				},
				"parentName": "KnwSources_AddTagsBulkAction",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwSources_ExportToExcelBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Export to Excel",
					"icon": "export-button-icon",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "KnwSourcesGridDetail",
							"filters": "$KnwSources | crt.ToCollectionFilters : 'KnwSources' : $KnwSources_SelectionState | crt.SkipIfSelectionEmpty : $KnwSources_SelectionState"
						}
					}
				},
				"parentName": "KnwSourcesGridDetail",
				"propertyName": "bulkActions",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "KnwSources_MergeBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Merge",
					"icon": "merge-icon",
					"clicked": {
						"request": "crt.MergeRecordsRequest",
						"params": {
							"dataSourceName": "KnwSourcesDS",
							"selectionState": "$KnwSources_SelectionState"
						}
					}
				},
				"parentName": "KnwSourcesGridDetail",
				"propertyName": "bulkActions",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "KnwSources_DeleteBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Delete",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.DeleteRecordsRequest",
						"params": {
							"dataSourceName": "KnwSourcesDS",
							"filters": "$KnwSources | crt.ToCollectionFilters : 'KnwSources' : $KnwSources_SelectionState | crt.SkipIfSelectionEmpty : $KnwSources_SelectionState"
						}
					}
				},
				"parentName": "KnwSourcesGridDetail",
				"propertyName": "bulkActions",
				"index": 3
			}
		]/**SCHEMA_VIEW_CONFIG_DIFF*/,
		viewModelConfigDiff: /**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"attributes"
				],
				"values": {
					"KnwSources": {
						"isCollection": true,
						"modelConfig": {
							"path": "KnwSourcesDS",
							"sortingConfig": {
								"default": [
									{
										"direction": "asc",
										"columnName": "KnwSource_Name"
									}
								]
							},
							"filterAttributes": [
								{
									"loadOnChange": true,
									"name": "KnwSources_PredefinedFilter"
								}
							]
						},
						"viewModelConfig": {
							"attributes": {
								"KnwSourcesDS_KnwSource": {
									"modelConfig": {
										"path": "KnwSourcesDS.KnwSource"
									}
								},
								"KnwSourcesDS_Status": {
									"modelConfig": {
										"path": "KnwSourcesDS.KnwSource_KnwSourceStatus"
									}
								},
								"KnwSourcesDS_UseCitations": {
									"modelConfig": {
										"path": "KnwSourcesDS.UseCitations"
									}
								},
								"KnwSourcesDS_ProviderConnection": {
									"modelConfig": {
										"path": "KnwSourcesDS.KnwSource_KnwProviderConnection"
									}
								},
								"KnwSourcesDS_Id": {
									"modelConfig": {
										"path": "KnwSourcesDS.Id"
									}
								}
							}
						}
					},
					"KnwSources_PredefinedFilter_Path": {
						"value": "IntentId"
					},
					"KnwSources_PredefinedFilter": {
						"from": "PDS_Id",
						"converter": "crt.ToEqualFilter: $KnwSources_PredefinedFilter_Path"
					}
				}
			},
			{
				"operation": "merge",
				"path": [
					"attributes",
					"CopilotSkillAgents",
					"modelConfig",
					"sortingConfig"
				],
				"values": {
					"default": [
						{
							"direction": "asc",
							"columnName": "Description"
						}
					]
				}
			}
		]/**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/,
		modelConfigDiff: /**SCHEMA_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"dataSources"
				],
				"values": {
					"KnwSourcesDS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "KnwSourceInSkill",
							"attributes": {
								"KnwSource": {
									"path": "KnwSource"
								},
								"KnwSource_KnwSourceStatus": {
									"type": "ForwardReference",
									"path": "KnwSource.KnwSourceStatus"
								},
								"KnwSource_KnwProviderConnection": {
									"type": "ForwardReference",
									"path": "KnwSource.KnwProviderConnection"
								},
								"UseCitations": {
									"path": "UseCitations"
								}
							}
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
