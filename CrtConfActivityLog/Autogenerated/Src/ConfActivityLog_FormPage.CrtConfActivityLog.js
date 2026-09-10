define("ConfActivityLog_FormPage", /**SCHEMA_DEPS*/["@creatio-devkit/common", "ConfActivityLogDiffBuilder"]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/(sdk, diffBuilder)/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"name": "CancelButton",
				"values": {
					"clicked": {
						"request": "crt.RunBusinessProcessRequest",
						"params": {
							"processRunType": "RegardlessOfThePage",
							"saveAtProcessStart": true,
							"showNotification": true
						}
					},
					"caption": "#ResourceString(CancelButton_caption)#",
					"color": "default",
					"size": "large",
					"iconPosition": "only-text",
					"clickMode": "default"
				}
			},
			{
				"operation": "remove",
				"name": "TagSelect"
			},
			{
				"operation": "merge",
				"name": "CardButtonToggleGroup",
				"values": {
					"badgeConfig": {
						"color": "warn",
						"offset": -4
					}
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
				"name": "CardToggleTabPanel",
				"values": {
					"styleType": "default",
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
				"name": "Feed",
				"values": {
					"dataSourceName": "PDS",
					"entitySchemaName": "ConfActivityLog"
				}
			},
			{
				"operation": "merge",
				"name": "AttachmentsTabContainer",
				"values": {
					"badge": "$AttachmentsTabContainer_Badge_Value"
				}
			},
			{
				"operation": "merge",
				"name": "AttachmentList",
				"values": {
					"columns": [
						{
							"id": "b2e52605-920b-0c08-673e-057b8a240aa5",
							"code": "AttachmentListDS_Name",
							"caption": "#ResourceString(AttachmentListDS_Name)#",
							"dataValueType": 28
						},
						{
							"id": "e68f2dc3-6321-133b-0387-ee53c8445c47",
							"code": "AttachmentListDS_CreatedOn",
							"caption": "#ResourceString(AttachmentListDS_CreatedOn)#",
							"dataValueType": 7
						},
						{
							"id": "33d12d8e-7432-3c8a-d188-9b234cbfea30",
							"code": "AttachmentListDS_CreatedBy",
							"caption": "#ResourceString(AttachmentListDS_CreatedBy)#",
							"dataValueType": 10
						},
						{
							"id": "4bd80f01-28d1-7d56-6be2-2cc496bf85f3",
							"code": "AttachmentListDS_Size",
							"caption": "#ResourceString(AttachmentListDS_Size)#",
							"dataValueType": 4
						}
					]
				}
			},
			{
				"operation": "insert",
				"name": "SideAreaProfileFlexContainer",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.FlexContainer",
					"direction": "column",
					"items": [],
					"fitContent": true
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "OperationLookup",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_Operation_0azj2ry",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_Operation_0azj2ry",
					"visible": true,
					"readonly": true,
					"placeholder": ""
				},
				"parentName": "SideAreaProfileFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "AddOperationAction",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(AddOperationActionCaption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "OperationLookup",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ParentOperationLookup",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_Parent_pe43nkw",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_Parent_pe43nkw",
					"visible": true,
					"readonly": true,
					"placeholder": ""
				},
				"parentName": "SideAreaProfileFlexContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "AddParentOperationAction",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(AddParentOperationActionCaption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "ParentOperationLookup",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ElementInput",
				"values": {
					"type": "crt.Input",
					"label": "$Resources.Strings.PDS_Element_91740r6",
					"control": "$PDS_Element_91740r6",
					"placeholder": "",
					"tooltip": "",
					"readonly": true,
					"multiline": false,
					"labelPosition": "auto",
					"visible": true
				},
				"parentName": "SideAreaProfileFlexContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "TypeLookup",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_Type_3kx7h8q",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_Type_3kx7h8q",
					"visible": true,
					"readonly": true,
					"placeholder": ""
				},
				"parentName": "SideAreaProfileFlexContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "AddTypeAction",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(AddTypeActionCaption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "TypeLookup",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NumberInput",
				"values": {
					"type": "crt.Input",
					"label": "$Resources.Strings.PDS_Number_ghurdcp",
					"control": "$PDS_Number_ghurdcp",
					"placeholder": "",
					"tooltip": "",
					"readonly": true,
					"multiline": false,
					"labelPosition": "auto",
					"visible": true
				},
				"parentName": "SideAreaProfileFlexContainer",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "ApplicationInput",
				"values": {
					"type": "crt.Input",
					"label": "$Resources.Strings.PDS_ApplicationCode_p6sayp2",
					"control": "$PDS_ApplicationCode_p6sayp2",
					"placeholder": "#ResourceString(ApplicationInput_placeholder)#",
					"tooltip": "",
					"readonly": true,
					"multiline": false,
					"labelPosition": "auto",
					"visible": true
				},
				"parentName": "SideAreaProfileFlexContainer",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "PackageNameInput",
				"values": {
					"type": "crt.Input",
					"label": "$Resources.Strings.PDS_PackageName_0n5x60v",
					"control": "$PDS_PackageName_0n5x60v",
					"placeholder": "#ResourceString(PackageNameInput_placeholder)#",
					"tooltip": "",
					"readonly": true,
					"multiline": false,
					"labelPosition": "auto",
					"visible": true
				},
				"parentName": "SideAreaProfileFlexContainer",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "CreatedByLookup",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_CreatedBy_vlftpvp",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_CreatedBy_vlftpvp"
				},
				"parentName": "SideAreaProfileFlexContainer",
				"propertyName": "items",
				"index": 7
			},
			{
				"operation": "insert",
				"name": "AddCreatedByAction",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(AddCreatedByActionCaption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "CreatedByLookup",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "CreatedOnDateTimePicker",
				"values": {
					"type": "crt.DateTimePicker",
					"label": "$Resources.Strings.PDS_CreatedOn_k3gt0jl",
					"placeholder": "",
					"readonly": false,
					"labelPosition": "auto",
					"tooltip": "",
					"pickerType": "datetime",
					"control": "$PDS_CreatedOn_k3gt0jl"
				},
				"parentName": "SideAreaProfileFlexContainer",
				"propertyName": "items",
				"index": 8
			},
			{
				"operation": "insert",
				"name": "StatusLookup",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_Status_xn7b10l",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_Status_xn7b10l",
					"visible": true,
					"readonly": true,
					"placeholder": ""
				},
				"parentName": "SideAreaProfileFlexContainer",
				"propertyName": "items",
				"index": 9
			},
			{
				"operation": "insert",
				"name": "AddStatusAction",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(AddStatusActionCaption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "StatusLookup",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "DescriptionRichTextEditor",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 1,
						"rowSpan": 6
					},
					"type": "crt.RichTextEditor",
					"label": "$Resources.Strings.PDS_Description_dxn58xg",
					"control": "$PDS_Description_dxn58xg",
					"placeholder": "",
					"tooltip": "",
					"readonly": true,
					"multiline": true,
					"labelPosition": "above",
					"visible": true,
					"filesStorage": {
						"masterRecordColumnValue": "$Id",
						"entitySchemaName": "SysFile",
						"recordColumnName": "RecordId"
					},
					"toolbarDisplayMode": null
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedOperationsTab",
				"values": {
					"type": "crt.TabContainer",
					"items": [],
					"caption": "#ResourceString(RelatedOperationsTabCaption)#",
					"iconPosition": "only-text",
					"visible": true
				},
				"parentName": "Tabs",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsContainer",
				"values": {
					"type": "crt.GridContainer",
					"items": [],
					"rows": "minmax(32px, max-content)",
					"columns": [
						"minmax(32px, 1fr)",
						"minmax(32px, 1fr)"
					],
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
				},
				"parentName": "RelatedOperationsTab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsHeaderContainer",
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
					"padding": {
						"top": "small",
						"bottom": "small",
						"right": "medium",
						"left": "extra-small"
					},
					"color": "primary",
					"borderRadius": "none",
					"visible": true,
					"alignItems": "stretch",
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedChildOperationsContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsLabel",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(RelatedChildOperationsLabel_caption)#)#",
					"labelType": "headline-2",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "RelatedChildOperationsHeaderContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsFlexContainer",
				"values": {
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.FlexContainer",
					"direction": "row",
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
					"alignItems": "center",
					"justifyContent": "end",
					"gap": "small",
					"wrap": "wrap"
				},
				"parentName": "RelatedChildOperationsHeaderContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RefreshRelatedChildOperationsButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(RefreshRelatedChildOperationsButtonCaption)#",
					"color": "default",
					"disabled": false,
					"size": "medium",
					"iconPosition": "only-icon",
					"visible": true,
					"clicked": {
						"request": "crt.LoadDataRequest",
						"params": {
							"config": {
								"loadType": "reload"
							},
							"refreshDataConfig": {
								"mode": "RefreshAll"
							}
						}
					},
					"clickMode": "default",
					"icon": "reload-icon"
				},
				"parentName": "RelatedChildOperationsFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsSearchFilter",
				"values": {
					"type": "crt.SearchFilter",
					"placeholder": "#ResourceString(RelatedChildOperationsSearchFilter_placeholder)#",
					"_filterOptions": {
						"expose": [
							{
								"attribute": "RelatedChildOperationsSearchFilter_RelatedChildOperationsItems",
								"converters": [
									{
										"converter": "crt.SearchFilterAttributeConverter",
										"args": [
											"RelatedChildOperationsItems"
										]
									}
								]
							}
						],
						"from": [
							"RelatedChildOperationsSearchFilter_SearchValue",
							"RelatedChildOperationsSearchFilter_FilteredColumnsGroups"
						]
					},
					"iconOnly": true
				},
				"parentName": "RelatedChildOperationsFlexContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsGrid",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 2,
						"rowSpan": 19
					},
					"type": "crt.DataGrid",
					"features": {
						"rows": {
							"selection": false,
							"numeration": false,
							"toolbar": false
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						}
					},
					"items": "$RelatedChildOperationsItems",
					"primaryColumnName": "RelatedChildOperationsItemsDS_Id",
					"columns": [
						{
							"id": "57f223b9-fc5f-43ff-352f-c7b2dc9a5f8d",
							"code": "RelatedChildOperationsItemsDS_Number",
							"caption": "#ResourceString(RelatedChildOperationsItemsDS_Number)#",
							"dataValueType": 27,
							"width": 121
						},
						{
							"id": "63f5a07e-07d9-e070-cfbc-c198042ae66c",
							"code": "RelatedChildOperationsItemsDS_CreatedOn",
							"caption": "#ResourceString(RelatedChildOperationsItemsDS_CreatedOn)#",
							"dataValueType": 7,
							"width": 167
						},
						{
							"id": "a0f01e11-b153-2907-7ebe-5d18e2991cd9",
							"code": "RelatedChildOperationsItemsDS_Operation",
							"caption": "#ResourceString(RelatedChildOperationsItemsDS_Operation)#",
							"dataValueType": 10,
							"width": 168
						},
						{
							"id": "11b089d8-deed-14ca-d509-e3b4e3e71530",
							"code": "RelatedChildOperationsItemsDS_Parent",
							"caption": "#ResourceString(RelatedChildOperationsGrid_columns_3_caption)#",
							"dataValueType": 10,
							"width": 188
						},
						{
							"id": "bf942afa-d7bc-7b2e-a76a-f6d5531bd274",
							"code": "RelatedChildOperationsItemsDS_Element",
							"caption": "#ResourceString(RelatedChildOperationsItemsDS_Element)#",
							"dataValueType": 28,
							"width": 163
						},
						{
							"id": "5c58c2ad-9b7c-9a86-3b71-2d46f102d503",
							"code": "RelatedChildOperationsItemsDS_Type",
							"caption": "#ResourceString(RelatedChildOperationsItemsDS_Type)#",
							"dataValueType": 10,
							"width": 106
						},
						{
							"id": "69b63019-3497-e653-2981-429565389d3a",
							"code": "RelatedChildOperationsItemsDS_CreatedBy",
							"caption": "#ResourceString(RelatedChildOperationsItemsDS_CreatedBy)#",
							"dataValueType": 10,
							"width": 173
						},
						{
							"id": "1b0a42f5-54b0-fa45-3762-6257889123dd",
							"code": "RelatedChildOperationsItemsDS_ApplicationCode",
							"caption": "#ResourceString(RelatedChildOperationsItemsDS_ApplicationCode)#",
							"dataValueType": 28,
							"width": 173
						},
						{
							"id": "befb6bf6-f362-32ca-fac5-4dec2996d5b1",
							"code": "RelatedChildOperationsItemsDS_PackageName",
							"caption": "#ResourceString(RelatedChildOperationsItemsDS_PackageName)#",
							"dataValueType": 28,
							"width": 131
						},
						{
							"id": "88439016-0fd5-8220-7965-fa926985159c",
							"code": "RelatedChildOperationsItemsDS_Status",
							"caption": "#ResourceString(RelatedChildOperationsItemsDS_Status)#",
							"dataValueType": 10,
							"width": 107
						}
					],
					"placeholder": null,
					"activeRow": "$RelatedChildOperationsItems_ActiveRow",
					"selectionState": "$RelatedChildOperationsItems_SelectionState",
					"_selectionOptions": {
						"attribute": "RelatedChildOperationsItems_SelectionState"
					},
					"bulkActions": [],
					"visible": true,
					"fitContent": true
				},
				"parentName": "RelatedChildOperationsContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsItems_AddTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Add tag",
					"icon": "tag-icon",
					"clicked": {
						"request": "crt.AddTagsInRecordsRequest",
						"params": {
							"dataSourceName": "RelatedChildOperationsItemsDS",
							"filters": "$RelatedChildOperationsItems | crt.ToCollectionFilters : 'RelatedChildOperationsItems' : $RelatedChildOperationsItems_SelectionState | crt.SkipIfSelectionEmpty : $RelatedChildOperationsItems_SelectionState"
						}
					},
					"items": []
				},
				"parentName": "RelatedChildOperationsGrid",
				"propertyName": "bulkActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsItems_RemoveTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Remove tag",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.RemoveTagsInRecordsRequest",
						"params": {
							"dataSourceName": "RelatedChildOperationsItemsDS",
							"filters": "$RelatedChildOperationsItems | crt.ToCollectionFilters : 'RelatedChildOperationsItems' : $RelatedChildOperationsItems_SelectionState | crt.SkipIfSelectionEmpty : $RelatedChildOperationsItems_SelectionState"
						}
					}
				},
				"parentName": "RelatedChildOperationsItems_AddTagsBulkAction",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsItems_ExportToExcelBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Export to Excel",
					"icon": "export-button-icon",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "RelatedChildOperationsGrid",
							"filters": "$RelatedChildOperationsItems | crt.ToCollectionFilters : 'RelatedChildOperationsItems' : $RelatedChildOperationsItems_SelectionState | crt.SkipIfSelectionEmpty : $RelatedChildOperationsItems_SelectionState"
						}
					}
				},
				"parentName": "RelatedChildOperationsGrid",
				"propertyName": "bulkActions",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsItems_MergeBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Merge",
					"icon": "merge-icon",
					"clicked": {
						"request": "crt.MergeRecordsRequest",
						"params": {
							"dataSourceName": "RelatedChildOperationsItemsDS",
							"selectionState": "$RelatedChildOperationsItems_SelectionState"
						}
					}
				},
				"parentName": "RelatedChildOperationsGrid",
				"propertyName": "bulkActions",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "RelatedChildOperationsItems_DeleteBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Delete",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.DeleteRecordsRequest",
						"params": {
							"dataSourceName": "RelatedChildOperationsItemsDS",
							"filters": "$RelatedChildOperationsItems | crt.ToCollectionFilters : 'RelatedChildOperationsItems' : $RelatedChildOperationsItems_SelectionState | crt.SkipIfSelectionEmpty : $RelatedChildOperationsItems_SelectionState"
						}
					}
				},
				"parentName": "RelatedChildOperationsGrid",
				"propertyName": "bulkActions",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsContainer",
				"values": {
					"type": "crt.GridContainer",
					"items": [],
					"rows": "minmax(32px, max-content)",
					"columns": [
						"minmax(32px, 1fr)",
						"minmax(32px, 1fr)"
					],
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
				},
				"parentName": "RelatedOperationsTab",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsHeaderContainer",
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
					"padding": {
						"top": "small",
						"bottom": "small",
						"right": "medium",
						"left": "extra-small"
					},
					"color": "primary",
					"borderRadius": "none",
					"visible": true,
					"alignItems": "stretch",
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedParentOperationsContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsLabel",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(RelatedParentOperationsLabel_caption)#)#",
					"labelType": "headline-2",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "RelatedParentOperationsHeaderContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsFlexContainer",
				"values": {
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.FlexContainer",
					"direction": "row",
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
					"alignItems": "center",
					"justifyContent": "end",
					"gap": "small",
					"wrap": "wrap"
				},
				"parentName": "RelatedParentOperationsHeaderContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RefreshRelatedParentOperationsButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(RefreshRelatedParentOperationsButtonCaption)#",
					"color": "default",
					"disabled": false,
					"size": "medium",
					"iconPosition": "only-icon",
					"visible": true,
					"clicked": {
						"request": "crt.LoadDataRequest",
						"params": {
							"config": {
								"loadType": "reload"
							},
							"refreshDataConfig": {
								"mode": "RefreshAll"
							}
						}
					},
					"clickMode": "default",
					"icon": "reload-icon"
				},
				"parentName": "RelatedParentOperationsFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsSearchFilter",
				"values": {
					"type": "crt.SearchFilter",
					"placeholder": "#ResourceString(RelatedParentOperationsSearchFilterPlaceholder)#",
					"_filterOptions": {
						"expose": [
							{
								"attribute": "RelatedParentOperationsSearchFilter_RelatedParentOperationsItems",
								"converters": [
									{
										"converter": "crt.SearchFilterAttributeConverter",
										"args": [
											"RelatedParentOperationsItems"
										]
									}
								]
							}
						],
						"from": [
							"RelatedParentOperationsSearchFilter_SearchValue",
							"RelatedParentOperationsSearchFilter_FilteredColumnsGroups"
						]
					},
					"iconOnly": true
				},
				"parentName": "RelatedParentOperationsFlexContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsGrid",
				"values": {
					"type": "crt.DataGrid",
					"features": {
						"rows": {
							"selection": false,
							"numeration": false,
							"toolbar": false
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						}
					},
					"items": "$RelatedParentOperationsItems",
					"primaryColumnName": "RelatedParentOperationsItemsDS_Id",
					"columns": [
						{
							"id": "39e57312-0aa0-bc2d-e780-cb09f542f818",
							"code": "RelatedParentOperationsItemsDS_Number",
							"caption": "#ResourceString(RelatedParentOperationsItemsDS_Number)#",
							"dataValueType": 27,
							"width": 121
						},
						{
							"id": "4c14cbd9-1aa8-6b78-b83a-5d20b14de42c",
							"code": "RelatedParentOperationsItemsDS_CreatedOn",
							"caption": "#ResourceString(RelatedParentOperationsItemsDS_CreatedOn)#",
							"dataValueType": 7,
							"width": 167
						},
						{
							"id": "dc53ec75-22e4-baeb-c83a-c24f00f3021c",
							"code": "RelatedParentOperationsItemsDS_Operation",
							"caption": "#ResourceString(RelatedParentOperationsItemsDS_Operation)#",
							"dataValueType": 10,
							"width": 168
						},
						{
							"id": "93512fc8-2470-5b24-0caf-e23ff56c8eb2",
							"code": "RelatedParentOperationsItemsDS_Parent",
							"caption": "#ResourceString(RelatedParentOperationsItemsDS_Parent)#",
							"dataValueType": 10,
							"width": 188
						},
						{
							"id": "06ec9945-08e8-c37a-dbee-cb22255d7d60",
							"code": "RelatedParentOperationsItemsDS_Element",
							"caption": "#ResourceString(RelatedParentOperationsItemsDS_Element)#",
							"dataValueType": 28,
							"width": 163
						},
						{
							"id": "4accc3ed-0fe9-c013-5d62-703ea23c1bb0",
							"code": "RelatedParentOperationsItemsDS_Type",
							"caption": "#ResourceString(RelatedParentOperationsItemsDS_Type)#",
							"dataValueType": 10,
							"width": 106
						},
						{
							"id": "b558826f-12a2-f07f-98a4-b767fb990493",
							"code": "RelatedParentOperationsItemsDS_CreatedBy",
							"caption": "#ResourceString(RelatedParentOperationsItemsDS_CreatedBy)#",
							"dataValueType": 10,
							"width": 173
						},
						{
							"id": "511b8952-36bc-830c-7444-783e8fe2579d",
							"code": "RelatedParentOperationsItemsDS_ApplicationCode",
							"caption": "#ResourceString(RelatedParentOperationsItemsDS_ApplicationCode)#",
							"dataValueType": 28,
							"width": 135
						},
						{
							"id": "12411c2c-fe6b-fe56-30ee-46a22483a930",
							"code": "RelatedParentOperationsItemsDS_PackageName",
							"caption": "#ResourceString(RelatedParentOperationsItemsDS_PackageName)#",
							"dataValueType": 28,
							"width": 147
						},
						{
							"id": "64a2c808-64a5-87fb-6698-f5da7167e764",
							"code": "RelatedParentOperationsItemsDS_Status",
							"caption": "#ResourceString(RelatedParentOperationsItemsDS_Status)#",
							"dataValueType": 10,
							"width": 107
						}
					],
					"activeRow": "$RelatedParentOperationsItems_ActiveRow",
					"selectionState": "$RelatedParentOperationsItems_SelectionState",
					"_selectionOptions": {
						"attribute": "RelatedParentOperationsItems_SelectionState"
					},
					"bulkActions": [],
					"visible": true,
					"fitContent": true,
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 2,
						"rowSpan": 20
					}
				},
				"parentName": "RelatedParentOperationsContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsItems_AddTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Add tag",
					"icon": "tag-icon",
					"clicked": {
						"request": "crt.AddTagsInRecordsRequest",
						"params": {
							"dataSourceName": "RelatedParentOperationsItemsDS",
							"filters": "$RelatedParentOperationsItems | crt.ToCollectionFilters : 'RelatedParentOperationsItems' : $RelatedParentOperationsItems_SelectionState | crt.SkipIfSelectionEmpty : $RelatedParentOperationsItems_SelectionState"
						}
					},
					"items": []
				},
				"parentName": "RelatedParentOperationsGrid",
				"propertyName": "bulkActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsItems_RemoveTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Remove tag",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.RemoveTagsInRecordsRequest",
						"params": {
							"dataSourceName": "RelatedParentOperationsItemsDS",
							"filters": "$RelatedParentOperationsItems | crt.ToCollectionFilters : 'RelatedParentOperationsItems' : $RelatedParentOperationsItems_SelectionState | crt.SkipIfSelectionEmpty : $RelatedParentOperationsItems_SelectionState"
						}
					}
				},
				"parentName": "RelatedParentOperationsItems_AddTagsBulkAction",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsItems_ExportToExcelBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Export to Excel",
					"icon": "export-button-icon",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "RelatedParentOperationsGrid",
							"filters": "$RelatedParentOperationsItems | crt.ToCollectionFilters : 'RelatedParentOperationsItems' : $RelatedParentOperationsItems_SelectionState | crt.SkipIfSelectionEmpty : $RelatedParentOperationsItems_SelectionState"
						}
					}
				},
				"parentName": "RelatedParentOperationsGrid",
				"propertyName": "bulkActions",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsItems_MergeBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Merge",
					"icon": "merge-icon",
					"clicked": {
						"request": "crt.MergeRecordsRequest",
						"params": {
							"dataSourceName": "RelatedParentOperationsItemsDS",
							"selectionState": "$RelatedParentOperationsItems_SelectionState"
						}
					}
				},
				"parentName": "RelatedParentOperationsGrid",
				"propertyName": "bulkActions",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "RelatedParentOperationsItems_DeleteBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Delete",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.DeleteRecordsRequest",
						"params": {
							"dataSourceName": "RelatedParentOperationsItemsDS",
							"filters": "$RelatedParentOperationsItems | crt.ToCollectionFilters : 'RelatedParentOperationsItems' : $RelatedParentOperationsItems_SelectionState | crt.SkipIfSelectionEmpty : $RelatedParentOperationsItems_SelectionState"
						}
					}
				},
				"parentName": "RelatedParentOperationsGrid",
				"propertyName": "bulkActions",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "ChangeDetailsTab",
				"values": {
					"type": "crt.TabContainer",
					"items": [],
					"caption": "#ResourceString(ChangeDetailsTab_caption)#",
					"iconPosition": "only-text",
					"visible": false
				},
				"parentName": "Tabs",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "IFrame_ChangeDetails",
				"values": {
					"type": "crt.IFrame",
					"isSandbox": false,
					"sandbox": null,
					"htmlContent": "$ChangedFilesHtml",
					"stretch": true
				},
				"parentName": "ChangeDetailsTab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "CompilationHistoryTab",
				"values": {
					"type": "crt.TabContainer",
					"items": [],
					"caption": "#ResourceString(CompilationHistoryTabCaption)#",
					"iconPosition": "only-text",
					"visible": false
				},
				"parentName": "Tabs",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "CompilationHistoryContainer",
				"values": {
					"type": "crt.GridContainer",
					"items": [],
					"rows": "minmax(32px, max-content)",
					"columns": [
						"minmax(32px, 1fr)",
						"minmax(32px, 1fr)"
					],
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
				},
				"parentName": "CompilationHistoryTab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "CompilationHistoryHeaderContainer",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 1,
						"rowSpan": 1
					},
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
					"padding": {
						"top": "small",
						"bottom": "small",
						"right": "medium",
						"left": "extra-small"
					},
					"color": "primary",
					"borderRadius": "none",
					"visible": true,
					"alignItems": "stretch"
				},
				"parentName": "CompilationHistoryContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "CompilationHistoryLabel",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(CompilationHistoryLabelCaption)#)#",
					"labelType": "headline-2",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "CompilationHistoryHeaderContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "CompilationHistoryFlexContainer",
				"values": {
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.FlexContainer",
					"direction": "row",
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
					"alignItems": "center",
					"justifyContent": "end",
					"gap": "small",
					"wrap": "wrap"
				},
				"parentName": "CompilationHistoryHeaderContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ErrorsWarningsQuickFilter",
				"values": {
					"type": "crt.QuickFilter",
					"config": {
						"caption": "#ResourceString(ErrorsWarningsQuickFilterCaption)#",
						"hint": "",
						"defaultValue": false,
						"approachState": true,
						"icon": "settings-button-icon",
						"iconPosition": "left-icon"
					},
					"_filterOptions": {
						"expose": [
							{
								"attribute": "ErrorsWarningsQuickFilter_CompilationHistory",
								"converters": [
									{
										"converter": "crt.QuickFilterAttributeConverter",
										"args": [
											{
												"target": {
													"viewAttributeName": "CompilationHistory",
													"customFilter": {
														"items": {
															"cf696c87-5dfa-4307-bec8-ed29d9b33e19": {
																"filterType": 1,
																"comparisonType": 4,
																"isEnabled": true,
																"trimDateTimeParameterToDate": false,
																"leftExpression": {
																	"expressionType": 0,
																	"columnPath": "ErrorsWarnings"
																},
																"isAggregative": false,
																"dataValueType": 1,
																"rightExpression": {
																	"expressionType": 2,
																	"parameter": {
																		"dataValueType": 1,
																		"value": "[]"
																	}
																}
															}
														},
														"logicalOperation": 0,
														"isEnabled": true,
														"filterType": 6,
														"rootSchemaName": "CompilationHistory"
													},
													"dependencyFilters": null
												},
												"quickFilterType": "custom",
												"config": {
													"approachState": true
												}
											}
										]
									}
								]
							}
						],
						"from": [
							"ErrorsWarningsQuickFilter_Value"
						]
					},
					"filterType": "custom"
				},
				"parentName": "CompilationHistoryFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "CompilationHistorySummaries",
				"values": {
					"type": "crt.Summaries",
					"items": [],
					"visible": true,
					"_designOptions": {
						"modelName": "CompilationHistoryDS"
					},
					"expanded": "$CompilationHistorySummaries_Expanded"
				},
				"parentName": "CompilationHistoryFlexContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "TotalDurationSummaryItem",
				"values": {
					"type": "crt.SummaryItem",
					"label": "#ResourceString(TotalDurationSummaryItemLabel)#",
					"_designOptions": {
						"value": {
							"attribute": "TotalDurationSummaryItemValue",
							"modelName": "CompilationHistoryDS",
							"expression": {
								"function": "sum",
								"columns": [
									{
										"type": "Column",
										"path": "DurationInSeconds"
									}
								],
								"resultDataValueType": 4
							}
						}
					}
				},
				"parentName": "CompilationHistorySummaries",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RefreshCompilationHistoryButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(RefreshCompilationHistoryButtonCaption)#",
					"color": "default",
					"disabled": false,
					"size": "medium",
					"iconPosition": "only-icon",
					"visible": true,
					"clicked": {
						"request": "crt.LoadDataRequest",
						"params": {
							"config": {
								"loadType": "reload"
							},
							"refreshDataConfig": {
								"mode": "RefreshSpecific",
								"targetDataSourceNames": [
									"CompilationHistoryDS"
								]
							}
						}
					},
					"clickMode": "default",
					"icon": "reload-icon"
				},
				"parentName": "CompilationHistoryFlexContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "CompilationHistorySearchFilter",
				"values": {
					"type": "crt.SearchFilter",
					"placeholder": "#ResourceString(CompilationHistorySearchFilter_placeholder)#",
					"_filterOptions": {
						"expose": [
							{
								"attribute": "CompilationHistorySearchFilter_CompilationHistory",
								"converters": [
									{
										"converter": "crt.SearchFilterAttributeConverter",
										"args": [
											"CompilationHistory"
										]
									}
								]
							}
						],
						"from": [
							"CompilationHistorySearchFilter_SearchValue",
							"CompilationHistorySearchFilter_FilteredColumnsGroups"
						]
					},
					"iconOnly": true
				},
				"parentName": "CompilationHistoryFlexContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "CompilationHistoryGrid",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 2,
						"rowSpan": 17
					},
					"type": "crt.DataGrid",
					"features": {
						"rows": {
							"selection": false,
							"numeration": false,
							"toolbar": false
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						}
					},
					"items": "$CompilationHistory",
					"primaryColumnName": "CompilationHistoryDS_Id",
					"columns": [
						{
							"id": "5f035b08-1c0b-12b6-225a-3add4cc9ab0d",
							"code": "CompilationHistoryDS_ProjectName",
							"caption": "#ResourceString(CompilationHistoryDS_ProjectName)#",
							"dataValueType": 30,
							"width": 347
						},
						{
							"id": "d306d231-8a39-6a98-5b3f-24d0f059c84c",
							"code": "CompilationHistoryDS_ErrorsWarnings",
							"caption": "#ResourceString(CompilationHistoryDS_ErrorsWarnings)#",
							"dataValueType": 29,
							"width": 396
						},
						{
							"id": "bc489b8e-68b6-dcb2-383b-6629a8efa435",
							"code": "CompilationHistoryDS_Result",
							"caption": "#ResourceString(CompilationHistoryGridColumnsCaption)#",
							"dataValueType": 12,
							"width": 121
						},
						{
							"id": "14aab55e-6d3c-0c55-b597-402dc214e91c",
							"code": "CompilationHistoryDS_DurationInSeconds",
							"caption": "#ResourceString(CompilationHistoryDS_DurationInSeconds)#",
							"dataValueType": 4,
							"width": 190
						},
						{
							"id": "7d48e74c-f8c5-f62a-1d11-815bacf6fcba",
							"code": "CompilationHistoryDS_StartedBy",
							"caption": "#ResourceString(CompilationHistoryDS_StartedBy)#",
							"dataValueType": 10
						},
						{
							"id": "44cedc08-20ec-0730-6a07-034859df7631",
							"code": "CompilationHistoryDS_CreatedOn",
							"caption": "#ResourceString(CompilationHistoryDS_CreatedOn)#",
							"dataValueType": 7,
							"width": 186
						}
					],
					"placeholder": false,
					"visible": true,
					"fitContent": true,
					"activeRow": "$CompilationHistory_ActiveRow",
					"selectionState": "$CompilationHistory_SelectionState",
					"_selectionOptions": {
						"attribute": "CompilationHistory_SelectionState"
					},
					"bulkActions": []
				},
				"parentName": "CompilationHistoryContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "CompilationHistory_AddTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Add tag",
					"icon": "tag-icon",
					"clicked": {
						"request": "crt.AddTagsInRecordsRequest",
						"params": {
							"dataSourceName": "CompilationHistoryDS",
							"filters": "$CompilationHistory | crt.ToCollectionFilters : 'CompilationHistory' : $CompilationHistory_SelectionState | crt.SkipIfSelectionEmpty : $CompilationHistory_SelectionState"
						}
					},
					"items": []
				},
				"parentName": "CompilationHistoryGrid",
				"propertyName": "bulkActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "CompilationHistory_RemoveTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Remove tag",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.RemoveTagsInRecordsRequest",
						"params": {
							"dataSourceName": "CompilationHistoryDS",
							"filters": "$CompilationHistory | crt.ToCollectionFilters : 'CompilationHistory' : $CompilationHistory_SelectionState | crt.SkipIfSelectionEmpty : $CompilationHistory_SelectionState"
						}
					}
				},
				"parentName": "CompilationHistory_AddTagsBulkAction",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "CompilationHistory_ExportToExcelBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Export to Excel",
					"icon": "export-button-icon",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "CompilationHistoryGrid",
							"filters": "$CompilationHistory | crt.ToCollectionFilters : 'CompilationHistory' : $CompilationHistory_SelectionState | crt.SkipIfSelectionEmpty : $CompilationHistory_SelectionState"
						}
					}
				},
				"parentName": "CompilationHistoryGrid",
				"propertyName": "bulkActions",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "CompilationHistory_MergeBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Merge",
					"icon": "merge-icon",
					"clicked": {
						"request": "crt.MergeRecordsRequest",
						"params": {
							"dataSourceName": "CompilationHistoryDS",
							"selectionState": "$CompilationHistory_SelectionState"
						}
					}
				},
				"parentName": "CompilationHistoryGrid",
				"propertyName": "bulkActions",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "CompilationHistory_DeleteBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Delete",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.DeleteRecordsRequest",
						"params": {
							"dataSourceName": "CompilationHistoryDS",
							"filters": "$CompilationHistory | crt.ToCollectionFilters : 'CompilationHistory' : $CompilationHistory_SelectionState | crt.SkipIfSelectionEmpty : $CompilationHistory_SelectionState"
						}
					}
				},
				"parentName": "CompilationHistoryGrid",
				"propertyName": "bulkActions",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "LocalizationDetailsTab",
				"values": {
					"type": "crt.TabContainer",
					"items": [],
					"caption": "#ResourceString(LocalizationDetailsTab_caption)#",
					"iconPosition": "only-text",
					"visible": false
				},
				"parentName": "Tabs",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "LocalizationDetailsContainer",
				"values": {
					"type": "crt.GridContainer",
					"items": [],
					"rows": "minmax(32px, max-content)",
					"columns": [
						"minmax(32px, 1fr)",
						"minmax(32px, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": "none"
					},
					"visible": true,
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"color": "transparent",
					"borderRadius": "none",
					"alignItems": "stretch"
				},
				"parentName": "LocalizationDetailsTab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "LocalizationDetailsHeaderContainer",
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
					"alignItems": "stretch",
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "small",
						"right": "medium",
						"bottom": "medium",
						"left": "extra-small"
					},
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "LocalizationDetailsContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "LocalizationChangeLogLabel",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(LocalizationChangeLogLabel_caption)#)#",
					"labelType": "headline-2",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "LocalizationDetailsHeaderContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "SearchFilterFlexContainer",
				"values": {
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.FlexContainer",
					"direction": "row",
					"wrap": "wrap",
					"items": [],
					"fitContent": true,
					"visible": true,
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"color": "transparent",
					"borderRadius": "none",
					"alignItems": "stretch",
					"justifyContent": "end",
					"gap": "small"
				},
				"parentName": "LocalizationDetailsHeaderContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "LocalizationChangeLogSearchFilter",
				"values": {
					"type": "crt.SearchFilter",
					"placeholder": "#ResourceString(LocalizationChangeLogSearchFilter_placeholder)#",
					"_filterOptions": {
						"expose": [
							{
								"attribute": "LocalizationChangeLogSearchFilter_DataGrid_wb37191",
								"converters": [
									{
										"converter": "crt.SearchFilterAttributeConverter",
										"args": [
											"DataGrid_wb37191"
										]
									}
								]
							}
						],
						"from": [
							"LocalizationChangeLogSearchFilter_SearchValue",
							"LocalizationChangeLogSearchFilter_FilteredColumnsGroups"
						]
					},
					"iconOnly": true
				},
				"parentName": "SearchFilterFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "LocalizationChangeLogGrid",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 2,
						"rowSpan": 15
					},
					"type": "crt.DataGrid",
					"features": {
						"rows": {
							"selection": false,
							"numeration": false,
							"toolbar": false
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						}
					},
					"items": "$DataGrid_wb37191",
					"activeRow": "$DataGrid_wb37191_ActiveRow",
					"selectionState": "$DataGrid_wb37191_SelectionState",
					"_selectionOptions": {
						"attribute": "DataGrid_wb37191_SelectionState"
					},
					"primaryColumnName": "DataGrid_wb37191DS_Id",
					"columns": [
						{
							"id": "3c133224-e172-ad20-ee39-5e77cfe4de63",
							"code": "DataGrid_wb37191DS_Key",
							"caption": "#ResourceString(DataGrid_wb37191DS_Key)#",
							"dataValueType": 29,
							"width": 203
						},
						{
							"id": "22a0b879-135c-b619-9b30-e232ce6086de",
							"code": "DataGrid_wb37191DS_OldValue",
							"caption": "#ResourceString(DataGrid_wb37191DS_OldValue)#",
							"dataValueType": 29,
							"width": 269
						},
						{
							"id": "a291f4ec-5df5-eff9-06c7-cf82909ac314",
							"code": "DataGrid_wb37191DS_NewValue",
							"caption": "#ResourceString(DataGrid_wb37191DS_NewValue)#",
							"dataValueType": 29,
							"width": 328
						},
						{
							"id": "07774bf4-1efe-40dd-301b-627c22942a2d",
							"code": "DataGrid_wb37191DS_ModifiedBy",
							"caption": "#ResourceString(DataGrid_wb37191DS_ModifiedBy)#",
							"dataValueType": 10
						},
						{
							"id": "96807405-01ac-7b8c-64c4-3b047345a9b7",
							"code": "DataGrid_wb37191DS_ModifiedOn",
							"caption": "#ResourceString(DataGrid_wb37191DS_ModifiedOn)#",
							"dataValueType": 7
						}
					],
					"placeholder": false,
					"bulkActions": [],
					"visible": true,
					"fitContent": true
				},
				"parentName": "LocalizationDetailsContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "DataGrid_wb37191_AddTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Add tag",
					"icon": "tag-icon",
					"clicked": {
						"request": "crt.AddTagsInRecordsRequest",
						"params": {
							"dataSourceName": "DataGrid_wb37191DS",
							"filters": "$DataGrid_wb37191 | crt.ToCollectionFilters : 'DataGrid_wb37191' : $DataGrid_wb37191_SelectionState | crt.SkipIfSelectionEmpty : $DataGrid_wb37191_SelectionState"
						}
					},
					"items": []
				},
				"parentName": "LocalizationChangeLogGrid",
				"propertyName": "bulkActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "DataGrid_wb37191_RemoveTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Remove tag",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.RemoveTagsInRecordsRequest",
						"params": {
							"dataSourceName": "DataGrid_wb37191DS",
							"filters": "$DataGrid_wb37191 | crt.ToCollectionFilters : 'DataGrid_wb37191' : $DataGrid_wb37191_SelectionState | crt.SkipIfSelectionEmpty : $DataGrid_wb37191_SelectionState"
						}
					}
				},
				"parentName": "DataGrid_wb37191_AddTagsBulkAction",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "DataGrid_wb37191_ExportToExcelBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Export to Excel",
					"icon": "export-button-icon",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "LocalizationChangeLogGrid",
							"filters": "$DataGrid_wb37191 | crt.ToCollectionFilters : 'DataGrid_wb37191' : $DataGrid_wb37191_SelectionState | crt.SkipIfSelectionEmpty : $DataGrid_wb37191_SelectionState"
						}
					}
				},
				"parentName": "LocalizationChangeLogGrid",
				"propertyName": "bulkActions",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "DataGrid_wb37191_MergeBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Merge",
					"icon": "merge-icon",
					"clicked": {
						"request": "crt.MergeRecordsRequest",
						"params": {
							"dataSourceName": "DataGrid_wb37191DS",
							"selectionState": "$DataGrid_wb37191_SelectionState"
						}
					}
				},
				"parentName": "LocalizationChangeLogGrid",
				"propertyName": "bulkActions",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "DataGrid_wb37191_DeleteBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Delete",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.DeleteRecordsRequest",
						"params": {
							"dataSourceName": "DataGrid_wb37191DS",
							"filters": "$DataGrid_wb37191 | crt.ToCollectionFilters : 'DataGrid_wb37191' : $DataGrid_wb37191_SelectionState | crt.SkipIfSelectionEmpty : $DataGrid_wb37191_SelectionState"
						}
					}
				},
				"parentName": "LocalizationChangeLogGrid",
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
					"ChangedFilesHtml": {},
					"PDS_Operation_0azj2ry": {
						"modelConfig": {
							"path": "PDS.Operation"
						}
					},
					"PDS_Operation_0azj2ry_List": {
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
					"PDS_Parent_pe43nkw": {
						"modelConfig": {
							"path": "PDS.Parent"
						}
					},
					"PDS_Parent_pe43nkw_List": {
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
					"PDS_Element_91740r6": {
						"modelConfig": {
							"path": "PDS.Element"
						}
					},
					"PDS_Type_3kx7h8q": {
						"modelConfig": {
							"path": "PDS.Type"
						}
					},
					"PDS_Type_3kx7h8q_List": {
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
					"PDS_Number_ghurdcp": {
						"modelConfig": {
							"path": "PDS.Number"
						}
					},
					"PDS_CreatedBy_vlftpvp": {
						"modelConfig": {
							"path": "PDS.CreatedBy"
						}
					},
					"PDS_CreatedBy_vlftpvp_List": {
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
					"PDS_CreatedOn_k3gt0jl": {
						"modelConfig": {
							"path": "PDS.CreatedOn"
						}
					},
					"PDS_Status_xn7b10l": {
						"modelConfig": {
							"path": "PDS.Status"
						}
					},
					"PDS_Status_xn7b10l_List": {
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
					"PDS_Description_dxn58xg": {
						"modelConfig": {
							"path": "PDS.Description"
						}
					},
					"RelatedChildOperationsItems": {
						"isCollection": true,
						"modelConfig": {
							"path": "RelatedChildOperationsItemsDS",
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
									"name": "RelatedChildOperationsSearchFilter_RelatedChildOperationsItems",
									"loadOnChange": true
								}
							]
						},
						"viewModelConfig": {
							"attributes": {
								"RelatedChildOperationsItemsDS_Number": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.Number"
									}
								},
								"RelatedChildOperationsItemsDS_CreatedOn": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.CreatedOn"
									}
								},
								"RelatedChildOperationsItemsDS_Operation": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.Operation"
									}
								},
								"RelatedChildOperationsItemsDS_Parent": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.Parent"
									}
								},
								"RelatedChildOperationsItemsDS_Element": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.Element"
									}
								},
								"RelatedChildOperationsItemsDS_Type": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.Type"
									}
								},
								"RelatedChildOperationsItemsDS_CreatedBy": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.CreatedBy"
									}
								},
								"RelatedChildOperationsItemsDS_ApplicationCode": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.ApplicationCode"
									}
								},
								"RelatedChildOperationsItemsDS_PackageName": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.PackageName"
									}
								},
								"RelatedChildOperationsItemsDS_Status": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.Status"
									}
								},
								"RelatedChildOperationsItemsDS_Id": {
									"modelConfig": {
										"path": "RelatedChildOperationsItemsDS.Id"
									}
								}
							}
						}
					},
					"RelatedParentOperationsItems": {
						"isCollection": true,
						"modelConfig": {
							"path": "RelatedParentOperationsItemsDS",
							"filterAttributes": [
								{
									"name": "RelatedParentOperationsSearchFilter_RelatedParentOperationsItems",
									"loadOnChange": true
								}
							]
						},
						"viewModelConfig": {
							"attributes": {
								"RelatedParentOperationsItemsDS_Number": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.Number"
									}
								},
								"RelatedParentOperationsItemsDS_CreatedOn": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.CreatedOn"
									}
								},
								"RelatedParentOperationsItemsDS_Operation": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.Operation"
									}
								},
								"RelatedParentOperationsItemsDS_OperationName": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.Operation.Name"
									}
								},
								"RelatedParentOperationsItemsDS_Parent": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.Parent"
									}
								},
								"RelatedParentOperationsItemsDS_Element": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.Element"
									}
								},
								"RelatedParentOperationsItemsDS_Type": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.Type"
									}
								},
								"RelatedParentOperationsItemsDS_CreatedBy": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.CreatedBy"
									}
								},
								"RelatedParentOperationsItemsDS_ApplicationCode": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.ApplicationCode"
									}
								},
								"RelatedParentOperationsItemsDS_PackageName": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.PackageName"
									}
								},
								"RelatedParentOperationsItemsDS_Status": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.Status"
									}
								},
								"RelatedParentOperationsItemsDS_Id": {
									"modelConfig": {
										"path": "RelatedParentOperationsItemsDS.Id"
									}
								}
							}
						}
					},
					"PDS_ApplicationCode_p6sayp2": {
						"modelConfig": {
							"path": "PDS.ApplicationCode"
						}
					},
					"PDS_PackageName_0n5x60v": {
						"modelConfig": {
							"path": "PDS.PackageName"
						}
					},
					"CompilationHistory": {
						"isCollection": true,
						"modelConfig": {
							"path": "CompilationHistoryDS",
							"filterAttributes": [
								{
									"loadOnChange": true,
									"name": "CompilationHistory_PredefinedFilter"
								},
								{
									"name": "CompilationHistorySearchFilter_CompilationHistory",
									"loadOnChange": true
								},
								{
									"name": "ErrorsWarningsQuickFilter_CompilationHistory",
									"loadOnChange": true
								}
							]
						},
						"viewModelConfig": {
							"attributes": {
								"CompilationHistoryDS_ProjectName": {
									"modelConfig": {
										"path": "CompilationHistoryDS.ProjectName"
									}
								},
								"CompilationHistoryDS_ErrorsWarnings": {
									"modelConfig": {
										"path": "CompilationHistoryDS.ErrorsWarnings"
									}
								},
								"CompilationHistoryDS_Result": {
									"modelConfig": {
										"path": "CompilationHistoryDS.Result"
									}
								},
								"CompilationHistoryDS_DurationInSeconds": {
									"modelConfig": {
										"path": "CompilationHistoryDS.DurationInSeconds"
									}
								},
								"CompilationHistoryDS_StartedBy": {
									"modelConfig": {
										"path": "CompilationHistoryDS.StartedBy"
									}
								},
								"CompilationHistoryDS_CreatedOn": {
									"modelConfig": {
										"path": "CompilationHistoryDS.CreatedOn"
									}
								},
								"CompilationHistoryDS_Id": {
									"modelConfig": {
										"path": "CompilationHistoryDS.Id"
									}
								}
							}
						}
					},
					"CompilationHistory_PredefinedFilter": {
						"value": null
					},
					"CompilationHistorySummaries_Expanded": {
						"value": true
					},
					"AttachmentsTabContainer_Badge_VisibleProp": {
						"from": "AttachmentsFiles",
						"converter": "crt.ToObjectProp:'length' |crt.ToBoolean| crt.ToKeyValuePair :'visible'"
					},
					"AttachmentsTabContainer_Badge_Value": {
						"from": [
							"AttachmentsTabContainer_Badge_VisibleProp"
						],
						"converter": "crt.ToObjectFromEntries"
					},
					"DataGrid_wb37191": {
						"isCollection": true,
						"modelConfig": {
							"path": "DataGrid_wb37191DS",
							"filterAttributes": [
								{
									"name": "LocalizationChangeLogSearchFilter_DataGrid_wb37191",
									"loadOnChange": true
								}
							],
							"sortingConfig": {
								"default": [
									{
										"direction": "asc",
										"columnName": "NewValue"
									}
								]
							}
						},
						"viewModelConfig": {
							"attributes": {
								"DataGrid_wb37191DS_Key": {
									"modelConfig": {
										"path": "DataGrid_wb37191DS.Key"
									}
								},
								"DataGrid_wb37191DS_OldValue": {
									"modelConfig": {
										"path": "DataGrid_wb37191DS.OldValue"
									}
								},
								"DataGrid_wb37191DS_NewValue": {
									"modelConfig": {
										"path": "DataGrid_wb37191DS.NewValue"
									}
								},
								"DataGrid_wb37191DS_ModifiedBy": {
									"modelConfig": {
										"path": "DataGrid_wb37191DS.ModifiedBy"
									}
								},
								"DataGrid_wb37191DS_ModifiedOn": {
									"modelConfig": {
										"path": "DataGrid_wb37191DS.ModifiedOn"
									}
								},
								"DataGrid_wb37191DS_Id": {
									"modelConfig": {
										"path": "DataGrid_wb37191DS.Id"
									}
								}
							}
						}
					}
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
						"RelatedChildOperationsItemsDS": [
							{
								"attributePath": "Parent.Id",
								"relationPath": "PDS.Id"
							}
						],
						"RelatedParentOperationsItemsDS": [
							{
								"attributePath": "Id",
								"relationPath": "PDS.Parent"
							}
						],
						"CompilationHistoryDS": [
							{
								"attributePath": "ConfActivityLog",
								"relationPath": "PDS.Id"
							}
						],
						"DataGrid_wb37191DS": [
							{
								"attributePath": "ConfActivityLog",
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
							"entitySchemaName": "ConfActivityLog"
						},
						"scope": "page"
					},
					"RelatedChildOperationsItemsDS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "ConfActivityLog",
							"attributes": {
								"Number": {
									"path": "Number"
								},
								"CreatedOn": {
									"path": "CreatedOn"
								},
								"Operation": {
									"path": "Operation"
								},
								"Parent": {
									"path": "Parent"
								},
								"Element": {
									"path": "Element"
								},
								"Type": {
									"path": "Type"
								},
								"CreatedBy": {
									"path": "CreatedBy"
								},
								"ApplicationCode": {
									"path": "ApplicationCode"
								},
								"PackageName": {
									"path": "PackageName"
								},
								"Status": {
									"path": "Status"
								}
							}
						}
					},
					"RelatedParentOperationsItemsDS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "ConfActivityLog",
							"attributes": {
								"Number": {
									"path": "Number"
								},
								"CreatedOn": {
									"path": "CreatedOn"
								},
								"Operation": {
									"path": "Operation"
								},
								"Parent": {
									"path": "Parent"
								},
								"Element": {
									"path": "Element"
								},
								"Type": {
									"path": "Type"
								},
								"CreatedBy": {
									"path": "CreatedBy"
								},
								"ApplicationCode": {
									"path": "ApplicationCode"
								},
								"PackageName": {
									"path": "PackageName"
								},
								"Status": {
									"path": "Status"
								}
							}
						}
					},
					"CompilationHistoryDS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "CompilationHistory",
							"attributes": {
								"ProjectName": {
									"path": "ProjectName"
								},
								"ErrorsWarnings": {
									"path": "ErrorsWarnings"
								},
								"Result": {
									"path": "Result"
								},
								"DurationInSeconds": {
									"path": "DurationInSeconds"
								},
								"StartedBy": {
									"path": "StartedBy"
								},
								"CreatedOn": {
									"path": "CreatedOn"
								}
							}
						}
					},
					"DataGrid_wb37191DS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "LocalizationChangeLog",
							"attributes": {
								"Key": {
									"path": "Key"
								},
								"OldValue": {
									"path": "OldValue"
								},
								"NewValue": {
									"path": "NewValue"
								},
								"ModifiedBy": {
									"path": "ModifiedBy"
								},
								"ModifiedOn": {
									"path": "ModifiedOn"
								}
							}
						}
					}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[
			{
				request: "crt.HandleViewModelInitRequest",
				handler: async (request, next) => {
					await next?.handle(request);
					const logRecordId = await request.$context.Id;
					const fileModel = await sdk.Model.create("SysFile");
					const files = await fileModel.load({
						attributes: ["Id"],
						parameters: [{
							type: sdk.ModelParameterType.ColumnValue,
							value: {
								attributePath: "RecordSchemaName",
								attributeValue: "ConfActivityLog"
							}
						},
						{
							type: sdk.ModelParameterType.ColumnValue,
							value: {
								attributePath: "RecordId",
								attributeValue: logRecordId
							}
						}]
					});
					request.$context.setValue({ "AttachmentsFiles": files});
				}
			},
			{
				request: "crt.HandleViewModelInitRequest",
				handler: async (request, next) => {
					function getBasePath(pathname, isFramework) {
						if (isFramework) {
							const zeroIndex = pathname.indexOf("/0/");
							return zeroIndex > 0 ? pathname.substring(0, zeroIndex) : "";
						}
						const shellIndex = pathname.indexOf("/Shell");
						return shellIndex > 0 ? pathname.substring(0, shellIndex) : "";
					}

					function getChangedFilesUrl() {
						const pathname = window.location.pathname;
						const isFramework = pathname.includes("/0/");
						const serviceModelPath = isFramework ? "/0/ServiceModel" : "/ServiceModel";
						return `${getBasePath(pathname, isFramework)}${serviceModelPath}/ConfigurationActivityLogService.svc/ChangedFiles`;
					}

					function getCookieValue(name) {
						const cookies = document.cookie ? document.cookie.split(";") : [];
						for (let index = 0; index < cookies.length; index++) {
							const cookie = cookies[index].trim();
							const prefix = `${name}=`;
							if (cookie.indexOf(prefix) !== 0) {
								continue;
							}
							const value = cookie.substring(prefix.length);
							try {
								return decodeURIComponent(value);
							} catch (error) {
								return value;
							}
						}
						return "";
					}

					function getChangedFilesRequestHeaders() {
						const headers = {
							"Accept": "application/json, text/plain, */*",
							"Content-Type": "application/json"
						};
						const csrfToken = getCookieValue("CRT_CSRF");
						if (csrfToken) {
							headers.crt_csrf = csrfToken;
						}
						return headers;
					}

					function getChangedFilesErrorMessage(payload, response) {
						const errorInfo = payload && (payload.errorInfo || payload.ErrorInfo);
						if (errorInfo && (errorInfo.message || errorInfo.Message)) {
							return errorInfo.message || errorInfo.Message;
						}
						return `Changed files request failed (${response.status}).`;
					}

					async function readChangedFilesResponse(response) {
						const text = await response.text();
						let payload = null;
						if (text) {
							try {
								payload = JSON.parse(text);
							} catch (error) {
								if (response.ok) {
									throw new Error("Unable to parse changed files response.");
								}
							}
						}
						if (!response.ok) {
							throw new Error(getChangedFilesErrorMessage(payload, response));
						}
						return payload || {};
					}

					async function loadChangedFilesHtml(logRecordId) {
						if (!logRecordId) {
							return diffBuilder.buildErrorHtml("Invalid logEntryId.");
						}
						const response = await fetch(getChangedFilesUrl(), {
							method: "POST",
							headers: getChangedFilesRequestHeaders(),
							credentials: "same-origin",
							body: JSON.stringify({
								logEntryId: logRecordId
							})
						});
						const payload = await readChangedFilesResponse(response);
						return diffBuilder.buildHtml(payload, {
							currentLogEntryId: logRecordId
						});
					}

					await next?.handle(request);
					const logRecordId = await request.$context.Id;
					let changedFilesHtml;
					try {
						changedFilesHtml = await loadChangedFilesHtml(logRecordId);
					} catch (error) {
						changedFilesHtml = diffBuilder.buildErrorHtml(error && error.message || String(error));
					}
					await request.$context.set("ChangedFilesHtml", changedFilesHtml);
				}
			}
		]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});