define("IdentityProvidersManagement_Page", /**SCHEMA_DEPS*/["@creatio-devkit/common", "RightUtilities"]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/(sdk, RightUtilities)/**SCHEMA_ARGS*/ {
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
				"operation": "remove",
				"name": "SetRecordRightsButton"
			},
			{
				"operation": "remove",
				"name": "MainHeaderBottom"
			},
			{
				"operation": "remove",
				"name": "CardToolsContainer"
			},
			{
				"operation": "remove",
				"name": "TagSelect"
			},
			{
				"operation": "remove",
				"name": "CardToggleContainer"
			},
			{
				"operation": "merge",
				"name": "MainContainer",
				"values": {
					"direction": "row",
					"padding": {
						"left": "small",
						"right": "small",
						"top": "none",
						"bottom": "none"
					},
					"visible": true,
					"color": "transparent",
					"borderRadius": "none",
					"alignItems": "stretch",
					"justifyContent": "start",
					"gap": "small",
					"wrap": "wrap"
				}
			},
			{
				"operation": "remove",
				"name": "TopAreaProfileContainer"
			},
			{
				"operation": "remove",
				"name": "Tabs",
				"properties": [
					"layoutConfig"
				]
			},
			{
				"operation": "move",
				"name": "Tabs",
				"parentName": "FlexContainer_ukruz6b",
				"propertyName": "items",
				"index": 0
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
					"iconPosition": "only-text",
					"visible": true
				}
			},
			{
				"operation": "merge",
				"name": "GridContainer_uxln7d4",
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
				"operation": "insert",
				"name": "FlexContainer_wzkxbmm",
				"values": {
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
				"parentName": "MainHeader",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ButtonToggleGroup_ynfrdod",
				"values": {
					"for": "TabPanel_qdfzqly",
					"fitContent": true,
					"toggleViewMode": "button",
					"type": "crt.ButtonToggleGroup"
				},
				"parentName": "FlexContainer_wzkxbmm",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_6agkpsz",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"wrap": "nowrap",
					"items": [],
					"fitContent": false,
					"visible": true,
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"styles": {
						"height": "100%",
						"min-height": "100%"
					},
					"color": "transparent",
					"borderRadius": "none",
					"alignItems": "stretch",
					"justifyContent": "start",
					"gap": "small"
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_ukruz6b",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "column",
					"wrap": "nowrap",
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
					"justifyContent": "start",
					"gap": "small"
				},
				"parentName": "FlexContainer_6agkpsz",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ExpansionPanel_v0knqpd",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 1,
						"rowSpan": 3
					},
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"title": "#ResourceString(ExpansionPanel_v0knqpd_title)#",
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
				"parentName": "GridContainer_uxln7d4",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridContainer_lmwazvz",
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
				"parentName": "ExpansionPanel_v0knqpd",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_8tvsk5v",
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
				"parentName": "GridContainer_lmwazvz",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailAddBtn_pfzpzfw",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailAddBtn_pfzpzfw_caption)#",
					"icon": "add-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "usr.OpenIdentityProviderSettingsModalRequest"
					}
				},
				"parentName": "FlexContainer_8tvsk5v",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailRefreshBtn_4hojsus",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailRefreshBtn_4hojsus_caption)#",
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
							"dataSourceName": "GridDetail_l6id9tzDS"
						}
					}
				},
				"parentName": "FlexContainer_8tvsk5v",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridDetailSettingsBtn_9urdxx1",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailSettingsBtn_9urdxx1_caption)#",
					"icon": "actions-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clickMode": "menu",
					"menuItems": []
				},
				"parentName": "FlexContainer_8tvsk5v",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "GridDetailExportDataBtn_sdgp61i",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(GridDetailExportDataBtn_sdgp61i_caption)#",
					"icon": "export-button-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "GridDetail_l6id9tz"
						}
					}
				},
				"parentName": "GridDetailSettingsBtn_9urdxx1",
				"propertyName": "menuItems",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailImportDataBtn_wpb5ohg",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(GridDetailImportDataBtn_wpb5ohg_caption)#",
					"icon": "import-button-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.ImportDataRequest",
						"params": {
							"entitySchemaName": "SysIdentityProvider"
						}
					}
				},
				"parentName": "GridDetailSettingsBtn_9urdxx1",
				"propertyName": "menuItems",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridDetailSearchFilter_73otd7j",
				"values": {
					"type": "crt.SearchFilter",
					"placeholder": "#ResourceString(GridDetailSearchFilter_73otd7j_placeholder)#",
					"iconOnly": true,
					"_filterOptions": {
						"expose": [
							{
								"attribute": "GridDetailSearchFilter_73otd7j_GridDetail_l6id9tz",
								"converters": [
									{
										"converter": "crt.SearchFilterAttributeConverter",
										"args": [
											"GridDetail_l6id9tz"
										]
									}
								]
							}
						],
						"from": [
							"GridDetailSearchFilter_73otd7j_SearchValue",
							"GridDetailSearchFilter_73otd7j_FilteredColumnsGroups"
						]
					}
				},
				"parentName": "FlexContainer_8tvsk5v",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "Label_o6g2892",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(Label_o6g2892_caption)#)#",
					"labelType": "body",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "#757575",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "ExpansionPanel_v0knqpd",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridContainer_qpjittf",
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
				"parentName": "ExpansionPanel_v0knqpd",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridDetail_l6id9tz",
				"values": {
					"type": "crt.DataGrid",
					"layoutConfig": {
						"colSpan": 2,
						"column": 1,
						"row": 1,
						"rowSpan": 10
					},
					"features": {
						"rows": {
							"selection": {
								"enable": true,
								"multiple": true
							}
						}
					},
					"items": "$GridDetail_l6id9tz",
					"primaryColumnName": "GridDetail_l6id9tzDS_Id",
					"columns": [
						{
							"id": "854b42bb-8b72-6716-d16a-1b1c8e6a099b",
							"code": "GridDetail_l6id9tzDS_Name",
							"caption": "#ResourceString(GridDetail_l6id9tzDS_Name)#",
							"dataValueType": 28
						},
						{
							"id": "ac64f89b-01e3-5893-efb6-9304a9b92e1b",
							"code": "GridDetail_l6id9tzDS_Description",
							"caption": "#ResourceString(GridDetail_l6id9tzDS_Description)#",
							"dataValueType": 28
						},
						{
							"id": "aeebb9a9-0506-a5ac-0fb3-ef5acbf76095",
							"code": "GridDetail_l6id9tzDS_ServerUrl",
							"caption": "#ResourceString(GridDetail_l6id9tzDS_ServerUrl)#",
							"dataValueType": 30
						},
						{
							"id": "d01d1e2a-1e73-1564-b61c-e628cbe5694b",
							"code": "GridDetail_l6id9tzDS_ClientId",
							"caption": "#ResourceString(GridDetail_l6id9tzDS_ClientId)#",
							"dataValueType": 28
						},
						{
							"id": "556a51d5-c5e4-4d1c-7519-7a00e55e50a6",
							"code": "GridDetail_l6id9tzDS_ClientSecret",
							"caption": "#ResourceString(GridDetail_l6id9tzDS_ClientSecret)#",
							"dataValueType": 24
						}
					],
					"placeholder": false
				},
				"parentName": "GridContainer_qpjittf",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ExpansionPanel_2pz7pos",
				"values": {
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"title": "#ResourceString(ExpansionPanel_2pz7pos_title)#",
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
				"parentName": "GeneralInfoTab",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridContainer_udvkvjm",
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
				"parentName": "ExpansionPanel_2pz7pos",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_7evc4c9",
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
				"parentName": "GridContainer_udvkvjm",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridContainer_e1xst8h",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(max-content, 32px)",
					"columns": [
						"minmax(32px, 1fr)",
						"minmax(32px, 1fr)",
						"minmax(32px, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": null
					},
					"styles": {
						"overflow-x": "hidden"
					},
					"items": [],
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
				"parentName": "ExpansionPanel_2pz7pos",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Label_rgb5td9",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(Label_rgb5td9_caption)#)#",
					"labelType": "body",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "#757575",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true,
					"layoutConfig": {
						"column": 1,
						"colSpan": 3,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "GridContainer_e1xst8h",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ComboBox_to2ecyw",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PageParameters_LookupParameter1_vjmuvec",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "above",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PageParameters_LookupParameter1_vjmuvec",
					"visible": true,
					"readonly": false,
					"placeholder": "",
					"valueDetails": null,
					"mode": "List"
				},
				"parentName": "GridContainer_e1xst8h",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ExpansionPanel_6rwxxnz",
				"values": {
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"title": "#ResourceString(ExpansionPanel_6rwxxnz_title)#",
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
				"parentName": "GeneralInfoTab",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "GridContainer_5qhvjs9",
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
				"parentName": "ExpansionPanel_6rwxxnz",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_qua65ip",
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
				"parentName": "GridContainer_5qhvjs9",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailRefreshBtn_rqpozn7",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailRefreshBtn_rqpozn7_caption)#",
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
							"dataSourceName": "GridDetail_z19oy3cDS"
						}
					}
				},
				"parentName": "FlexContainer_qua65ip",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailSettingsBtn_ymjiu1r",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailSettingsBtn_ymjiu1r_caption)#",
					"icon": "actions-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clickMode": "menu",
					"menuItems": []
				},
				"parentName": "FlexContainer_qua65ip",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridDetailExportDataBtn_ekuwodt",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(GridDetailExportDataBtn_ekuwodt_caption)#",
					"icon": "export-button-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "GridDetail_z19oy3c"
						}
					}
				},
				"parentName": "GridDetailSettingsBtn_ymjiu1r",
				"propertyName": "menuItems",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailImportDataBtn_0v8f2zs",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(GridDetailImportDataBtn_0v8f2zs_caption)#",
					"icon": "import-button-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.ImportDataRequest",
						"params": {
							"entitySchemaName": "SysIdPToExtSvcMapping"
						}
					}
				},
				"parentName": "GridDetailSettingsBtn_ymjiu1r",
				"propertyName": "menuItems",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridDetailSearchFilter_qnkfcxl",
				"values": {
					"type": "crt.SearchFilter",
					"placeholder": "#ResourceString(GridDetailSearchFilter_qnkfcxl_placeholder)#",
					"iconOnly": true,
					"_filterOptions": {
						"expose": [
							{
								"attribute": "GridDetailSearchFilter_qnkfcxl_GridDetail_z19oy3c",
								"converters": [
									{
										"converter": "crt.SearchFilterAttributeConverter",
										"args": [
											"GridDetail_z19oy3c"
										]
									}
								]
							}
						],
						"from": [
							"GridDetailSearchFilter_qnkfcxl_SearchValue",
							"GridDetailSearchFilter_qnkfcxl_FilteredColumnsGroups"
						]
					}
				},
				"parentName": "FlexContainer_qua65ip",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "Label_toitcmb",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(Label_toitcmb_caption)#)#",
					"labelType": "body",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "#757575",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "ExpansionPanel_6rwxxnz",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridContainer_w1sprdc",
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
				"parentName": "ExpansionPanel_6rwxxnz",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridDetail_z19oy3c",
				"values": {
					"type": "crt.DataGrid",
					"layoutConfig": {
						"colSpan": 2,
						"column": 1,
						"row": 1,
						"rowSpan": 10
					},
					"features": {
						"rows": {
							"selection": {
								"enable": true,
								"multiple": true
							}
						}
					},
					"items": "$GridDetail_z19oy3c",
					"primaryColumnName": "GridDetail_z19oy3cDS_Id",
					"columns": [
						{
							"id": "2e38dedb-10cb-8d71-75fd-337dad44887a",
							"code": "GridDetail_z19oy3cDS_Service",
							"caption": "#ResourceString(GridDetail_z19oy3cDS_Service)#",
							"dataValueType": 10
						},
						{
							"id": "5d51c611-bc95-61b6-a0e9-53658003b631",
							"code": "GridDetail_z19oy3cDS_Service_Code",
							"caption": "#ResourceString(GridDetail_z19oy3cDS_Service_Code)#",
							"dataValueType": 28
						},
						{
							"id": "a634f929-93a8-16a4-9497-1504b1828ae3",
							"code": "GridDetail_z19oy3cDS_IdentityProvider",
							"caption": "#ResourceString(GridDetail_z19oy3cDS_IdentityProvider)#",
							"dataValueType": 10
						}
					],
					"placeholder": false,
					"visible": true,
					"fitContent": true
				},
				"parentName": "GridContainer_w1sprdc",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "TabPanel_qdfzqly",
				"values": {
					"type": "crt.TabPanel",
					"items": [],
					"mode": "toggle",
					"fitContent": true,
					"styleType": "default",
					"bodyBackgroundColor": "primary-contrast-500",
					"selectedTabTitleColor": "auto",
					"tabTitleColor": "auto",
					"underlineSelectedTabColor": "auto",
					"headerBackgroundColor": "auto",
					"allowToggleClose": true
				},
				"parentName": "FlexContainer_6agkpsz",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "TabContainer_1ydvbaj",
				"values": {
					"type": "crt.TabContainer",
					"tools": [],
					"items": [],
					"caption": "#ResourceString(TabContainer_1ydvbaj_caption)#",
					"iconPosition": "left-icon",
					"visible": true,
					"icon": "book-open-icon"
				},
				"parentName": "TabPanel_qdfzqly",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_dso4rn0",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"alignItems": "center",
					"items": []
				},
				"parentName": "TabContainer_1ydvbaj",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Label_tni8gmr",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(Label_tni8gmr_caption)#)#",
					"labelType": "headline-3",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "#0D2E4E",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"visible": true,
					"headingLevel": "label"
				},
				"parentName": "FlexContainer_dso4rn0",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_82fgbn1",
				"values": {
					"type": "crt.FlexContainer",
					"items": [],
					"direction": "column",
					"styles": {
						"min-width": "360px"
					}
				},
				"parentName": "TabContainer_1ydvbaj",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Label_o1j1755",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(Label_o1j1755_caption)#)#",
					"labelType": "body",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "FlexContainer_82fgbn1",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Label_o9j546q",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(Label_o9j546q_caption)#)#",
					"labelType": "body",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "FlexContainer_82fgbn1",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Label_4mkm1f7",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(Label_4mkm1f7_caption)#)#",
					"labelType": "body",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "FlexContainer_82fgbn1",
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
					"GridDetail_z19oy3c": {
						"isCollection": true,
						"modelConfig": {
							"path": "GridDetail_z19oy3cDS",
							"filterAttributes": [
								{
									"name": "GridDetailSearchFilter_qnkfcxl_GridDetail_z19oy3c",
									"loadOnChange": true
								},
								{
									"loadOnChange": true,
									"name": "GridDetail_z19oy3c_PredefinedFilter"
								}
							],
							"sortingConfig": {
								"default": [
									{
										"direction": "desc",
										"columnName": "IdentityProvider"
									}
								]
							}
						},
						"viewModelConfig": {
							"attributes": {
								"GridDetail_z19oy3cDS_Service": {
									"modelConfig": {
										"path": "GridDetail_z19oy3cDS.Service"
									}
								},
								"GridDetail_z19oy3cDS_Service_Code": {
									"modelConfig": {
										"path": "GridDetail_z19oy3cDS.Service_Code"
									}
								},
								"GridDetail_z19oy3cDS_IdentityProvider": {
									"modelConfig": {
										"path": "GridDetail_z19oy3cDS.IdentityProvider"
									}
								},
								"GridDetail_z19oy3cDS_Id": {
									"modelConfig": {
										"path": "GridDetail_z19oy3cDS.Id"
									}
								}
							}
						}
					},
					"GridDetail_l6id9tz": {
						"isCollection": true,
						"modelConfig": {
							"path": "GridDetail_l6id9tzDS",
							"filterAttributes": [
								{
									"name": "GridDetailSearchFilter_73otd7j_GridDetail_l6id9tz",
									"loadOnChange": true
								},
								{
									"loadOnChange": true,
									"name": "GridDetail_l6id9tz_PredefinedFilter"
								}
							],
							"sortingConfig": {
								"default": [
									{
										"columnName": "Name",
										"direction": "asc"
									}
								]
							}
						},
						"viewModelConfig": {
							"attributes": {
								"GridDetail_l6id9tzDS_Name": {
									"modelConfig": {
										"path": "GridDetail_l6id9tzDS.Name"
									}
								},
								"GridDetail_l6id9tzDS_Description": {
									"modelConfig": {
										"path": "GridDetail_l6id9tzDS.Description"
									}
								},
								"GridDetail_l6id9tzDS_ServerUrl": {
									"modelConfig": {
										"path": "GridDetail_l6id9tzDS.ServerUrl"
									}
								},
								"GridDetail_l6id9tzDS_ClientId": {
									"modelConfig": {
										"path": "GridDetail_l6id9tzDS.ClientId"
									}
								},
								"GridDetail_l6id9tzDS_ClientSecret": {
									"modelConfig": {
										"path": "GridDetail_l6id9tzDS.ClientSecret"
									}
								},
								"GridDetail_l6id9tzDS_Id": {
									"modelConfig": {
										"path": "GridDetail_l6id9tzDS.Id"
									}
								}
							}
						}
					},
					"GridDetail_z19oy3c_PredefinedFilter": {
						"value": null
					},
					"GridDetail_l6id9tz_PredefinedFilter": {
						"value": null
					},
					"undefined_List": {
						"isCollection": true,
						"modelConfig": {}
					},
					"PageParameters_LookupParameter1_vjmuvec": {
						"modelConfig": {
							"path": "PageParameters.DefaultIdentityProviderCode"
						}
					},
					"PageParameters_LookupParameter1_vjmuvec_List": {
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
					}
				}
			}
		]/**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/,
		modelConfigDiff: /**SCHEMA_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [],
				"values": {
					"dataSources": {
						"GridDetail_z19oy3cDS": {
							"type": "crt.EntityDataSource",
							"scope": "viewElement",
							"config": {
								"entitySchemaName": "SysIdPToExtSvcMapping",
								"attributes": {
									"Service": {
										"path": "Service"
									},
									"Service_Code": {
										"type": "ForwardReference",
										"path": "Service.Code"
									},
									"IdentityProvider": {
										"path": "IdentityProvider"
									}
								}
							}
						},
						"GridDetail_l6id9tzDS": {
							"type": "crt.EntityDataSource",
							"scope": "viewElement",
							"config": {
								"entitySchemaName": "SysIdentityProvider",
								"attributes": {
									"Name": {
										"path": "Name"
									},
									"Description": {
										"path": "Description"
									},
									"ServerUrl": {
										"path": "ServerUrl"
									},
									"ClientId": {
										"path": "ClientId"
									},
									"ClientSecret": {
										"path": "ClientSecret"
									}
								}
							}
						}
					},
					"loadingConfig": {}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[
			{
				request: "crt.HandleViewModelInitRequest",
				handler: async (request, next) => {
					const canUseIdentityProvidersManagement = await new Promise((resolve) => {
						RightUtilities.checkCanExecuteOperation({
							operation: "CanUseIdentityProvidersManagement"
						}, function(result) {
							resolve(result === true);
						}, null);
					});
		
					if (!canUseIdentityProvidersManagement) {
						Terrasoft.showErrorMessage("You do not have permission to open Identity Providers Management.");
	
						window.location.hash = "#Desktop";

						return;
					}
		
					return next?.handle(request);
				}
			},
			{
				request: "usr.OpenIdentityProviderSettingsModalRequest",
				handler: async (request, next) => {
					const handlerChain = sdk.HandlerChainService.instance;
					await handlerChain.process({
						type: "crt.OpenPageRequest",
						schemaName: "IdentityProviderSettings_Modal",
						$context: request.$context
					});

					return next?.handle(request);
				}
			},
			{
				request: "crt.LoadDataRequest",
				handler: async (request, next) => {
					await next?.handle(request);

					const isInitialized = await request.$context.IsDefaultIdentityProviderInitialized;
					if (isInitialized) {
						return;
					}

					const currentValue = await request.$context.$PageParameters_LookupParameter1_vjmuvec;
					if (currentValue?.value) {
						await request.$context.set("IsDefaultIdentityProviderInitialized", true);
						return;
					}

					const sysSettingsService = new sdk.SysSettingsService();
					const sysSetting = await sysSettingsService.getByCode("DefaultIdentityProvider");
					if (!sysSetting?.value || !sysSetting?.displayValue) {
						return;
					}
		
					await request.$context.set("PageParameters_LookupParameter1_vjmuvec", {
						value: sysSetting.value,
						displayValue: sysSetting.displayValue
					}, 
					{
						preventStateChange: true,
						preventAttributeChangeRequest: true
					});
					
					await request.$context.set("IsDefaultIdentityProviderInitialized", true);
				}
			},
			{
				request: "crt.SaveRecordRequest",
				handler: async (request, next) => {
					try {
						const selectedIdentityProvider =
							await request.$context.PageParameters_LookupParameter1_vjmuvec;
			
						const identityProviderId = selectedIdentityProvider?.value ?? null;
						const sysSettingsService = new sdk.SysSettingsService();
						await sysSettingsService.update({
							code: "DefaultIdentityProvider",
							value: identityProviderId
						});
			
						return await next?.handle(request);
					} catch (error) {
						Terrasoft.showErrorMessage(error?.message || "Failed to save DefaultIdentityProvider");
						return;
					}
				}
			}
		]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});