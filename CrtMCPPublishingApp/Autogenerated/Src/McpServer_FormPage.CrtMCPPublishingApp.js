define("McpServer_FormPage", /**SCHEMA_DEPS*/["@creatio-devkit/common", "McpServer_FormPageResources"]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/(sdk, resources)/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/
[
	{
		"operation": "merge",
		"name": "PageTitle",
		"values": {
			"styles": {
				"min-width": "0",
				"overflow-wrap": "anywhere",
				"word-break": "break-word"
			}
		}
	},
	{
		"operation": "merge",
		"name": "TitleContainer",
		"values": {
			"styles": {
				"flex": "1 1 auto",
				"min-width": "0"
			}
		}
	},
	{
		"operation": "merge",
		"name": "SaveButton",
		"values": {
			"clicked": {
				"request": "crt.SaveRecordRequest",
				"params": {
					"showSuccessMessage": true,
					"preventCardClose": true
				}
			},
			"caption": "#ResourceString(SaveButton_caption)#",
			"size": "large",
			"iconPosition": "only-text",
			"clickMode": "default"
		}
	},
	{
		"operation": "merge",
		"name": "TopAreaProfileContainer",
		"values": {
			"columns": [
				"minmax(0, 1fr)",
				"minmax(0, 1fr)",
				"minmax(0, 1fr)",
				"minmax(0, 1fr)",
				"minmax(0, 1fr)",
				"minmax(0, 1fr)",
				"minmax(0, 1fr)",
				"minmax(0, 1fr)",
				"minmax(0, 1fr)",
				"max-content"
			],
			"visible": true,
			"alignItems": "stretch"
		}
	},
	{
		"operation": "merge",
		"name": "GeneralInfoTab",
		"values": {
			"caption": "#ResourceString(GeneralInfoTab_caption)#",
			"iconPosition": "only-text"
		}
	},
	{
		"operation": "insert",
		"name": "MakeOnlineButton",
		"values": {
			"type": "crt.Button",
			"caption": "#ResourceString(MakeOnlineButton_caption)#",
			"color": "accent",
			"size": "large",
			"visible": "$PDS_IsOnline | crt.InvertBooleanValue",
			"clicked": {
				"request": "usr.ToggleOnlineRequest"
			},
			"iconPosition": "only-text"
		},
		"parentName": "ActionButtonsContainer",
		"propertyName": "items",
		"index": 0
	},
	{
		"operation": "insert",
		"name": "MakeOfflineButton",
		"values": {
			"type": "crt.Button",
			"caption": "#ResourceString(MakeOfflineButton_caption)#",
			"color": "warn",
			"size": "large",
			"visible": "$PDS_IsOnline",
			"clicked": {
				"request": "usr.ToggleOnlineRequest"
			}
		},
		"parentName": "ActionButtonsContainer",
		"propertyName": "items",
		"index": 1
	},
	{
		"operation": "insert",
		"name": "Name",
		"values": {
			"layoutConfig": {
				"column": 1,
				"row": 1,
				"colSpan": 5,
				"rowSpan": 1
			},
			"type": "crt.Input",
			"label": "#ResourceString(NameInput_label)#",
			"control": "$Name",
			"labelPosition": "auto"
		},
		"parentName": "TopAreaProfileContainer",
		"propertyName": "items",
		"index": 0
	},
	{
		"operation": "insert",
		"name": "CodeInput",
		"values": {
			"layoutConfig": {
				"column": 6,
				"row": 1,
				"colSpan": 5,
				"rowSpan": 1
			},
			"type": "crt.Input",
			"label": "#ResourceString(CodeInput_label)#",
			"control": "$PDS_Code",
			"labelPosition": "auto"
		},
		"parentName": "TopAreaProfileContainer",
		"propertyName": "items",
		"index": 1
	},
	{
		"operation": "insert",
		"name": "FullUrlInput",
		"values": {
			"layoutConfig": {
				"column": 1,
				"row": 2,
				"colSpan": 9,
				"rowSpan": 1
			},
			"type": "crt.Input",
			"label": "#ResourceString(FullUrlInput_label)#",
			"control": "$UsrServerUrlRaw",
			"labelPosition": "auto",
			"readonly": true
		},
		"parentName": "TopAreaProfileContainer",
		"propertyName": "items",
		"index": 2
	},
	{
		"operation": "insert",
		"name": "CopyUrlButtonContainer",
		"values": {
			"layoutConfig": {
				"column": 10,
				"row": 2,
				"colSpan": 1,
				"rowSpan": 1
			},
			"type": "crt.FlexContainer",
			"direction": "row",
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
			"alignItems": "center",
			"justifyContent": "start"
		},
		"parentName": "TopAreaProfileContainer",
		"propertyName": "items",
		"index": 3
	},
	{
		"operation": "insert",
		"name": "CopyUrlButton",
		"values": {
			"type": "crt.Button",
			"caption": "#ResourceString(CopyUrlButton_caption)#",
			"color": "default",
			"size": "medium",
			"clicked": {
				"request": "usr.CopyUrlRequest"
			},
			"iconPosition": "only-icon",
			"visible": true,
			"icon": "copy-icon",
			"clickMode": "default"
		},
		"parentName": "CopyUrlButtonContainer",
		"propertyName": "items",
		"index": 0
	},
	{
		"operation": "insert",
		"name": "DescriptionInput",
		"values": {
			"layoutConfig": {
				"column": 1,
				"row": 3,
				"colSpan": 10,
				"rowSpan": 1
			},
			"type": "crt.Input",
			"label": "#ResourceString(DescriptionInput_label)#",
			"control": "$PDS_Description",
			"multiline": true,
			"autoHeight": true,
			"labelPosition": "auto"
		},
		"parentName": "TopAreaProfileContainer",
		"propertyName": "items",
		"index": 4
	},
	{
		"operation": "merge",
		"name": "GridContainer_uxln7d4",
		"values": {
			"columns": [
				"minmax(0, 1fr)"
			],
			"rows": "min-content minmax(0, 1fr)",
			"fitContent": false,
			"styles": {
				"flex": "1 1 auto",
				"min-height": "0",
				"overflow": "hidden"
			}
		}
	},
	{
		"operation": "insert",
		"name": "ToolsToolsFlexContainer",
		"values": {
			"type": "crt.FlexContainer",
			"direction": "row",
			"gap": "small",
			"alignItems": "center",
			"justifyContent": "start",
			"layoutConfig": {
				"colSpan": 1,
				"column": 1,
				"row": 1,
				"rowSpan": 1
			},
			"padding": {
				"top": "none",
				"right": "none",
				"bottom": "small",
				"left": "none"
			},
			"items": []
		},
		"parentName": "GridContainer_uxln7d4",
		"propertyName": "items",
		"index": 0
	},
	{
		"operation": "insert",
		"name": "ToolsAddButton",
		"values": {
			"type": "crt.Button",
			"caption": "#ResourceString(ToolsAddButton_caption)#",
			"icon": "add-button-icon",
			"iconPosition": "left-icon",
			"color": "default",
			"size": "medium",
			"clickMode": "menu",
			"menuItems": []
		},
		"parentName": "ToolsToolsFlexContainer",
		"propertyName": "items",
		"index": 0
	},
	{
		"operation": "insert",
		"name": "AddToolFromProcessMenuItem",
		"values": {
			"type": "crt.MenuItem",
			"caption": "#ResourceString(AddToolFromProcessMenuItem_caption)#",
			"icon": "business-process",
			"clicked": {
				"request": "usr.AddToolFromProcessRequest"
			}
		},
		"parentName": "ToolsAddButton",
		"propertyName": "menuItems",
		"index": 0
	},
	{
		"operation": "insert",
		"name": "AddToolFromSourceActionMenuItem",
		"values": {
			"type": "crt.MenuItem",
			"caption": "#ResourceString(AddToolFromSourceActionMenuItem_caption)#",
			"icon": "codeblock-icon",
			"clicked": {
				"request": "usr.AddToolFromSourceActionRequest"
			}
		},
		"parentName": "ToolsAddButton",
		"propertyName": "menuItems",
		"index": 1
	},
	{
		"operation": "insert",
		"name": "ToolsRefreshButton",
		"values": {
			"type": "crt.Button",
			"caption": "#ResourceString(ToolsRefreshButton_caption)#",
			"icon": "reload-button-icon",
			"iconPosition": "only-icon",
			"color": "default",
			"size": "medium",
			"clicked": {
				"request": "crt.LoadDataRequest",
				"params": {
					"config": {
						"loadType": "reload",
						"useLastLoadParameters": true
					},
					"dataSourceName": "DataGrid_toolsDS"
				}
			}
		},
		"parentName": "ToolsToolsFlexContainer",
		"propertyName": "items",
		"index": 1
	},
	{
		"operation": "insert",
		"name": "ToolsSettingsButton",
		"values": {
			"type": "crt.Button",
			"caption": "#ResourceString(ToolsSettingsButton_caption)#",
			"icon": "actions-button-icon",
			"iconPosition": "only-icon",
			"color": "default",
			"size": "medium",
			"clickMode": "menu",
			"menuItems": [],
			"visible": true
		},
		"parentName": "ToolsToolsFlexContainer",
		"propertyName": "items",
		"index": 2
	},
	{
		"operation": "insert",
		"name": "ToolsExportButton",
		"values": {
			"type": "crt.MenuItem",
			"caption": "#ResourceString(ExportToExcel_caption)#",
			"icon": "export-button-icon",
			"clicked": {
				"request": "crt.ExportDataGridToExcelRequest",
				"params": {
					"viewName": "DataGrid_tools"
				}
			}
		},
		"parentName": "ToolsSettingsButton",
		"propertyName": "menuItems",
		"index": 0
	},
	{
		"operation": "insert",
		"name": "ToolsSearchFilter",
		"values": {
			"type": "crt.SearchFilter",
			"placeholder": "#ResourceString(ToolsSearchFilter_placeholder)#",
			"iconOnly": true,
			"_filterOptions": {
				"expose": [
					{
						"attribute": "ToolsSearchFilter_DataGrid_tools",
						"converters": [
							{
								"converter": "crt.SearchFilterAttributeConverter",
								"args": [
									"DataGrid_tools"
								]
							}
						]
					}
				],
				"from": [
					"ToolsSearchFilter_SearchValue",
					"ToolsSearchFilter_FilteredColumnsGroups"
				]
			}
		},
		"parentName": "ToolsToolsFlexContainer",
		"propertyName": "items",
		"index": 3
	},
	{
		"operation": "insert",
		"name": "DataGrid_tools",
		"values": {
			"type": "crt.DataGrid",
			"layoutConfig": {
				"colSpan": 1,
				"column": 1,
				"row": 2,
				"rowSpan": 1
			},
			"items": "$DataGrid_tools",
			"primaryColumnName": "DataGrid_toolsDS_Id",
			"fitContent": false,
			"features": {
				"editable": {
					"enable": false,
					"itemsCreation": false,
					"floatingEditPanel": false
				},
				"rows": {
					"selection": {
						"enable": true,
						"multiple": true
					}
				}
			},
			"columns": [
				{
					"id": "b1f0a2c4-1111-4aaa-9bbb-000000000002",
					"code": "DataGrid_toolsDS_ExternalName",
					"caption": "#ResourceString(Col_ExternalName_caption)#",
					"dataValueType": 1,
					"width": 314
				},
				{
					"id": "b1f0a2c4-1111-4aaa-9bbb-000000000005",
					"code": "DataGrid_toolsDS_Title",
					"caption": "#ResourceString(Col_Title_caption)#",
					"dataValueType": 1,
					"width": 385
				},
				{
					"id": "b1f0a2c4-1111-4aaa-9bbb-000000000003",
					"code": "DataGrid_toolsDS_IsEnabled",
					"caption": "#ResourceString(Col_IsEnabled_caption)#",
					"dataValueType": 12,
					"width": 139
				}
			],
			"selectionState": "$DataGrid_tools_SelectionState",
			"_selectionOptions": {
				"attribute": "DataGrid_tools_SelectionState"
			},
			"bulkActions": [],
			"visible": true
		},
		"parentName": "GridContainer_uxln7d4",
		"propertyName": "items",
		"index": 1
	},
	{
		"operation": "insert",
		"name": "DataGrid_tools_AddTagsBulkAction",
		"values": {
			"type": "crt.MenuItem",
			"caption": "#ResourceString(AddTagsBulkAction_caption)#",
			"icon": "tag-icon",
			"clicked": {
				"request": "crt.AddTagsInRecordsRequest",
				"params": {
					"dataSourceName": "DataGrid_toolsDS",
					"filters": "$DataGrid_tools | crt.ToCollectionFilters : 'DataGrid_tools' : $DataGrid_tools_SelectionState | crt.SkipIfSelectionEmpty : $DataGrid_tools_SelectionState"
				}
			},
			"items": []
		},
		"parentName": "DataGrid_tools",
		"propertyName": "bulkActions",
		"index": 0
	},
	{
		"operation": "insert",
		"name": "DataGrid_tools_RemoveTagsBulkAction",
		"values": {
			"type": "crt.MenuItem",
			"caption": "#ResourceString(RemoveTagsBulkAction_caption)#",
			"icon": "delete-button-icon",
			"clicked": {
				"request": "crt.RemoveTagsInRecordsRequest",
				"params": {
					"dataSourceName": "DataGrid_toolsDS",
					"filters": "$DataGrid_tools | crt.ToCollectionFilters : 'DataGrid_tools' : $DataGrid_tools_SelectionState | crt.SkipIfSelectionEmpty : $DataGrid_tools_SelectionState"
				}
			}
		},
		"parentName": "DataGrid_tools_AddTagsBulkAction",
		"propertyName": "items",
		"index": 0
	},
	{
		"operation": "insert",
		"name": "DataGrid_tools_ExportToExcelBulkAction",
		"values": {
			"type": "crt.MenuItem",
			"caption": "#ResourceString(ExportToExcel_caption)#",
			"icon": "export-button-icon",
			"clicked": {
				"request": "crt.ExportDataGridToExcelRequest",
				"params": {
					"viewName": "DataGrid_tools",
					"filters": "$DataGrid_tools | crt.ToCollectionFilters : 'DataGrid_tools' : $DataGrid_tools_SelectionState | crt.SkipIfSelectionEmpty : $DataGrid_tools_SelectionState"
				}
			}
		},
		"parentName": "DataGrid_tools",
		"propertyName": "bulkActions",
		"index": 1
	},
	{
		"operation": "insert",
		"name": "DataGrid_tools_DeleteBulkAction",
		"values": {
			"type": "crt.MenuItem",
			"caption": "#ResourceString(DeleteBulkAction_caption)#",
			"icon": "delete-button-icon",
			"clicked": {
				"request": "crt.DeleteRecordsRequest",
				"params": {
					"dataSourceName": "DataGrid_toolsDS",
					"filters": "$DataGrid_tools | crt.ToCollectionFilters : 'DataGrid_tools' : $DataGrid_tools_SelectionState | crt.SkipIfSelectionEmpty : $DataGrid_tools_SelectionState"
				}
			}
		},
		"parentName": "DataGrid_tools",
		"propertyName": "bulkActions",
		"index": 2
	},
	{
		"operation": "insert",
		"name": "AttachmentsTab",
		"values": {
			"type": "crt.TabContainer",
			"caption": "#ResourceString(AttachmentsTab_caption)#",
			"items": [],
			"iconPosition": "only-text"
		},
		"parentName": "Tabs",
		"propertyName": "items",
		"index": 1
	},
	{
		"operation": "insert",
		"name": "AttachmentsTabGrid",
		"values": {
			"type": "crt.GridContainer",
			"rows": "minmax(32px, max-content)",
			"columns": [
				"minmax(32px, 1fr)"
			],
			"gap": {
				"columnGap": "large",
				"rowGap": "none"
			},
			"items": [
				{
					"type": "crt.FileList",
					"masterRecordColumnValue": "$Id",
					"recordColumnName": "RecordId",
					"layoutConfig": {
						"colSpan": 1,
						"column": 1,
						"row": 1,
						"rowSpan": 1
					},
					"items": "$AttachmentList",
					"primaryColumnName": "AttachmentListDS_Id",
					"columns": [
						{
							"id": "17143c9b-2ad9-46b4-bf21-9c55b13a050a",
							"code": "AttachmentListDS_Name",
							"caption": "#ResourceString(AttachmentCol_Name_caption)#",
							"dataValueType": 28,
							"width": 200
						}
					],
					"viewType": "gallery",
					"tileSize": "small",
					"name": "AttachmentList"
				}
			]
		},
		"parentName": "AttachmentsTab",
		"propertyName": "items",
		"index": 0
	},
	{
		"operation": "insert",
		"name": "FeedTab",
		"values": {
			"type": "crt.TabContainer",
			"caption": "#ResourceString(FeedTab_caption)#",
			"items": [],
			"iconPosition": "only-text"
		},
		"parentName": "Tabs",
		"propertyName": "items",
		"index": 2
	},
	{
		"operation": "insert",
		"name": "FeedTabGrid",
		"values": {
			"type": "crt.GridContainer",
			"rows": "minmax(32px, max-content)",
			"columns": [
				"minmax(32px, 1fr)"
			],
			"gap": {
				"columnGap": "large",
				"rowGap": "none"
			},
			"items": [
				{
					"type": "crt.Feed",
					"feedType": "Record",
					"primaryColumnValue": "$Id",
					"cardState": "$CardState",
					"dataSourceName": "PDS",
					"entitySchemaName": "McpServer",
					"layoutConfig": {
						"colSpan": 1,
						"column": 1,
						"row": 1,
						"rowSpan": 1
					},
					"name": "Feed"
				}
			]
		},
		"parentName": "FeedTab",
		"propertyName": "items",
		"index": 0
	}
]
			/**SCHEMA_VIEW_CONFIG_DIFF*/,
		viewModelConfigDiff: /**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/
[
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
				},
				"validators": {
					"CodeValidator": {
						"type": "usr.CodeValidator",
						"params": {
							"message": "#ResourceString(CodeValidationMessage)#"
						}
					}
				}
			},
			"PDS_IsOnline": {
				"modelConfig": {
					"path": "PDS.IsOnline"
				}
			},
			"PDS_Description": {
				"modelConfig": {
					"path": "PDS.Description"
				}
			},
			"UsrServerUrlRaw": {},
			"ToolsMasterFilter": {},
			"ProcessFilter": {},
			"DataGrid_tools": {
				"isCollection": true,
				"modelConfig": {
					"path": "DataGrid_toolsDS",
					"filterAttributes": [
						{
							"name": "ToolsMasterFilter",
							"loadOnChange": true
						},
						{
							"name": "ToolsSearchFilter_DataGrid_tools",
							"loadOnChange": true
						}
					]
				},
				"viewModelConfig": {
					"attributes": {
						"DataGrid_toolsDS_ExternalName": {
							"modelConfig": {
								"path": "DataGrid_toolsDS.ExternalName"
							}
						},
						"DataGrid_toolsDS_Title": {
							"modelConfig": {
								"path": "DataGrid_toolsDS.Title"
							}
						},
						"DataGrid_toolsDS_IsEnabled": {
							"modelConfig": {
								"path": "DataGrid_toolsDS.IsEnabled"
							}
						},
						"DataGrid_toolsDS_Id": {
							"modelConfig": {
								"path": "DataGrid_toolsDS.Id"
							}
						}
					}
				}
			},
			"AttachmentList": {
				"isCollection": true,
				"modelConfig": {
					"path": "AttachmentListDS",
					"sortingConfig": {
						"default": [
							{
								"columnName": "CreatedOn",
								"direction": "desc"
							}
						]
					}
				},
				"viewModelConfig": {
					"attributes": {
						"AttachmentListDS_Id": {
							"modelConfig": {
								"path": "AttachmentListDS.Id"
							}
						},
						"AttachmentListDS_Name": {
							"modelConfig": {
								"path": "AttachmentListDS.Name"
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
]
			/**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/,
		modelConfigDiff: /**SCHEMA_MODEL_CONFIG_DIFF*/
[
	{
		"operation": "merge",
		"path": [],
		"values": {
			"dataSources": {
				"PDS": {
					"type": "crt.EntityDataSource",
					"config": {
						"entitySchemaName": "McpServer"
					},
					"scope": "page"
				},
				"AttachmentListDS": {
					"type": "crt.EntityDataSource",
					"scope": "viewElement",
					"config": {
						"entitySchemaName": "SysFile",
						"attributes": {
							"Name": {
								"path": "Name"
							}
						}
					}
				},
				"DataGrid_toolsDS": {
					"type": "crt.EntityDataSource",
					"scope": "viewElement",
					"config": {
						"entitySchemaName": "McpTool",
						"attributes": {
							"ExternalName": {
								"path": "ExternalName"
							},
							"Title": {
								"path": "Title"
							},
							"IsEnabled": {
								"path": "IsEnabled"
							}
						}
					}
				}
			},
			"primaryDataSourceName": "PDS"
		}
	}
]
			/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[
			{
				request: "usr.AddToolFromSourceActionRequest",
				handler: async (request, next) => {
					const { $context } = request;
					let saved = false;
					try {
						saved = await sdk.HandlerChainService.instance.process({
							type: "crt.SaveRecordRequest",
							$context,
							scopes: [...request.scopes],
							preventCardClose: true
						});
					} catch (e) { saved = false; }
					if (!saved) { return next?.handle(request); }
					const serverId = await $context["Id"];
					// Canonical id: McpToolSourceType "Source code action" seed row / C#
					// McpToolSourceTypes.SourceCodeAction. Keep in sync if that id changes.
					const SOURCE_CODE_ACTION_TYPE_ID = "ebed4287-7fd3-4d75-843f-c2ea0b795e66";
					const slug = function (text) {
						return (text || "").toLowerCase().replace(/[^a-z0-9]+/g, "_").replace(/^_+|_+$/g, "");
					};
					const formatJson = function (jsonString) {
						if (!jsonString) { return ""; }
						try { return JSON.stringify(JSON.parse(jsonString), null, 2); } catch (e) { return jsonString; }
					};
					const httpClientService = new sdk.HttpClientService();
					await sdk.HandlerChainService.instance.process({
						type: "crt.OpenSelectionWindowRequest",
						$context,
						scopes: [...request.scopes],
						entitySchemaName: "McpSourceCodeAction",
						schemaName: "McpSourceActionSelectionPage",
						caption: resources.localizableStrings.SelectSourceActionWindowCaption,
						features: { select: { multiple: true } },
						afterClosed: async (result) => {
							if (!result || result.canceled) { return; }
							const values = await result.getLookupValues();
							if (!values || !values.length) { return; }
							const mask = new sdk.MaskService();
							mask.showBodyMask();
							let addedCount = 0;
							let failedCount = 0;
							try {
								const actionModel = await sdk.Model.create("McpSourceCodeAction");
								const toolModel = await sdk.Model.create("McpTool");
								const toolsToInsert = [];
								for (let i = 0; i < values.length; i++) {
									const sel = values[i];
									const rowId = sel.value || sel.Id || sel.id;
									let fullTypeName = null;
									let caption = sel.displayValue || sel.displayColumnValue || "";
									let description = "";
									try {
										const f = new sdk.FilterGroup();
										await f.addSchemaColumnFilterWithParameter(sdk.ComparisonType.Equal, "Id", rowId);
										const rows = await actionModel.load({ attributes: ["Id", "Name", "FullTypeName", "Description"], parameters: [{ type: sdk.ModelParameterType.Filter, value: f }] });
										if (rows && rows.length) {
											fullTypeName = rows[0].FullTypeName;
											caption = rows[0].Name || caption;
											description = rows[0].Description || "";
										}
									} catch (e) { /* fall back to selection display value */ }
									if (!fullTypeName) { failedCount = failedCount + 1; continue; }
									let inputSchema = "";
									let outputSchema = "";
									try {
										const resp = await httpClientService.post("/rest/McpToolSchemaService/GetToolSchemas", { sourceType: "sourcecode", identifier: fullTypeName });
										const data = resp && resp.body;
										const inner = (data && data.GetToolSchemasResult !== undefined) ? data.GetToolSchemasResult : data;
										const schemas = (typeof inner === "string") ? JSON.parse(inner) : inner;
										if (schemas) {
											inputSchema = formatJson(schemas.inputSchema);
											outputSchema = formatJson(schemas.outputSchema);
										}
									} catch (e) { /* leave schemas empty if the service is unavailable */ }
									toolsToInsert.push({
										McpServer: serverId,
										ToolSourceType: SOURCE_CODE_ACTION_TYPE_ID,
										SourceCodeAction: fullTypeName,
										ExternalName: slug(caption),
										Title: caption,
										Description: description || caption,
										InputSchema: inputSchema,
										OutputSchema: outputSchema,
										IsEnabled: true
									});
								}
								if (toolsToInsert.length) {
									try {
										const transaction = await sdk.TransactionFactoryService.create();
										toolsToInsert.forEach(function (toolValues) {
											toolModel.insertInTransaction(transaction, toolValues);
										});
										await transaction.commit();
										addedCount = addedCount + toolsToInsert.length;
									} catch (e) {
										for (let j = 0; j < toolsToInsert.length; j++) {
											try { await toolModel.insert(toolsToInsert[j]); addedCount = addedCount + 1; }
											catch (err) { failedCount = failedCount + 1; }
										}
									}
								}
							} finally {
								mask.hideBodyMask();
							}
							await $context.executeRequest({
								type: "crt.LoadDataRequest",
								$context,
								scopes: [...request.scopes],
								config: { loadType: "reload", useLastLoadParameters: true },
								dataSourceName: "DataGrid_toolsDS"
							});
							if (failedCount === 0 && addedCount > 0) {
								await $context.executeRequest({ type: "crt.NotificationRequest", message: resources.localizableStrings.ToolsAddedMessage });
							} else if (addedCount > 0 && failedCount > 0) {
								await $context.executeRequest({ type: "crt.NotificationRequest", message: addedCount + resources.localizableStrings.ToolsPartialAddedMiddle + failedCount + resources.localizableStrings.ToolsPartialAddedSuffix });
							} else if (failedCount > 0) {
								await $context.executeRequest({ type: "crt.NotificationRequest", message: resources.localizableStrings.ToolsAddFailedMessage });
							}
						}
					});
					return next?.handle(request);
				}
			},
			{
				request: "crt.HandleViewModelInitRequest",
				handler: async (request, next) => {
					await next?.handle(request);
					const { $context } = request;
					const recordId = await $context["Id"];
					const filters = new sdk.FilterGroup();
					await filters.addSchemaColumnFilterWithParameter(sdk.ComparisonType.Equal, "McpServer", recordId || "00000000-0000-0000-0000-000000000000");
					await $context.set("ToolsMasterFilter", filters);
					const initCode = await $context["PDS_Code"];
					const initBase = window.location.origin + window.location.pathname.split("/0/")[0];
					await $context.set("UsrServerUrlRaw", initBase + "/0/rest/ToolServiceMcp/" + (initCode || "") + "/v1/mcp");
					const procFilters = new sdk.FilterGroup();
					await procFilters.addSchemaColumnFilterWithParameter(sdk.ComparisonType.Equal, "ManagerName", "ProcessSchemaManager");
					await $context.set("ProcessFilter", procFilters);
				}
			},
			{
				request: "crt.HandleViewModelAttributeChangeRequest",
				handler: async (request, next) => {
					const { $context, attributeName, value } = request;
					if (attributeName === "Name" && (await $context["CardState"]) !== "edit") {
						const generated = ((value || "").match(/[a-zA-Z0-9]+/g) || []).map(w => w.charAt(0).toUpperCase() + w.slice(1)).join("") || "CustomMCP";
						$context.set("PDS_Code", generated);
					}
					if (attributeName === "PDS_Code") {
						const base = window.location.origin + window.location.pathname.split("/0/")[0];
						await $context.set("UsrServerUrlRaw", base + "/0/rest/ToolServiceMcp/" + (value || "") + "/v1/mcp");
					}
					if (attributeName === "Id") {
						const filters = new sdk.FilterGroup();
						await filters.addSchemaColumnFilterWithParameter(sdk.ComparisonType.Equal, "McpServer", value || "00000000-0000-0000-0000-000000000000");
						await $context.set("ToolsMasterFilter", filters);
					}
					return next?.handle(request);
				}
			},
			{
				request: "crt.SaveRecordRequest",
				handler: async (request, next) => {
					const { $context } = request;
					if ((await $context["CardState"]) !== "edit") {
						const name = await $context["Name"];
						const generated = ((name || "").match(/[a-zA-Z0-9]+/g) || []).map(w => w.charAt(0).toUpperCase() + w.slice(1)).join("");
						if (generated) {
							await $context.set("PDS_Code", generated);
						}
					}
					return next?.handle(request);
				}
			},
			{
				request: "usr.ToggleOnlineRequest",
				handler: async (request, next) => {
					const { $context } = request;
					const current = await $context["PDS_IsOnline"];
					await $context.set("PDS_IsOnline", !current);
					const saved = await sdk.HandlerChainService.instance.process({
						type: "crt.SaveRecordRequest",
						$context,
						scopes: [...request.scopes]
					});
					if (!saved) {
						await $context.set("PDS_IsOnline", current);
					}
					return next?.handle(request);
				}
			},
			{
				request: "usr.CopyUrlRequest",
				handler: async (request, next) => {
					const { $context } = request;
					const codeVal = await $context["PDS_Code"];
					const urlBase = window.location.origin + window.location.pathname.split("/0/")[0];
					const url = urlBase + "/0/rest/ToolServiceMcp/" + (codeVal || "") + "/v1/mcp";
					// crt.CopyClipboardRequest already shows a "Copied" notification,
					// so we don't raise our own to avoid a duplicate snackbar.
					await sdk.HandlerChainService.instance.process({
						type: "crt.CopyClipboardRequest",
						$context,
						scopes: [...request.scopes],
						value: url
					});
					return next?.handle(request);
				}
			},
									{
				request: "usr.AddToolFromProcessRequest",
				handler: async (request, next) => {
					const { $context } = request;
					let saved = false;
					try {
						saved = await sdk.HandlerChainService.instance.process({
							type: "crt.SaveRecordRequest",
							$context,
							scopes: [...request.scopes],
							preventCardClose: true
						});
					} catch (e) { saved = false; }
					if (!saved) { return next?.handle(request); }
					const serverId = await $context["Id"];
					const formatJson = function (jsonString) {
						if (!jsonString) { return ""; }
						try { return JSON.stringify(JSON.parse(jsonString), null, 2); } catch (e) { return jsonString; }
					};
					await sdk.HandlerChainService.instance.process({
						type: "crt.OpenSelectionWindowRequest",
						$context,
						scopes: [...request.scopes],
						entitySchemaName: "VwProcessLib",
						schemaName: "McpProcessSelectionPage",
						caption: resources.localizableStrings.SelectProcessWindowCaption,
						features: { select: { multiple: true } },
						afterClosed: async (result) => {
							if (!result || result.canceled) { return; }
							const values = await result.getLookupValues();
							if (!values || !values.length) { return; }
							const mask = new sdk.MaskService();
							mask.showBodyMask();
							let addedCount = 0;
							let failedCount = 0;
							try {
								const httpClientService = new sdk.HttpClientService();
								const procModel = await sdk.Model.create("VwProcessLib");
								const toolModel = await sdk.Model.create("McpTool");
								const toolsToInsert = [];
								for (let i = 0; i < values.length; i++) {
									const sel = values[i];
									const processLibId = sel.value || sel.Id || sel.id;
									let sysSchemaId = null;
									let caption = sel.displayValue || sel.displayColumnValue || "";
									let description = "";
									let processName = "";
									try {
										const f = new sdk.FilterGroup();
										await f.addSchemaColumnFilterWithParameter(sdk.ComparisonType.Equal, "Id", processLibId);
										const rows = await procModel.load({ attributes: ["Id", "Name", "Caption", "SysSchemaId"], parameters: [{ type: sdk.ModelParameterType.Filter, value: f }] });
										if (rows && rows.length) { sysSchemaId = rows[0].SysSchemaId; caption = rows[0].Caption || caption; processName = rows[0].Name || ""; }
									} catch (e) { /* fall back to selection display value */ }
									if (!sysSchemaId) { failedCount = failedCount + 1; continue; }
									const externalName = (caption || "").toLowerCase().replace(/[^a-z0-9]+/g, "_").replace(/^_+|_+$/g, "");
									let inputSchema = "";
									let outputSchema = "";
									try {
										const resp = await httpClientService.post("/rest/McpToolSchemaService/GetToolSchemas", { sourceType: "process", identifier: processName });
										const data = resp && resp.body;
										const inner = (data && data.GetToolSchemasResult !== undefined) ? data.GetToolSchemasResult : data;
										const schemas = (typeof inner === "string") ? JSON.parse(inner) : inner;
										if (schemas) {
											inputSchema = formatJson(schemas.inputSchema);
											outputSchema = formatJson(schemas.outputSchema);
											// Process description (from the designer) comes back with the schemas;
											// the DB-level VwProcessLib.Description is virtually always empty.
											if (schemas.description) { description = schemas.description; }
										}
									} catch (e) { /* leave schemas empty if the service is unavailable */ }
									toolsToInsert.push({
											McpServer: serverId,
											BusinessProcess: sysSchemaId,
											ExternalName: externalName,
											Title: caption,
											Description: description || caption,
											InputSchema: inputSchema,
											OutputSchema: outputSchema,
											IsEnabled: true
										});
								}
								// Insert all selected tools within a single transaction so the grid data
								// source observes one consistent change. Inserting them one-by-one makes
								// each insert trigger its own asynchronous grid load by id; those loads
								// race with (and abort) one another, so freshly added rows are intermittently
								// missing until a manual refresh.
								if (toolsToInsert.length) {
									try {
										const transaction = await sdk.TransactionFactoryService.create();
										toolsToInsert.forEach(function (toolValues) {
											toolModel.insertInTransaction(transaction, toolValues);
										});
										await transaction.commit();
										addedCount = addedCount + toolsToInsert.length;
									} catch (e) {
										// Fallback when batch transactions are unavailable: insert sequentially.
										for (let j = 0; j < toolsToInsert.length; j++) {
											try { await toolModel.insert(toolsToInsert[j]); addedCount = addedCount + 1; }
											catch (err) { failedCount = failedCount + 1; }
										}
									}
								}
							} finally {
								mask.hideBodyMask();
							}
							await $context.executeRequest({
								type: "crt.LoadDataRequest",
								$context,
								scopes: [...request.scopes],
								config: { loadType: "reload", useLastLoadParameters: true },
								dataSourceName: "DataGrid_toolsDS"
							});
							if (failedCount === 0 && addedCount > 0) {
								await $context.executeRequest({ type: "crt.NotificationRequest", message: resources.localizableStrings.ToolsAddedMessage });
							} else if (addedCount > 0 && failedCount > 0) {
								await $context.executeRequest({ type: "crt.NotificationRequest", message: addedCount + resources.localizableStrings.ToolsPartialAddedMiddle + failedCount + resources.localizableStrings.ToolsPartialAddedSuffix });
							} else if (failedCount > 0) {
								await $context.executeRequest({ type: "crt.NotificationRequest", message: resources.localizableStrings.ToolsAddFailedMessage });
							}
						}
					});
					return next?.handle(request);
				}
			}
		]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{
			"usr.CodeValidator": {
				"validator": function (config) {
					return function (control) {
						const value = control.value;
						if (!value) {
							return null;
						}
						const isValid = /^[A-Za-z0-9]+$/.test(value);
						const message = resources.localizableStrings.CodeValidationMessage || config.message;
						return isValid ? null : { "usr.CodeValidator": { message: message } };
					};
				},
				"params": [{ "name": "message" }],
				"async": false
			}
		}/**SCHEMA_VALIDATORS*/
	};
});