define("McpTool_FormPage", /**SCHEMA_DEPS*/["@creatio-devkit/common", "McpTool_FormPageResources"]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/(sdk, resources)/**SCHEMA_ARGS*/ {
	// McpToolSourceType lookup id for "Source code action" (matches McpToolSourceTypes.SourceCodeAction).
	const SOURCE_CODE_ACTION_TYPE_ID = "ebed4287-7fd3-4d75-843f-c2ea0b795e66";
	function getProcessId(proc) {
		if (!proc) { return null; }
		if (typeof proc === "string") { return proc; }
		return proc.value || proc.Id || proc.id || null;
	}
	function formatJson(jsonString) {
		if (!jsonString) { return ""; }
		try { return JSON.stringify(JSON.parse(jsonString), null, 2); } catch (e) { return jsonString; }
	}
	async function loadSysSchema(procId) {
		const model = await sdk.Model.create("SysSchema");
		const filters = new sdk.FilterGroup();
		await filters.addSchemaColumnFilterWithParameter(sdk.ComparisonType.Equal, "Id", procId);
		const rows = await model.load({ attributes: ["Id", "Name", "UId"], parameters: [{ type: sdk.ModelParameterType.Filter, value: filters }] });
		return (rows && rows.length) ? rows[0] : null;
	}
	// Unified schema fetch for both tool kinds: sourceType "process" (identifier =
	// process name) or "sourcecode" (identifier = action full type name).
	async function fetchToolSchemas(sourceType, identifier) {
		const httpClientService = new sdk.HttpClientService();
		const resp = await httpClientService.post("/rest/McpToolSchemaService/GetToolSchemas", { sourceType: sourceType, identifier: identifier });
		const data = resp && resp.body;
		const inner = (data && data.GetToolSchemasResult !== undefined) ? data.GetToolSchemasResult : data;
		return (typeof inner === "string") ? JSON.parse(inner) : inner;
	}
	function notify($context, message) {
		return $context.executeRequest({ type: "crt.NotificationRequest", message: message });
	}
	// Returns true when the process has input parameters of types the MCP runtime can't
	// represent (the flag that drives the warning). Mirrors RuntimeToolFacade: such a
	// tool is excluded from tools/list and fails at call time.
	function hasUnsupportedParameters(schemas) {
		const list = schemas && schemas.unsupportedParameters;
		return Array.isArray(list) && list.length > 0;
	}
	// Localized warning with the offending parameters interpolated dynamically. The
	// static lead-in lives in the UnsupportedParamsWarningPrefix resource string; only
	// the parameter list (name + type, from the process) and the closing period are
	// appended here.
	function buildUnsupportedMessage(schemas) {
		const details = schemas.unsupportedParameters
			.map(function (p) { return (p && p.name ? p.name : "?") + " (" + (p && p.type ? p.type : "?") + ")"; })
			.join(", ");
		return resources.localizableStrings.UnsupportedParamsWarningPrefix + details + ".";
	}
	// Drives the persistent warning bar (see UnsupportedParamsWarningLabel) from a
	// freshly fetched schemas payload. Re-evaluated both on open and on every
	// "Refresh from process" — otherwise a refresh that re-introduces (or clears) an
	// unsupported parameter would leave the warning stale until the next reload.
	async function applyUnsupportedWarning($context, schemas) {
		if (hasUnsupportedParameters(schemas)) {
			await $context.set("UnsupportedParamsWarningText", buildUnsupportedMessage(schemas));
			await $context.set("HasUnsupportedParams", true);
		} else {
			await $context.set("HasUnsupportedParams", false);
		}
	}
	async function openProcessDesigner($context) {
		const procId = getProcessId(await $context["BusinessProcess"]);
		if (!procId) { await notify($context, resources.localizableStrings.SelectProcessFirstMessage); return; }
		const sysSchema = await loadSysSchema(procId);
		if (sysSchema && sysSchema.UId) {
			const base = window.location.origin + window.location.pathname.split("/0/")[0];
			window.open(base + "/0/Nui/ViewModule.aspx?vm=SchemaDesigner#process/" + sysSchema.UId, "_blank");
		}
	}
	// The source-code action is stored as an assembly-qualified type name
	// ("Namespace.Class, Assembly"); the SourceCode schema's Name matches the class
	// name. Extract it to resolve the schema row.
	function getActionClassName(fullTypeName) {
		if (!fullTypeName) { return null; }
		const typePart = String(fullTypeName).split(",")[0].trim();
		const lastDot = typePart.lastIndexOf(".");
		return lastDot >= 0 ? typePart.substring(lastDot + 1) : typePart;
	}
	async function loadSourceCodeSchema(className) {
		if (!className) { return null; }
		const model = await sdk.Model.create("SysSchema");
		const filters = new sdk.FilterGroup();
		await filters.addSchemaColumnFilterWithParameter(sdk.ComparisonType.Equal, "Name", className);
		await filters.addSchemaColumnFilterWithParameter(sdk.ComparisonType.Equal, "ManagerName", "SourceCodeSchemaManager");
		const rows = await model.load({ attributes: ["Id", "Name", "UId", "ManagerName"], parameters: [{ type: sdk.ModelParameterType.Filter, value: filters }] });
		return (rows && rows.length) ? rows[0] : null;
	}
	async function openSourceCodeDesigner($context) {
		const className = getActionClassName(await $context["SourceCodeAction"]);
		if (!className) { return; }
		const schema = await loadSourceCodeSchema(className);
		if (schema && schema.UId) {
			const base = window.location.origin + window.location.pathname.split("/0/")[0];
			window.open(base + "/0/ClientApp/#/SourceCodeSchemaDesigner/" + schema.UId + "?showMetaItemSourceCode=true", "_blank");
		} else {
			await notify($context, resources.localizableStrings.SourceActionNotFoundMessage);
		}
	}
	// Success toast wording depends on which schema was refreshed and the tool kind
	// (source-code action vs business process).
	function refreshedMessage(schemaKey, isSourceCode) {
		if (schemaKey === "inputSchema") {
			return isSourceCode
				? resources.localizableStrings.InputSchemaRefreshedFromActionMessage
				: resources.localizableStrings.InputSchemaRefreshedMessage;
		}
		return isSourceCode
			? resources.localizableStrings.OutputSchemaRefreshedFromActionMessage
			: resources.localizableStrings.OutputSchemaRefreshedMessage;
	}
	async function repopulateSchema($context, attributeName, schemaKey) {
		const isSourceCode = await $context["IsSourceCodeAction"];
		const mask = new sdk.MaskService();
		// Source-code-action tools refresh from the action's full type name; business
		// processes refresh from the process schema. Both fetch through the unified
		// McpToolSchemaService, so only the source type + identifier differ.
		if (isSourceCode) {
			const typeName = await $context["SourceCodeAction"];
			if (!typeName) { return; }
			mask.showBodyMask();
			let schemas;
			try {
				schemas = await fetchToolSchemas("sourcecode", typeName);
			} finally {
				mask.hideBodyMask();
			}
			// If the action type no longer resolves, the service returns an error and
			// no schema. Don't overwrite the existing field or claim success — surface
			// the failure instead.
			if (!schemas || schemas.error) {
				await notify($context, resources.localizableStrings.SourceActionNotFoundMessage);
				return;
			}
			await $context.set(attributeName, formatJson(schemas[schemaKey]));
			await applyUnsupportedWarning($context, schemas);
			await notify($context, refreshedMessage(schemaKey, true));
			return;
		}
		const procId = getProcessId(await $context["BusinessProcess"]);
		if (!procId) { await notify($context, resources.localizableStrings.SelectProcessFirstMessage); return; }
		mask.showBodyMask();
		try {
			const sysSchema = await loadSysSchema(procId);
			if (!sysSchema || !sysSchema.Name) { return; }
			const schemas = await fetchToolSchemas("process", sysSchema.Name);
			await $context.set(attributeName, formatJson(schemas && schemas[schemaKey]));
			// Keep the unsupported-params warning in sync with the just-refreshed
			// process; the check is input-only but the payload carries it either way,
			// so refreshing input OR output reflects the current parameter set.
			await applyUnsupportedWarning($context, schemas);
		} finally {
			mask.hideBodyMask();
		}
		await notify($context, refreshedMessage(schemaKey, false));
	}
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "remove",
				"name": "RequeueQueueItemButton"
			},
			{
				"operation": "remove",
				"name": "PostponeQueueItemButton"
			},
			{
				"operation": "merge",
				"name": "GeneralInfoTabContainer",
				"values": {
					"gap": {
						"columnGap": "none",
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
					"entitySchemaName": "McpTool"
				}
			},
			{
				"operation": "merge",
				"name": "AttachmentList",
				"values": {
					"columns": [
						{
							"id": "d3231673-d652-4d48-9501-958dbb95b759",
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
				"name": "ExternalNameProfile",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 1,
						"colSpan": 1,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.ExternalName",
					"control": "$ExternalName",
					"tooltip": "$Resources.Strings.TtExternalName",
					"labelPosition": "auto"
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "DescriptionInput",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 2,
						"colSpan": 1,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.Description",
					"control": "$Description",
					"tooltip": "$Resources.Strings.TtDescription",
					"multiline": true,
					"autoHeight": true,
					"labelPosition": "auto"
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "TitleInput",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 3,
						"colSpan": 1,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "#ResourceString(TitleInput_label)#",
					"control": "$Title",
					"tooltip": "#ResourceString(TitleInput_tooltip)#",
					"labelPosition": "auto"
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "IsEnabledInput",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 4,
						"colSpan": 1,
						"rowSpan": 1
					},
					"type": "crt.Checkbox",
					"label": "$Resources.Strings.IsEnabled",
					"control": "$IsEnabled",
					"labelPosition": "auto"
				},
				"parentName": "SideAreaProfileContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "ProcessInput",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.Process",
					"control": "$BusinessProcess",
					"readonly": true,
					"visible": "$IsSourceCodeAction | crt.InvertBooleanValue",
					"labelPosition": "above",
					"styles": {
						"flex": "1 1 320px",
						"min-width": "0"
					},
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
				"name": "ProcessRowContainer",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"alignItems": "flex-end",
					"justifyContent": "start",
					"gap": "small",
					"wrap": "nowrap",
					"visible": "$IsSourceCodeAction | crt.InvertBooleanValue",
					"layoutConfig": {
						"column": 2,
						"row": 1,
						"colSpan": 1,
						"rowSpan": 1
					},
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"items": []
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "OpenProcessButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(OpenProcessButton_caption)#",
					"tooltip": "#ResourceString(OpenProcessButton_tooltip)#",
					"color": "default",
					"size": "large",
					"iconPosition": "only-icon",
					"clicked": {
						"request": "usr.OpenProcessDesignerRequest"
					},
					"visible": true,
					"clickMode": "default",
					"icon": "open-button-icon"
				},
				"parentName": "ProcessRowContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RefreshSchemasButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(RefreshSchemasButton_caption)#",
					"tooltip": "#ResourceString(RefreshSchemasButton_tooltip)#",
					"color": "default",
					"size": "large",
					"iconPosition": "left-icon",
					"clickMode": "menu",
					"menuItems": [],
					"visible": true,
					"icon": "image-update"
				},
				"parentName": "ProcessRowContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "RefreshInputMenuItem",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(RefreshInputMenuItem_caption)#",
					"icon": null,
					"clicked": {
						"request": "usr.RepopulateInputSchemaRequest"
					},
					"visible": true
				},
				"parentName": "RefreshSchemasButton",
				"propertyName": "menuItems",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RefreshOutputMenuItem",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(RefreshOutputMenuItem_caption)#",
					"icon": null,
					"clicked": {
						"request": "usr.RepopulateOutputSchemaRequest"
					},
					"visible": true
				},
				"parentName": "RefreshSchemasButton",
				"propertyName": "menuItems",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "InputSchemaWarningGroup",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "column",
					"alignItems": "stretch",
					"justifyContent": "start",
					"gap": "small",
					"wrap": "nowrap",
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"layoutConfig": {
						"column": 1,
						"row": 2,
						"colSpan": 2,
						"rowSpan": 1
					},
					"items": []
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "UnsupportedParamsWarningLabel",
				"values": {
					"type": "crt.Label",
					"caption": "$UnsupportedParamsWarningText",
					"visible": "$HasUnsupportedParams",
					"labelType": "body",
					"labelThickness": "semibold",
					"labelColor": "#D2310D",
					"labelBackgroundColor": "#FF401333",
					"labelTextAlign": "start"
				},
				"parentName": "InputSchemaWarningGroup",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "InputSchemaInput",
				"values": {
					"type": "crt.Input",
					"label": "$Resources.Strings.InputSchema",
					"control": "$InputSchema",
					"tooltip": "$Resources.Strings.TtInputSchema",
					"multiline": true,
					"autoHeight": true,
					"labelPosition": "above"
				},
				"parentName": "InputSchemaWarningGroup",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "OutputSchemaInput",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 3,
						"colSpan": 2,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.OutputSchema",
					"control": "$OutputSchema",
					"tooltip": "$Resources.Strings.TtOutputSchema",
					"multiline": true,
					"autoHeight": true,
					"labelPosition": "above"
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "AnnotationsInput",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 4,
						"colSpan": 2,
						"rowSpan": 1
					},
					"type": "crt.Input",
					"label": "$Resources.Strings.Annotations",
					"control": "$Annotations",
					"tooltip": "$Resources.Strings.TtAnnotations",
					"multiline": true,
					"autoHeight": true,
					"labelPosition": "above"
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "SourceCodeActionInput",
				"values": {
					"type": "crt.Input",
					"label": "#ResourceString(SourceCodeActionInput_label)#",
					"control": "$SourceCodeAction",
					"readonly": true,
					"visible": "$IsSourceCodeAction",
					"labelPosition": "above",
					"styles": {
						"flex": "1 1 320px",
						"min-width": "0"
					},
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "SourceActionRowContainer",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"alignItems": "flex-end",
					"justifyContent": "start",
					"gap": "small",
					"wrap": "nowrap",
					"visible": "$IsSourceCodeAction",
					"layoutConfig": {
						"column": 2,
						"row": 1,
						"colSpan": 1,
						"rowSpan": 1
					},
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"items": []
				},
				"parentName": "GeneralInfoTabContainer",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "OpenSourceActionButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(OpenSourceActionButton_caption)#",
					"tooltip": "#ResourceString(OpenSourceActionButton_tooltip)#",
					"color": "default",
					"size": "large",
					"iconPosition": "only-icon",
					"clicked": {
						"request": "usr.OpenSourceActionDesignerRequest"
					},
					"visible": true,
					"clickMode": "default",
					"icon": "open-button-icon"
				},
				"parentName": "SourceActionRowContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "RefreshSourceActionButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(RefreshSourceActionButton_caption)#",
					"tooltip": "#ResourceString(RefreshSourceActionButton_tooltip)#",
					"color": "default",
					"size": "large",
					"iconPosition": "left-icon",
					"clickMode": "menu",
					"menuItems": [],
					"visible": true,
					"icon": "image-update"
				},
				"parentName": "SourceActionRowContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "SourceRefreshInputMenuItem",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(RefreshInputMenuItem_caption)#",
					"icon": null,
					"clicked": {
						"request": "usr.RepopulateInputSchemaRequest"
					},
					"visible": true
				},
				"parentName": "RefreshSourceActionButton",
				"propertyName": "menuItems",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "SourceRefreshOutputMenuItem",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(RefreshOutputMenuItem_caption)#",
					"icon": null,
					"clicked": {
						"request": "usr.RepopulateOutputSchemaRequest"
					},
					"visible": true
				},
				"parentName": "RefreshSourceActionButton",
				"propertyName": "menuItems",
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
					"ExternalName": {
						"modelConfig": {
							"path": "PDS.ExternalName"
						},
						"validators": {
							"ExternalNameValidator": {
								"type": "usr.ExternalNameValidator",
								"params": {
									"message": "#ResourceString(ExternalNameValidationMessage)#"
								}
							}
						}
					},
					"McpServer": {
						"modelConfig": {
							"path": "PDS.McpServer"
						}
					},
					"BusinessProcess": {
						"modelConfig": {
							"path": "PDS.BusinessProcess"
						}
					},
					"ToolSourceType": {
						"modelConfig": {
							"path": "PDS.ToolSourceType"
						}
					},
					"Description": {
						"modelConfig": {
							"path": "PDS.Description"
						}
					},
					"InputSchema": {
						"modelConfig": {
							"path": "PDS.InputSchema"
						},
						"validators": {
							"JsonValidator": {
								"type": "usr.JsonValidator",
								"params": {
									"message": "#ResourceString(JsonValidationMessage)#"
								}
							}
						}
					},
					"OutputSchema": {
						"modelConfig": {
							"path": "PDS.OutputSchema"
						},
						"validators": {
							"JsonValidator": {
								"type": "usr.JsonValidator",
								"params": {
									"message": "#ResourceString(JsonValidationMessage)#"
								}
							}
						}
					},
					"Annotations": {
						"modelConfig": {
							"path": "PDS.Annotations"
						},
						"validators": {
							"JsonValidator": {
								"type": "usr.JsonValidator",
								"params": {
									"message": "#ResourceString(JsonValidationMessage)#"
								}
							}
						}
					},
					"IsEnabled": {
						"modelConfig": {
							"path": "PDS.IsEnabled"
						}
					},
					"Title": {
						"modelConfig": {
							"path": "PDS.Title"
						}
					},
					"SourceCodeAction": {
						"modelConfig": {
							"path": "PDS.SourceCodeAction"
						}
					},
					"IsSourceCodeAction": {},
					"HasUnsupportedParams": {},
					"UnsupportedParamsWarningText": {}
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
					"primaryDataSourceName": "PDS"
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
							"entitySchemaName": "McpTool"
						},
						"scope": "page"
					}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[
			{
				// On open, warn if the tool's process has parameter types the MCP
				// runtime can't represent — the tool is excluded from tools/list and
				// fails at call time. Best-effort (advisory notification).
				request: "crt.HandleViewModelInitRequest",
				handler: async (request, next) => {
					const result = await next?.handle(request);
					try {
						const { $context } = request;
						await $context.set("HasUnsupportedParams", false);
						// At init the record data isn't loaded yet, so $context["BusinessProcess"]
						// is usually empty — resolve the process from the persisted record
						// by Id (available from the route) instead of relying on it.
						await $context.set("IsSourceCodeAction", false);
							let procId = getProcessId(await $context["BusinessProcess"]);
							let sourceTypeId = getProcessId(await $context["ToolSourceType"]);
						if (!procId) {
							const recordId = await $context["Id"];
							if (recordId) {
								const toolModel = await sdk.Model.create("McpTool");
								const filters = new sdk.FilterGroup();
								await filters.addSchemaColumnFilterWithParameter(sdk.ComparisonType.Equal, "Id", recordId);
								const rows = await toolModel.load({ attributes: ["Id", "BusinessProcess", "ToolSourceType"], parameters: [{ type: sdk.ModelParameterType.Filter, value: filters }] });
								if (rows && rows.length) { procId = getProcessId(rows[0].BusinessProcess); sourceTypeId = sourceTypeId || getProcessId(rows[0].ToolSourceType); }
							}
						}
						const isSourceCode = !!sourceTypeId && String(sourceTypeId).toLowerCase() === SOURCE_CODE_ACTION_TYPE_ID;
							await $context.set("IsSourceCodeAction", isSourceCode);
							if (!isSourceCode && procId) {
							const sysSchema = await loadSysSchema(procId);
							if (sysSchema && sysSchema.Name) {
								const schemas = await fetchToolSchemas("process", sysSchema.Name);
								await applyUnsupportedWarning($context, schemas);
							}
						}
					} catch (e) {
						// Non-fatal: the on-open warning is advisory.
					}
					return result;
				}
			},
			{
				request: "usr.OpenProcessDesignerRequest",
				handler: async (request, next) => {
					await openProcessDesigner(request.$context);
					return next?.handle(request);
				}
			},
			{
				request: "usr.OpenSourceActionDesignerRequest",
				handler: async (request, next) => {
					await openSourceCodeDesigner(request.$context);
					return next?.handle(request);
				}
			},
			{
				request: "usr.RepopulateInputSchemaRequest",
				handler: async (request, next) => {
					await repopulateSchema(request.$context, "InputSchema", "inputSchema");
					return next?.handle(request);
				}
			},
			{
				request: "usr.RepopulateOutputSchemaRequest",
				handler: async (request, next) => {
					await repopulateSchema(request.$context, "OutputSchema", "outputSchema");
					return next?.handle(request);
				}
			}
		]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{
			"usr.ExternalNameValidator": {
				"validator": function (config) {
					return function (control) {
						const value = control.value;
						if (!value) {
							return null;
						}
						// Mirrors the control-plane ToolNameValidator ({1,64}) — the single
						// authoritative gate. Same charset (letters, digits, _, -; no dots), so a
						// name accepted here is accepted at discovery. Authoring-time feedback only:
						// the server no longer filters names out of tools/list.
						const isValid = /^[A-Za-z0-9_-]{1,64}$/.test(value);
						const message = resources.localizableStrings.ExternalNameValidationMessage || config.message;
						return isValid ? null : { "usr.ExternalNameValidator": { message: message } };
					};
				},
				"params": [{ "name": "message" }],
				"async": false
			},
			"usr.JsonValidator": {
				"validator": function (config) {
					return function (control) {
						const value = control.value;
						if (!value || !String(value).trim()) {
							return null;
						}
						const message = resources.localizableStrings.JsonValidationMessage || config.message;
						try {
							JSON.parse(value);
							return null;
						} catch (e) {
							return { "usr.JsonValidator": { message: message } };
						}
					};
				},
				"params": [{ "name": "message" }],
				"async": false
			}
		}/**SCHEMA_VALIDATORS*/
	};
});
