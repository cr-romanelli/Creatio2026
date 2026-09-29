define("TrnCRealty_FormPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
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
				"name": "GeneralInfoTabContainer",
				"values": {
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
				}
			},
			{
				"operation": "merge",
				"name": "Feed",
				"values": {
					"dataSourceName": "PDS",
					"entitySchemaName": "TrnCRealty"
				}
			},
			{
				"operation": "merge",
				"name": "AttachmentList",
				"values": {
					"columns": [
						{
							"id": "5dea0de4-bd15-48bc-a586-397c3e8d5b4f",
							"code": "AttachmentListDS_Name",
							"caption": "#ResourceString(AttachmentListDS_Name)#",
							"dataValueType": 28,
							"width": 200
						}
					]
				}
			},
			{
				"operation": "insert",
				"name": "PushMeButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(PushMeButton_caption)#",
					"color": "outline",
					"disabled": false,
					"size": "medium",
					"iconPosition": "left-icon",
					"visible": true,
					"clicked": {
						"request": "usr.PushButtonRequest"
					},
					"clickMode": "default",
					"icon": "copilot-rewrite-friendly-icon"
				},
				"parentName": "CardToolsContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Actions",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(Actions_caption)#",
					"color": "default",
					"disabled": false,
					"size": "medium",
					"iconPosition": "left-icon",
					"visible": true,
					"icon": "actions-button-icon",
					"menuItems": [],
					"clickMode": "menu"
				},
				"parentName": "CardToggleContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RunCalcAVGProcess",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(RunCalcAVGProcess_caption)#",
					"visible": true,
					"clicked": {
						"request": "crt.RunBusinessProcessRequest",
						"params": {
							"processName": "TrnCCalcAverageRealtyPriceProcess",
							"processRunType": "ForTheSelectedPage",
							"saveAtProcessStart": true,
							"showNotification": true,
							"recordIdProcessParameterName": "RealtyIdParameter"
						}
					}
				},
				"parentName": "Actions",
				"propertyName": "menuItems",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "TrnCName",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 1,
						"colSpan": 1,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.TrnCName",
					"control": "$TrnCName",
					"labelPosition": "auto",
					"multiline": false
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Price",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"type": "crt.NumberInput",
					"label": "$Resources.Strings.PDS_TrnCPrice_i9yu4io",
					"control": "$PDS_TrnCPrice_i9yu4io",
					"readonly": true,
					"placeholder": "",
					"labelPosition": "auto",
					"tooltip": "",
					"visible": true
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "NumberInput_rv0v61j",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					},
					"type": "crt.NumberInput",
					"label": "$Resources.Strings.PDS_TrnCColumn9_hc3vedn",
					"control": "$PDS_TrnCColumn9_hc3vedn",
					"readonly": false,
					"placeholder": "",
					"labelPosition": "auto",
					"tooltip": ""
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "Commission",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 4,
						"rowSpan": 1
					},
					"type": "crt.NumberInput",
					"label": "$Resources.Strings.PDS_TrnCCommission_a4o5h20",
					"control": "$PDS_TrnCCommission_a4o5h20",
					"readonly": true,
					"placeholder": "",
					"labelPosition": "auto",
					"tooltip": "",
					"visible": true
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "Number",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 5,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.PDS_TrnCNumber_sl52trc",
					"control": "$PDS_TrnCNumber_sl52trc",
					"placeholder": "",
					"tooltip": "",
					"readonly": true,
					"multiline": false,
					"labelPosition": "auto",
					"visible": true
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "Type",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_TrnCType_3fo66p2",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_TrnCType_3fo66p2",
					"visible": true,
					"readonly": false,
					"placeholder": "",
					"valueDetails": null
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "addRecord_nnwrl1g",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(addRecord_nnwrl1g_caption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "Type",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "OfferType",
				"values": {
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_TrnCOfferType_civ8j2j",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_TrnCOfferType_civ8j2j",
					"valueDetails": null,
					"visible": true,
					"readonly": false,
					"placeholder": ""
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Comment",
				"values": {
					"type": "crt.RichTextEditor",
					"label": "$Resources.Strings.PDS_TrnCComment_tpha2od",
					"control": "$PDS_TrnCComment_tpha2od",
					"labelPosition": "auto",
					"placeholder": "",
					"tooltip": "",
					"needHandleSave": true,
					"filesStorage": {
						"masterRecordColumnValue": "$Id",
						"entitySchemaName": "SysFile",
						"recordColumnName": "RecordId"
					},
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"visible": false,
					"readonly": false,
					"toolbarDisplayMode": null
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "Manager",
				"values": {
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_TrnCManager_rf086l9",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_TrnCManager_rf086l9",
					"visible": true,
					"readonly": false,
					"placeholder": "",
					"valueDetails": null
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "Country",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_TrnCCountry_zfnx63s",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_TrnCCountry_zfnx63s",
					"visible": true,
					"readonly": false,
					"placeholder": "",
					"valueDetails": null
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "addRecord_mvvr9ja",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(addRecord_mvvr9ja_caption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "Country",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "CommissionType",
				"values": {
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					},
					"type": "crt.NumberInput",
					"label": "$Resources.Strings.PDS_TrnCOfferTypeTrnCCommissionPercent_ny8zdj4",
					"control": "$PDS_TrnCOfferTypeTrnCCommissionPercent_ny8zdj4",
					"readonly": true,
					"placeholder": "",
					"labelPosition": "auto",
					"tooltip": ""
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "City",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_TrnCCity_zghad1j",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "auto",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_TrnCCity_zghad1j",
					"valueDetails": null,
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 4,
						"rowSpan": 1
					}
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "addRecord_dpe87y5",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(addRecord_dpe87y5_caption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "City",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ExpansionPanel_gt8xh42",
				"values": {
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"title": "#ResourceString(ExpansionPanel_gt8xh42_title)#",
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
					"fitContent": true
				},
				"parentName": "GeneralInfoTab",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridContainer_0acirew",
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
				"parentName": "ExpansionPanel_gt8xh42",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_ofsr7d6",
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
				"parentName": "GridContainer_0acirew",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailAddBtn_focmm1a",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailAddBtn_focmm1a_caption)#",
					"icon": "add-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.CreateRecordRequest",
						"params": {
							"entityName": "TrnCRealtyVisit",
							"defaultValues": [
								{
									"attributeName": "TrnCParentRealty",
									"value": "$Id"
								}
							]
						}
					},
					"visible": true,
					"clickMode": "default"
				},
				"parentName": "FlexContainer_ofsr7d6",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailRefreshBtn_jtusbv6",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailRefreshBtn_jtusbv6_caption)#",
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
							"dataSourceName": "GridDetail_f1sahn6DS"
						}
					}
				},
				"parentName": "FlexContainer_ofsr7d6",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridDetailSettingsBtn_7vlf3g5",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailSettingsBtn_7vlf3g5_caption)#",
					"icon": "actions-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clickMode": "menu",
					"menuItems": []
				},
				"parentName": "FlexContainer_ofsr7d6",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "GridDetailExportDataBtn_4qtqde1",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(GridDetailExportDataBtn_4qtqde1_caption)#",
					"icon": "export-button-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "VisitGridDetail"
						}
					}
				},
				"parentName": "GridDetailSettingsBtn_7vlf3g5",
				"propertyName": "menuItems",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailImportDataBtn_ecy64kx",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(GridDetailImportDataBtn_ecy64kx_caption)#",
					"icon": "import-button-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.ImportDataRequest",
						"params": {
							"entitySchemaName": "TrnCRealtyVisit"
						}
					}
				},
				"parentName": "GridDetailSettingsBtn_7vlf3g5",
				"propertyName": "menuItems",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridDetailSearchFilter_kiwjoow",
				"values": {
					"type": "crt.SearchFilter",
					"placeholder": "#ResourceString(GridDetailSearchFilter_kiwjoow_placeholder)#",
					"iconOnly": true,
					"_filterOptions": {
						"expose": [
							{
								"attribute": "GridDetailSearchFilter_kiwjoow_GridDetail_f1sahn6",
								"converters": [
									{
										"converter": "crt.SearchFilterAttributeConverter",
										"args": [
											"GridDetail_f1sahn6"
										]
									}
								]
							}
						],
						"from": [
							"GridDetailSearchFilter_kiwjoow_SearchValue",
							"GridDetailSearchFilter_kiwjoow_FilteredColumnsGroups"
						]
					}
				},
				"parentName": "FlexContainer_ofsr7d6",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "GridContainer_grzm9lr",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(max-content, 32px)",
					"columns": [
						"minmax(32px, 1fr)",
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
				"parentName": "ExpansionPanel_gt8xh42",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "VisitGridDetail",
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
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						}
					},
					"items": "$GridDetail_f1sahn6",
					"activeRow": "$GridDetail_f1sahn6_ActiveRow",
					"selectionState": "$GridDetail_f1sahn6_SelectionState",
					"_selectionOptions": {
						"attribute": "GridDetail_f1sahn6_SelectionState"
					},
					"primaryColumnName": "GridDetail_f1sahn6DS_Id",
					"columns": [
						{
							"id": "b35e6d7a-285c-cb76-b339-5e08a1879b70",
							"code": "GridDetail_f1sahn6DS_TrnCVisitDateTime",
							"caption": "#ResourceString(GridDetail_f1sahn6DS_TrnCVisitDateTime)#",
							"dataValueType": 7
						},
						{
							"id": "ea0819aa-78ba-4458-1161-b414a9d9dd58",
							"code": "GridDetail_f1sahn6DS_TrnCManager",
							"caption": "#ResourceString(GridDetail_f1sahn6DS_TrnCManager)#",
							"dataValueType": 10
						},
						{
							"id": "36c25482-c507-d918-291e-4633a1362f40",
							"code": "GridDetail_f1sahn6DS_TrnCComment",
							"caption": "#ResourceString(GridDetail_f1sahn6DS_TrnCComment)#",
							"dataValueType": 28,
							"width": 164
						},
						{
							"id": "abf481e8-69e3-066d-eb46-d4868b6ddc0c",
							"code": "GridDetail_f1sahn6DS_TrnCPotentialCustomer",
							"caption": "#ResourceString(GridDetail_f1sahn6DS_TrnCPotentialCustomer)#",
							"dataValueType": 10,
							"width": 165
						},
						{
							"id": "c0ad6b86-eec2-49af-3de5-2ef84202ce0d",
							"code": "GridDetail_f1sahn6DS_CreatedOn",
							"caption": "#ResourceString(GridDetail_f1sahn6DS_CreatedOn)#",
							"dataValueType": 7
						}
					],
					"placeholder": false,
					"bulkActions": [],
					"visible": true,
					"fitContent": true
				},
				"parentName": "GridContainer_grzm9lr",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetail_f1sahn6_AddTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Add tag",
					"icon": "tag-icon",
					"clicked": {
						"request": "crt.AddTagsInRecordsRequest",
						"params": {
							"dataSourceName": "GridDetail_f1sahn6DS",
							"filters": "$GridDetail_f1sahn6 | crt.ToCollectionFilters : 'GridDetail_f1sahn6' : $GridDetail_f1sahn6_SelectionState | crt.SkipIfSelectionEmpty : $GridDetail_f1sahn6_SelectionState"
						}
					},
					"items": []
				},
				"parentName": "VisitGridDetail",
				"propertyName": "bulkActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetail_f1sahn6_RemoveTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Remove tag",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.RemoveTagsInRecordsRequest",
						"params": {
							"dataSourceName": "GridDetail_f1sahn6DS",
							"filters": "$GridDetail_f1sahn6 | crt.ToCollectionFilters : 'GridDetail_f1sahn6' : $GridDetail_f1sahn6_SelectionState | crt.SkipIfSelectionEmpty : $GridDetail_f1sahn6_SelectionState"
						}
					}
				},
				"parentName": "GridDetail_f1sahn6_AddTagsBulkAction",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetail_f1sahn6_ExportToExcelBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Export to Excel",
					"icon": "export-button-icon",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "VisitGridDetail",
							"filters": "$GridDetail_f1sahn6 | crt.ToCollectionFilters : 'GridDetail_f1sahn6' : $GridDetail_f1sahn6_SelectionState | crt.SkipIfSelectionEmpty : $GridDetail_f1sahn6_SelectionState"
						}
					}
				},
				"parentName": "VisitGridDetail",
				"propertyName": "bulkActions",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridDetail_f1sahn6_MergeBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Merge",
					"icon": "merge-icon",
					"clicked": {
						"request": "crt.MergeRecordsRequest",
						"params": {
							"dataSourceName": "GridDetail_f1sahn6DS",
							"selectionState": "$GridDetail_f1sahn6_SelectionState"
						}
					}
				},
				"parentName": "VisitGridDetail",
				"propertyName": "bulkActions",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "GridDetail_f1sahn6_DeleteBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Delete",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.DeleteRecordsRequest",
						"params": {
							"dataSourceName": "GridDetail_f1sahn6DS",
							"filters": "$GridDetail_f1sahn6 | crt.ToCollectionFilters : 'GridDetail_f1sahn6' : $GridDetail_f1sahn6_SelectionState | crt.SkipIfSelectionEmpty : $GridDetail_f1sahn6_SelectionState"
						}
					}
				},
				"parentName": "VisitGridDetail",
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
					"TrnCName": {
						"modelConfig": {
							"path": "PDS.TrnCName"
						}
					},
					"PDS_TrnCPrice_i9yu4io": {
						"modelConfig": {
							"path": "PDS.TrnCPrice"
						},
						"validators": {
							"MySuperValidator": {
								"type": "usr.DGValidator",
								"params": {
									"minValue": 50,
									"message": "#ResourceString(PriceCannotBeLess)#"
								}
							}
						}
					},
					"PDS_TrnCColumn9_hc3vedn": {
						"modelConfig": {
							"path": "PDS.TrnCColumn9"
						},
						"validators": {
							"MySuperValidator": {
								"type": "usr.DGValidator",
								"params": {
									"minValue": 10,
									"message": "#ResourceString(AreaCannotBeLess)#"
								}
							}
						}
					},
					"PDS_TrnCOfferType_civ8j2j": {
						"modelConfig": {
							"path": "PDS.TrnCOfferType"
						}
					},
					"PDS_TrnCOfferType_civ8j2j_List": {
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
					"PDS_TrnCType_3fo66p2": {
						"modelConfig": {
							"path": "PDS.TrnCType"
						}
					},
					"PDS_TrnCType_3fo66p2_List": {
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
					"PDS_TrnCComment_tpha2od": {
						"modelConfig": {
							"path": "PDS.TrnCComment"
						}
					},
					"PDS_TrnCManager_rf086l9": {
						"modelConfig": {
							"path": "PDS.TrnCManager"
						}
					},
					"PDS_TrnCManager_rf086l9_List": {
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
					"PDS_TrnCNumber_sl52trc": {
						"modelConfig": {
							"path": "PDS.TrnCNumber"
						}
					},
					"PDS_TrnCCountry_zfnx63s": {
						"modelConfig": {
							"path": "PDS.TrnCCountry"
						}
					},
					"PDS_TrnCCountry_zfnx63s_List": {
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
					"PDS_TrnCCity_zghad1j": {
						"modelConfig": {
							"path": "PDS.TrnCCity"
						}
					},
					"PDS_TrnCCity_zghad1j_List": {
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
					"PDS_TrnCCommission_a4o5h20": {
						"modelConfig": {
							"path": "PDS.TrnCCommission"
						}
					},
					"PDS_TrnCOfferTypeTrnCCommissionPercent_ny8zdj4": {
						"modelConfig": {
							"path": "PDS.TrnCOfferTypeTrnCCommissionPercent_ny8zdj4"
						}
					},
					"GridDetail_f1sahn6": {
						"isCollection": true,
						"modelConfig": {
							"path": "GridDetail_f1sahn6DS",
							"filterAttributes": [
								{
									"name": "GridDetailSearchFilter_kiwjoow_GridDetail_f1sahn6",
									"loadOnChange": true
								}
							],
							"sortingConfig": {
								"default": [
									{
										"direction": "asc",
										"columnName": "TrnCComment"
									}
								]
							}
						},
						"viewModelConfig": {
							"attributes": {
								"GridDetail_f1sahn6DS_TrnCVisitDateTime": {
									"modelConfig": {
										"path": "GridDetail_f1sahn6DS.TrnCVisitDateTime"
									}
								},
								"GridDetail_f1sahn6DS_TrnCManager": {
									"modelConfig": {
										"path": "GridDetail_f1sahn6DS.TrnCManager"
									}
								},
								"GridDetail_f1sahn6DS_TrnCComment": {
									"modelConfig": {
										"path": "GridDetail_f1sahn6DS.TrnCComment"
									}
								},
								"GridDetail_f1sahn6DS_TrnCPotentialCustomer": {
									"modelConfig": {
										"path": "GridDetail_f1sahn6DS.TrnCPotentialCustomer"
									}
								},
								"GridDetail_f1sahn6DS_CreatedOn": {
									"modelConfig": {
										"path": "GridDetail_f1sahn6DS.CreatedOn"
									}
								},
								"GridDetail_f1sahn6DS_Id": {
									"modelConfig": {
										"path": "GridDetail_f1sahn6DS.Id"
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
						"GridDetail_f1sahn6DS": [
							{
								"attributePath": "TrnCParentRealty",
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
							"entitySchemaName": "TrnCRealty",
							"attributes": {
								"TrnCOfferTypeTrnCCommissionPercent_ny8zdj4": {
									"path": "TrnCOfferType.TrnCCommissionPercent",
									"type": "ForwardReference"
								}
							}
						},
						"scope": "page"
					},
					"GridDetail_f1sahn6DS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "TrnCRealtyVisit",
							"attributes": {
								"TrnCVisitDateTime": {
									"path": "TrnCVisitDateTime"
								},
								"TrnCManager": {
									"path": "TrnCManager"
								},
								"TrnCComment": {
									"path": "TrnCComment"
								},
								"TrnCPotentialCustomer": {
									"path": "TrnCPotentialCustomer"
								},
								"CreatedOn": {
									"path": "CreatedOn"
								}
							}
						}
					}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[
			{
				request: "usr.PushButtonRequest",
				/* Implementation of the custom query handler. */
				handler: async (request, next) => {
					console.log("Button works...");
					Terrasoft.showInformation("My button was pressed.");
					var price = await request.$context.PDS_TrnCPrice_i9yu4io;
					console.log("Price = " + price);
					request.$context.PDS_TrnCColumn9_hc3vedn = price * 0.2;
					/* Call the next handler if it exists and return its result. */
					return next?.handle(request);
				}
			},
			{
				request: "crt.HandleViewModelAttributeChangeRequest",
				/* The custom implementation of the system query handler. */
				handler: async (request, next) => {
      					if (request.attributeName === 'PDS_TrnCPrice_i9yu4io' || 				             // if price changed
					   request.attributeName === 'PDS_TrnCOfferTypeTrnCCommissionPercent_ny8zdj4' ) { 		// or percent changed
							// debugger;
						var price = await request.$context.PDS_TrnCPrice_i9yu4io;
						var percent = await request.$context.PDS_TrnCOfferTypeTrnCCommissionPercent_ny8zdj4;
						var commission = price * percent / 100;
						request.$context.PDS_TrnCCommission_a4o5h20 = commission;
					}
					/* Call the next handler if it exists and return its result. */
					return next?.handle(request);
				}
			},
		]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{
			/* The validator type must contain a vendor prefix.
			Format the validator type in PascalCase. */
			"usr.DGValidator": {
				validator: function (config) {
					return function (control) {
						let value = control.value;
						let minValue = config.minValue;
						let valueIsCorrect = value >= minValue;
						var result;
						if (valueIsCorrect) {
							result = null;
						} else {
							result = {
								"usr.DGValidator": { 
									message: config.message
								}
							};
						}
						return result;
					};
				},
				params: [
					{
						name: "minValue"
					},
					{
						name: "message"
					}
				],
				async: false
			},
			
		}/**SCHEMA_VALIDATORS*/
	};
});