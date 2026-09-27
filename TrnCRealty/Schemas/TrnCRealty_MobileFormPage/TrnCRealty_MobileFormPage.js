{
	"viewConfigDiff": [
		{
			"operation": "insert",
			"name": "TrnCName",
			"values": {
				"type": "crt.Input",
				"label": "$Resources.Strings.TrnCName",
				"control": "$TrnCName",
				"placeholder": "",
				"readonly": false,
				"multiline": false,
				"labelPosition": "above"
			},
			"parentName": "AreaProfileContainer",
			"propertyName": "items",
			"index": 0
		},
		{
			"operation": "merge",
			"name": "Feed",
			"values": {
				"dataSourceName": "PDS",
				"entitySchemaName": "TrnCRealty"
			}
		}
	],
	"viewModelConfigDiff": [
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
				"Id": {
					"modelConfig": {
						"path": "PDS.Id"
					}
				}
			}
		}
	],
	"modelConfigDiff": [
		{
			"operation": "merge",
			"path": [],
			"values": {
				"dataSources": {
					"PDS": {
						"type": "crt.EntityDataSource",
						"scope": "page",
						"config": {
							"entitySchemaName": "TrnCRealty",
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
				"primaryDataSourceName": "PDS"
			}
		}
	]
}