define("KnwLib_FormPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	const DEFAULT_SCORE_THRESHOLD = 0.4;
	const DEFAULT_KEYWORD_BOOSTING_ENABLED = false;
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"name": "SaveButton",
				"values": {
					"size": "large",
					"iconPosition": "only-text"
				}
			},
			{
				"operation": "remove",
				"name": "CardButtonToggleGroup"
			},
			{
				"operation": "merge",
				"name": "CardContentWrapper",
				"values": {
					"padding": {
						"left": "small",
						"right": "small",
						"top": "none",
						"bottom": "none"
					},
					"visible": true,
					"color": "transparent",
					"borderRadius": "none",
					"alignItems": "stretch"
				}
			},
			{
				"operation": "merge",
				"name": "SideAreaProfileContainer",
				"values": {
					"columns": [
						"minmax(64px, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": "none"
					},
					"visible": true,
					"alignItems": "stretch"
				}
			},
			{
				"operation": "merge",
				"name": "Tabs",
				"values": {
					"styleType": "default",
					"mode": "tab",
					"bodyBackgroundColor": "primary-contrast-500",
					"selectedTabTitleColor": "auto",
					"tabTitleColor": "auto",
					"underlineSelectedTabColor": "auto",
					"headerBackgroundColor": "auto",
					"allowToggleClose": true
				}
			},
			{
				"operation": "merge",
				"name": "GeneralInfoTab",
				"values": {
					"iconPosition": "only-text"
				}
			},
			{
				"operation": "merge",
				"name": "GeneralInfoTabContainer",
				"values": {
					"gap": {
						"columnGap": "large",
						"rowGap": "none"
					},
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
				}
			},
			{
				"operation": "remove",
				"name": "CardToggleTabPanel"
			},
			{
				"operation": "remove",
				"name": "FeedTabContainer"
			},
			{
				"operation": "remove",
				"name": "Feed"
			},
			{
				"operation": "remove",
				"name": "FeedTabContainerHeaderContainer"
			},
			{
				"operation": "remove",
				"name": "FeedTabContainerHeaderLabel"
			},
			{
				"operation": "remove",
				"name": "AttachmentsTabContainer"
			},
			{
				"operation": "remove",
				"name": "AttachmentList"
			},
			{
				"operation": "remove",
				"name": "AttachmentsTabContainerHeaderContainer"
			},
			{
				"operation": "remove",
				"name": "AttachmentsTabContainerHeaderLabel"
			},
			{
				"operation": "remove",
				"name": "AttachmentAddButton"
			},
			{
				"operation": "remove",
				"name": "AttachmentRefreshButton"
			},
			{
				"operation": "insert",
				"name": "Name",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 1,
						"colSpan": 1,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.Name",
					"control": "$PDS_Name",
					"labelPosition": "auto"
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwProvider",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_KnwProvider",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_KnwProvider"
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "addRecord_gjvwghw",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(addRecord_gjvwghw_caption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "KnwProvider",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwConnection",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_KnwProviderConnection",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_KnwProviderConnection"
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "addRecord_xd4o779",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(addRecord_xd4o779_caption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "KnwConnection",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwStatus",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 4,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_KnwSourceStatus",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_KnwSourceStatus",
					"visible": true,
					"readonly": true,
					"placeholder": ""
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "addRecord_gotvgye",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(addRecord_gotvgye_caption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "KnwStatus",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "IndexingControlsFlexContainer",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 5,
						"rowSpan": 1
					},
					"type": "crt.FlexContainer",
					"direction": "row",
					"items": [],
					"fitContent": true,
					"visible": "$CardState | crt.IsEqual : 'edit'",
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"alignItems": "stretch",
					"justifyContent": "end",
					"gap": "none",
					"wrap": "wrap"
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "ReindexButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(ReindexButton_caption)#",
					"color": "accent",
					"disabled": "$IndexingButtonsDisabled",
					"size": "large",
					"iconPosition": "left-icon",
					"visible": false,
					"clicked": [
						{
							"request": "crt.SetViewModelAttributeRequest",
							"params": {
								"attributeName": "IndexingButtonsDisabled",
								"value": true
							}
						},
						{
							"request": "crt.StartKnwSourceIndexingRequest",
							"params": {
								"knwSourceId": "$PDS_Id"
							}
						}
					],
					"clickMode": "default",
					"icon": "reload-icon"
				},
				"parentName": "IndexingControlsFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "StartIndexingButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(StartIndexingButton_caption)#",
					"color": "accent",
					"disabled": "$IndexingButtonsDisabled",
					"size": "large",
					"iconPosition": "left-icon",
					"visible": false,
					"clicked": [
						{
							"request": "crt.SetViewModelAttributeRequest",
							"params": {
								"attributeName": "IndexingButtonsDisabled",
								"value": true
							}
						},
						{
							"request": "crt.StartKnwSourceIndexingRequest",
							"params": {
								"knwSourceId": "$PDS_Id"
							}
						}
					],
					"clickMode": "default",
					"icon": "process-button-icon"
				},
				"parentName": "IndexingControlsFlexContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "CancelIndexingButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(CancelIndexingButton_caption)#",
					"color": "warn",
					"disabled": "$IndexingButtonsDisabled",
					"size": "large",
					"iconPosition": "left-icon",
					"visible": false,
					"clicked": [
						{
							"request": "crt.SetViewModelAttributeRequest",
							"params": {
								"attributeName": "IndexingButtonsDisabled",
								"value": true
							}
						},
						{
							"request": "crt.CancelKnwSourceIndexingRequest",
							"params": {
								"knwSourceId": "$PDS_Id"
							}
						}
					],
					"clickMode": "default",
					"icon": "close-button-icon"
				},
				"parentName": "IndexingControlsFlexContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "DescriptionInput",
				"values": {
					"type": "crt.Input",
					"label": "$Resources.Strings.PDS_Description",
					"control": "$PDS_Description",
					"placeholder": "",
					"tooltip": "",
					"readonly": false,
					"multiline": false,
					"labelPosition": "above",
					"visible": true,
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwProviderSettingsOutletContainer",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"items": [],
					"visible": "$CardState | crt.IsEqual : 'edit'",
					"alignItems": "stretch"
				},
				"parentName": "GeneralInfoTab",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "KnwProviderSettingsOutletSectionKnwSourceProvider",
				"values": {
					"type": "crt.SchemaOutlet",
					"schemaName": "SectionKnwSourceProviderSettingsPage",
					"features": {
						"tools": {
							"canExpand": false,
							"canDesign": false
						}
					},
					"KnwSourceId": "$PDS_Id",
					"KnwSourceConfig": "$PDS_KnwSourceConfig",
					"DynamicFolderWarningDismissSignal": "$DynamicFolderWarningDismissSignal",
					"visible": "$PDS_KnwProvider | crt.ToObjectProp : 'value' | crt.IsEqual : 'a7c9d123-8f45-4b21-9a2c-7e31bc456789'"
				},
				"parentName": "KnwProviderSettingsOutletContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwProviderSettingsOutletFileKnwSourceProvider",
				"values": {
					"type": "crt.SchemaOutlet",
					"schemaName": "FileKnwSourceProviderSettingsPage",
					"features": {
						"tools": {
							"canExpand": false,
							"canDesign": false
						}
					},
					"KnwSourceId": "$PDS_Id",
					"KnwSourceConfig": "$PDS_KnwSourceConfig",
					"visible": "$PDS_KnwProvider | crt.ToObjectProp : 'value' | crt.IsEqual : '4b151874-b720-4e17-a4c3-5b74675965b6'"
				},
				"parentName": "KnwProviderSettingsOutletContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "IndexingSessionsTab",
				"values": {
					"type": "crt.TabContainer",
					"items": [],
					"caption": "#ResourceString(IndexingSessionsTab_caption)#",
					"iconPosition": "only-text",
					"visible": "$CardState | crt.IsEqual : 'edit'",
					"icon": null
				},
				"parentName": "Tabs",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "IndexingSessionsExpansionPanel",
				"values": {
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"title": "#ResourceString(IndexingSessionsExpansionPanel_title)#",
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
					"alignItems": "stretch"
				},
				"parentName": "IndexingSessionsTab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "IndexingSessionsActionsGridContainer",
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
				"parentName": "IndexingSessionsExpansionPanel",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "IndexingSessionsActionsFlexContainer",
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
				"parentName": "IndexingSessionsActionsGridContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RefreshIndexingSessionsBtn",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(RefreshIndexingSessionsBtn_caption)#",
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
							"dataSourceName": "IndexingSessionDataGridDS"
						}
					}
				},
				"parentName": "IndexingSessionsActionsFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "IndexingSessionsDataGridContainer",
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
				"parentName": "IndexingSessionsExpansionPanel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "IndexingSessionsDataGrid",
				"values": {
					"type": "crt.DataGrid",
					"layoutConfig": {
						"colSpan": 1,
						"column": 1,
						"row": 1,
						"rowSpan": 10
					},
					"features": {
						"rows": {
							"selection": false,
							"toolbar": false
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						}
					},
					"items": "$IndexingSessionDataGrid",
					"activeRow": "$IndexingSessionDataGrid_ActiveRow",
					"selectionState": "$IndexingSessionDataGrid_SelectionState",
					"_selectionOptions": {
						"attribute": "IndexingSessionDataGrid_SelectionState"
					},
					"primaryColumnName": "IndexingSessionDataGridDS_Id",
					"columns": [
						{
							"id": "15546984-5e69-f2b6-c00b-816e7b63ef04",
							"code": "IndexingSessionDataGridDS_KnwIndexingStatus",
							"caption": "#ResourceString(IndexingSessionDataGridDS_KnwIndexingStatus)#",
							"dataValueType": 10
						},
						{
							"id": "a21d39d8-4914-8665-2474-2a2e8f0664b6",
							"code": "IndexingSessionDataGridDS_StartedOn",
							"caption": "#ResourceString(IndexingSessionDataGridDS_StartedOn)#",
							"dataValueType": 7
						},
						{
							"id": "4565bf14-5ba4-5974-17ce-52f393ef13e7",
							"code": "IndexingSessionDataGridDS_ModifiedOn",
							"caption": "#ResourceString(IndexingSessionDataGridDS_ModifiedOn)#",
							"dataValueType": 7
						},
						{
							"id": "e216f3ec-a5c2-d020-d3fb-143e14571255",
							"code": "IndexingSessionDataGridDS_TransferredOn",
							"caption": "#ResourceString(IndexingSessionDataGridDS_TransferredOn)#",
							"dataValueType": 7
						},
						{
							"id": "70f78dd1-d6eb-71a0-aafc-e891f5e03a5d",
							"code": "IndexingSessionDataGridDS_CompletedOn",
							"caption": "#ResourceString(IndexingSessionDataGridDS_CompletedOn)#",
							"dataValueType": 7
						}
					],
					"rowToolbarItems": [],
					"placeholder": false,
					"bulkActions": [],
					"visible": true
				},
				"parentName": "IndexingSessionsDataGridContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "IndexingSessionDataGrid_AddTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Add tag",
					"icon": "tag-icon",
					"clicked": {
						"request": "crt.AddTagsInRecordsRequest",
						"params": {
							"dataSourceName": "IndexingSessionDataGridDS",
							"filters": "$IndexingSessionDataGrid | crt.ToCollectionFilters : 'IndexingSessionDataGrid' : $IndexingSessionDataGrid_SelectionState | crt.SkipIfSelectionEmpty : $IndexingSessionDataGrid_SelectionState"
						}
					},
					"items": []
				},
				"parentName": "IndexingSessionsDataGrid",
				"propertyName": "bulkActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "IndexingSessionDataGrid_RemoveTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Remove tag",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.RemoveTagsInRecordsRequest",
						"params": {
							"dataSourceName": "IndexingSessionDataGridDS",
							"filters": "$IndexingSessionDataGrid | crt.ToCollectionFilters : 'IndexingSessionDataGrid' : $IndexingSessionDataGrid_SelectionState | crt.SkipIfSelectionEmpty : $IndexingSessionDataGrid_SelectionState"
						}
					}
				},
				"parentName": "IndexingSessionDataGrid_AddTagsBulkAction",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "IndexingSessionDataGrid_ExportToExcelBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Export to Excel",
					"icon": "export-button-icon",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "IndexingSessionsDataGrid",
							"filters": "$IndexingSessionDataGrid | crt.ToCollectionFilters : 'IndexingSessionDataGrid' : $IndexingSessionDataGrid_SelectionState | crt.SkipIfSelectionEmpty : $IndexingSessionDataGrid_SelectionState"
						}
					}
				},
				"parentName": "IndexingSessionsDataGrid",
				"propertyName": "bulkActions",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "IndexingSessionDataGrid_MergeBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Merge",
					"icon": "merge-icon",
					"clicked": {
						"request": "crt.MergeRecordsRequest",
						"params": {
							"dataSourceName": "IndexingSessionDataGridDS",
							"selectionState": "$IndexingSessionDataGrid_SelectionState"
						}
					}
				},
				"parentName": "IndexingSessionsDataGrid",
				"propertyName": "bulkActions",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "IndexingSessionDataGrid_DeleteBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Delete",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.DeleteRecordsRequest",
						"params": {
							"dataSourceName": "IndexingSessionDataGridDS",
							"filters": "$IndexingSessionDataGrid | crt.ToCollectionFilters : 'IndexingSessionDataGrid' : $IndexingSessionDataGrid_SelectionState | crt.SkipIfSelectionEmpty : $IndexingSessionDataGrid_SelectionState"
						}
					}
				},
				"parentName": "IndexingSessionsDataGrid",
				"propertyName": "bulkActions",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "SettingsTab",
				"values": {
					"type": "crt.TabContainer",
					"items": [],
					"caption": "#ResourceString(SettingsTab_caption)#",
					"iconPosition": "only-text",
					"visible": "$CardState | crt.IsEqual : 'edit'",
					"icon": null
				},
				"parentName": "Tabs",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "SettingsTabGridContainer",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(max-content, 32px) minmax(max-content, 32px) minmax(max-content, 32px)",
					"columns": [
						"minmax(32px, 1fr)",
						"minmax(32px, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": "medium"
					},
					"items": [],
					"visible": true,
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "medium",
						"right": "large",
						"bottom": "medium",
						"left": "large"
					}
				},
				"parentName": "SettingsTab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwSettingsButtonsContainer",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"justifyContent": "start",
					"gap": "small",
					"items": [],
					"visible": true,
					"layoutConfig": {
						"column": 1,
						"row": 3,
						"colSpan": 2,
						"rowSpan": 1
					}
				},
				"parentName": "SettingsTabGridContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "KnwSettingsSaveButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(KnwSettingsSaveButton_caption)#",
					"color": "accent",
					"size": "large",
					"clickMode": "default",
					"clicked": [
						{
							"request": "crt.SaveKnwSourceConfigurationRequest",
							"params": {
								"knwSourceId": "$PDS_Id",
								"scoreThreshold": "$ScoreThreshold",
								"keywordBoostingEnabled": "$KeywordBoostingEnabled"
							}
						}
					],
					"iconPosition": "only-text",
					"visible": true
				},
				"parentName": "KnwSettingsButtonsContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "KnwSettingsResetButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(KnwSettingsResetButton_caption)#",
					"color": "default",
					"size": "large",
					"clickMode": "default",
					"clicked": [
						{
							"request": "crt.ResetKnwSourceConfigurationRequest",
							"params": {
								"knwSourceId": "$PDS_Id"
							}
						}
					],
					"iconPosition": "only-text",
					"visible": true
				},
				"parentName": "KnwSettingsButtonsContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ScoreThreshold",
				"values": {
					"type": "crt.NumberInput",
					"label": "#ResourceString(ScoreThreshold_label)#",
					"control": "$ScoreThreshold",
					"placeholder": "",
					"tooltip": "",
					"readonly": false,
					"min": 0,
					"max": 1,
					"step": 0.01,
					"layoutConfig": {
						"column": 1,
						"row": 1,
						"colSpan": 1,
						"rowSpan": 1
					},
					"visible": true,
					"labelPosition": "auto"
				},
				"parentName": "SettingsTabGridContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "KeywordBoostingEnabled",
				"values": {
					"type": "crt.Checkbox",
					"label": "#ResourceString(KeywordBoostingEnabled_label)#",
					"control": "$KeywordBoostingEnabled",
					"readonly": false,
					"layoutConfig": {
						"column": 1,
						"row": 2,
						"colSpan": 1,
						"rowSpan": 1
					},
					"visible": true,
					"labelPosition": "auto",
					"placeholder": "",
					"tooltip": ""
				},
				"parentName": "SettingsTabGridContainer",
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
					"PDS_Id": {
						"modelConfig": {
							"path": "PDS.Id"
						}
					},
					"PDS_Name": {
						"modelConfig": {
							"path": "PDS.Name"
						}
					},
					"PDS_KnwSourceConfig": {
						"modelConfig": {
							"path": "PDS.KnwSourceConfig"
						}
					},
					"SchemaName": {
						"value": "FileKnwSourceProviderSettingsPage"
					},
					"PDS_KnwProvider": {
						"modelConfig": {
							"path": "PDS.KnwProvider"
						}
					},
					"PDS_KnwProvider_List": {
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
					},
					"PDS_KnwProviderConnection": {
						"modelConfig": {
							"path": "PDS.KnwProviderConnection"
						}
					},
					"PDS_KnwProviderConnection_List": {
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
					},
					"PDS_KnwSourceStatus": {
						"modelConfig": {
							"path": "PDS.KnwSourceStatus"
						}
					},
					"PDS_KnwSourceStatus_List": {
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
					},
					"PDS_Description": {
						"modelConfig": {
							"path": "PDS.Description"
						}
					},
					"IndexingSessionDataGrid": {
						"isCollection": true,
						"modelConfig": {
							"path": "IndexingSessionDataGridDS",
							"filterAttributes": [
								{
									"loadOnChange": true,
									"name": "IndexingSessionDataGrid_PredefinedFilter"
								}
							],
							"sortingConfig": {
								"default": [
									{
										"direction": "asc",
										"columnName": "StartedOn"
									}
								]
							}
						},
						"viewModelConfig": {
							"attributes": {
								"IndexingSessionDataGridDS_KnwIndexingStatus": {
									"modelConfig": {
										"path": "IndexingSessionDataGridDS.KnwIndexingStatus"
									}
								},
								"IndexingSessionDataGridDS_StartedOn": {
									"modelConfig": {
										"path": "IndexingSessionDataGridDS.StartedOn"
									}
								},
								"IndexingSessionDataGridDS_ModifiedOn": {
									"modelConfig": {
										"path": "IndexingSessionDataGridDS.ModifiedOn"
									}
								},
								"IndexingSessionDataGridDS_TransferredOn": {
									"modelConfig": {
										"path": "IndexingSessionDataGridDS.TransferredOn"
									}
								},
								"IndexingSessionDataGridDS_CompletedOn": {
									"modelConfig": {
										"path": "IndexingSessionDataGridDS.CompletedOn"
									}
								},
								"IndexingSessionDataGridDS_Id": {
									"modelConfig": {
										"path": "IndexingSessionDataGridDS.Id"
									}
								}
							}
						}
					},
					"IndexingSessionDataGrid_PredefinedFilter": {
						"value": null
					},
					"IndexingButtonsDisabled": {
						"value": false
					},
					"ScoreThreshold": {
						"value": DEFAULT_SCORE_THRESHOLD,
						"modelConfig": {}
					},
					"KeywordBoostingEnabled": {
						"value": DEFAULT_KEYWORD_BOOSTING_ENABLED,
						"modelConfig": {}
					},
					"IsCustomConfiguration": {
						"value": false
					},
					"ConfigurationLoaded": {
						"value": false
					},
					"DynamicFolderWarningDismissSignal": {
						"value": 0
					}
				}
			},
			{
				"operation": "merge",
				"path": [
					"attributes",
					"CardState"
				],
				"values": {
					"modelConfig": {}
				}
			},
			{
				"operation": "merge",
				"path": [
					"attributes",
					"Id",
					"modelConfig"
				],
				"values": {
					"path": "PDS.Id"
				}
			}
		]/**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/,
		modelConfigDiff: /**SCHEMA_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [],
				"values": {
					"primaryDataSourceName": "PDS",
					"dependencies": {
						"IndexingSessionDataGridDS": [
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
					"PDS": {
						"type": "crt.EntityDataSource",
						"config": {
							"entitySchemaName": "KnwSource"
						},
						"scope": "page"
					},
					"IndexingSessionDataGridDS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "KnwIndexingSession",
							"attributes": {
								"KnwIndexingStatus": {
									"path": "KnwIndexingStatus"
								},
								"StartedOn": {
									"path": "StartedOn"
								},
								"ModifiedOn": {
									"path": "ModifiedOn"
								},
								"TransferredOn": {
									"path": "TransferredOn"
								},
								"CompletedOn": {
									"path": "CompletedOn"
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