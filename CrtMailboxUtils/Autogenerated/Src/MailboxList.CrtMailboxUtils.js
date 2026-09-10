define("MailboxList", /**SCHEMA_DEPS*/["@creatio-devkit/common"]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/(sdk)/**SCHEMA_ARGS*/ {
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
				"name": "ActionButtonsContainer"
			},
			{
				"operation": "remove",
				"name": "AddButton"
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
					"alignItems": "stretch"
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
				"operation": "remove",
				"name": "DataTable_Summaries"
			},
			{
				"operation": "remove",
				"name": "MainButtonToggleGroup"
			},
			{
				"operation": "remove",
				"name": "FolderTree"
			},
			{
				"operation": "remove",
				"name": "MainTabPanel"
			},
			{
				"operation": "remove",
				"name": "ListTabContainer"
			},
			{
				"operation": "remove",
				"name": "ListContainer"
			},
			{
				"operation": "merge",
				"name": "DataTable",
				"values": {
					"columns": [
						{
							"id": "8ff318e6-1568-ee0d-757c-34fa17d8c221",
							"code": "PDS_SenderEmailAddress",
							"caption": "#ResourceString(PDS_SenderEmailAddress)#",
							"dataValueType": 28
						},
						{
							"id": "5501713a-9433-1f18-661a-d86eb7043611",
							"code": "PDS_MailServer",
							"caption": "#ResourceString(PDS_MailServer)#",
							"dataValueType": 10
						},
						{
							"id": "da32bc64-19c4-815d-9d07-393eba0676bb",
							"code": "PDS_MailServer_Type",
							"caption": "#ResourceString(PDS_MailServer_Type)#",
							"dataValueType": 10,
							"width": 250
						},
						{
							"id": "c558ab81-bc8d-8a54-097e-bd6e3e7a3771",
							"code": "PDS_MailServer_UseImpersonation",
							"caption": "#ResourceString(PDS_MailServer_UseImpersonation)#",
							"dataValueType": 12,
							"width": 200
						}
					],
					"placeholder": false,
					"features": {
						"rows": {
							"selection": {
								"enable": true,
								"multiple": true
							}
						},
						"columns": {
							"resizing": true,
							"dragAndDrop": false
						},
						"editable": {
							"enable": false,
							"itemsCreation": false,
							"floatingEditPanel": false
						}
					},
					"selectionState": "$DataTable_SelectionState",
					"_selectionOptions": {
						"attribute": "DataTable_SelectionState"
					},
					"bulkActions": []
				}
			},
			{
				"operation": "move",
				"name": "DataTable",
				"parentName": "SectionContentWrapper",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "remove",
				"name": "DashboardsTabContainer"
			},
			{
				"operation": "remove",
				"name": "DashboardsContainer"
			},
			{
				"operation": "remove",
				"name": "Dashboards"
			},
			{
				"operation": "insert",
				"name": "GridContainer_l6b4arb",
				"values": {
					"type": "crt.GridContainer",
					"columns": [
						"minmax(32px, 1fr)",
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
					"color": "primary",
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
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NewMailServer",
				"values": {
					"type": "crt.ComboBox",
					"label": "#ResourceString(NewMailServer_label)#",
					"ariaLabel": "#ResourceString(NewMailServer_ariaLabel)#",
					"isAddAllowed": true,
					"showValueAsLink": false,
					"labelPosition": "left",
					"controlActions": [],
					"listActions": [],
					"showList": {
						"request": "crt.ComboboxLoadDataRequest",
						"params": {
							"dataSourceName": "MailServerListDS",
							"parameters": [],
							"primaryDisplayFilterValue": "@event.filterValue",
							"additionalFilteredColumnPaths": [],
							"config": {
								"loadType": "reload"
							}
						},
						"useRelativeContext": true
					},
					"value": "$NewMailServer",
					"items": "$MailServerList",
					"tooltip": "",
					"visible": true,
					"readonly": false,
					"placeholder": "",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "GridContainer_l6b4arb",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "SetMigrateToMailServerButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(SetMigrateToMailServerButton_caption)#",
					"color": "outline",
					"clicked": {
						"request": "crt.SetMigrateToMailServerRequest"
					},
					"disabled": "$IsSetMigrateToMailServerButtonDisabled",
					"size": "large",
					"iconPosition": "only-text",
					"visible": true,
					"layoutConfig": {
						"column": 2,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "GridContainer_l6b4arb",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "QuickFilter_trbci50",
				"values": {
					"type": "crt.QuickFilter",
					"config": {
						"caption": "#ResourceString(QuickFilter_trbci50_config_caption)#",
						"hint": "",
						"icon": "filter-column-icon",
						"iconPosition": "left-icon",
						"defaultValue": [],
						"entitySchemaName": "MailServer",
						"recordsFilter": {
							"items": {
								"44ea07fe-e168-42a0-8936-590707146f49": {
									"filterType": 4,
									"comparisonType": 3,
									"isEnabled": true,
									"trimDateTimeParameterToDate": false,
									"leftExpression": {
										"expressionType": 0,
										"columnPath": "Type"
									},
									"isAggregative": false,
									"dataValueType": 10,
									"referenceSchemaName": "MailServerType",
									"rightExpressions": [
										{
											"expressionType": 2,
											"parameter": {
												"dataValueType": 10,
												"value": {
													"Name": "Office 365 (Graph API)",
													"Id": "7b9a3c2d-5e6f-4a1b-8c9d-2e3f4a5b6c7d",
													"value": "7b9a3c2d-5e6f-4a1b-8c9d-2e3f4a5b6c7d",
													"displayValue": "Office 365 (Graph API)"
												}
											}
										},
										{
											"expressionType": 2,
											"parameter": {
												"dataValueType": 10,
												"value": {
													"Name": "Office 365 / Exchange on-premise (legacy EWS API)",
													"Id": "3490bd45-4f4d-4613-aa06-454546f3342a",
													"value": "3490bd45-4f4d-4613-aa06-454546f3342a",
													"displayValue": "Office 365 / Exchange on-premise (legacy EWS API)"
												}
											}
										}
									]
								}
							},
							"logicalOperation": 0,
							"isEnabled": true,
							"filterType": 6,
							"rootSchemaName": "MailServer"
						}
					},
					"_filterOptions": {
						"expose": [
							{
								"attribute": "QuickFilter_trbci50_Items",
								"converters": [
									{
										"converter": "crt.QuickFilterAttributeConverter",
										"args": [
											{
												"target": {
													"viewAttributeName": "Items",
													"filterColumn": "MailServer"
												},
												"quickFilterType": "lookup"
											}
										]
									}
								]
							}
						],
						"from": "QuickFilter_trbci50_Value"
					},
					"filterType": "lookup"
				},
				"parentName": "LeftFilterContainerInner",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "MenuItem_h7ruh6b",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(MenuItem_h7ruh6b_caption)#",
					"visible": true,
					"disabled": "$IsSetMigrateToMailServerButtonDisabled | crt.InvertBooleanValue",
					"clicked": {
						"request": "crt.RunBulkMailboxMigrationRequest",
						"params": {
							"filters": "$Items | crt.ToCollectionFilters : 'Items' : $DataTable_SelectionState | crt.SkipIfSelectionEmpty : $DataTable_SelectionState",
							"sorting": "$ItemsSorting"
						}
					}
				},
				"parentName": "DataTable",
				"propertyName": "bulkActions",
				"index": 0
			},
			{
				"operation": "move",
				"name": "RightFilterContainer",
				"parentName": "MainFilterContainer",
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
					"NewMailServer": {},
					"SavedMigrateToMailServerId": {},
					"SavedMigrateToMailServerDisplayValue": {},
					"HasSavedMigrateToMailServer": {
						"value": false
					},
					"IsSetMigrateToMailServerButtonDisabled": {
						"value": true
					},
					"MailServerList": {
						"isCollection": true,
						"modelConfig": {
							"path": "MailServerListDS",
							"sortingConfig": {
								"default": []
							},
							"filterAttributes": [
								{
									"name": "MailServers_PredefinedFilter",
									"loadOnChange": true
								}
							]
						},
						"viewModelConfig": {
							"attributes": {
								"value": {
									"modelConfig": {
										"path": "MailServerListDS.Id"
									}
								},
								"displayValue": {
									"modelConfig": {
										"path": "MailServerListDS.Name"
									}
								}
							}
						}
					},
					"Items_PredefinedFilter": {
						"value": {
							"items": {
								"e0aaf84e-bb84-40b5-ab3d-f693ece96daa": {
									"filterType": 4,
									"comparisonType": 3,
									"isEnabled": true,
									"trimDateTimeParameterToDate": false,
									"leftExpression": {
										"expressionType": 0,
										"columnPath": "MailServer.Type"
									},
									"isAggregative": false,
									"dataValueType": 10,
									"referenceSchemaName": "MailServerType",
									"rightExpressions": [
										{
											"expressionType": 2,
											"parameter": {
												"dataValueType": 10,
												"value": {
													"Name": "Office 365 (Graph API)",
													"Id": "7b9a3c2d-5e6f-4a1b-8c9d-2e3f4a5b6c7d",
													"value": "7b9a3c2d-5e6f-4a1b-8c9d-2e3f4a5b6c7d",
													"displayValue": "Office 365 (Graph API)"
												}
											}
										},
										{
											"expressionType": 2,
											"parameter": {
												"dataValueType": 10,
												"value": {
													"Name": "Office 365 / Exchange on-premise (legacy EWS API)",
													"Id": "3490bd45-4f4d-4613-aa06-454546f3342a",
													"value": "3490bd45-4f4d-4613-aa06-454546f3342a",
													"displayValue": "Office 365 / Exchange on-premise (legacy EWS API)"
												}
											}
										}
									]
								}
							},
							"logicalOperation": 0,
							"isEnabled": true,
							"filterType": 6,
							"rootSchemaName": "MailboxSyncSettings"
						}
					},
					"MailServers_PredefinedFilter": {
						"value": {
							"items": {
								"e0aaf84e-bb84-40b5-ab3d-f693ece96dbb": {
									"filterType": 4,
									"comparisonType": 3,
									"isEnabled": true,
									"trimDateTimeParameterToDate": false,
									"leftExpression": {
										"expressionType": 0,
										"columnPath": "Type"
									},
									"isAggregative": false,
									"dataValueType": 10,
									"referenceSchemaName": "MailServerType",
									"rightExpressions": [
										{
											"expressionType": 2,
											"parameter": {
												"dataValueType": 10,
												"value": {
													"Name": "Office 365 (Graph API)",
													"Id": "7b9a3c2d-5e6f-4a1b-8c9d-2e3f4a5b6c7d",
													"value": "7b9a3c2d-5e6f-4a1b-8c9d-2e3f4a5b6c7d",
													"displayValue": "Office 365 (Graph API)"
												}
											}
										},
										{
											"expressionType": 2,
											"parameter": {
												"dataValueType": 10,
												"value": {
													"Name": "Office 365 / Exchange on-premise (legacy EWS API)",
													"Id": "3490bd45-4f4d-4613-aa06-454546f3342a",
													"value": "3490bd45-4f4d-4613-aa06-454546f3342a",
													"displayValue": "Office 365 / Exchange on-premise (legacy EWS API)"
												}
											}
										}
									]
								}
							},
							"logicalOperation": 0,
							"isEnabled": true,
							"filterType": 6,
							"rootSchemaName": "MailServer"
						}
					},
					"undefined_List": {
						"isCollection": true,
						"modelConfig": {}
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
					"PDS_SenderEmailAddress": {
						"modelConfig": {
							"path": "PDS.SenderEmailAddress"
						}
					},
					"PDS_MailServer": {
						"modelConfig": {
							"path": "PDS.MailServer"
						}
					},
					"PDS_MailServer_Type": {
						"modelConfig": {
							"path": "PDS.MailServer_Type"
						}
					},
					"PDS_MailServer_UseImpersonation": {
						"modelConfig": {
							"path": "PDS.MailServer_UseImpersonation"
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
							"name": "Items_PredefinedFilter",
							"loadOnChange": true
						},
						{
							"name": "SearchFilter_Items",
							"loadOnChange": true
						},
						{
							"name": "QuickFilter_trbci50_Items",
							"loadOnChange": true
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
							"columnName": "SenderEmailAddress"
						}
					]
				}
			},
			{
				"operation": "merge",
				"path": [
					"attributes",
					"FolderTree_visible"
				],
				"values": {
					"modelConfig": {}
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
					"MailServerListDS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "MailServer",
							"loadParameters": {
								"options": {
									"pagingConfig": {
										"rowCount": 15,
										"rowsOffset": -1
									},
									"sortingConfig": {
										"columns": []
									}
								}
							},
							"attributes": {
								"Id": {
									"path": "Id"
								},
								"Name": {
									"path": "Name"
								}
							}
						}
					}
				}
			},
			{
				"operation": "merge",
				"path": [
					"dataSources",
					"PDS",
					"config"
				],
				"values": {
					"entitySchemaName": "MailboxSyncSettings",
					"attributes": {
						"SenderEmailAddress": {
							"path": "SenderEmailAddress"
						},
						"MailServer": {
							"path": "MailServer"
						},
						"MailServer_Type": {
							"type": "ForwardReference",
							"path": "MailServer.Type"
						},
						"MailServer_UseImpersonation": {
							"type": "ForwardReference",
							"path": "MailServer.UseImpersonation"
						}
					}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[
			{
				request: "crt.HandleViewModelInitRequest",
				handler: async (request, next) => {
					const result = await next?.handle(request);
					let savedMailServerId = null;
					let savedMailServerDisplayValue = null;
					let newMailServer = null;
					try {
						const sysSettingsService = new sdk.SysSettingsService();
						const sysSetting = await sysSettingsService.getByCode("MigrateToMailServer");
						savedMailServerId = sysSetting?.value ?? null;
						savedMailServerDisplayValue = sysSetting?.displayValue ?? null;
						newMailServer = savedMailServerId
							? {
								value: savedMailServerId,
								displayValue: savedMailServerDisplayValue
							}
							: null;
					} catch (error) {
						console.error("Failed to load MigrateToMailServer.", error);
					}
					request.$context["SavedMigrateToMailServerId"] = savedMailServerId;
					request.$context["SavedMigrateToMailServerDisplayValue"] = savedMailServerDisplayValue;
					request.$context["HasSavedMigrateToMailServer"] = Boolean(savedMailServerId);
					request.$context["IsSetMigrateToMailServerButtonDisabled"] = true;
					request.$context["NewMailServer"] = newMailServer;
					return result;
				}
			},
			{
				request: "crt.HandleViewModelAttributeChangeRequest",
				handler: async (request, next) => {
					if (request.attributeName === "NewMailServer") {
						const selectedMailServer = await request.$context.NewMailServer;
						const selectedMailServerId = selectedMailServer?.value ?? null;
						const savedMailServerId = await request.$context.SavedMigrateToMailServerId;
						if (savedMailServerId === undefined) {
							request.$context["IsSetMigrateToMailServerButtonDisabled"] = true;
							return next?.handle(request);
						}
						request.$context["IsSetMigrateToMailServerButtonDisabled"] =
							selectedMailServerId === savedMailServerId;
					}
					return next?.handle(request);
				}
			},
			{
				request: "crt.SetMigrateToMailServerRequest",
				handler: async (request, next) => {
					const selectedMailServer = await request.$context.NewMailServer;
					const selectedMailServerId = selectedMailServer?.value ?? null;
					try {
						const sysSettingsService = new sdk.SysSettingsService();
						await sysSettingsService.update({
							code: "MigrateToMailServer",
							value: selectedMailServerId
						});
						const message = await request.$context.Resources.Strings.SetMigrateToMailServer_Result;
						request.$context["SavedMigrateToMailServerId"] = selectedMailServerId;
						request.$context["SavedMigrateToMailServerDisplayValue"] = selectedMailServer?.displayValue ?? null;
						request.$context["HasSavedMigrateToMailServer"] = Boolean(selectedMailServerId);
						request.$context["IsSetMigrateToMailServerButtonDisabled"] = true;
						request.$context["NewMailServer"] = selectedMailServer;
						await sdk.HandlerChainService.instance.process({
							type: "crt.NotificationRequest",
							$context: request.$context,
							scopes: request.scopes,
							message: message,
							duration: 5000
						});
					} catch (error) {
						console.error("Failed to save MigrateToMailServer.", error);
						const message = await request.$context.Resources.Strings.SetMigrateToMailServer_Error;
						await sdk.HandlerChainService.instance.process({
							type: "crt.NotificationRequest",
							$context: request.$context,
							scopes: request.scopes,
							message: message,
							duration: 5000
						});
					}
					return next?.handle(request);
				}
			},
			{
				request: "crt.RunBulkMailboxMigrationRequest",
				handler: async (request, next) => {
					const savedMailServerId = await request.$context.SavedMigrateToMailServerId;
					let savedMailServer = null;
					if (savedMailServerId) {
						const selectedMailServer = await request.$context.NewMailServer;
						if (selectedMailServer?.value === savedMailServerId) {
							savedMailServer = selectedMailServer;
						} else {
							const savedMailServerDisplayValue = await request.$context.SavedMigrateToMailServerDisplayValue;
							savedMailServer = {
								value: savedMailServerId,
								displayValue: savedMailServerDisplayValue ?? ""
							};
						}
					}
					if (!savedMailServer?.value) {
						const message = await request.$context.Resources.Strings.RunBulkMailboxMigration_MissingTarget;
						await sdk.HandlerChainService.instance.process({
							type: "crt.NotificationRequest",
							$context: request.$context,
							scopes: request.scopes,
							message: message,
							duration: 5000
						});
						return next?.handle(request);
					}
					const title = await request.$context.Resources.Strings.BulkMigrationConfirmationTitle;
					const prompt = await request.$context.Resources.Strings.BulkMigrationConfirmationPrompt;
					const cancelCaption = await request.$context.Resources.Strings.BulkMigrationCancelButtonCaption;
					const confirmCaption = await request.$context.Resources.Strings.BulkMigrationConfirmButtonCaption;
					const isConfirmed = await sdk.HandlerChainService.instance.process({
						type: "crt.ShowDialogRequest",
						$context: request.$context,
						scopes: request.scopes,
						dialogConfig: {
							data: {
								title: title,
								message: `${prompt} "${savedMailServer.displayValue}"?`,
								actions: [
									{
										key: "cancel",
										config: {
											color: "default",
											caption: cancelCaption
										}
									},
									{
										key: "confirm",
										config: {
											color: "primary",
											caption: confirmCaption
										}
									}
								]
							}
						}
					}) === "confirm";
					if (!isConfirmed) {
						return next?.handle(request);
					}
					const handlerChain = sdk.HandlerChainService.instance;
					await handlerChain.process({
						type: "crt.RunBusinessProcessRequest",
						$context: request.$context,
						scopes: request.scopes,
						processName: "CrtRunMultipleMailboxMailServerChange",
						processRunType: "ForTheSelectedRecords",
						saveAtProcessStart: true,
						showNotification: true,
						dataSourceName: "PDS",
						parameterMappings: {
							"Mailbox": "Id"
						},
						filters: request.filters,
						sorting: request.sorting,
						selectionStateAttributeName: "DataTable_SelectionState"
					});
					return next?.handle(request);
				}
			}
		]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});
