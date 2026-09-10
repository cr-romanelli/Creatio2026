define("ConfActivityLogDiffMapper", ["ConfActivityLogDiffMapperResources"], function(resources) {
	const Texts = {
		defaultErrorMessage: resources.localizableStrings.DefaultErrorMessage,
		sourceCode: resources.localizableStrings.SourceCode,
		metadata: resources.localizableStrings.Metadata,
		descriptor: resources.localizableStrings.Descriptor,
		noFilesTitle: resources.localizableStrings.NoFilesTitle,
		noFilesMessage: resources.localizableStrings.NoFilesMessage,
		noSupportedContentTitle: resources.localizableStrings.NoSupportedContentTitle,
		noSupportedContentMessage: resources.localizableStrings.NoSupportedContentMessage
	};
	const ResponseValueNames = ["values", "Values"];
	const ResponseOperationNames = ["operation", "Operation"];
	const EntryIdNames = ["logEntryId", "LogEntryId"];
	const EntryModifiedOnNames = ["modifiedOn", "ModifiedOn"];
	const EntryModifiedByNames = ["modifiedBy", "ModifiedBy"];
	const EntryFilesNames = ["files", "Files"];
	const FilePathNames = ["path", "Path"];
	const FileContentNames = ["content", "Content"];
	const FileContentTypeNames = ["contentType", "ContentType"];
	const ErrorInfoNames = ["errorInfo", "ErrorInfo"];
	const ErrorMessageNames = ["message", "Message"];
	const SuccessNames = ["success", "Success"];
	const EmptyGuid = "00000000-0000-0000-0000-000000000000";
	const CreateOperationName = "create";
	const UpdateOperationName = "update";
	const DescriptorFileNames = ["descriptor.json", "app-descriptor.json"];
	const ModeDefinitions = [
		{
			key: "sourceCode",
			title: Texts.sourceCode,
			matches: function(fileName) {
				return /\.(cs|js|less|sql|css|html)$/.test(fileName);
			}
		},
		{
			key: "metadata",
			title: Texts.metadata,
			matches: function(fileName) {
				return /\.json$/.test(fileName) && DescriptorFileNames.indexOf(fileName) === -1;
			}
		},
		{
			key: "descriptor",
			title: Texts.descriptor,
			matches: function(fileName) {
				return DescriptorFileNames.indexOf(fileName) !== -1;
			}
		}
	];

	function normalizeText(value) {
		return value === null || value === undefined ? "" : String(value);
	}

	function normalizeLowerCase(value) {
		return normalizeText(value).trim().toLowerCase();
	}

	function getFirstDefinedValue(source, propertyNames, defaultValue) {
		if (!source) {
			return defaultValue;
		}
		for (let index = 0; index < propertyNames.length; index++) {
			const propertyName = propertyNames[index];
			if (source[propertyName] !== undefined) {
				return source[propertyName];
			}
		}
		return defaultValue;
	}

	function getArrayValue(source, propertyNames) {
		const value = getFirstDefinedValue(source, propertyNames, []);
		return Array.isArray(value) ? value : [];
	}

	function normalizePath(path) {
		return normalizeText(path)
			.replace(/\\/g, "/")
			.replace(/\/+/g, "/")
			.toLowerCase();
	}

	function getResponseValues(response) {
		return getArrayValue(response, ResponseValueNames);
	}

	function getOperationName(response, currentEntry) {
		return getFirstDefinedValue(response, ResponseOperationNames,
			getFirstDefinedValue(currentEntry, ResponseOperationNames, ""));
	}

	function isCreateOperation(response, currentEntry) {
		return normalizeLowerCase(getOperationName(response, currentEntry)) === CreateOperationName;
	}

	function isUpdateOperation(response, currentEntry) {
		return normalizeLowerCase(getOperationName(response, currentEntry)) === UpdateOperationName;
	}

	function getEntryId(entry) {
		return normalizeText(getFirstDefinedValue(entry, EntryIdNames, ""));
	}

	function getEntryFiles(entry) {
		return getArrayValue(entry, EntryFilesNames);
	}

	function createEntryVersionInfo(entry) {
		if (!entry || isEmptyGuid(getEntryId(entry))) {
			return null;
		}
		return {
			modifiedOn: normalizeText(getFirstDefinedValue(entry, EntryModifiedOnNames, "")),
			modifiedBy: normalizeText(getFirstDefinedValue(entry, EntryModifiedByNames, ""))
		};
	}

	function getFileName(path) {
		const normalizedPath = normalizeText(path);
		const parts = normalizedPath.split(/[\\/]/);
		return parts[parts.length - 1] || normalizedPath;
	}

	function getNormalizedFileName(path) {
		return normalizeLowerCase(getFileName(path));
	}

	function getFileKey(path) {
		const normalizedPath = normalizePath(path);
		if (normalizedPath) {
			return normalizedPath;
		}
		return normalizeLowerCase(getFileName(path));
	}

	function createFileModel(file) {
		return {
			path: normalizeText(getFirstDefinedValue(file, FilePathNames, "")),
			contentType: normalizeText(getFirstDefinedValue(file, FileContentTypeNames, "")),
			content: normalizeText(getFirstDefinedValue(file, FileContentNames, ""))
		};
	}

	function buildFileMap(entry) {
		return getEntryFiles(entry).reduce(function(fileMap, file) {
			const fileModel = createFileModel(file);
			const fileKey = getFileKey(fileModel.path);
			if (!fileKey || Object.prototype.hasOwnProperty.call(fileMap, fileKey)) {
				return fileMap;
			}
			fileMap[fileKey] = fileModel;
			return fileMap;
		}, Object.create(null));
	}

	function findEntryById(values, currentLogEntryId) {
		const normalizedCurrentId = normalizeLowerCase(currentLogEntryId);
		if (!normalizedCurrentId) {
			return null;
		}
		return values.find(function(entry) {
			return normalizeLowerCase(getEntryId(entry)) === normalizedCurrentId;
		}) || null;
	}

	function findPreviousEntry(values, currentEntry) {
		return values.find(function(entry) {
			return entry !== currentEntry;
		}) || null;
	}

	function resolveEntries(values, currentLogEntryId) {
		const currentEntry = findEntryById(values, currentLogEntryId);
		if (currentEntry) {
			return {
				previous: findPreviousEntry(values, currentEntry),
				current: currentEntry
			};
		}
		return {
			previous: values[0] || null,
			current: values[1] || null
		};
	}

	function addKeys(target, source) {
		Object.keys(source).forEach(function(key) {
			target[key] = true;
		});
	}

	function collectFileKeys(previousFiles, currentFiles) {
		const keys = Object.create(null);
		addKeys(keys, previousFiles);
		addKeys(keys, currentFiles);
		return Object.keys(keys);
	}

	function createComparableFile(key, previousFile, currentFile) {
		return {
			key: key,
			displayPath: currentFile && currentFile.path || previousFile && previousFile.path || key,
			oldContent: previousFile ? previousFile.content : "",
			newContent: currentFile ? currentFile.content : "",
			oldContentType: previousFile ? previousFile.contentType : "",
			newContentType: currentFile ? currentFile.contentType : "",
			hasOld: !!previousFile,
			hasNew: !!currentFile
		};
	}

	function hasChanges(file) {
		return !file.hasOld || !file.hasNew || file.oldContent !== file.newContent;
	}

	function compareFiles(left, right) {
		return (left.displayPath || left.key).localeCompare(right.displayPath || right.key);
	}

	function isEmptyGuid(value) {
		return normalizeLowerCase(value) === EmptyGuid;
	}

	function createEmptyMode(definition) {
		return {
			key: definition.key,
			title: definition.title,
			files: []
		};
	}

	function createEmptyModeMap() {
		return ModeDefinitions.reduce(function(modeMap, definition) {
			modeMap[definition.key] = createEmptyMode(definition);
			return modeMap;
		}, Object.create(null));
	}

	function getFileMode(file) {
		const fileName = getNormalizedFileName(file.displayPath || file.key);
		for (let index = 0; index < ModeDefinitions.length; index++) {
			if (ModeDefinitions[index].matches(fileName)) {
				return ModeDefinitions[index].key;
			}
		}
		return "";
	}

	function groupFilesByMode(files) {
		const modeMap = createEmptyModeMap();
		files.forEach(function(file) {
			const modeKey = getFileMode(file);
			if (!modeKey) {
				return;
			}
			modeMap[modeKey].files.push(file);
		});
		return modeMap;
	}

	function getAvailableModes(filesByMode) {
		return ModeDefinitions.map(function(definition) {
			const mode = filesByMode[definition.key];
			return {
				key: definition.key,
				title: definition.title,
				count: mode ? mode.files.length : 0
			};
		});
	}

	function getDefaultMode(filesByMode) {
		for (let index = 0; index < ModeDefinitions.length; index++) {
			const mode = filesByMode[ModeDefinitions[index].key];
			if (mode && mode.files.length) {
				return mode.key;
			}
		}
		return "";
	}

	function createDiffModel(files, hasPreviousVersion, versionInfo, emptyState, options) {
		const filesByMode = groupFilesByMode(files);
		options = options || {};
		return {
			hasPreviousVersion: hasPreviousVersion,
			showCurrentContentOnly: !!options.showCurrentContentOnly,
			versionInfo: versionInfo || {
				previous: null,
				current: null
			},
			availableModes: getAvailableModes(filesByMode),
			defaultMode: getDefaultMode(filesByMode),
			filesByMode: filesByMode,
			emptyState: emptyState || null
		};
	}

	function createVersionInfo(previousEntry, currentEntry, hasPreviousVersion) {
		return {
			previous: hasPreviousVersion ? createEntryVersionInfo(previousEntry) : null,
			current: createEntryVersionInfo(currentEntry)
		};
	}

	function buildComparableFiles(response, options) {
		const entries = resolveEntries(
			getResponseValues(response),
			options && options.currentLogEntryId
		);
		if (!entries.current) {
			return [];
		}

		const hasPreviousVersion = !isCreateOperation(response, entries.current) && !!entries.previous &&
			!isEmptyGuid(getEntryId(entries.previous));
		const previousFiles = hasPreviousVersion ? buildFileMap(entries.previous) : Object.create(null);
		const currentFiles = buildFileMap(entries.current);
		return collectFileKeys(previousFiles, currentFiles).map(function(key) {
			return createComparableFile(key, previousFiles[key], currentFiles[key]);
		}).filter(function(file) {
			return hasChanges(file);
		}).sort(function(left, right) {
			return compareFiles(left, right);
		});
	}

	function buildDiffModel(response, options) {
		const entries = resolveEntries(
			getResponseValues(response),
			options && options.currentLogEntryId
		);
		if (!entries.current) {
			return createDiffModel([], true, createVersionInfo(null, null, false), {
				title: Texts.noFilesTitle,
				message: Texts.noFilesMessage
			});
		}
		const hasPreviousVersion = !isCreateOperation(response, entries.current) && !!entries.previous &&
			!isEmptyGuid(getEntryId(entries.previous));
		const showCurrentContentOnly = isUpdateOperation(response, entries.current) && !hasPreviousVersion;
		const versionInfo = createVersionInfo(entries.previous, entries.current, hasPreviousVersion);
		const previousFiles = hasPreviousVersion ? buildFileMap(entries.previous) : Object.create(null);
		const currentFiles = buildFileMap(entries.current);
		const files = collectFileKeys(previousFiles, currentFiles).map(function(key) {
			return createComparableFile(key, previousFiles[key], currentFiles[key]);
		}).filter(function(file) {
			return hasChanges(file);
		}).sort(function(left, right) {
			return compareFiles(left, right);
		});
		const model = createDiffModel(files, hasPreviousVersion, versionInfo, null, {
			showCurrentContentOnly: showCurrentContentOnly
		});
		if (!model.defaultMode) {
			model.emptyState = {
				title: Texts.noSupportedContentTitle,
				message: Texts.noSupportedContentMessage
			};
		}
		return model;
	}

	function getErrorMessage(response) {
		const errorInfo = getFirstDefinedValue(response, ErrorInfoNames, null);
		return normalizeText(getFirstDefinedValue(errorInfo, ErrorMessageNames, Texts.defaultErrorMessage)) ||
			Texts.defaultErrorMessage;
	}

	function isSuccessful(response) {
		if (!response) {
			return false;
		}
		const success = getFirstDefinedValue(response, SuccessNames, undefined);
		if (typeof success === "boolean") {
			return success;
		}
		return true;
	}

	return {
		buildDiffModel: buildDiffModel,
		buildComparableFiles: buildComparableFiles,
		getErrorMessage: getErrorMessage,
		isSuccessful: isSuccessful
	};
});
