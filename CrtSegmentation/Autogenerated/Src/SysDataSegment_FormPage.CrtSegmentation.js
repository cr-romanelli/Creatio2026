define("SysDataSegment_FormPage", /**SCHEMA_DEPS*/["@creatio-devkit/common", "css!SysDataSegmentFormPageCustomCSS"]/**SCHEMA_DEPS*/, function/**SCHEMA_ARGS*/(sdk)/**SCHEMA_ARGS*/ {
	return {
		viewConfigDiff: /**SCHEMA_VIEW_CONFIG_DIFF*/[
			{
				"operation": "merge",
				"name": "MainHeader",
				"values": {
					"gap": "medium",
					"visible": true,
					"alignItems": "stretch",
					"justifyContent": "start",
					"wrap": "nowrap"
				}
			},
			{
				"operation": "merge",
				"name": "ActionButtonsContainer",
				"values": {
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
					"gap": "small"
				}
			},
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
				"name": "RequeueQueueItemButton"
			},
			{
				"operation": "remove",
				"name": "PostponeQueueItemButton"
			},
			{
				"operation": "merge",
				"name": "TagSelect",
				"values": {
					"tagInRecordSourceSchemaName": "TagInRecord"
				}
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
				"operation": "remove",
				"name": "SideAreaProfileContainer"
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
					"allowToggleClose": true,
					"visible": true,
					"stretch": true,
					"selectedTab": {
						"value": "EntryRules_Tab"
					}
				}
			},
			{
				"operation": "remove",
				"name": "GeneralInfoTab"
			},
			{
				"operation": "remove",
				"name": "GeneralInfoTabContainer"
			},
			{
				"operation": "merge",
				"name": "CardToggleTabPanel",
				"values": {
					"visible": true,
					"styleType": "default",
					"bodyBackgroundColor": "primary-contrast-500",
					"selectedTabTitleColor": "auto",
					"tabTitleColor": "auto",
					"underlineSelectedTabColor": "auto",
					"headerBackgroundColor": "auto",
					"selectedTab": null,
					"allowToggleClose": true
				}
			},
			{
				"operation": "merge",
				"name": "Feed",
				"values": {
					"dataSourceName": "PDS",
					"entitySchemaName": "SysDataSegment"
				}
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
				"name": "Button_Enable",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(Button_Enable_caption)#",
					"color": "accent",
					"disabled": false,
					"size": "large",
					"iconPosition": "left-icon",
					"visible": false,
					"clicked": {
						"request": "crt.RunBusinessProcessRequest",
						"params": {
							"processName": "EnableSegmentProcess",
							"processRunType": "ForTheSelectedPage",
							"saveAtProcessStart": true,
							"recordIdProcessParameterName": "SegmentId"
						}
					},
					"clickMode": "default",
					"icon": "process-button-icon"
				},
				"parentName": "ActionButtonsContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Button_Disable",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(Button_Disable_caption)#",
					"color": "outline",
					"disabled": false,
					"size": "large",
					"iconPosition": "left-icon",
					"visible": false,
					"icon": "pause-bars-icon",
					"clicked": {
						"request": "crt.RunBusinessProcessRequest",
						"params": {
							"processName": "DisableSegmentProcess",
							"processRunType": "ForTheSelectedPage",
							"saveAtProcessStart": true,
							"recordIdProcessParameterName": "SegmentId"
						}
					},
					"clickMode": "default"
				},
				"parentName": "ActionButtonsContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ResumeSegmentButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(ResumeSegmentButton_caption)#",
					"color": "accent",
					"disabled": false,
					"size": "large",
					"iconPosition": "left-icon",
					"visible": false,
					"icon": "process-button-icon",
					"clicked": {
						"request": "crt.RunBusinessProcessRequest",
						"params": {
							"processName": "ResumeSegmentProcess",
							"processRunType": "ForTheSelectedPage",
							"saveAtProcessStart": true,
							"recordIdProcessParameterName": "SegmentId"
						}
					},
					"clickMode": "default"
				},
				"parentName": "ActionButtonsContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "SegmentProcessingInfoOuterGridContainer",
				"values": {
					"type": "crt.GridContainer",
					"columns": [
						"minmax(32px, 1fr)"
					],
					"rows": "minmax(max-content, 32px)",
					"gap": {
						"columnGap": "large",
						"rowGap": "extra-small"
					},
					"items": [],
					"fitContent": true,
					"visible": false,
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "small",
						"right": "small",
						"bottom": "small",
						"left": "small"
					},
					"alignItems": "stretch"
				},
				"parentName": "Main",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "SegmentProcessingInfoGridContainer",
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
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"color": "primary",
					"borderRadius": "medium",
					"visible": false,
					"alignItems": "stretch",
					"layoutConfig": {
						"column": 1,
						"row": 1,
						"colSpan": 1,
						"rowSpan": 1
					},
					"styles": {
						"border-color": "var(--crt-color-border-primary-soft)"
					}
				},
				"parentName": "SegmentProcessingInfoOuterGridContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "SegmentProcessingInfoFlexContainer",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 1,
						"colSpan": 1,
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
						"top": "small",
						"right": "medium",
						"bottom": "small",
						"left": "medium"
					},
					"alignItems": "center",
					"justifyContent": "start",
					"gap": "extra-small",
					"wrap": "wrap"
				},
				"parentName": "SegmentProcessingInfoGridContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ProcessingInfoIcon",
				"values": {
					"type": "crt.Icon",
					"iconName": "info-icon",
					"size": "16",
					"color": "#0058EF",
					"backgroundType": "none",
					"backgroundColor": "#E7ECFA",
					"padding": "xs",
					"visible": true,
					"ariaLabel": "",
					"tooltip": ""
				},
				"parentName": "SegmentProcessingInfoFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "SegmentProcessingInfoFirstLabel",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(SegmentProcessingInfoFirstLabel_caption)#)#",
					"labelType": "body",
					"labelThickness": "bold",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "SegmentProcessingInfoFlexContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "SegmentProcessingInfoSecondLabel",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(SegmentProcessingInfoSecondLabel_caption)#)#",
					"labelType": "body",
					"labelThickness": "normal",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "SegmentProcessingInfoFlexContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "SegmentErrorGridContainer",
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
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"color": "primary",
					"borderRadius": "medium",
					"visible": false,
					"alignItems": "stretch",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"rowSpan": 1,
						"row": 2
					},
					"styles": {
						"border-color": "var(--crt-color-border-error-soft)"
					}
				},
				"parentName": "SegmentProcessingInfoOuterGridContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "SegmentProcessingErrorFlexContainer",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 1,
						"colSpan": 1,
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
						"top": "small",
						"right": "medium",
						"bottom": "small",
						"left": "medium"
					},
					"alignItems": "center",
					"justifyContent": "start",
					"gap": "extra-small",
					"wrap": "wrap"
				},
				"parentName": "SegmentErrorGridContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ProcessingErrorIcon",
				"values": {
					"type": "crt.Icon",
					"iconName": "info-icon",
					"size": "16",
					"color": "#E00022",
					"backgroundType": "none",
					"backgroundColor": "#FD3F11",
					"padding": "xs",
					"visible": true,
					"ariaLabel": "",
					"tooltip": ""
				},
				"parentName": "SegmentProcessingErrorFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "SegmentProcessingErrorFirstLabel",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(SegmentProcessingErrorFirstLabel_caption)#)#",
					"labelType": "body",
					"labelThickness": "bold",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "SegmentProcessingErrorFlexContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "SegmentProcessingErrorSecondLabel",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(SegmentProcessingErrorSecondLabel_caption)#)#",
					"labelType": "body",
					"labelThickness": "normal",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "SegmentProcessingErrorFlexContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "SidebarLifecycleContainer",
				"values": {
					"type": "crt.GridContainer",
					"columns": [
						"minmax(32px, 1fr)"
					],
					"rows": "minmax(max-content, 32px)",
					"gap": {
						"columnGap": "large",
						"rowGap": "small"
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
				"parentName": "SideContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "LifecycleHeader",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"justifyContent": "space-between",
					"alignItems": "center",
					"gap": "small",
					"items": [],
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "SidebarLifecycleContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "LifecycleLabel",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(LifecycleLabel_caption)#)#",
					"labelType": "headline-2",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "#0D2E4E",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label"
				},
				"parentName": "LifecycleHeader",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "StatusChipList",
				"values": {
					"type": "crt.ChipList",
					"items": "$StatusChip_Items",
					"readonly": true,
					"wrap": false,
					"visible": true
				},
				"parentName": "LifecycleHeader",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ComboBox_zpkc1ym",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_RefreshMode_2pd62c1",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "above",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_RefreshMode_2pd62c1",
					"visible": true,
					"readonly": true,
					"placeholder": "",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"secondaryDisplayValue": null,
					"valueDetails": "$ComboBox_zpkc1ym_ValueDetails"
				},
				"parentName": "SidebarLifecycleContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "PopulateSegmentButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(PopulateSegmentButton_caption)#",
					"color": "outline",
					"disabled": false,
					"size": "large",
					"iconPosition": "left-icon",
					"visible": false,
					"clicked": {
						"request": "crt.RunBusinessProcessRequest",
						"params": {
							"processName": "UpdateSegmentProcess",
							"processRunType": "ForTheSelectedPage",
							"saveAtProcessStart": true,
							"recordIdProcessParameterName": "SegmentId"
						}
					},
					"clickMode": "default",
					"icon": "image-update",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					}
				},
				"parentName": "SidebarLifecycleContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "MatchingContactsMetric",
				"values": {
					"type": "crt.IndicatorWidget",
					"config": {
						"title": "#ResourceString(MatchingContactsMetric_title)#",
						"theme": "without-fill",
						"layout": {
							"color": "blue",
							"icon": {
								"iconName": "contact-group-icon"
							}
						},
						"text": {
							"template": "#ResourceString(MatchingContactsMetric_config_text_template)#",
							"metricMacros": "{0}",
							"labelPosition": "above-under",
							"fontSizeMode": "small"
						},
						"data": {
							"formatting": {
								"type": "number",
								"decimalPrecision": 0,
								"decimalSeparator": ".",
								"thousandSeparator": ","
							},
							"providing": {
								"attribute": "MatchingContactsMetric_Data",
								"schemaName": "Contact",
								"filters": {
									"filterAttributes": [
										{
											"attribute": "MatchingContactsMetric_EffectiveFilter",
											"loadOnChange": true
										}
									]
								},
								"aggregation": {
									"column": {
										"orderDirection": 0,
										"orderPosition": -1,
										"isVisible": true,
										"expression": {
											"expressionType": 1,
											"functionArgument": {
												"expressionType": 0,
												"columnPath": "Id"
											},
											"functionType": 2,
											"aggregationType": 1,
											"aggregationEvalType": 2
										}
									}
								},
								"dependencies": []
							}
						},
						"comparison": {
							"type": null,
							"text": ""
						},
						"hint": "#ResourceString(MatchingContactsMetric_config_hint)#"
					},
					"visible": true,
					"layoutConfig": {
						"height": 93.80000305175781
					}
				},
				"parentName": "SideContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "IndicatorWidget_w5rbywt",
				"values": {
					"type": "crt.IndicatorWidget",
					"config": {
						"title": "#ResourceString(IndicatorWidget_w5rbywt_title)#",
						"theme": "without-fill",
						"layout": {
							"color": "dark-blue",
							"icon": {
								"iconName": "reload-icon",
								"color": "violet"
							}
						},
						"text": {
							"template": "#ResourceString(IndicatorWidget_w5rbywt_config_text_template)#",
							"metricMacros": "{0}",
							"labelPosition": "above-under",
							"fontSizeMode": "small"
						},
						"data": {
							"formatting": {
								"type": "datetime",
								"date": {
									"display": true
								},
								"time": {
									"display": true
								}
							},
							"providing": {
								"attribute": "IndicatorWidget_w5rbywt_Data",
								"schemaName": "SysDataSegment",
								"filters": null,
								"aggregation": {
									"column": {
										"orderDirection": 0,
										"orderPosition": -1,
										"isVisible": true,
										"expression": {
											"expressionType": 1,
											"functionArgument": {
												"expressionType": 0,
												"columnPath": "LastActualizationDate"
											},
											"functionType": 2,
											"aggregationType": 5,
											"aggregationEvalType": 0
										}
									}
								},
								"dependencies": [
									{
										"attributePath": "Id",
										"relationPath": "PDS.Id"
									}
								]
							}
						},
						"comparison": {
							"type": null,
							"text": ""
						},
						"hint": "#ResourceString(IndicatorWidget_w5rbywt_hint)#"
					},
					"visible": true,
					"layoutConfig": {
						"height": 93.80000305175781
					}
				},
				"parentName": "SideContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "EntryRules_Tab",
				"values": {
					"type": "crt.TabContainer",
					"items": [],
					"caption": "#ResourceString(EntryRules_Tab_caption)#",
					"iconPosition": "only-text",
					"visible": true
				},
				"parentName": "Tabs",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "EntryRulesContainer",
				"values": {
					"type": "crt.GridContainer",
					"items": [],
					"rows": "minmax(32px, max-content)",
					"columns": [
						"minmax(32px, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": "extra-small"
					},
					"visible": true,
					"padding": {
						"top": "small",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"color": "transparent",
					"borderRadius": "none",
					"alignItems": "stretch"
				},
				"parentName": "EntryRules_Tab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "EntryRulesLabel",
				"values": {
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					},
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(EntryRulesLabel_caption)#)#",
					"labelType": "body",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "#757575",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "EntryRulesContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "EntryRulesFilter",
				"values": {
					"type": "crt.FilterBuilderSource",
					"stretch": false,
					"_filterOptions": {
						"expose": [
							{
								"attribute": "EntryRulesFilter_Filter",
								"converters": [
									{
										"converter": "crt.FilterBuilderAttributeConverter"
									}
								]
							}
						],
						"from": "PDS_EntryFilterData_cuzzx0r"
					},
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					},
					"applyFiltersAutomatically": true,
					"closeButtonVisible": false,
					"schemaName": "$PDS_EntitySchemaName",
					"filterStorage": "$PDS_EntryFilterData_cuzzx0r",
					"visible": true,
					"headerTemplate": "#ResourceString(EntryRulesFilter_headerTemplate)#"
				},
				"parentName": "EntryRulesContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ExitRules_Tab",
				"values": {
					"type": "crt.TabContainer",
					"items": [],
					"caption": "#ResourceString(ExitRules_Tab_caption)#",
					"iconPosition": "only-text",
					"visible": true
				},
				"parentName": "Tabs",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ExitRulesContainer",
				"values": {
					"type": "crt.GridContainer",
					"items": [],
					"rows": "minmax(32px, max-content)",
					"columns": [
						"minmax(32px, 1fr)"
					],
					"gap": {
						"columnGap": "large",
						"rowGap": "none"
					},
					"visible": true,
					"padding": {
						"top": "small",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"color": "transparent",
					"borderRadius": "none",
					"alignItems": "stretch"
				},
				"parentName": "ExitRules_Tab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ExitConditionsLabel",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(ExitConditionsLabel_caption)#)#",
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
						"colSpan": 1,
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "ExitRulesContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ExitModeToggleContainer",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"wrap": "wrap",
					"gap": "small",
					"items": [],
					"fitContent": true,
					"visible": true,
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "extra-small",
						"right": "none",
						"bottom": "small",
						"left": "none"
					},
					"alignItems": "center",
					"justifyContent": "start",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 2,
						"rowSpan": 1
					}
				},
				"parentName": "ExitRulesContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ExitModeOnConditionsButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(ExitModeOnConditionsButton_caption)#",
					"color": null,
					"classes": "$PDS_ExitMode_kbq5ezg | usr.GetExitModeButtonClasses : '2766f77a-f64c-4dce-b539-e7fc03212a87'",
					"disabled": false,
					"size": "large",
					"iconPosition": "left-icon",
					"icon": "flowchart-icon",
					"visible": true,
					"clicked": {
						"request": "usr.SetSegmentExitModeRequest",
						"params": {
							"exitModeId": "2766f77a-f64c-4dce-b539-e7fc03212a87",
							"exitModeName": "#ResourceString(ExitModeOnConditionsButton_TemporaryLabel)#"
						}
					},
					"clickMode": "default"
				},
				"parentName": "ExitModeToggleContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ExitModeNoLongerMatchesButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(ExitModeNoLongerMatchesButton_caption)#",
					"color": null,
					"classes": "$PDS_ExitMode_kbq5ezg | usr.GetExitModeButtonClasses : '0492ce17-e3dd-40ed-a000-018cf034183f'",
					"disabled": false,
					"size": "large",
					"iconPosition": "left-icon",
					"icon": "filter-cross-icon",
					"visible": true,
					"clicked": {
						"request": "usr.SetSegmentExitModeRequest",
						"params": {
							"exitModeId": "0492ce17-e3dd-40ed-a000-018cf034183f",
							"exitModeName": "#ResourceString(ExitModeNoLongerMatchesButton_TemporaryLabel)#"
						}
					},
					"clickMode": "default"
				},
				"parentName": "ExitModeToggleContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ExitModeNeverButton",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(ExitModeNeverButton_caption)#",
					"color": null,
					"classes": "$PDS_ExitMode_kbq5ezg | usr.GetExitModeButtonClasses : '9b09bb73-6ca8-4b5d-87d0-db5307f62b49'",
					"disabled": false,
					"size": "large",
					"iconPosition": "left-icon",
					"icon": "door-cross-icon",
					"visible": true,
					"clicked": {
						"request": "usr.SetSegmentExitModeRequest",
						"params": {
							"exitModeId": "9b09bb73-6ca8-4b5d-87d0-db5307f62b49",
							"exitModeName": "#ResourceString(ExitModeNeverButton_TemporaryLabel)#"
						}
					},
					"clickMode": "default"
				},
				"parentName": "ExitModeToggleContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "ExitMessagesFlexContainer",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "column",
					"items": [],
					"fitContent": true,
					"visible": "$PDS_ExitMode_kbq5ezg | usr.IsAnyExitMode : '2766f77a-f64c-4dce-b539-e7fc03212a87' : '0492ce17-e3dd-40ed-a000-018cf034183f' : '9b09bb73-6ca8-4b5d-87d0-db5307f62b49'",
					"color": "transparent",
					"borderRadius": "none",
					"padding": {
						"top": "small",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"alignItems": "stretch",
					"justifyContent": "start",
					"gap": "extra-small",
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 3,
						"rowSpan": 1
					},
					"wrap": "nowrap"
				},
				"parentName": "ExitRulesContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "NoExitRulesGridContainter",
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
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"color": "#E3EBFA",
					"borderRadius": "medium",
					"visible": "$PDS_ExitMode_kbq5ezg | usr.IsExitMode : '9b09bb73-6ca8-4b5d-87d0-db5307f62b49'",
					"alignItems": "stretch",
					"styles": {
						"border-color": "var(--crt-color-border-primary-soft)"
					},
					"layoutConfig": {
						"row": 2,
						"column": 1,
						"rowSpan": 1,
						"colSpan": 1
					}
				},
				"parentName": "ExitMessagesFlexContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NoExitRulesFlexContainter",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 1,
						"colSpan": 1,
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
						"top": "small",
						"right": "medium",
						"bottom": "small",
						"left": "medium"
					},
					"alignItems": "center",
					"justifyContent": "start",
					"gap": "extra-small",
					"wrap": "wrap"
				},
				"parentName": "NoExitRulesGridContainter",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NoExitRulesIcon",
				"values": {
					"type": "crt.Icon",
					"iconName": "info-icon",
					"size": "16",
					"color": "#0058EF",
					"backgroundType": "none",
					"backgroundColor": "#004FD5",
					"padding": "xs",
					"visible": true,
					"ariaLabel": "",
					"tooltip": ""
				},
				"parentName": "NoExitRulesFlexContainter",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NoExitRulesLabel1",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(NoExitRulesLabel1_caption)#)#",
					"labelType": "body",
					"labelThickness": "bold",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "NoExitRulesFlexContainter",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "NoExitRulesLabel2",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(NoExitRulesLabel2_caption)#)#",
					"labelType": "body",
					"labelThickness": "normal",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "NoExitRulesFlexContainter",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "NoLongerMatchGridContainter",
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
					"padding": {
						"top": "none",
						"right": "none",
						"bottom": "none",
						"left": "none"
					},
					"color": "#E3EBFA",
					"borderRadius": "medium",
					"visible": "$PDS_ExitMode_kbq5ezg | usr.IsExitMode : '0492ce17-e3dd-40ed-a000-018cf034183f'",
					"alignItems": "stretch",
					"styles": {
						"border-color": "var(--crt-color-border-primary-soft)"
					},
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"rowSpan": 1,
						"row": 3
					}
				},
				"parentName": "ExitMessagesFlexContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "FlexContainer_ldqucrz",
				"values": {
					"layoutConfig": {
						"column": 1,
						"row": 1,
						"colSpan": 1,
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
						"top": "small",
						"right": "medium",
						"bottom": "small",
						"left": "medium"
					},
					"alignItems": "center",
					"justifyContent": "start",
					"gap": "extra-small",
					"wrap": "wrap"
				},
				"parentName": "NoLongerMatchGridContainter",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NoLongerMatchGridContainterIcon",
				"values": {
					"type": "crt.Icon",
					"iconName": "info-icon",
					"size": "16",
					"color": "#0058EF",
					"backgroundType": "none",
					"backgroundColor": "#004FD5",
					"padding": "xs",
					"visible": true,
					"ariaLabel": "",
					"tooltip": ""
				},
				"parentName": "FlexContainer_ldqucrz",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NoLongerMatchLabel1",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(NoLongerMatchLabel1_caption)#)#",
					"labelType": "body",
					"labelThickness": "normal",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "FlexContainer_ldqucrz",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "NoLongerMatchLabel2",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(NoLongerMatchLabel2_caption)#)#",
					"labelType": "body",
					"labelThickness": "bold",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "FlexContainer_ldqucrz",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "NoLongerMatchLabel3",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(NoLongerMatchLabel3_caption)#)#",
					"labelType": "body",
					"labelThickness": "normal",
					"labelEllipsis": false,
					"labelColor": "auto",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"headingLevel": "label",
					"visible": true
				},
				"parentName": "FlexContainer_ldqucrz",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "ExitRulesFilter",
				"values": {
					"type": "crt.FilterBuilderSource",
					"stretch": true,
					"_filterOptions": {
						"expose": [],
						"from": "ExitRulesFilter_Value"
					},
					"layoutConfig": {
						"column": 1,
						"colSpan": 1,
						"row": 4,
						"rowSpan": 1,
						"alignSelf": "stretch"
					},
					"applyFiltersAutomatically": true,
					"closeButtonVisible": false,
					"schemaName": "$PDS_EntitySchemaName",
					"filterStorage": "$PDS_ExitFilterData_xyikz5x",
					"visible": "$PDS_ExitMode_kbq5ezg | usr.IsExitMode : '2766f77a-f64c-4dce-b539-e7fc03212a87'",
					"headerTemplate": "#ResourceString(ExitRulesFilter_headerTemplate)#"
				},
				"parentName": "ExitMessagesFlexContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "Contacts_Tab",
				"values": {
					"type": "crt.TabContainer",
					"items": [],
					"caption": "#ResourceString(Contacts_Tab_caption)#",
					"iconPosition": "only-text",
					"visible": true
				},
				"parentName": "Tabs",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "GridContainer_gqna3kx",
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
						"rowGap": null
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
				"parentName": "Contacts_Tab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "AllContactsInSegment_ExpansionPanel",
				"values": {
					"type": "crt.ExpansionPanel",
					"tools": [],
					"items": [],
					"title": "#ResourceString(AllContactsInSegment_ExpansionPanel_title)#",
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
						"row": 1,
						"rowSpan": 1
					}
				},
				"parentName": "GridContainer_gqna3kx",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridContainer_2ek9mfp",
				"values": {
					"type": "crt.GridContainer",
					"rows": "minmax(max-content, 24px)",
					"columns": [
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
				"parentName": "AllContactsInSegment_ExpansionPanel",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_vzifeya",
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
				"parentName": "GridContainer_2ek9mfp",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailRefreshBtn_56wtcv3",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailRefreshBtn_56wtcv3_caption)#",
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
							"dataSourceName": "ContactsInSegmentDataGridDS"
						}
					}
				},
				"parentName": "FlexContainer_vzifeya",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailSettingsBtn_l8io2n0",
				"values": {
					"type": "crt.Button",
					"caption": "#ResourceString(GridDetailSettingsBtn_l8io2n0_caption)#",
					"icon": "actions-button-icon",
					"iconPosition": "only-icon",
					"color": "default",
					"size": "medium",
					"clickMode": "menu",
					"menuItems": [],
					"visible": false
				},
				"parentName": "FlexContainer_vzifeya",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GridDetailExportDataBtn_lkm2swr",
				"values": {
					"type": "crt.MenuItem",
					"caption": "#ResourceString(GridDetailExportDataBtn_lkm2swr_caption)#",
					"icon": "export-button-icon",
					"color": "default",
					"size": "medium",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "ContactsInSegmentDataGrid"
						}
					}
				},
				"parentName": "GridDetailSettingsBtn_l8io2n0",
				"propertyName": "menuItems",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GridDetailSearchFilter_0bascus",
				"values": {
					"type": "crt.SearchFilter",
					"placeholder": "#ResourceString(GridDetailSearchFilter_0bascus_placeholder)#",
					"iconOnly": true,
					"_filterOptions": {
						"expose": [
							{
								"attribute": "GridDetailSearchFilter_0bascus_ContactsInSegmentDataGrid",
								"converters": [
									{
										"converter": "crt.SearchFilterAttributeConverter",
										"args": [
											"ContactsInSegmentDataGrid"
										]
									}
								]
							}
						],
						"from": [
							"GridDetailSearchFilter_0bascus_SearchValue",
							"GridDetailSearchFilter_0bascus_FilteredColumnsGroups"
						]
					}
				},
				"parentName": "FlexContainer_vzifeya",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "GridContainer_uwsfglu",
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
				"parentName": "AllContactsInSegment_ExpansionPanel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ContactsInSegmentDataGrid",
				"values": {
					"type": "crt.DataGrid",
					"layoutConfig": {
						"colSpan": 2,
						"column": 1,
						"row": 1,
						"rowSpan": 21
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
					"items": "$GridDetail_xs1mio6",
					"activeRow": "$GridDetail_xs1mio6_ActiveRow",
					"selectionState": "$GridDetail_xs1mio6_SelectionState",
					"primaryColumnName": "ContactsInSegmentDataGridDS_Id",
					"columns": [
						{
							"id": "5abff659-9fab-be19-0f17-9114a04c88b9",
							"code": "ContactsInSegmentDataGridDS_Name",
							"caption": "#ResourceString(ContactsInSegmentDataGridDS_Name)#",
							"dataValueType": 28,
							"width": 220
						},
						{
							"id": "2aeb416e-4a1f-d674-0fd1-5eca8e5d6332",
							"code": "ContactsInSegmentDataGridDS_Email",
							"caption": "#ResourceString(ContactsInSegmentDataGridDS_Email)#",
							"dataValueType": 45,
							"width": 195
						},
						{
							"id": "096e81cb-8928-06b5-c060-a91bc91b6424",
							"code": "ContactsInSegmentDataGridDS_Account",
							"caption": "#ResourceString(ContactsInSegmentDataGridDS_Account)#",
							"dataValueType": 10,
							"width": 175
						},
						{
							"id": "ffa5d1db-1547-306b-2d1e-7ca9bb7f4e04",
							"code": "ContactsInSegmentDataGridDS_Job",
							"caption": "#ResourceString(ContactsInSegmentDataGridDS_Job)#",
							"dataValueType": 10
						},
						{
							"id": "2e1f3cc2-3523-589b-8a8a-94cc489fe304",
							"code": "ContactsInSegmentDataGridDS_Country",
							"caption": "#ResourceString(ContactsInSegmentDataGridDS_Country)#",
							"dataValueType": 10
						}
					],
					"placeholder": false,
					"_selectionOptions": {
						"attribute": "GridDetail_xs1mio6_SelectionState"
					},
					"rowToolbarItems": [
						{
							"type": "crt.MenuItem",
							"caption": "#ResourceString(ContactsInSegmentDataGrid_OpenAction)#",
							"icon": "edit-row-action",
							"disabled": "$GridDetail_xs1mio6.PrimaryModelMode | crt.IsEqual : 'create'",
							"clicked": {
								"request": "crt.UpdateRecordRequest",
								"params": {
									"itemsAttributeName": "GridDetail_xs1mio6",
									"recordId": "$GridDetail_xs1mio6.ContactsInSegmentDataGridDS_Id"
								},
								"useRelativeContext": true
							}
						},
						{
							"type": "crt.MenuItem",
							"caption": "#ResourceString(ContactsInSegmentDataGrid_CopyAction)#",
							"icon": "copy-row-action",
							"disabled": "$GridDetail_xs1mio6.PrimaryModelMode | crt.IsEqual : 'create'",
							"clicked": {
								"request": "crt.CopyRecordRequest",
								"params": {
									"itemsAttributeName": "GridDetail_xs1mio6",
									"recordId": "$GridDetail_xs1mio6.ContactsInSegmentDataGridDS_Id"
								},
								"useRelativeContext": true
							}
						}
					],
					"bulkActions": [],
					"visible": true,
					"fitContent": true
				},
				"parentName": "GridContainer_uwsfglu",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ContactsInSegmentDataGrid_AddTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Add tag",
					"icon": "tag-icon",
					"clicked": {
						"request": "crt.AddTagsInRecordsRequest",
						"params": {
							"dataSourceName": "ContactsInSegmentDataGridDS",
							"filters": "$GridDetail_xs1mio6 | crt.ToCollectionFilters : 'GridDetail_xs1mio6' : $GridDetail_xs1mio6_SelectionState | crt.SkipIfSelectionEmpty : $GridDetail_xs1mio6_SelectionState"
						}
					},
					"items": []
				},
				"parentName": "ContactsInSegmentDataGrid",
				"propertyName": "bulkActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ContactsInSegmentDataGrid_RemoveTagsBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Remove tag",
					"icon": "delete-button-icon",
					"clicked": {
						"request": "crt.RemoveTagsInRecordsRequest",
						"params": {
							"dataSourceName": "ContactsInSegmentDataGridDS",
							"filters": "$GridDetail_xs1mio6 | crt.ToCollectionFilters : 'GridDetail_xs1mio6' : $GridDetail_xs1mio6_SelectionState | crt.SkipIfSelectionEmpty : $GridDetail_xs1mio6_SelectionState"
						}
					}
				},
				"parentName": "ContactsInSegmentDataGrid_AddTagsBulkAction",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ContactsInSegmentDataGrid_ExportToExcelBulkAction",
				"values": {
					"type": "crt.MenuItem",
					"caption": "Export to Excel",
					"icon": "export-button-icon",
					"clicked": {
						"request": "crt.ExportDataGridToExcelRequest",
						"params": {
							"viewName": "ContactsInSegmentDataGrid",
							"filters": "$GridDetail_xs1mio6 | crt.ToCollectionFilters : 'GridDetail_xs1mio6' : $GridDetail_xs1mio6_SelectionState | crt.SkipIfSelectionEmpty : $GridDetail_xs1mio6_SelectionState"
						}
					}
				},
				"parentName": "ContactsInSegmentDataGrid",
				"propertyName": "bulkActions",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "SegmentInfoTab",
				"values": {
					"type": "crt.TabContainer",
					"tools": [],
					"items": [],
					"caption": "#ResourceString(SegmentInfoTab_caption)#",
					"iconPosition": "left-icon",
					"visible": true,
					"icon": "segments-icon"
				},
				"parentName": "CardToggleTabPanel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_w9etajj",
				"values": {
					"type": "crt.FlexContainer",
					"direction": "row",
					"alignItems": "center",
					"items": []
				},
				"parentName": "SegmentInfoTab",
				"propertyName": "tools",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Label_zv2w74e",
				"values": {
					"type": "crt.Label",
					"caption": "#MacrosTemplateString(#ResourceString(Label_zv2w74e_caption)#)#",
					"labelType": "headline-3",
					"labelThickness": "default",
					"labelEllipsis": false,
					"labelColor": "#0D2E4E",
					"labelBackgroundColor": "transparent",
					"labelTextAlign": "start",
					"visible": true,
					"headingLevel": "label"
				},
				"parentName": "FlexContainer_w9etajj",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FlexContainer_1x95a2r",
				"values": {
					"type": "crt.FlexContainer",
					"items": [],
					"direction": "column"
				},
				"parentName": "SegmentInfoTab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Name",
				"values": {
					"type": "crt.Input",
					"label": "$Resources.Strings.Name",
					"control": "$Name",
					"labelPosition": "above",
					"visible": true,
					"readonly": false,
					"placeholder": "",
					"tooltip": ""
				},
				"parentName": "FlexContainer_1x95a2r",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Input_gptgaxr",
				"values": {
					"type": "crt.Input",
					"label": "$Resources.Strings.PDS_Description_3ifsj63",
					"control": "$PDS_Description_3ifsj63",
					"placeholder": "",
					"tooltip": "",
					"readonly": false,
					"multiline": true,
					"labelPosition": "above",
					"visible": true
				},
				"parentName": "FlexContainer_1x95a2r",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "ComboBox_d9u56vv",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_ExitMode_kbq5ezg",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "above",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_ExitMode_kbq5ezg",
					"visible": true,
					"readonly": false,
					"placeholder": ""
				},
				"parentName": "FlexContainer_1x95a2r",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "addRecord_9fgkhcu",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(addRecord_9fgkhcu_caption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "ComboBox_d9u56vv",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ComboBox_ts8y1q1",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_SysUserStatus_acuppsa",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "above",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_SysUserStatus_acuppsa",
					"visible": true,
					"readonly": true,
					"placeholder": ""
				},
				"parentName": "FlexContainer_1x95a2r",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "addRecord_94uvagk",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(addRecord_94uvagk_caption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "ComboBox_ts8y1q1",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "ComboBox_2bwzvxn",
				"values": {
					"type": "crt.ComboBox",
					"label": "$Resources.Strings.PDS_Status_h9bzq20",
					"ariaLabel": "",
					"isAddAllowed": true,
					"showValueAsLink": true,
					"labelPosition": "above",
					"controlActions": [],
					"listActions": [],
					"tooltip": "",
					"control": "$PDS_Status_h9bzq20",
					"visible": true,
					"readonly": true,
					"placeholder": ""
				},
				"parentName": "FlexContainer_1x95a2r",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "addRecord_qaekg69",
				"values": {
					"code": "addRecord",
					"type": "crt.ComboboxSearchTextAction",
					"icon": "combobox-add-new",
					"caption": "#ResourceString(addRecord_qaekg69_caption)#",
					"clicked": {
						"request": "crt.CreateRecordFromLookupRequest",
						"params": {}
					}
				},
				"parentName": "ComboBox_2bwzvxn",
				"propertyName": "listActions",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NumberInput_qh94iue",
				"values": {
					"type": "crt.NumberInput",
					"label": "$Resources.Strings.PDS_RecordCount_0a0c60u",
					"control": "$PDS_RecordCount_0a0c60u",
					"readonly": true,
					"placeholder": "",
					"labelPosition": "above",
					"tooltip": "",
					"visible": true
				},
				"parentName": "FlexContainer_1x95a2r",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "DateTimePicker_w61gk8g",
				"values": {
					"type": "crt.DateTimePicker",
					"label": "#ResourceString(DateTimePicker_w61gk8g_label)#",
					"placeholder": "",
					"readonly": false,
					"labelPosition": "auto",
					"tooltip": "",
					"pickerType": "datetime",
					"control": "$PDS_LastActualizationDate_d24g641",
					"visible": true
				},
				"parentName": "FlexContainer_1x95a2r",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "DateTimePicker_hfkuv0m",
				"values": {
					"type": "crt.DateTimePicker",
					"label": "#ResourceString(DateTimePicker_hfkuv0m_label)#",
					"placeholder": "",
					"readonly": false,
					"labelPosition": "auto",
					"tooltip": "",
					"pickerType": "datetime",
					"control": "$PDS_CreatedOn_0ta15f8",
					"visible": true
				},
				"parentName": "FlexContainer_1x95a2r",
				"propertyName": "items",
				"index": 7
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
					"PDS_EntitySchemaName": {
						"modelConfig": {
							"path": "PDS.EntitySchema.Name"
						}
					},
					"PDS_Description_3ifsj63": {
						"modelConfig": {
							"path": "PDS.Description"
						}
					},
					"PDS_RefreshMode_2pd62c1": {
						"modelConfig": {
							"path": "PDS.SysRefreshMode"
						}
					},
					"PDS_EntryFilterData_rhcluyw": {
						"modelConfig": {
							"path": "PDS.EntryFilterData"
						}
					},
					"PDS_RefreshMode_2pd62c1_List": {
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
					"PDS_Status_h9bzq20": {
						"modelConfig": {
							"path": "PDS.SysStatus"
						}
					},
					"PDS_Status_h9bzq20_List": {
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
					"PDS_ExitMode_kbq5ezg": {
						"modelConfig": {
							"path": "PDS.SysExitMode"
						}
					},
					"PDS_EntryFilterData_cuzzx0r": {
						"modelConfig": {
							"path": "PDS.EntryFilterData"
						}
					},
					"PDS_ExitFilterData_xyikz5x": {
						"modelConfig": {
							"path": "PDS.ExitFilterData"
						}
					},
					"PDS_ExitMode_kbq5ezg_List": {
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
					"PDS_RecordCount_0a0c60u": {
						"modelConfig": {
							"path": "PDS.RecordCount"
						}
					},
					"PDS_SysUserStatus_acuppsa": {
						"modelConfig": {
							"path": "PDS.SysUserStatus"
						}
					},
					"PDS_SysUserStatus_acuppsa_List": {
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
					"GridDetail_xs1mio6": {
						"isCollection": true,
						"modelConfig": {
							"path": "ContactsInSegmentDataGridDS",
							"filterAttributes": [
								{
									"loadOnChange": true,
									"name": "GridDetail_xs1mio6_PredefinedFilter"
								}
							]
						},
						"viewModelConfig": {
							"attributes": {
								"ContactsInSegmentDataGridDS_Name": {
									"modelConfig": {
										"path": "ContactsInSegmentDataGridDS.Name"
									}
								},
								"ContactsInSegmentDataGridDS_Email": {
									"modelConfig": {
										"path": "ContactsInSegmentDataGridDS.Email"
									}
								},
								"ContactsInSegmentDataGridDS_Account": {
									"modelConfig": {
										"path": "ContactsInSegmentDataGridDS.Account"
									}
								},
								"ContactsInSegmentDataGridDS_Job": {
									"modelConfig": {
										"path": "ContactsInSegmentDataGridDS.Job"
									}
								},
								"ContactsInSegmentDataGridDS_Country": {
									"modelConfig": {
										"path": "ContactsInSegmentDataGridDS.Country"
									}
								},
								"ContactsInSegmentDataGridDS_Id": {
									"modelConfig": {
										"path": "ContactsInSegmentDataGridDS.Id"
									}
								}
							}
						}
					},
					"MatchingContactsMetric_EffectiveFilter": {
					    "from": "EntryRulesFilter_Filter",
					    "converter": "usr.BlockEmptyEntryFilter : $EntryRulesFilter_Filter"
					},					"GridDetail_xs1mio6_PredefinedFilter": {
						"from": "PDS_SysUserStatus_acuppsa",
						"converter": "usr.GetSegmentContactsFilter : $Id"
					},
					"PDS_LastActualizationDate_d24g641": {
						"modelConfig": {
							"path": "PDS.LastActualizationDate"
						}
					},
					"PDS_CreatedOn_0ta15f8": {
						"modelConfig": {
							"path": "PDS.CreatedOn"
						}
					},
					"ComboBox_zpkc1ym_ValueDetails": {
						"modelConfig": {
							"path": "PDS.SysRefreshModeDescription"
						}
					},
					"PDS_SysUserStatusColor": {
						"modelConfig": {
							"path": "PDS.SysUserStatusColor"
						}
					},
					"StatusChip_Items": {
						"from": "PDS_SysUserStatus_acuppsa",
						"converter": "usr.GetStatusChipItems : $PDS_SysUserStatusColor"
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
						"IndicatorWidget_vuak35a_DataDS": [
							{
								"attributePath": "Id",
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
							"entitySchemaName": "SysDataSegment",
							"attributes": {
								"SysRefreshModeDescription": {
									"path": "SysRefreshMode.Description",
									"type": "ForwardReference"
								},
								"SysUserStatusColor": {
									"path": "SysUserStatus.Color",
									"type": "ForwardReference"
								}
							}
						},
						"scope": "page"
					},
					"ContactsInSegmentDataGridDS": {
						"type": "crt.EntityDataSource",
						"scope": "viewElement",
						"config": {
							"entitySchemaName": "Contact",
							"attributes": {
								"Name": {
									"path": "Name"
								},
								"Email": {
									"path": "Email"
								},
								"Account": {
									"path": "Account"
								},
								"Job": {
									"path": "Job"
								},
								"Country": {
									"path": "Country"
								}
							}
						}
					}
				}
			}
		]/**SCHEMA_MODEL_CONFIG_DIFF*/,
		handlers: /**SCHEMA_HANDLERS*/[
			{
				request: "crt.FilterBuilderValueChangeRequest",
				handler: async (request, next) => {
					const status = await request.$context.PDS_SysUserStatus_acuppsa;
					// equals Active
					if (status?.value !== '9adc43e5-9f50-476f-a481-aa95e22c20f2') {
						return next?.handle(request);
					}
					const storageAttribute = request.targetAttribute;
					if (storageAttribute) {
						// The blocked change never reached the storage attribute, so the
						// FilterBuilderSource still renders the user's edit from its internal
						// state. The data input binding is distinctUntilChanged, so a plain
						// re-set of the unchanged attribute is swallowed: push an intermediate
						// value, let a change detection cycle pass, then write the stored
						// value back to rebuild the panel from it.
						const restoreOptions = {
							preventStateChange: true,
							preventRunBusinessRules: true
						};
						const storedValue = (await request.$context[storageAttribute]) ?? null;
						const intermediateValue = storedValue === null ? '{}' : null;
						await request.$context.set(storageAttribute, intermediateValue, restoreOptions);
						await new Promise((resolve) => setTimeout(resolve));
						await request.$context.set(storageAttribute, storedValue, restoreOptions);
					}
					const handlerChain = sdk.HandlerChainService.instance;
					const message = await request.$context.Resources.Strings.ActiveSegmentReadOnlyMessage;
					await handlerChain.process({
						type: 'crt.NotificationRequest',
						message: message,
						$context: request.$context,
						scopes: [...request.scopes]
					});
				}
			},
			{
				request: "usr.SetSegmentExitModeRequest",
				handler: async (request, next) => {
					const status = await request.$context.PDS_SysUserStatus_acuppsa;
					// equals Active
					if (status?.value === '9adc43e5-9f50-476f-a481-aa95e22c20f2') {
						// The exit strategy can't be changed while the segment is active.
						// The toggle's selected state is bound to the unchanged exit mode
						// attribute, so blocking the write keeps the buttons in sync — no
						// value restore is required (unlike the filter builder case).
						const handlerChain = sdk.HandlerChainService.instance;
						const message = await request.$context.Resources.Strings.ActiveSegmentReadOnlyMessage;
						await handlerChain.process({
							type: 'crt.NotificationRequest',
							message: message,
							$context: request.$context,
							scopes: [...request.scopes]
						});
						return;
					}
					const exitModeId = request.exitModeId ?? request.params?.exitModeId;
					const exitModeName = request.exitModeName ?? request.params?.exitModeName;
					if (exitModeId) {
						await request.$context.set("PDS_ExitMode_kbq5ezg", {
							value: exitModeId,
							displayValue: exitModeName ?? ""
						});
					}
					return next?.handle(request);
				}
			}
		]/**SCHEMA_HANDLERS*/,
		converters: /**SCHEMA_CONVERTERS*/{
			"usr.GetExitModeButtonColor": function(mode, _, expectedId) {
				const current = Array.isArray(mode) ? mode[0] : mode;
				const selected = current && String(current.value).toLowerCase() === String(expectedId).toLowerCase();
				return selected ? "accent" : "default";
			},
			"usr.GetExitModeButtonClasses": function(mode, _, expectedId) {
				const current = Array.isArray(mode) ? mode[0] : mode;
				const selected = current && String(current.value).toLowerCase() === String(expectedId).toLowerCase();
				return selected ? ["custom-toggle-active-button"]: [];
			},
			"usr.IsExitMode": function(mode, _, expectedId) {
				const current = Array.isArray(mode) ? mode[0] : mode;
				return !!(current && String(current.value).toLowerCase() === String(expectedId).toLowerCase());
			},
			"usr.IsAnyExitMode": function(mode, _, ...ids) {
				const current = Array.isArray(mode) ? mode[0] : mode;
				if (!current || current.value == null) {
					return false;
				}
				const value = String(current.value).toLowerCase();
				return ids.some((id) => String(id).toLowerCase() === value);
			},
			"usr.GetStatusChipItems": function(status, _, color) {
				if (!status || !status.displayValue) {
					return [];
				}
				return [{
					caption: status.displayValue,
					color: color || "#626262"
				}];
			},
			"usr.GetSegmentContactsFilter": function(status, _, id) {
			    const segmentId = Array.isArray(id) ? id[0] : id;
			    if (!segmentId) {
			        return null;
			    }
			    const DRAFT_USER_STATUS_ID = '08f98887-eacc-4510-8d94-648f8f5d5f2f';
			    if (String(status?.value).toLowerCase() === DRAFT_USER_STATUS_ID) {
			        // Draft segments have no actualized data; skip the segment API query.
			        return {
			            "items": {
			                "Noop": {
			                    "isEnabled": true,
			                    "trimDateTimeParameterToDate": false,
			                    "filterType": 1,
			                    "comparisonType": 3,
			                    "leftExpression": {
			                        "expressionType": 0,
			                        "columnPath": "Id"
			                    },
			                    "rightExpression": {
			                        "expressionType": 2,
			                        "parameter": {
			                            "dataValueType": 1,
			                            "value": "00000000-0000-0000-0000-000000000000"
			                        }
			                    }
			                }
			            },
			            "logicalOperation": 0,
			            "isEnabled": true,
			            "filterType": 6,
			            "rootSchemaName": "Contact"
			        };
			    }
			    return {
			        "items": {
			            "Seg": {
			                "filterType": 7,
			                "comparisonType": 15,
			                "isEnabled": true,
			                "segmentFilterOptions": {
			                    "segmentId": segmentId,
			                    "ignoreUserStatus": true
			                }
			            }
			        },
			        "logicalOperation": 0,
			        "isEnabled": true,
			        "filterType": 6,
			        "rootSchemaName": "Contact"
			    };
			},
			"usr.BlockEmptyEntryFilter": function(value) {
			    // FilterType: Compare=1 IsNull=2 Between=3 In=4 Exists=5 Group=6 Segment=7
			    const FT = { COMPARE: 1, IS_NULL: 2, BETWEEN: 3, IN: 4, EXISTS: 5, GROUP: 6, SEGMENT: 7 };
			    const hasParamValue = (e) => e?.parameter?.value != null && e.parameter.value !== "";
			
			    const isEffective = (f) => {
			        if (!f || f.isEnabled === false) return false;
			        switch (f.filterType) {
			            case FT.IS_NULL: case FT.EXISTS: return true;
			            case FT.SEGMENT: return !!f.segmentFilterOptions?.segmentId;
			            case FT.GROUP:
			                return Object.values(f.items || f.filters || {}).some(isEffective);
			            case FT.COMPARE: case FT.BETWEEN: case FT.IN:
			                return hasParamValue(f.rightExpression)
			                    || (f.rightExpressions || []).some(hasParamValue);
			            default: return true;
			        }
			    };
			
			    const input = Array.isArray(value) ? value[0] : value;
			    if (isEffective(input)) return input;
			
			    return {
			        filterType: FT.GROUP,
			        isEnabled: true,
			        logicalOperation: 0,
			        rootSchemaName: "Contact",
			        items: {
			            NeverMatch: {
			                filterType: FT.IS_NULL,
			                comparisonType: 1,
			                isEnabled: true,
			                leftExpression: { expressionType: 0, columnPath: "Id" },
			                dataValueType: 0
			            }
			        }
			    };
			}
		}/**SCHEMA_CONVERTERS*/,
		validators: /**SCHEMA_VALIDATORS*/{}/**SCHEMA_VALIDATORS*/
	};
});