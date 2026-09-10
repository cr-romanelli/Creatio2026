define("ConfActivityLogDiffRuntime", ["ConfActivityLogDiffRuntimeResources"], function(resources) {
	const RuntimeTextsMarker = "__RUNTIME_TEXTS_JSON__";

	function normalizeResourceText(value) {
		return value === null || value === undefined ? "" : String(value);
	}

	function getTexts() {
		const localizableStrings = resources.localizableStrings || {};
		return {
			noFilesTitle: normalizeResourceText(localizableStrings.NoFilesTitle),
			noFilesMeta: normalizeResourceText(localizableStrings.NoFilesMeta),
			noOldContent: normalizeResourceText(localizableStrings.NoOldContent),
			noNewContent: normalizeResourceText(localizableStrings.NoNewContent),
			noChangedContent: normalizeResourceText(localizableStrings.NoChangedContent),
			modeContentUnavailable: normalizeResourceText(localizableStrings.ModeContentUnavailable),
			largeContentTitle: normalizeResourceText(localizableStrings.LargeContentTitle),
			largeContentMeta: normalizeResourceText(localizableStrings.LargeContentMeta),
			largeContentPane: normalizeResourceText(localizableStrings.LargeContentPane),
			added: normalizeResourceText(localizableStrings.Added),
			removed: normalizeResourceText(localizableStrings.Removed),
			modified: normalizeResourceText(localizableStrings.Modified),
			sourceCode: normalizeResourceText(localizableStrings.SourceCode),
			metadata: normalizeResourceText(localizableStrings.Metadata),
			descriptor: normalizeResourceText(localizableStrings.Descriptor),
			hiddenUnchangedLinesPrefix: "... ",
			hiddenUnchangedLinesSuffix: normalizeResourceText(localizableStrings.HiddenUnchangedLinesSuffix),
			modeNoFilesPrefix: normalizeResourceText(localizableStrings.ModeNoFilesPrefix),
			modeNoFilesSuffix: normalizeResourceText(localizableStrings.ModeNoFilesSuffix),
			metaSeparator: " | ",
			oldLinesLabel: normalizeResourceText(localizableStrings.OldLinesLabel),
			newLinesLabel: normalizeResourceText(localizableStrings.NewLinesLabel),
			previousVersion: normalizeResourceText(localizableStrings.PreviousVersion),
			currentVersion: normalizeResourceText(localizableStrings.CurrentVersion),
			noPreviousVersion: normalizeResourceText(localizableStrings.NoPreviousVersion),
			noPreviousVersionDeletedDuringCleanup: normalizeResourceText(
				localizableStrings.NoPreviousVersionDeletedDuringCleanup
			),
			enterFullscreen: normalizeResourceText(localizableStrings.EnterFullscreen),
			exitFullscreen: normalizeResourceText(localizableStrings.ExitFullscreen)
		};
	}

	const RuntimeScript = `(function() {
		var Texts = ${RuntimeTextsMarker};
		var ModeOrder = [
			{ key: 'sourceCode', title: Texts.sourceCode },
			{ key: 'metadata', title: Texts.metadata },
			{ key: 'descriptor', title: Texts.descriptor }
		];
		var RowTypes = {
			same: 'same',
			added: 'added',
			removed: 'removed',
			changed: 'changed',
			empty: 'emptyline'
		};
		var ViewModes = {
			side: 'side',
			unified: 'unified'
		};
		var ContextLinesCount = 3;
		var MaxDiffLinesPerSide = 5000;
		var MaxDiffCharsPerSide = 500000;
		var MaxDiffResyncLookahead = 100;
		var MaxInlineDiffLineChars = 2000;
		var MaxInlineDiffTokens = 300;
		var ScrollEndTolerance = 2;
		var FullscreenFrameClassName = 'conf-activity-log-diff-fullscreen-frame';
		var FullscreenBodyClassName = 'conf-activity-log-diff-fullscreen-body';
		var dom = {
			root: document.querySelector('.gidiff'),
			compareMenu: document.querySelector('[data-compare-menu]'),
			compareMenuToggle: document.querySelector('[data-compare-menu-toggle]'),
			compareModes: document.querySelector('[data-compare-modes]'),
			tabs: document.querySelector('[data-tabs]'),
			title: document.querySelector('[data-title]'),
			meta: document.querySelector('[data-meta]'),
			leftVersionTitle: document.querySelector('[data-left-version-title]'),
			leftVersionMeta: document.querySelector('[data-left-version-meta]'),
			rightVersionTitle: document.querySelector('[data-right-version-title]'),
			rightVersionMeta: document.querySelector('[data-right-version-meta]'),
			unifiedLeftVersionTitle: document.querySelector('[data-unified-left-version-title]'),
			unifiedLeftVersionMeta: document.querySelector('[data-unified-left-version-meta]'),
			unifiedRightVersionTitle: document.querySelector('[data-unified-right-version-title]'),
			unifiedRightVersionMeta: document.querySelector('[data-unified-right-version-meta]'),
			left: document.querySelector('[data-left]'),
			right: document.querySelector('[data-right]'),
			unified: document.querySelector('[data-unified]'),
			compactToggle: document.querySelector('[data-compact-toggle]'),
			viewMode: document.querySelector('.view-mode'),
			modeButtons: Array.prototype.slice.call(document.querySelectorAll('[data-view-mode]')),
			changeNavButtons: Array.prototype.slice.call(document.querySelectorAll('[data-change-nav]')),
			fullscreenToggle: document.querySelector('[data-fullscreen-toggle]'),
			fullscreenEnterIcon: document.querySelector('[data-fullscreen-enter-icon]'),
			fullscreenExitIcon: document.querySelector('[data-fullscreen-exit-icon]')
		};
		dom.leftPane = dom.left && dom.left.closest('.pane');
		dom.compactToggleLabel = dom.compactToggle && dom.compactToggle.closest('.toggle');
		dom.rightVersionHead = dom.rightVersionTitle && dom.rightVersionTitle.closest('.version-head');
		var state = {
			activeKey: '',
			selectedMode: '',
			viewMode: ViewModes.side,
			compact: !!(dom.compactToggle && dom.compactToggle.checked),
			changeCount: 0,
			changeNavigationFrame: 0,
			syncLock: false
		};
		var model = normalizeDiffModel(diffModel);

		function setCompareMenuOpen(isOpen) {
			if (!dom.compareMenu || !dom.compareMenuToggle) {
				return;
			}
			dom.compareMenu.classList.toggle('is-open', isOpen);
			dom.compareMenuToggle.classList.toggle('active', isOpen);
			dom.compareMenuToggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
		}

		function closeCompareMenu() {
			setCompareMenuOpen(false);
		}

		function getFullscreenTarget() {
			var frameElement = null;
			try {
				frameElement = window.frameElement;
			} catch (error) {
				frameElement = null;
			}
			return frameElement || document.documentElement;
		}

		function isFullscreenActive() {
			var target = getFullscreenTarget();
			return target.classList.contains(FullscreenFrameClassName) || dom.root.classList.contains('is-fullscreen');
		}

		function updateFullscreenButton() {
			if (!dom.fullscreenToggle) {
				return;
			}
			var isActive = isFullscreenActive();
			var caption = isActive ? Texts.exitFullscreen : Texts.enterFullscreen;
			dom.fullscreenToggle.classList.toggle('active', isActive);
			dom.fullscreenToggle.setAttribute('aria-label', caption);
			dom.fullscreenToggle.setAttribute('title', caption);
			if (dom.fullscreenEnterIcon) {
				dom.fullscreenEnterIcon.classList.toggle('is-hidden', isActive);
			}
			if (dom.fullscreenExitIcon) {
				dom.fullscreenExitIcon.classList.toggle('is-hidden', !isActive);
			}
		}

		function getFullscreenStyleElement(doc) {
			var styleElement = doc.getElementById('conf-activity-log-diff-fullscreen-style');
			if (styleElement) {
				return styleElement;
			}
			styleElement = doc.createElement('style');
			styleElement.id = 'conf-activity-log-diff-fullscreen-style';
			styleElement.textContent = '.' + FullscreenBodyClassName + ' { overflow: hidden !important; }' +
				'.' + FullscreenFrameClassName + ' { position: fixed !important; inset: 0 !important; ' +
				'z-index: 2147483647 !important; width: 100vw !important; height: 100vh !important; ' +
				'max-width: none !important; max-height: none !important; margin: 0 !important; ' +
				'border: 0 !important; border-radius: 0 !important; background: #fff !important; }';
			doc.head.appendChild(styleElement);
			return styleElement;
		}

		function setFullscreenActive(isActive) {
			var target = getFullscreenTarget();
			var parentDocument = target.ownerDocument || document;
			getFullscreenStyleElement(parentDocument);
			target.classList.toggle(FullscreenFrameClassName, isActive);
			dom.root.classList.toggle('is-fullscreen', isActive);
			if (parentDocument.body) {
				parentDocument.body.classList.toggle(FullscreenBodyClassName, isActive);
			}
			updateFullscreenButton();
		}

		function toggleFullscreen() {
			setFullscreenActive(!isFullscreenActive());
		}

		function normalizeDiffModel(value) {
			var defaultModes = ModeOrder.map(function(mode) {
				return {
					key: mode.key,
					title: mode.title,
					count: 0
				};
			});
			if (Array.isArray(value)) {
				return {
					hasPreviousVersion: true,
					showCurrentContentOnly: false,
					versionInfo: {
						previous: null,
						current: null
					},
					availableModes: defaultModes,
					defaultMode: '',
					filesByMode: {},
					emptyState: null
				};
			}
			value = value || {};
			return {
				hasPreviousVersion: value.hasPreviousVersion !== false,
				showCurrentContentOnly: !!value.showCurrentContentOnly,
				versionInfo: value.versionInfo || {
					previous: null,
					current: null
				},
				availableModes: Array.isArray(value.availableModes) ? value.availableModes : defaultModes,
				defaultMode: value.defaultMode || '',
				filesByMode: value.filesByMode || {},
				emptyState: value.emptyState || null
			};
		}

		function getModeTitle(modeKey) {
			var modes = model.availableModes || [];
			for (var index = 0; index < modes.length; index++) {
				if (modes[index].key === modeKey) {
					return modes[index].title;
				}
			}
			for (var orderIndex = 0; orderIndex < ModeOrder.length; orderIndex++) {
				if (ModeOrder[orderIndex].key === modeKey) {
					return ModeOrder[orderIndex].title;
				}
			}
			return modeKey || '';
		}

		function getFilesForMode(modeKey) {
			var mode = model.filesByMode && model.filesByMode[modeKey];
			return mode && Array.isArray(mode.files) ? mode.files : [];
		}

		function getInitialMode() {
			if (model.defaultMode) {
				return model.defaultMode;
			}
			return model.availableModes && model.availableModes[0] && model.availableModes[0].key || '';
		}

		function getDisplayPath(file) {
			return file && (file.displayPath || file.key) || '';
		}

		function normalizeText(value) {
			return value === null || value === undefined ? '' : String(value);
		}

		function parseDate(value) {
			var text = normalizeText(value);
			var dotNetDateMatch = /\\/Date\\((-?\\d+)/.exec(text);
			var date = dotNetDateMatch ? new Date(Number(dotNetDateMatch[1])) : new Date(text);
			return isNaN(date.getTime()) ? null : date;
		}

		function formatDate(value) {
			var date = parseDate(value);
			return date ? date.toLocaleString() : normalizeText(value);
		}

		function formatVersionMeta(versionInfo) {
			if (!versionInfo) {
				return '';
			}
			var parts = [];
			var modifiedBy = normalizeText(versionInfo.modifiedBy);
			var modifiedOn = formatDate(versionInfo.modifiedOn);
			if (modifiedBy) {
				parts.push(modifiedBy);
			}
			if (modifiedOn) {
				parts.push('(' + modifiedOn + ')');
			}
			return parts.join(' ');
		}

		function updateVersionHeaders() {
			var versionInfo = model.versionInfo || {};
			var hasPreviousVersion = model.hasPreviousVersion !== false;
			var currentVersionMeta = formatVersionMeta(versionInfo.current);
			var currentVersionText = Texts.currentVersion;
			var previousVersionText = Texts.previousVersion;
			if (dom.root) {
				dom.root.classList.toggle('no-previous-version', !hasPreviousVersion);
			}
			if (dom.leftPane) {
				dom.leftPane.classList.toggle('is-hidden', !hasPreviousVersion);
			}
			if (dom.viewMode) {
				dom.viewMode.classList.toggle('is-hidden', !hasPreviousVersion);
			}
			if (dom.compactToggleLabel) {
				dom.compactToggleLabel.classList.toggle('is-hidden', !hasPreviousVersion);
			}
			if (dom.rightVersionHead) {
				dom.rightVersionHead.classList.toggle('is-stacked', !hasPreviousVersion);
			}
			dom.leftVersionTitle.textContent = previousVersionText;
			dom.leftVersionMeta.textContent = formatVersionMeta(versionInfo.previous);
			dom.unifiedLeftVersionTitle.textContent = previousVersionText;
			dom.unifiedLeftVersionMeta.textContent = formatVersionMeta(versionInfo.previous);
			dom.unifiedRightVersionTitle.textContent = currentVersionText;
			dom.unifiedRightVersionMeta.textContent = currentVersionMeta;
			if (hasPreviousVersion) {
				dom.rightVersionTitle.textContent = currentVersionText;
				dom.rightVersionMeta.textContent = currentVersionMeta;
				return;
			}
			dom.rightVersionTitle.textContent = Texts.noPreviousVersion;
			if (model.showCurrentContentOnly) {
				dom.rightVersionTitle.textContent = Texts.noPreviousVersionDeletedDuringCleanup;
			}
			dom.rightVersionMeta.textContent = currentVersionText + ' ' + currentVersionMeta;
		}

		function compareFiles(left, right) {
			return getDisplayPath(left).localeCompare(getDisplayPath(right));
		}

		function getSortedFiles() {
			return getFilesForMode(state.selectedMode).slice().sort(compareFiles);
		}

		function getActiveFile() {
			var items = getSortedFiles();
			for (var index = 0; index < items.length; index++) {
				if (items[index].key === state.activeKey) {
					return items[index];
				}
			}
			return items[0] || null;
		}

		function splitLines(text) {
			var lines = String(text || '').replace(/\\r/g, '').split('\\n');
			return lines.length === 1 && lines[0] === '' ? [] : lines;
		}

		function createLinePair(type, left, right, hasLeft, hasRight) {
			return {
				type: type,
				left: left || '',
				right: right || '',
				hasLeft: !!hasLeft,
				hasRight: !!hasRight
			};
		}

		function findLookaheadOffset(lines, startIndex, value) {
			var endIndex = Math.min(lines.length, startIndex + MaxDiffResyncLookahead + 1);
			for (var index = startIndex + 1; index < endIndex; index++) {
				if (lines[index] === value) {
					return index - startIndex;
				}
			}
			return -1;
		}

		function buildLinePairs(leftLines, rightLines) {
			var pairs = [];
			var leftIndex = 0;
			var rightIndex = 0;

			while (leftIndex < leftLines.length && rightIndex < rightLines.length) {
				if (leftLines[leftIndex] === rightLines[rightIndex]) {
					pairs.push(createLinePair(RowTypes.same, leftLines[leftIndex], rightLines[rightIndex], true, true));
					leftIndex++;
					rightIndex++;
					continue;
				}

				var rightOffset = findLookaheadOffset(rightLines, rightIndex, leftLines[leftIndex]);
				var leftOffset = findLookaheadOffset(leftLines, leftIndex, rightLines[rightIndex]);

				if (rightOffset > 0 && (leftOffset < 0 || rightOffset < leftOffset)) {
					while (rightOffset > 0) {
						pairs.push(createLinePair(RowTypes.added, '', rightLines[rightIndex], false, true));
						rightIndex++;
						rightOffset--;
					}
					continue;
				}

				if (leftOffset > 0 && (rightOffset < 0 || leftOffset < rightOffset)) {
					while (leftOffset > 0) {
						pairs.push(createLinePair(RowTypes.removed, leftLines[leftIndex], '', true, false));
						leftIndex++;
						leftOffset--;
					}
					continue;
				}

				pairs.push(createLinePair(RowTypes.changed, leftLines[leftIndex], rightLines[rightIndex], true, true));
				leftIndex++;
				rightIndex++;
			}

			while (leftIndex < leftLines.length) {
				pairs.push(createLinePair(RowTypes.removed, leftLines[leftIndex], '', true, false));
				leftIndex++;
			}

			while (rightIndex < rightLines.length) {
				pairs.push(createLinePair(RowTypes.added, '', rightLines[rightIndex], false, true));
				rightIndex++;
			}

			return pairs;
		}

		function tokenize(text) {
			return (text || '').match(/(\\s+|[^\\s]+)/g) || [''];
		}

		function buildFragments(tokens, changedStartIndex, changedEndIndex) {
			var fragments = [];

			function appendFragment(changed, startIndex, endIndex) {
				if (startIndex >= endIndex) {
					return;
				}
				fragments.push({
					changed: changed,
					text: tokens.slice(startIndex, endIndex).join('')
				});
			}

			appendFragment(false, 0, changedStartIndex);
			appendFragment(true, changedStartIndex, changedEndIndex);
			appendFragment(false, changedEndIndex, tokens.length);
			return fragments;
		}

		function buildInlineDiff(leftText, rightText) {
			var leftTokens = tokenize(leftText);
			var rightTokens = tokenize(rightText);
			var prefixLength = 0;
			var leftSuffixIndex = leftTokens.length;
			var rightSuffixIndex = rightTokens.length;

			if (leftTokens.length > MaxInlineDiffTokens || rightTokens.length > MaxInlineDiffTokens) {
				return null;
			}

			while (prefixLength < leftTokens.length && prefixLength < rightTokens.length &&
					leftTokens[prefixLength] === rightTokens[prefixLength]) {
				prefixLength++;
			}

			while (leftSuffixIndex > prefixLength && rightSuffixIndex > prefixLength &&
					leftTokens[leftSuffixIndex - 1] === rightTokens[rightSuffixIndex - 1]) {
				leftSuffixIndex--;
				rightSuffixIndex--;
			}

			return {
				left: buildFragments(leftTokens, prefixLength, leftSuffixIndex),
				right: buildFragments(rightTokens, prefixLength, rightSuffixIndex)
			};
		}

		function createRow(pair, leftLineNumber, rightLineNumber) {
			var row = {
				leftNo: pair.hasLeft ? leftLineNumber : 0,
				rightNo: pair.hasRight ? rightLineNumber : 0,
				left: pair.left || '',
				right: pair.right || '',
				hasLeft: !!pair.hasLeft,
				hasRight: !!pair.hasRight,
				type: pair.type
			};

			if (pair.type === RowTypes.changed &&
					(row.left.length <= MaxInlineDiffLineChars && row.right.length <= MaxInlineDiffLineChars)) {
				row.inline = buildInlineDiff(row.left, row.right);
			}

			return row;
		}

		function buildRows(file) {
			if (model.showCurrentContentOnly) {
				return splitLines(file.newContent || '').map(function(line, index) {
					return {
						type: RowTypes.same,
						leftNo: 0,
						rightNo: index + 1,
						left: '',
						right: line,
						hasLeft: false,
						hasRight: true
					};
				});
			}
			var linePairs = buildLinePairs(
				splitLines(file.oldContent || ''),
				splitLines(file.newContent || '')
			);
			var leftLineNumber = 1;
			var rightLineNumber = 1;

			return linePairs.map(function(pair) {
				var row = createRow(pair, leftLineNumber, rightLineNumber);
				if (pair.hasLeft) {
					leftLineNumber++;
				}
				if (pair.hasRight) {
					rightLineNumber++;
				}
				return row;
			});
		}

		function buildHiddenRow(count) {
			var hiddenText = Texts.hiddenUnchangedLinesPrefix + count + Texts.hiddenUnchangedLinesSuffix;
			return {
				type: RowTypes.empty,
				leftNo: 0,
				rightNo: 0,
				left: hiddenText,
				right: hiddenText,
				hasLeft: true,
				hasRight: true
			};
		}

		function collectVisibleRanges(rows) {
			var ranges = [];

			for (var index = 0; index < rows.length; index++) {
				if (rows[index].type === RowTypes.same) {
					continue;
				}

				var start = Math.max(0, index - ContextLinesCount);
				var end = Math.min(rows.length - 1, index + ContextLinesCount);
				var lastRange = ranges[ranges.length - 1];

				if (!lastRange || start > lastRange.end + 1) {
					ranges.push({ start: start, end: end });
					continue;
				}

				lastRange.end = Math.max(lastRange.end, end);
			}

			return ranges;
		}

		function compactRows(rows) {
			if (!state.compact || !rows.length) {
				return rows;
			}

			var ranges = collectVisibleRanges(rows);
			if (!ranges.length) {
				return rows;
			}

			var compactedRows = [];
			var cursor = 0;

			ranges.forEach(function(range) {
				if (range.start > cursor) {
					compactedRows.push(buildHiddenRow(range.start - cursor));
				}

				for (var index = range.start; index <= range.end; index++) {
					compactedRows.push(rows[index]);
				}

				cursor = range.end + 1;
			});

			if (cursor < rows.length) {
				compactedRows.push(buildHiddenRow(rows.length - cursor));
			}

			return compactedRows;
		}

		function appendFragments(target, fragments, changedClassName) {
			(fragments || []).forEach(function(fragment) {
				var span = document.createElement('span');
				if (fragment.changed) {
					span.className = changedClassName;
				}
				span.textContent = fragment.text;
				target.appendChild(span);
			});
		}

		function appendLine(target, rowType, lineNumber, marker, textValue, fragments, changedClassName) {
			var line = document.createElement('div');
			var lineNumberEl = document.createElement('span');
			var markerEl = document.createElement('span');
			var textEl = document.createElement('span');

			line.className = 'line ' + rowType;
			lineNumberEl.className = 'num';
			lineNumberEl.textContent = lineNumber || '';
			markerEl.className = 'mark';
			markerEl.textContent = marker || '';
			textEl.className = 'text';

			if (fragments) {
				appendFragments(textEl, fragments, changedClassName);
			} else {
				textEl.textContent = textValue || '';
			}

			line.appendChild(lineNumberEl);
			line.appendChild(markerEl);
			line.appendChild(textEl);
			target.appendChild(line);
			return line;
		}

		function createLinesContainer(target) {
			var lines = document.createElement('div');
			target.innerHTML = '';
			lines.className = 'lines';
			target.appendChild(lines);
			return lines;
		}

		function fitLinesToScrollableWidth(lines) {
			var lineElements = Array.prototype.slice.call(lines.querySelectorAll('.line'));
			var content = lines.parentElement;
			var width = content ? content.clientWidth : 0;
			lines.style.width = '';
			lineElements.forEach(function(line) {
				width = Math.max(width, line.scrollWidth);
			});
			if (width > 0) {
				lines.style.width = width + 'px';
			}
		}

		function getSideMarker(rowType, side) {
			if (rowType === RowTypes.added) {
				return side === 'right' ? '+' : '';
			}
			if (rowType === RowTypes.removed) {
				return side === 'left' ? '-' : '';
			}
			if (rowType === RowTypes.changed) {
				return side === 'left' ? '-' : '+';
			}
			return '';
		}

		function getSideRenderConfig(row, side) {
			var isLeftSide = side === 'left';
			var hasContent = isLeftSide ? row.hasLeft : row.hasRight;

			return {
				rowType: row.type === RowTypes.same ? RowTypes.same : hasContent ? row.type : RowTypes.empty,
				lineNumber: isLeftSide ? (row.leftNo || '') : (row.rightNo || ''),
				marker: getSideMarker(row.type, side),
				text: hasContent ? (isLeftSide ? row.left : row.right) : '',
				fragments: row.type === RowTypes.changed && row.inline && hasContent ? row.inline[side] : null,
				changedClassName: isLeftSide ? 'inline-removed' : 'inline-added'
			};
		}

		function isChangeRow(row) {
			return row && row.type !== RowTypes.same && row.type !== RowTypes.empty;
		}

		function setLineChangeIndex(line, changeIndex) {
			if (line && changeIndex >= 0) {
				line.setAttribute('data-change-index', String(changeIndex));
			}
		}

		function assignChangeIndexes(rows) {
			var changeIndex = -1;
			var previousRowHasChange = false;
			rows.forEach(function(row) {
				if (!isChangeRow(row)) {
					previousRowHasChange = false;
					return;
				}
				if (!previousRowHasChange) {
					changeIndex++;
				}
				row.changeIndex = changeIndex;
				previousRowHasChange = true;
			});
			return changeIndex + 1;
		}

		function renderSide(target, rows, side) {
			var lines = createLinesContainer(target);
			rows.forEach(function(row) {
				var config = getSideRenderConfig(row, side);
				var line = appendLine(
					lines,
					config.rowType,
					config.lineNumber,
					config.marker,
					config.text,
					config.fragments,
					config.changedClassName
				);
				if (isChangeRow(row)) {
					setLineChangeIndex(line, row.changeIndex);
				}
			});
			fitLinesToScrollableWidth(lines);
		}

		function renderUnified(target, rows) {
			var lines = createLinesContainer(target);
			rows.forEach(function(row) {
				var firstLine;
				var secondLine;
				if (row.type === RowTypes.empty) {
					appendLine(lines, RowTypes.empty, '', '', row.left || row.right || '', null, '');
					return;
				}
				if (row.type === RowTypes.same) {
					appendLine(
						lines,
						RowTypes.same,
						row.rightNo || row.leftNo || '',
						'',
						row.right || row.left || '',
						null,
						''
					);
					return;
				}
				if (row.type === RowTypes.removed) {
					firstLine = appendLine(lines, RowTypes.removed, row.leftNo || '', '-', row.left, null, '');
					setLineChangeIndex(firstLine, row.changeIndex);
					return;
				}
				if (row.type === RowTypes.added) {
					firstLine = appendLine(lines, RowTypes.added, row.rightNo || '', '+', row.right, null, '');
					setLineChangeIndex(firstLine, row.changeIndex);
					return;
				}
				firstLine = appendLine(
					lines,
					RowTypes.removed,
					row.leftNo || '',
					'-',
					row.left,
					row.inline && row.inline.left,
					'inline-removed'
				);
				secondLine = appendLine(
					lines,
					RowTypes.added,
					row.rightNo || '',
					'+',
					row.right,
					row.inline && row.inline.right,
					'inline-added'
				);
				setLineChangeIndex(firstLine, row.changeIndex);
				setLineChangeIndex(secondLine, row.changeIndex);
			});
			fitLinesToScrollableWidth(lines);
		}

		function createEmptyBlock(text) {
			var empty = document.createElement('div');
			empty.className = 'empty';
			empty.textContent = text;
			return empty;
		}

		function getFileState(file) {
			if (!file.hasOld) {
				return Texts.added;
			}
			if (!file.hasNew) {
				return Texts.removed;
			}
			return Texts.modified;
		}

		function createTabButton(file) {
			var button = document.createElement('button');
			button.type = 'button';
			button.className = 'tab' + (file.key === state.activeKey ? ' active' : '');
			button.setAttribute('data-key', file.key);
			button.innerHTML = '<span class="tab-title"></span><span class="tab-meta"></span>';
			button.querySelector('.tab-title').textContent = getDisplayPath(file);
			button.querySelector('.tab-meta').textContent = getFileState(file);
			return button;
		}

		function createModeButton(mode) {
			var button = document.createElement('button');
			var isActive = mode.key === state.selectedMode;
			button.type = 'button';
			button.className = 'compare-mode' + (isActive ? ' active' : '');
			button.setAttribute('data-mode', mode.key);
			button.setAttribute('aria-pressed', isActive ? 'true' : 'false');
			button.innerHTML = '<span></span><span class="compare-mode-count"></span>';
			button.querySelector('span').textContent = mode.title;
			button.querySelector('.compare-mode-count').textContent = String(mode.count || 0);
			return button;
		}

		function renderModes() {
			dom.compareModes.innerHTML = '';
			(model.availableModes || []).forEach(function(mode) {
				dom.compareModes.appendChild(createModeButton(mode));
			});
		}

		function getModeUnavailableMessage() {
			var modeTitle = getModeTitle(state.selectedMode);
			return {
				title: modeTitle + Texts.modeContentUnavailable,
				message: Texts.modeNoFilesPrefix + modeTitle.toLowerCase() + Texts.modeNoFilesSuffix
			};
		}

		function renderTabs() {
			var items = getSortedFiles();
			dom.tabs.innerHTML = '';

			if (!items.length) {
				dom.tabs.appendChild(createEmptyBlock(
					model.emptyState && model.emptyState.title || getModeUnavailableMessage().title
				));
				return;
			}

			items.forEach(function(file) {
				dom.tabs.appendChild(createTabButton(file));
			});
		}

		function setEmptyContent(target, text) {
			target.innerHTML = '';
			target.appendChild(createEmptyBlock(text));
		}

		function renderEmptyState(title, meta) {
			dom.title.textContent = title;
			dom.meta.textContent = meta;
			setEmptyContent(dom.left, meta);
			setEmptyContent(dom.right, meta);
			setEmptyContent(dom.unified, meta);
		}

		function getLineCount(text) {
			return splitLines(text).length;
		}

		function isLargeFile(file) {
			var oldContent = file.oldContent || '';
			var newContent = file.newContent || '';
			return oldContent.length > MaxDiffCharsPerSide ||
				newContent.length > MaxDiffCharsPerSide ||
				getLineCount(oldContent) > MaxDiffLinesPerSide ||
				getLineCount(newContent) > MaxDiffLinesPerSide;
		}

		function renderLargeFileState(file) {
			dom.title.textContent = Texts.largeContentTitle;
			dom.meta.textContent = getDisplayPath(file) + Texts.metaSeparator + Texts.largeContentMeta;
			setEmptyContent(dom.left, Texts.largeContentPane);
			setEmptyContent(dom.right, Texts.largeContentPane);
			setEmptyContent(dom.unified, Texts.largeContentPane);
			updateChangeNavigation(0);
		}

		function resetScroll(target) {
			target.scrollTop = 0;
			target.scrollLeft = 0;
		}

		function updateHeader(file) {
			var oldLinesCount = splitLines(file.oldContent || '').length;
			var newLinesCount = splitLines(file.newContent || '').length;
			dom.title.textContent = getDisplayPath(file);
			dom.meta.textContent = getFileState(file) + Texts.metaSeparator + Texts.oldLinesLabel + oldLinesCount +
				Texts.metaSeparator + Texts.newLinesLabel + newLinesCount;
		}

		function renderActive() {
			var file = getActiveFile();
			if (!file) {
				var emptyState = model.emptyState || getModeUnavailableMessage();
				renderEmptyState(emptyState.title, emptyState.message);
				updateChangeNavigation(0);
				renderTabs();
				return;
			}

			state.activeKey = file.key;
			updateHeader(file);

			if (isLargeFile(file)) {
				renderLargeFileState(file);
				renderTabs();
				return;
			}

			var rows = compactRows(buildRows(file));
			var changeCount = assignChangeIndexes(rows);
			renderSide(dom.left, rows, 'left');
			renderSide(dom.right, rows, 'right');
			renderUnified(dom.unified, rows);
			resetScroll(dom.left);
			resetScroll(dom.right);
			resetScroll(dom.unified);
			updateChangeNavigation(changeCount);
			renderTabs();
		}

		function syncScroll(source, target) {
			if (state.syncLock) {
				return;
			}
			state.syncLock = true;
			target.scrollTop = source.scrollTop;
			target.scrollLeft = source.scrollLeft;
			state.syncLock = false;
		}

		function setViewMode(mode) {
			var hasPreviousVersion = model.hasPreviousVersion !== false;
			state.viewMode = hasPreviousVersion && mode === ViewModes.unified ? ViewModes.unified : ViewModes.side;
			if (dom.root) {
				dom.root.classList.toggle('is-unified', state.viewMode === ViewModes.unified);
			}
			dom.modeButtons.forEach(function(button) {
				var isActive = button.getAttribute('data-view-mode') === state.viewMode;
				button.classList.toggle('active', isActive);
				button.setAttribute('aria-pressed', isActive ? 'true' : 'false');
			});
		}

		function updateChangeNavigation(changeCount) {
			if (typeof changeCount === 'number') {
				state.changeCount = changeCount;
			}
			var scrollTarget = getPrimaryScrollTarget();
			var currentChangeIndex = getCurrentChangeIndex();
			dom.changeNavButtons.forEach(function(button) {
				var direction = button.getAttribute('data-change-nav');
				button.disabled = !state.changeCount ||
					(direction === 'previous' && (currentChangeIndex <= 0 || isScrollAtStart(scrollTarget))) ||
					(direction === 'next' &&
						(currentChangeIndex >= state.changeCount - 1 || isScrollAtEnd(scrollTarget)));
			});
		}

		function isScrollAtStart(target) {
			return !target || target.scrollTop <= ScrollEndTolerance;
		}

		function isScrollAtEnd(target) {
			return !target || target.scrollTop + target.clientHeight >= target.scrollHeight - ScrollEndTolerance;
		}

		function getPrimaryScrollTarget() {
			if (state.viewMode === ViewModes.unified) {
				return dom.unified;
			}
			return model.hasPreviousVersion === false ? dom.right : dom.left;
		}

		function getActiveScrollTargets() {
			var primaryTarget = getPrimaryScrollTarget();
			if (state.viewMode === ViewModes.unified) {
				return [primaryTarget];
			}
			return model.hasPreviousVersion === false ? [primaryTarget] : [dom.left, dom.right];
		}

		function getChangeLinePositions(target) {
			var itemsByIndex = {};
			return Array.prototype.slice.call(target.querySelectorAll('[data-change-index]')).reduce(function(items, line) {
				var changeIndex = Number(line.getAttribute('data-change-index'));
				var lineTop = line.offsetTop;
				var lineBottom = lineTop + line.offsetHeight;
				var item = itemsByIndex[changeIndex];
				if (isNaN(changeIndex)) {
					return items;
				}
				if (item) {
					item.bottom = Math.max(item.bottom, lineBottom);
					return items;
				}
				items.push({
					index: changeIndex,
					top: lineTop,
					bottom: lineBottom
				});
				itemsByIndex[changeIndex] = items[items.length - 1];
				return items;
			}, []);
		}

		function getCurrentChangeIndex() {
			var target = getPrimaryScrollTarget();
			if (!target || !state.changeCount) {
				return -1;
			}
			var anchorTop = target.scrollTop + target.clientHeight / 2;
			var closestChange = getChangeLinePositions(target).reduce(function(closest, item) {
				var distance = anchorTop < item.top ? item.top - anchorTop :
					anchorTop > item.bottom ? anchorTop - item.bottom : 0;
				if (!closest || distance < closest.distance) {
					return {
						index: item.index,
						distance: distance
					};
				}
				return closest;
			}, null);
			return closestChange ? closestChange.index : -1;
		}

		function scrollTargetToChange(target, changeIndex) {
			var line = target && target.querySelector('[data-change-index="' + changeIndex + '"]');
			if (!line) {
				return;
			}
			target.scrollTop = Math.max(0, line.offsetTop - target.clientHeight / 2);
		}

		function navigateChange(direction) {
			if (!state.changeCount) {
				return;
			}
			var step = direction === 'previous' ? -1 : 1;
			var currentIndex = getCurrentChangeIndex();
			var nextIndex = currentIndex + step;
			if (currentIndex < 0 || nextIndex < 0 || nextIndex >= state.changeCount) {
				updateChangeNavigation();
				return;
			}
			getActiveScrollTargets().forEach(function(target) {
				scrollTargetToChange(target, nextIndex);
			});
			updateChangeNavigation();
		}

		function scheduleChangeNavigationUpdate() {
			if (state.changeNavigationFrame) {
				return;
			}
			state.changeNavigationFrame = window.requestAnimationFrame(function() {
				state.changeNavigationFrame = 0;
				updateChangeNavigation();
			});
		}

		function bindEvents() {
			dom.compareModes.addEventListener('click', function(event) {
				var button = event.target.closest('[data-mode]');
				if (!button || button.disabled) {
					return;
				}
				state.selectedMode = button.getAttribute('data-mode') || '';
				state.activeKey = '';
				renderModes();
				renderTabs();
				renderActive();
				closeCompareMenu();
			});

			dom.tabs.addEventListener('click', function(event) {
				var button = event.target.closest('[data-key]');
				if (!button) {
					return;
				}
				state.activeKey = button.getAttribute('data-key') || '';
				renderActive();
				closeCompareMenu();
			});

			if (dom.compactToggle) {
				dom.compactToggle.addEventListener('change', function() {
					state.compact = !!dom.compactToggle.checked;
					renderActive();
				});
			}

			dom.modeButtons.forEach(function(button) {
				button.addEventListener('click', function() {
					setViewMode(button.getAttribute('data-view-mode'));
					updateChangeNavigation();
				});
			});

			dom.changeNavButtons.forEach(function(button) {
				button.addEventListener('click', function() {
					navigateChange(button.getAttribute('data-change-nav'));
				});
			});

			if (dom.compareMenuToggle) {
				dom.compareMenuToggle.addEventListener('click', function(event) {
					event.stopPropagation();
					setCompareMenuOpen(!(dom.compareMenu && dom.compareMenu.classList.contains('is-open')));
				});
			}

			if (dom.compareMenu) {
				dom.compareMenu.addEventListener('click', function(event) {
					event.stopPropagation();
				});
			}

			if (dom.fullscreenToggle) {
				dom.fullscreenToggle.addEventListener('click', function(event) {
					event.stopPropagation();
					toggleFullscreen();
				});
			}

			document.addEventListener('click', closeCompareMenu);
			document.addEventListener('keydown', function(event) {
				if (event.key === 'Escape') {
					closeCompareMenu();
					if (isFullscreenActive()) {
						setFullscreenActive(false);
					}
				}
			});

			dom.left.addEventListener('scroll', function() {
				syncScroll(dom.left, dom.right);
				scheduleChangeNavigationUpdate();
			});
			dom.right.addEventListener('scroll', function() {
				syncScroll(dom.right, dom.left);
				scheduleChangeNavigationUpdate();
			});
			dom.unified.addEventListener('scroll', function() {
				scheduleChangeNavigationUpdate();
			});
		}

		function initialize() {
			state.selectedMode = getInitialMode();
			var firstFile = getSortedFiles()[0];
			state.activeKey = firstFile ? firstFile.key : '';
			setViewMode(state.viewMode);
			renderModes();
			renderTabs();
			renderActive();
			updateVersionHeaders();
			updateFullscreenButton();
		}

		bindEvents();
		initialize();
	})();`;

	function getScript(textsJson) {
		return RuntimeScript.replace(RuntimeTextsMarker, textsJson);
	}

	return {
		getScript: getScript,
		getTexts: getTexts
	};
});
