define("AIPlatformEntityEvents_FormPage", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
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
					"entitySchemaName": "AIPlatformEntityEvents"
				}
			},
			{
				"operation": "merge",
				"name": "AttachmentList",
				"values": {
					"columns": [
						{
							"id": "8c13f741-18ca-4ada-acfc-0ac95cebe89f",
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
					"control": "$Name",
					"labelPosition": "auto"
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Code",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.PDS_Code",
					"control": "$PDS_Code",
					"placeholder": "",
					"tooltip": "",
					"readonly": false,
					"multiline": false,
					"labelPosition": "auto",
					"visible": true
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Description",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.PDS_Description",
					"control": "$PDS_Description",
					"placeholder": "#ResourceString(DescriptionInputPlaceholder)#",
					"tooltip": "",
					"readonly": false,
					"multiline": true,
					"labelPosition": "above",
					"visible": true
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "EntitySchemaName",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_EntitySchema",
					"ariaLabel": "",
					"isAddAllowed": false,
					"showValueAsLink": false,
					"labelPosition": "above",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_EntitySchema"
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "EntityChangeType",
				"values": {
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_EntitySchemaChangeType",
					"ariaLabel": "",
					"isAddAllowed": false,
					"showValueAsLink": false,
					"labelPosition": "above",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_EntitySchemaChangeType",
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
				"name": "ChangedColumnsExpansionPanel",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 2,
						"rowSpan": 1
					},
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"title": "Columns",
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
					"visible": "$IsUpdatedChangeType",
					"alignItems": "stretch"
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "ChangedColumnsFilterContainer",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(max-content, 32px)",
					"columns": [
						"minmax(32px, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": 0
					},
					"items": [],
					"padding": {
						"top": "small",
						"right": "small",
						"bottom": "small",
						"left": "small"
					},
					"alignItems": "stretch",
					"styles": {
						"overflow-x": "hidden",
						"width": "100%"
					}
				},
				"parentName": "ChangedColumnsExpansionPanel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ChangedColumnsFilter",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.AiAgenticColumnFilter",
					"entitySchemaName": "$EDS_EntitySchemaName",
					"value": "$PDS_ChangedColumns",
					"styles": {
						"width": "100%"
					}
				},
				"parentName": "ChangedColumnsFilterContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ObjectFilterExpansionPanel",
				"values": {
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"title": "$Resources.Strings.PDS_EntitySchemaFilters",
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
						"row": 3,
						"rowSpan": 1
					}
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "ObjectFilterGridContainer",
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
					"padding": {
						"top": "small",
						"right": "small",
						"bottom": "small",
						"left": "small"
					},
					"alignItems": "stretch",
					"styles": {
						"overflow-x": "hidden",
						"width": "100%"
					},
					"items": []
				},
				"parentName": "ObjectFilterExpansionPanel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FilterEditor_by_EntitySchemaFilters",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 2,
						"row": 1,
						"rowSpan": 18
					},
					"type": "crt.AiAgenticFilters",
					"filterData": "$PDS_EntitySchemaFilters",
					"entitySchemaName": "$EDS_EntitySchemaName"
				},
				"parentName": "ObjectFilterGridContainer",
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
					"Name": {
						"modelConfig": {
							"path": "PDS.Name"
						}
					},
					"PDS_Code": {
						"modelConfig": {
							"path": "PDS.Code"
						}
					},
					"PDS_Description": {
						"modelConfig": {
							"path": "PDS.Description"
						}
					},
					"PDS_EntitySchemaFilters": {
						"modelConfig": {
							"path": "PDS.EntitySchemaFilters"
						}
					},
					"PDS_ChangedColumns": {
						"modelConfig": {
							"path": "PDS.ChangedColumns"
						}
					},
					"IsUpdatedChangeType": {
						"value": false
					},
					"EDS_EntitySchemaName": {
						"modelConfig": {
							"path": "PDS.EntitySchema.Name"
						}
					},
					"PDS_EntitySchemaChangeType": {
						"modelConfig": {
							"path": "PDS.EntitySchemaChangeType"
						}
					},
					"EntitySchemaChangeType_Filter": {
						"value": {
							"isEnabled": true,
							"trimDateTimeParameterToDate": false,
							"filterType": 6,
							"logicalOperation": 0,
							"items": {
								"NotCustomType": {
									"isEnabled": true,
									"trimDateTimeParameterToDate": false,
									"filterType": 1,
									"comparisonType": 4,
									"leftExpression": {
										"expressionType": 0,
										"columnPath": "EnumCode"
									},
									"rightExpression": {
										"expressionType": 2,
										"parameter": {
											"dataValueType": 1,
											"value": 0
										}
									}
								}
							}
						}
					},
					"PDS_EntitySchemaChangeType_List": {
						"isCollection": true,
						"modelConfig": {
							"sortingConfig": {
								"default": [
									{
										"columnName": "Name",
										"direction": "asc"
									}
								]
							},
							"filterAttributes": [
								{
									"loadOnChange": true,
									"name": "EntitySchemaChangeType_Filter"
								}
							]
						}
					},
					"PDS_EntitySchema": {
						"modelConfig": {
							"path": "PDS.EntitySchema"
						}
					},
					"PDS_EntitySchema_Filter": {
						"value": {
							"isEnabled": true,
							"trimDateTimeParameterToDate": false,
							"filterType": 6,
							"logicalOperation": 0,
							"items": {
								"NotAIPlatformEntityEventsObject": {
									"isEnabled": true,
									"trimDateTimeParameterToDate": false,
									"filterType": 1,
									"comparisonType": 4,
									"leftExpression": {
										"expressionType": 0,
										"columnPath": "Name"
									},
									"rightExpression": {
										"expressionType": 2,
										"parameter": {
											"dataValueType": 1,
											"value": "AIPlatformEntityEvents"
										}
									}
								}
							}
						}
					},
					"PDS_EntitySchema_List": {
						"isCollection": true,
						"modelConfig": {
							"sortingConfig": {
								"default": [
									{
										"columnName": "Title",
										"direction": "asc"
									}
								]
							},
							"filterAttributes": [
								{
									"loadOnChange": true,
									"name": "PDS_EntitySchema_Filter"
								}
							]
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
					"loadingConfig": {},
					"dependencies": {}
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
							"entitySchemaName": "AIPlatformEntityEvents"
						},
						"scope": "page"
					}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});
