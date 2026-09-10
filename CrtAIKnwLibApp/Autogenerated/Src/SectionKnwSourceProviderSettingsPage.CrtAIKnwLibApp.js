define("SectionKnwSourceProviderSettingsPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "insert",
				"name": "Input_hne8m7k",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.SectionCaption",
					"value": "$RootSchemaLookup",
					"valueChange": {
						"request": "crt.KnwSourceSettingsSectionChangeRequest",
						"params": {
							"id": "@event | crt.ToObjectProp: 'value'"
						}
					},
					"placeholder": "",
					"tooltip": "",
					"readonly": false,
					"labelPosition": "above",
					"items": "$SchemaLookupValues",
					"useStaticFiltering": true,
					"listActions": [],
					"controlActions": []
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ScopeTypeComboBox",
				"values": {
					"type": "crt.ComboBox",
					"label": "Scope",
					"control": "$ScopeTypeLookup",
					"placeholder": "",
					"tooltip": "",
					"readonly": "$RootSchemaUId | crt.IsValueEmpty",
					"labelPosition": "above",
					"items": "$ScopeTypeLookupValues",
					"useStaticFiltering": true,
					"listActions": [],
					"controlActions": []
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "DynamicFolderComboBox",
				"values": {
					"type": "crt.ComboBox",
					"label": "Dynamic folder",
					"control": "$DynamicFolderLookup",
					"placeholder": "",
					"tooltip": "",
					"readonly": "$RootSchemaUId | crt.IsValueEmpty",
					"labelPosition": "above",
					"items": "$DynamicFolderLookupValues",
					"useStaticFiltering": true,
					"visible": "$ScopeType | crt.IsEqual : 'DynamicFolder'",
					"listActions": [],
					"controlActions": []
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "DynamicFolderWarningAlert",
				"values": {
					"type": "crt.GridContainer",
					"columns": [
						"minmax(32px, 1fr)"
					],
					"rows": "minmax(max-content, 32px)",
					"gap": {
						"columnGap": "large",
						"rowGap": "none"
					},
					"items": [],
					"fitContent": true,
					"visible": "$IsDynamicFolderDeleted",
					"alignItems": "stretch",
					"color": "#FDD8CF",
					"borderRadius": "medium",
					"padding": {
						"top": "medium",
						"bottom": "medium",
						"right": "medium",
						"left": "medium"
					}
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "DynamicFolderWarningAlertLabel",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(DynamicFolderWarningAlertLabel_caption)#)#",
					"labelType": "headline-1-small",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "DynamicFolderWarningAlert",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "DynamicFolderWarningAlertBody",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(DynamicFolderWarningAlertBody_caption)#)#",
					"labelType": "body",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "DynamicFolderWarningAlert",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "SectionColumnsExpansionPanel",
				"values": {
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 18
					},
					"title": "#ResourceString(SectionColumnsExpansionPanel_title)#",
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
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "SectionColumnsActionsGridContainer",
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
				"parentName": "SectionColumnsExpansionPanel",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "SectionColumnsActionsFlexContainer",
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
				"parentName": "SectionColumnsActionsGridContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "AddSectionColumnButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(AddSectionColumnButton_caption)#",
					"icon": "add-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.KnwSourceSettingsSelectSectionColumnRequest",
						"params": {}
					},
					"visible": true,
					"clickMode": "default"
				},
				"parentName": "SectionColumnsActionsFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "SectionColumnsGridContainer",
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
						"overflow-x": "auto"
					},
					"items": [
						{
							"type": "crt.DataGrid",
							"items": "$ColumnsViewModelCollection",
							"rowToolbarItems": [
								{
									"type": "crt.MenuItem",
									"caption": "#ResourceString(DeleteSectionColumnButtonCaption)#",
									"icon": "delete-row-action",
									"clicked": {
										"request": "crt.KnwSourceSettingsDeleteSectionColumnRequest",
										"params": {
											"id": "$ColumnsViewModelCollection.Id"
										}
									}
								}
							],
							"createItem": {
								"request": "crt.KnwSourceSettingsSelectSectionColumnRequest",
								"params": {
									"index": "@event.index"
								}
							},
							"columns": [
								{
									"id": "Column",
									"code": "Column",
									"caption": "#ResourceString(SectionColumnCaption)#",
									"dataValueType": 10,
									"editingCellView": {
										"type": "crt.DataTableEditLookupCell",
										"value": "$ColumnsViewModelCollection.Column",
										"valueChange": {
											"request": "crt.KnwSourceSettingsChangeSectionColumnRequest",
											"params": {
												"id": "$ColumnsViewModelCollection.Id",
												"value": "@event"
											}
										},
										"items": "$FilteredSchemaColumnLookupValues",
										"useStaticFiltering": true
									}
								}
							],
							"layoutConfig": {
								"colSpan": 2,
								"column": 1,
								"row": 1,
								"rowSpan": 6
							},
							"features": "$ColumnsGridFeatures",
							"activeRow": "$ColumnsActiveRow",
							"selectionState": "$ColumnsSelectionState",
							"editingCells": "$ColumnsEditingCells"
						}
					],
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
				"parentName": "SectionColumnsExpansionPanel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsExpansionPanel",
				"values": {
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 19,
						"rowSpan": 18
					},
					"title": "#ResourceString(RelatedColumnsExpansionPanel_title)#",
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
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsActionsGridContainer",
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
				"parentName": "RelatedColumnsExpansionPanel",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsActionsFlexContainer",
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
				"parentName": "RelatedColumnsActionsGridContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "OpenRelatedColumnsSelectorButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(OpenRelatedColumnsSelectorButton_caption)#",
					"icon": "add-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.KnwSourceSettingsOpenRelatedColumnsSelectorRequest",
						"params": {}
					},
					"visible": true,
					"clickMode": "default"
				},
				"parentName": "RelatedColumnsActionsFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSummaryGridContainer",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(max-content, 32px)",
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
				"parentName": "RelatedColumnsExpansionPanel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSummaryGrid",
				"values": {
					"type": "crt.DataGrid",
					"items": "$RelatedColumnsViewModelCollection",
					"rowToolbarItems": [
						{
							"type": "crt.MenuItem",
							"caption": "Delete",
							"icon": "delete-row-action",
							"clicked": {
								"request": "crt.KnwSourceSettingsDeleteRelatedColumnRequest",
								"params": {
									"id": "$RelatedColumnsViewModelCollection.Id"
								}
							}
						}
					],
					"columns": [
						{
							"id": "DisplayPath",
							"code": "DisplayPath",
							"caption": "Reference column",
							"dataValueType": 28,
							"width": 720
						}
					],
					"features": {
						"columns": {
							"sorting": false,
							"resizing": false,
							"dragAndDrop": false,
							"adding": false,
							"editing": false,
							"toolbar": false
						},
						"rows": {
							"hierarchical": false,
							"numeration": false,
							"toolbar": true,
							"selection": {
								"enable": false,
								"multiple": false,
								"selectAll": false
							}
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						},
						"cells": {
							"selection": false
						}
					},
					"layoutConfig": {
						"colSpan": 1,
						"column": 1,
						"row": 1,
						"rowSpan": 8
					}
				},
				"parentName": "RelatedColumnsSummaryGridContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorBackdrop",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(0, 1fr)",
					"columns": [
						"minmax(0, 1fr)"
					],
					"gap": {
						"columnGap": "none",
						"rowGap": "none"
					},
					"styles": {
						"position": "fixed",
						"top": "0",
						"right": "0",
						"bottom": "0",
						"left": "0",
						"z-index": "1000",
						"background": "rgba(12, 17, 29, 0.32)"
					},
					"items": [],
					"visible": "$RelatedColumnsSelectorVisible",
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"alignItems": "stretch",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 40
					}
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorContainer",
				"values": {
					"type": "crt.GridContainer",
					"rows": "auto auto minmax(0, 1fr) auto",
					"columns": [
						"minmax(0, 1fr)",
						"minmax(0, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": "small"
					},
					"styles": {
						"overflow-x": "hidden",
						"position": "fixed",
						"top": "72px",
						"left": "50%",
						"transform": "translateX(-50%)",
						"width": "min(1535px, calc(100vw - 48px))",
						"height": "calc(100vh - 120px)",
						"max-height": "calc(100vh - 120px)",
						"overflow-y": "hidden",
						"z-index": "1001",
						"box-shadow": "0 24px 64px rgba(12, 17, 29, 0.24)"
					},
					"items": [],
					"visible": "$RelatedColumnsSelectorVisible",
					"color": "#FFFFFF",
					"borderRadius": "medium",
					"padding": {
						"top": "medium",
						"right": "medium",
						"bottom": "medium",
						"left": "medium"
					},
					"alignItems": "stretch"
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 7
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorDraftLabel",
				"values": {
					"type": "crt.Label",
					"caption": "Selected reference columns",
					"labelType": "body",
					"labelThickness": "semibold",
					"labelEllipsis": true,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedColumnsSelectorContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorDraftGrid",
				"values": {
					"type": "crt.DataGrid",
					"items": "$RelatedColumnsSelectorDraftViewModelCollection",
					"rowToolbarItems": [
						{
							"type": "crt.MenuItem",
							"caption": "Remove",
							"icon": "delete-row-action",
							"clicked": {
								"request": "crt.KnwSourceSettingsToggleRelatedColumnRequest",
								"params": {
									"id": "$RelatedColumnsSelectorDraftViewModelCollection.Id"
								}
							}
						}
					],
					"columns": [
						{
							"id": "DisplayPath",
							"code": "DisplayPath",
							"caption": "Reference column",
							"dataValueType": 28,
							"width": 720
						}
					],
					"features": {
						"columns": {
							"sorting": false,
							"resizing": false,
							"dragAndDrop": false,
							"adding": false,
							"editing": false,
							"toolbar": false
						},
						"rows": {
							"hierarchical": false,
							"numeration": false,
							"toolbar": true,
							"selection": {
								"enable": false,
								"multiple": false,
								"selectAll": false
							}
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						},
						"cells": {
							"selection": false
						}
					},
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					},
					"styles": {
						"overflow-x": "hidden",
						"padding-bottom": "44px"
					}
				},
				"parentName": "RelatedColumnsSelectorContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorPathLabel",
				"values": {
					"type": "crt.Label",
					"caption": "$RelatedColumnsSelectorCurrentPathCaption",
					"labelType": "body",
					"labelThickness": "semibold",
					"labelEllipsis": true,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedColumnsSelectorContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorTabPanel",
				"values": {
					"type": "crt.TabPanel",
					"items": [],
					"styleType": "default",
					"mode": "tab",
					"bodyBackgroundColor": "primary-contrast-500",
					"selectedTabTitleColor": "auto",
					"tabTitleColor": "auto",
					"underlineSelectedTabColor": "auto",
					"headerBackgroundColor": "auto",
					"allowToggleClose": true,
					"selectedTabIndex": "$RelatedColumnsSelectorSelectedTabIndex",
					"styles": {
						"overflow-x": "hidden",
						"height": "100%",
						"min-height": "0"
					},
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedColumnsSelectorContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorColumnsTab",
				"values": {
					"type": "crt.TabContainer",
					"caption": "Columns",
					"iconPosition": "only-text",
					"items": [],
					"icon": null
				},
				"parentName": "RelatedColumnsSelectorTabPanel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorColumnsTabContent",
				"values": {
					"type": "crt.GridContainer",
					"columns": ["minmax(0, 1fr)"],
					"rows": "auto minmax(0, 1fr) auto",
					"gap": {
						"columnGap": "large",
						"rowGap": "small"
					},
					"styles": {
						"overflow-x": "hidden"
					},
					"items": []
				},
				"parentName": "RelatedColumnsSelectorColumnsTab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorReferencesTab",
				"values": {
					"type": "crt.TabContainer",
					"caption": "References",
					"iconPosition": "only-text",
					"items": [],
					"icon": null
				},
				"parentName": "RelatedColumnsSelectorTabPanel",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorReferencesTabContent",
				"values": {
					"type": "crt.GridContainer",
					"columns": ["minmax(0, 1fr)"],
					"rows": "auto minmax(0, 1fr) auto",
					"gap": {
						"columnGap": "large",
						"rowGap": "small"
					},
					"styles": {
						"overflow-x": "hidden"
					},
					"items": []
				},
				"parentName": "RelatedColumnsSelectorReferencesTab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorActionsFlexContainer",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(max-content, 24px)",
					"columns": [
						"minmax(0, 1fr)",
						"minmax(0, 1fr)"
					],
					"gap": {
						"columnGap": "small",
						"rowGap": "none"
					},
					"items": [],
					"alignItems": "center",
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 1,
						"rowSpan": 1
					},
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "small",
						"left": "none"
					}
				},
				"parentName": "RelatedColumnsSelectorContainer",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorBackButtonContainer",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"justifyContent": "start",
					"gap": "small",
					"alignItems": "center",
					"items": [],
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedColumnsSelectorActionsFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorPrimaryButtonsContainer",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"justifyContent": "end",
					"gap": "small",
					"alignItems": "center",
					"items": [],
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedColumnsSelectorActionsFlexContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorBackButton",
				"values": {
					"type": "crt.Button",
					"icon": "back-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"visible": "$RelatedColumnsSelectorCanGoBack",
					"clicked": {
						"request": "crt.KnwSourceSettingsNavigateRelatedColumnsSelectorBackRequest",
						"params": {}
					}
				},
				"parentName": "RelatedColumnsSelectorBackButtonContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorBackButtonLabel",
				"values": {
					"type": "crt.Label",
					"caption": "$RelatedColumnsSelectorBackCaption",
					"labelType": "body",
					"labelThickness": "semibold",
					"labelEllipsis": true,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": "$RelatedColumnsSelectorCanGoBack"
				},
				"parentName": "RelatedColumnsSelectorBackButtonContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorSaveButton",
				"values": {
					"type": "crt.Button",
					"caption": "Save",
					"color": "primary",
					"size": "medium",
					"clicked": {
						"request": "crt.KnwSourceSettingsSaveRelatedColumnsSelectorRequest",
						"params": {}
					}
				},
				"parentName": "RelatedColumnsSelectorPrimaryButtonsContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorCancelButton",
				"values": {
					"type": "crt.Button",
					"caption": "Cancel",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.KnwSourceSettingsCancelRelatedColumnsSelectorRequest",
						"params": {}
					}
				},
				"parentName": "RelatedColumnsSelectorPrimaryButtonsContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorColumnsSearchInput",
				"values": {
					"type": "crt.Input",
					"placeholder": "Search columns...",
					"control": "$RelatedColumnsSelectorColumnsSearchText",
					"labelPosition": "hidden",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedColumnsSelectorColumnsTabContent",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorColumnsGrid",
				"values": {
					"type": "crt.DataGrid",
					"items": "$RelatedColumnsSelectorColumnsViewModelCollection",
					"rowToolbarItems": [],
					"selectionState": "$RelatedColumnsSelectorColumnsSelectionState",
					"_selectionOptions": {
						"attribute": "Id"
					},
					"columns": [
						{
							"id": "Caption",
							"code": "Caption",
							"caption": "Property",
							"dataValueType": 28
						}
					],
					"features": {
						"columns": {
							"sorting": false,
							"resizing": false,
							"dragAndDrop": false,
							"adding": false,
							"editing": false,
							"toolbar": false
						},
						"rows": {
							"hierarchical": false,
							"numeration": false,
							"toolbar": true,
							"selection": {
								"enable": true,
								"multiple": true,
								"selectAll": false
							}
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						},
						"cells": {
							"selection": false
						}
					},
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"styles": {
						"overflow-x": "hidden"
					}
				},
				"parentName": "RelatedColumnsSelectorColumnsTabContent",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorColumnsLoadMoreButton",
				"values": {
					"type": "crt.Button",
					"caption": "Load More",
					"color": "default",
					"size": "medium",
					"iconPosition": "only-text",
					"visible": "$RelatedColumnsSelectorColumnsLoadMoreVisible",
					"clicked": {
						"request": "crt.KnwSourceSettingsLoadMoreRelatedColumnsSelectorColumnsRequest",
						"params": {}
					},
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 4,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedColumnsSelectorContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorReferencesSearchInput",
				"values": {
					"type": "crt.Input",
					"placeholder": "Search references...",
					"control": "$RelatedColumnsSelectorReferencesSearchText",
					"labelPosition": "hidden",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedColumnsSelectorReferencesTabContent",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorReferencesGrid",
				"values": {
					"type": "crt.DataGrid",
					"items": "$RelatedColumnsSelectorReferencesViewModelCollection",
					"rowToolbarItems": [],
					"columns": [
						{
							"id": "Caption",
							"code": "Caption",
							"caption": "Property",
							"dataValueType": 28
						}
					],
					"features": {
						"columns": {
							"sorting": false,
							"resizing": false,
							"dragAndDrop": false,
							"adding": false,
							"editing": false,
							"toolbar": false
						},
						"rows": {
							"hierarchical": false,
							"numeration": false,
							"toolbar": true,
							"selection": {
								"enable": false,
								"multiple": false,
								"selectAll": false
							}
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						},
						"cells": {
							"selection": false
						}
					},
					"activeRow": "$RelatedColumnsSelectorReferencesActiveRow",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"styles": {
						"overflow-x": "hidden"
					}
				},
				"parentName": "RelatedColumnsSelectorReferencesTabContent",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "RelatedColumnsSelectorReferencesLoadMoreButton",
				"values": {
					"type": "crt.Button",
					"caption": "Load More",
					"color": "default",
					"size": "medium",
					"iconPosition": "only-text",
					"visible": "$RelatedColumnsSelectorReferencesLoadMoreVisible",
					"clicked": {
						"request": "crt.KnwSourceSettingsLoadMoreRelatedColumnsSelectorReferencesRequest",
						"params": {}
					},
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 4,
						"rowSpan": 1
					}
				},
				"parentName": "RelatedColumnsSelectorContainer",
				"propertyName": "items",
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
					"ScopeType": {
						"modelConfig": {
							"path": "PageParameters.ScopeType"
						}
					},
					"ScopeTypeLookup": {
						"change": {
							"request": "crt.KnwSourceSettingsScopeTypeChangeRequest"
						}
					},
					"ScopeTypeLookupValues": {
						"isCollection": true
					},
					"DynamicFolderId": {
						"modelConfig": {
							"path": "PageParameters.DynamicFolderId"
						}
					},
					"DynamicFolderLookup": {
						"change": {
							"request": "crt.KnwSourceSettingsDynamicFolderChangeRequest"
						}
					},
					"DynamicFolderLookupValues": {
						"isCollection": true
					},
					"IsDynamicFolderDeleted": {
						"value": false
					},
					"DynamicFolderWarningDismissSignal": {
						"modelConfig": {
							"path": "PageParameters.DynamicFolderWarningDismissSignal"
						}
					},
					"RootSchemaUId": {
						"modelConfig": {
							"path": "PageParameters.RootSchemaUId"
						},
						"change": {
							"request": "crt.KnwSourceSettingsLoadSectionColumnsRequest"
						}
					},
					"RootSchemaLookup": {
						"from": "RootSchemaUId",
						"converter": "crt.PickLookupValueFromCollection : $SchemaLookupValues"
					},
					"Columns": {
						"isCollection": true,
						"modelConfig": {
							"path": "PageParameters.Columns"
						}
					},
					"ColumnsViewModelCollection": {
						"isCollection": true,
						"viewModelConfig": {
							"attributes": {
								"Id": {},
								"Column": {},
								"ColumnIndex": {}
							}
						},
						"modelConfig": {}
					},
					"SchemaLookupValues": {
						"isCollection": true
					},
					"SchemaColumnLookupValues": {
						"isCollection": true
					},
					"FilteredSchemaColumnLookupValues": {
						"from": [
							"Columns",
							"SchemaColumnLookupValues"
						],
						"converter": "crt.FilterCollectionByExclusion : $SchemaColumnLookupValues : $Columns"
					},
					"ColumnsSelectionState": {},
					"ColumnsActiveRow": {},
					"ColumnsEditingCells": [],
					"ColumnsGridFeatures": {
						"value": {
							"columns": {
								"sorting": false,
								"resizing": false,
								"dragAndDrop": false,
								"adding": false,
								"editing": false,
								"toolbar": false
							},
							"rows": {
								"hierarchical": false,
								"numeration": true,
								"selection": {
									"enable": false,
									"multiple": false,
									"selectAll": false
								}
							},
							"header": {
								"visible": true
							},
							"editable": {
								"enable": true,
								"itemsCreation": false,
								"floatingEditPanel": false,
								"lookupItemsCreation": false
							},
							"cells": {
								"selection": true,
								"disableTextSelectionOnDoubleClick": false
							}
						}
					},
					"RelatedColumns": {
						"isCollection": true,
						"modelConfig": {
							"path": "PageParameters.RelatedColumns"
						}
					},
					"RelatedColumnsViewModelCollection": {
						"isCollection": true,
						"viewModelConfig": {
							"attributes": {
								"Id": {},
								"DisplayPath": {}
							}
						},
						"modelConfig": {}
					},
					"RelatedColumnsSelectorVisible": {
						"value": false
					},
					"RelatedColumnsSelectorDraft": {
						"value": []
					},
					"RelatedColumnsSelectorCurrentSchemaUId": {},
					"RelatedColumnsSelectorCurrentSchemaName": {},
					"RelatedColumnsSelectorCurrentPathCaption": {},
					"RelatedColumnsSelectorCurrentPathSegments": {
						"value": []
					},
					"RelatedColumnsSelectorPathStack": {
						"value": []
					},
					"RelatedColumnsSelectorBackCaption": {
						"value": null
					},
					"RelatedColumnsSelectorCanGoBack": {
						"value": false
					},
					"RelatedColumnsSelectorSelectedTabIndex": {
						"value": 0
					},
					"RelatedColumnsSelectorColumnsViewModelCollection": {
						"isCollection": true,
						"viewModelConfig": {
							"attributes": {
								"Id": {},
								"Caption": {},
								"Kind": {},
								"IsSelected": {},
								"CanNavigate": {},
								"PathKey": {},
								"PathCaption": {},
								"PathSegments": {},
								"ReferenceSchemaUId": {},
								"ReferenceSchemaName": {},
								"TerminalSchemaUId": {},
								"TerminalSchemaName": {},
								"TerminalColumnUId": {},
								"TerminalColumnName": {}
							}
						},
						"modelConfig": {}
					},
					"RelatedColumnsSelectorReferencesActiveRow": {},
					"RelatedColumnsSelectorReferencesViewModelCollection": {
						"isCollection": true,
						"viewModelConfig": {
							"attributes": {
								"Id": {},
								"Caption": {},
								"Kind": {},
								"CanNavigate": {},
								"PathKey": {},
								"PathCaption": {},
								"PathSegments": {},
								"ReferenceSchemaUId": {},
								"ReferenceSchemaName": {}
							}
						},
						"modelConfig": {}
					},
					"RelatedColumnsSelectorDraftViewModelCollection": {
						"isCollection": true,
						"viewModelConfig": {
							"attributes": {
								"Id": {},
								"DisplayPath": {},
								"Kind": {},
								"PathKey": {},
								"PathSegments": {},
								"TerminalSchemaUId": {},
								"TerminalSchemaName": {},
								"TerminalColumnUId": {},
								"TerminalColumnName": {}
							}
						},
						"modelConfig": {}
					},
					"RelatedColumnsSelectorColumnsSelectionState": {
						"value": {
							"type": "none",
							"selected": []
						}
					},
					"RelatedColumnsSelectorColumnsSearchText": {
						"value": ""
					},
					"RelatedColumnsSelectorReferencesSearchText": {
						"value": ""
					},
					"RelatedColumnsSelectorColumnsHasMore": {
						"value": false
					},
					"RelatedColumnsSelectorReferencesHasMore": {
						"value": false
					},
					"RelatedColumnsSelectorColumnsLoadMoreVisible": {
						"value": false
					},
					"RelatedColumnsSelectorReferencesLoadMoreVisible": {
						"value": false
					},
					"RelatedColumnsSelectorColumnsLastCursor": {
						"value": null
					},
					"RelatedColumnsSelectorReferencesLastCursor": {
						"value": null
					},
					"RelatedColumnsSelectorColumnsIsLoading": {
						"value": false
					},
					"RelatedColumnsSelectorReferencesIsLoading": {
						"value": false
					},
					"Id_78r7zib": {
						"modelConfig": {
							"path": "PDS.Id"
						}
					},
					"Playbook_KnowledgeBase": {
						"isCollection": true,
						"modelConfig": {
							"path": "Playbook_KnowledgeBaseDS",
							"filterAttributes": [
								{
									"name": "Filter_by_DCM",
									"loadOnChange": true
								}
							]
						},
						"viewModelConfig": {
							"attributes": {
								"Playbook_KnowledgeBaseDS_Id": {
									"modelConfig": {
										"path": "Playbook_KnowledgeBaseDS.Id"
									}
								},
								"Playbook_KnowledgeBaseDS_Name": {
									"modelConfig": {
										"path": "Playbook_KnowledgeBaseDS.Name"
									}
								},
								"Playbook_KnowledgeBaseDS_Notes": {
									"modelConfig": {
										"path": "Playbook_KnowledgeBaseDS.Notes"
									}
								},
								"Playbook_KnowledgeBaseDS_CreatedBy": {
									"modelConfig": {
										"path": "Playbook_KnowledgeBaseDS.CreatedBy"
									}
								},
								"Playbook_KnowledgeBaseDS_ModifiedOn": {
									"modelConfig": {
										"path": "Playbook_KnowledgeBaseDS.ModifiedOn"
									}
								},
								"Playbook_KnowledgeBaseDS_Type": {
									"modelConfig": {
										"path": "Playbook_KnowledgeBaseDS.Type"
									}
								}
							}
						}
					},
					"Filter_by_DCM": {}
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
					"Playbook_KnowledgeBaseDS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "KnowledgeBase",
							"attributes": [
								{
									"Id": {
										"path": "Id"
									}
								},
								{
									"Name": {
										"path": "Name"
									}
								},
								{
									"Notes": {
										"path": "Notes"
									}
								},
								{
									"Type": {
										"path": "Type"
									}
								},
								{
									"ModifiedOn": {
										"path": "ModifiedOn"
									}
								},
								{
									"CreatedBy": {
										"path": "CreatedBy"
									}
								}
							]
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
