define("IdentityProviderSettings_Modal", /**SCHEMA_DEPS*/[]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/()/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"name": "PageTitle",
				"values": {
					"caption": "$IdentityProviderSettingsModalTitle",
					"visible": true
				}
			},
			{
				"operation": "merge",
				"name": "FooterContainer",
				"values": {
					"alignItems": "center",
					"color": "transparent",
					"borderRadius": "none",
					"gap": "small"
				}
			},
			{
				"operation": "move",
				"name": "CancelButton",
				"parentName": "FlexContainer_xjnzq3a",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "merge",
				"name": "CancelButton",
				"values": {
					"color": "default",
					"size": "large",
					"iconPosition": "only-text"
				}
			},
			{
				"operation": "move",
				"name": "SaveButton",
				"parentName": "FlexContainer_xjnzq3a",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Input_ltkesvw",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.SysIdentityProviderDS_Name_w81isky",
					"control": "$SysIdentityProviderDS_Name_w81isky",
					"placeholder": "",
					"tooltip": "#ResourceString(Input_ltkesvw_tooltip)#",
					"readonly": false,
					"multiline": false,
					"labelPosition": "above",
					"visible": true
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Input_xhlz464",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "#ResourceString(Input_xhlz464_label)#",
					"control": "$SysIdentityProviderDS_Description_nmatp93",
					"placeholder": "",
					"tooltip": "#ResourceString(Input_xhlz464_tooltip)#",
					"readonly": false,
					"multiline": false,
					"labelPosition": "above",
					"visible": true
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "WebInput_IdentityUrl",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					},
					"type": "crt.WebInput",
					"label": "$Resources.Strings.SysIdentityProviderDS_ServerUrl_a3v3sjs",
					"control": "$SysIdentityProviderDS_ServerUrl_a3v3sjs",
					"placeholder": "",
					"tooltip": "#ResourceString(WebInput_IdentityUrl_tooltip)#",
					"readonly": false,
					"multiline": false,
					"labelPosition": "above",
					"visible": true
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "Input_d172vy2",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 4,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.SysIdentityProviderDS_ClientId_gktsvg9",
					"control": "$SysIdentityProviderDS_ClientId_gktsvg9",
					"placeholder": "",
					"tooltip": "#ResourceString(Input_d172vy2_tooltip)#",
					"readonly": false,
					"multiline": false,
					"labelPosition": "above",
					"visible": true
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "EncryptedInput_rot2itz",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 5,
						"rowSpan": 1
					},
					"type": "crt.EncryptedInput",
					"label": "$Resources.Strings.SysIdentityProviderDS_ClientSecret_aejvcs1",
					"labelPosition": "above",
					"control": "$SysIdentityProviderDS_ClientSecret_aejvcs1",
					"visible": true,
					"readonly": false,
					"placeholder": "",
					"tooltip": "#ResourceString(EncryptedInput_rot2itz_tooltip)#"
				},
				"parentName": "MainContainer",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "Button_CheckConnection",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(Button_CheckConnection_caption)#",
					"color": "outline",
					"size": "large",
					"iconPosition": "only-text",
					"clicked": {
						"request": "crt.TestOAuthConnectionResourceRequest",
						"params": {
							"serverUrl": "$SysIdentityProviderDS_ServerUrl_a3v3sjs",
							"clientId": "$SysIdentityProviderDS_ClientId_gktsvg9",
							"clientSecret": "$SysIdentityProviderDS_ClientSecret_aejvcs1"
						}
					},
					"clickMode": "default",
					"visible": false
				},
				"parentName": "FooterContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_xjnzq3a",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"items": [],
					"justifyContent": "center"
				},
				"parentName": "FooterContainer",
				"propertyName": "items",
				"index": 1
			}
		]/**SCHEMA_VIEW_CONFIG_DIFF*/,
		viewModelConfigDiff: /**SCHEMA_VIEW_MODEL_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"path": [
					"attributes"
				],
				"values": {
					"IdentityProviderSettingsModalTitle": {
						"value": "$Resources.Strings.PageTitle_caption"
					},
					"SysIdentityProviderDS_Id": {
						"modelConfig": {
							"path": "SysIdentityProviderDS.Id"
						}
					},
					"SysIdentityProviderDS_Name_w81isky": {
						"modelConfig": {
							"path": "SysIdentityProviderDS.Name"
						}
					},
					"SysIdentityProviderDS_Description_nmatp93": {
						"modelConfig": {
							"path": "SysIdentityProviderDS.Description"
						}
					},
					"SysIdentityProviderDS_ServerUrl_a3v3sjs": {
						"modelConfig": {
							"path": "SysIdentityProviderDS.ServerUrl"
						}
					},
					"SysIdentityProviderDS_ClientId_gktsvg9": {
						"modelConfig": {
							"path": "SysIdentityProviderDS.ClientId"
						}
					},
					"SysIdentityProviderDS_ClientSecret_aejvcs1": {
						"modelConfig": {
							"path": "SysIdentityProviderDS.ClientSecret"
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
						"SysIdentityProviderDS": {
							"type": "crt.EntityDataSource",
							"scope": "page",
							"config": {
								"entitySchemaName": "SysIdentityProvider",
								"loadParameters": {
									"options": {
										"pagingConfig": {
											"rowCount": 1,
											"rowsOffset": -1
										},
										"sortingConfig": {
											"columns": []
										}
									}
								},
								"allowCopyingRecords": false
							}
						}
					},
					"primaryDataSourceName": "SysIdentityProviderDS"
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[
			{
				request: "crt.LoadDataRequest",
				handler: async (request, next) => {
					const result = await next?.handle(request);

					const identityProviderId = await request.$context.SysIdentityProviderDS_Id;
					const isNewRecord = !identityProviderId;
					const title = isNewRecord
						? request.$context.Resources.Strings.PageTitle_caption
						: request.$context.Resources.Strings.EditPageTitle_caption;

					await request.$context.set(
						"IdentityProviderSettingsModalTitle",
						title
					);

					return result;
				}
			}
		]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});