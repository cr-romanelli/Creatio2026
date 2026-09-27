 {
	"viewConfigDiff": [
		{
			"operation": "merge",
			"name": "FolderTreeActions",
			"values": {
				"rootSchemaName": "TrnCRealty"
			}
		},
		{
			"operation": "merge",
			"name": "FolderTreeActions",
			"values": {
				"caption": "Folders"
			}
		},
		{
			"operation": "merge",
			"name": "ListItem",
			"values": {
				"title": "$PDS_TrnCName",
				"body": [
					{
						"value": "$PDS_CreatedOn"
					},
					{
						"value": "$PDS_CreatedBy"
					}
				]
			}
		}
	],
	"viewModelConfigDiff": [
		{
			"operation": "merge",
			"path": ["attributes", "Items", "viewModelConfig", "attributes"],
			"values": {
				"PDS_Id": {
					"modelConfig": {
						"path": "PDS.Id"
					}
				},
				"PDS_TrnCName": {
					"modelConfig": {
						"path": "PDS.TrnCName"
					}
				},
				"PDS_CreatedOn": {
					"modelConfig": {
						"path": "PDS.CreatedOn"
					}
				},
				"PDS_CreatedBy": {
					"modelConfig": {
						"path": "PDS.CreatedBy"
					}
				}
			}
		}
	],
	"modelConfigDiff": [
		{
			"operation": "merge",
			"path": ["dataSources", "PDS", "config"],
			"values": {
				"entitySchemaName": "TrnCRealty"
			}
		}
	]
}
 