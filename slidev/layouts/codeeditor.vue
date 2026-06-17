<template>
  <div class="slidev-layout default">
    <slot />
  </div>
</template>

<script setup lang="ts">
import type { SharedState } from '@slidev/client'
import type { IDisposable, editor as MonacoEditorNamespace } from 'monaco-editor'
import { sharedState, useIsSlideActive, useSlideContext } from '@slidev/client'
import { computed, onBeforeUnmount, watch } from 'vue'

interface LiveCodeSyncPayload {
  editorKey: string
  content: string
  sourceId: string
  version: number
}

interface LiveCursorRange {
  startLineNumber: number
  startColumn: number
  endLineNumber: number
  endColumn: number
}

interface LiveCursorPayload {
  editorKey: string
  sourceId: string
  version: number
  /** Cursor and selection are only shown on clients while the host editor keeps focus. */
  visible: boolean
  cursor: { lineNumber: number, column: number } | null
  selection: LiveCursorRange | null
}

type SharedStateWithLiveCode = SharedState & {
  liveCodeSync?: LiveCodeSyncPayload
  liveCursorSync?: LiveCursorPayload
  $patch?: (patch: Partial<SharedStateWithLiveCode>) => Promise<boolean>
}

type StandaloneCodeEditor = MonacoEditorNamespace.IStandaloneCodeEditor
type CodeEditor = MonacoEditorNamespace.ICodeEditor
type EditorSubscriptions = {
  onChange: IDisposable
  onCursorPosition: IDisposable
  onCursorSelection: IDisposable
  onFocus: IDisposable
  onBlur: IDisposable
  onDispose: IDisposable
}

const syncState = sharedState as SharedStateWithLiveCode
const sourceId = typeof crypto !== 'undefined' && 'randomUUID' in crypto
  ? crypto.randomUUID()
  : `live-code-sync-${Math.random().toString(36).slice(2)}`

const { $page, $renderContext } = useSlideContext()
const isSlideActive = useIsSlideActive()
const slideNo = computed(() => String($page.value))
const isMainRenderContext = computed(() => ['slide', 'presenter'].includes($renderContext.value))
const syncEnabled = computed(() => isMainRenderContext.value && isSlideActive.value)

let codeVersionCounter = 0
let cursorVersionCounter = 0
let monaco: typeof import('monaco-editor') | null = null
let createEditorListener: IDisposable | null = null
let syncRunning = false

const trackedEditors = new Map<string, StandaloneCodeEditor>()
const editorSubscriptions = new Map<string, EditorSubscriptions>()
const publishTimers = new Map<string, ReturnType<typeof setTimeout>>()
const cursorPublishTimers = new Map<string, ReturnType<typeof setTimeout>>()
const applyingRemoteByEditorId = new Set<string>()

const latestRemoteVersionByEditorKey = new Map<string, number>()
const cachedPayloadByEditorKey = new Map<string, LiveCodeSyncPayload>()
const latestRemoteCursorVersionByEditorKey = new Map<string, number>()
const cachedCursorPayloadByEditorKey = new Map<string, LiveCursorPayload>()
const cursorDecorationIdsByEditorId = new Map<string, string[]>()

function isStandaloneCodeEditor(editor: CodeEditor): editor is StandaloneCodeEditor {
  return typeof (editor as StandaloneCodeEditor).onDidChangeModelContent === 'function'
}
function isConnectedEditor(editor: StandaloneCodeEditor): boolean {
  return editor.getContainerDomNode().isConnected
}

function isEditorInPresenterMainPane(editor: StandaloneCodeEditor): boolean {
  if ($renderContext.value !== 'presenter')
    return true

  const editorNode = editor.getContainerDomNode()
  const gridSection = editorNode.closest<HTMLElement>('.grid-section')
  if (!gridSection)
    return true

  return gridSection.classList.contains('main')
}

function parseEditorKey(editorKey: string): { slideNo: string, editorIndex: number } | null {
  const [slideNo, rawEditorIndex] = editorKey.split(':')
  if (!slideNo)
    return null

  const parsedEditorIndex = Number.parseInt(rawEditorIndex ?? '0', 10)
  return {
    slideNo,
    editorIndex: Number.isFinite(parsedEditorIndex) ? parsedEditorIndex : 0,
  }
}

function isVisibleEditor(editor: StandaloneCodeEditor): boolean {
  const { width, height } = editor.getContainerDomNode().getBoundingClientRect()
  return width > 0 && height > 0
}

function nextCodeVersion() {
  codeVersionCounter += 1
  return codeVersionCounter
}

function nextCursorVersion() {
  cursorVersionCounter += 1
  return cursorVersionCounter
}

function getEditorKey(editor: StandaloneCodeEditor): string | null {
  if (!isConnectedEditor(editor))
    return null
  if (!isEditorInPresenterMainPane(editor))
    return null
  const editorNode = editor.getContainerDomNode()
  const slideNode = editorNode.closest<HTMLElement>('.slidev-page[data-slidev-no]')
  if (!slideNode?.dataset.slidevNo)
    return null

  const monacoContainer = editorNode.closest<HTMLElement>('.slidev-monaco-container')
  const siblingEditors = Array.from(slideNode.querySelectorAll<HTMLElement>('.slidev-monaco-container'))
  const editorIndex = monacoContainer ? siblingEditors.indexOf(monacoContainer) : -1

  return `${slideNode.dataset.slidevNo}:${editorIndex >= 0 ? editorIndex : 0}`
}

function getSlideNoFromEditorKey(editorKey: string): string {
  return parseEditorKey(editorKey)?.slideNo ?? ''
}

function getEditorIndexFromEditorKey(editorKey: string): number {
  return parseEditorKey(editorKey)?.editorIndex ?? 0
}

function isEditorFromCurrentSlide(editorKey: string | null): editorKey is string {
  return Boolean(editorKey) && getSlideNoFromEditorKey(editorKey) === slideNo.value
}
function isCollapsedRange(range: LiveCursorRange): boolean {
  return range.startLineNumber === range.endLineNumber && range.startColumn === range.endColumn
}

function canPublishCursorState(editor: StandaloneCodeEditor): boolean {
  if ($renderContext.value !== 'presenter')
    return false
  if (!isConnectedEditor(editor))
    return false
  if (!isEditorInPresenterMainPane(editor))
    return false
  if (!editor.hasTextFocus())
    return false
  return true
}
function touchLocalSharedStateMeta() {
  syncState.lastUpdate = {
    id: sourceId,
    type: $renderContext.value === 'presenter' ? 'presenter' : 'viewer',
    time: Date.now(),
  }

  const currentSlideNo = Number.parseInt(slideNo.value, 10)
  if (Number.isFinite(currentSlideNo))
    syncState.page = currentSlideNo
}

function publishCodePayload(payload: LiveCodeSyncPayload) {
  if (typeof syncState.$patch === 'function') {
    void syncState.$patch({ liveCodeSync: payload })
    return
  }
  touchLocalSharedStateMeta()
  syncState.liveCodeSync = payload
}
function publishCursorPayload(payload: LiveCursorPayload) {
  if (typeof syncState.$patch === 'function') {
    void syncState.$patch({ liveCursorSync: payload })
    return
  }
  touchLocalSharedStateMeta()
  syncState.liveCursorSync = payload
}

function clearCursorDecorations(editor: StandaloneCodeEditor) {
  const editorId = editor.getId()
  const currentDecorationIds = cursorDecorationIdsByEditorId.get(editorId) ?? []
  if (currentDecorationIds.length === 0)
    return

  const nextDecorationIds = editor.deltaDecorations(currentDecorationIds, [])
  cursorDecorationIdsByEditorId.set(editorId, nextDecorationIds)
}

function toLiveCursorRange(selection: MonacoEditorNamespace.ISelection | null): LiveCursorRange | null {
  if (!selection)
    return null

  return {
    startLineNumber: selection.startLineNumber,
    startColumn: selection.startColumn,
    endLineNumber: selection.endLineNumber,
    endColumn: selection.endColumn,
  }
}

function publishEditorCursorState(editor: StandaloneCodeEditor, visibleOverride?: boolean, immediate = false) {
  if ($renderContext.value !== 'presenter')
    return
  if (!syncEnabled.value && visibleOverride !== false)
    return

  const editorId = editor.getId()
  const editorKey = getEditorKey(editor)
  if (!isEditorFromCurrentSlide(editorKey))
    return

  const existingTimer = cursorPublishTimers.get(editorId)
  if (existingTimer)
    clearTimeout(existingTimer)

  const delay = immediate ? 0 : 30
  const timer = setTimeout(() => {
    cursorPublishTimers.delete(editorId)
    const visible = visibleOverride ?? canPublishCursorState(editor)
    const position = visible ? editor.getPosition() : null
    const selection = visible ? toLiveCursorRange(editor.getSelection()) : null
    const payload: LiveCursorPayload = {
      editorKey,
      sourceId,
      version: nextCursorVersion(),
      visible,
      cursor: position ? { lineNumber: position.lineNumber, column: position.column } : null,
      selection,
    }
    publishCursorPayload(payload)
  }, delay)

  cursorPublishTimers.set(editorId, timer)
}
function reapplyCachedCursorForEditor(editor: StandaloneCodeEditor) {
  const editorKey = getEditorKey(editor)
  if (!isEditorFromCurrentSlide(editorKey))
    return

  const cursorPayload = cachedCursorPayloadByEditorKey.get(editorKey)
  if (cursorPayload)
    applyCursorPayloadToEditor(editor, cursorPayload)
}

function applyPayloadToEditor(editor: StandaloneCodeEditor, payload: LiveCodeSyncPayload) {
  if (!isConnectedEditor(editor))
    return
  if (!isEditorInPresenterMainPane(editor))
    return
  const model = editor.getModel()
  if (!model)
    return
  if (model.getValue() === payload.content)
    return

  const editorId = editor.getId()
  applyingRemoteByEditorId.add(editorId)
  let didUpdateContent = false
  try {
    model.setValue(payload.content)
    didUpdateContent = true
  }
  catch {
    // Ignore transient editor lifecycle errors during slide transitions.
  }
  finally {
    applyingRemoteByEditorId.delete(editorId)
    if (didUpdateContent)
      reapplyCachedCursorForEditor(editor)
  }
}

function pickPreferredEditor(editors: StandaloneCodeEditor[]): StandaloneCodeEditor | null {
  if (editors.length === 0)
    return null

  const focusedEditor = editors.find(editor => editor.hasTextFocus())
  if (focusedEditor)
    return focusedEditor

  const visibleEditor = editors.find(editor => isVisibleEditor(editor))
  if (visibleEditor)
    return visibleEditor

  return editors[0] ?? null
}
function applyCursorPayloadToEditor(editor: StandaloneCodeEditor, payload: LiveCursorPayload) {
  if (!isConnectedEditor(editor))
    return
  if (!isEditorInPresenterMainPane(editor))
    return

  if (!payload.visible || !payload.cursor || !monaco) {
    clearCursorDecorations(editor)
    return
  }

  const model = editor.getModel()
  if (!model) {
    clearCursorDecorations(editor)
    return
  }

  const cursorPosition = model.validatePosition(payload.cursor)
  const decorations: MonacoEditorNamespace.IModelDeltaDecoration[] = [
    {
      range: new monaco.Range(
        cursorPosition.lineNumber,
        cursorPosition.column,
        cursorPosition.lineNumber,
        cursorPosition.column,
      ),
      options: {
        afterContentClassName: 'live-host-cursor-caret',
        stickiness: monaco.editor.TrackedRangeStickiness.NeverGrowsWhenTypingAtEdges,
      },
    },
  ]

  if (payload.selection && !isCollapsedRange(payload.selection)) {
    const selectionRange = model.validateRange(payload.selection)
    decorations.push({
      range: selectionRange,
      options: {
        className: 'live-host-cursor-selection',
        stickiness: monaco.editor.TrackedRangeStickiness.NeverGrowsWhenTypingAtEdges,
      },
    })
  }

  const editorId = editor.getId()
  const currentDecorationIds = cursorDecorationIdsByEditorId.get(editorId) ?? []
  const nextDecorationIds = editor.deltaDecorations(currentDecorationIds, decorations)
  cursorDecorationIdsByEditorId.set(editorId, nextDecorationIds)
}

function applyPayloadToMatchingEditors(payload: LiveCodeSyncPayload) {
  if (!monaco)
    return

  const payloadSlideNo = getSlideNoFromEditorKey(payload.editorKey)
  if (payloadSlideNo !== slideNo.value)
    return
  const payloadEditorIndex = getEditorIndexFromEditorKey(payload.editorKey)
  const standaloneEditors = monaco.editor.getEditors().filter(isStandaloneCodeEditor)
  const currentSlideEditors: { editor: StandaloneCodeEditor, editorKey: string }[] = []
  for (const editor of standaloneEditors) {
    const editorKey = getEditorKey(editor)
    if (!isEditorFromCurrentSlide(editorKey))
      continue
    currentSlideEditors.push({ editor, editorKey })
  }

  const exactMatches = currentSlideEditors
    .filter(({ editorKey }) => editorKey === payload.editorKey)
    .map(({ editor }) => editor)
  const exactMatch = pickPreferredEditor(exactMatches)
  if (exactMatch) {
    applyPayloadToEditor(exactMatch, payload)
    return
  }

  const indexMatches = currentSlideEditors
    .filter(({ editorKey }) => getEditorIndexFromEditorKey(editorKey) === payloadEditorIndex)
    .map(({ editor }) => editor)
  const indexMatch = pickPreferredEditor(indexMatches)
  if (indexMatch) {
    applyPayloadToEditor(indexMatch, payload)
    return
  }

  if (currentSlideEditors.length === 1)
    applyPayloadToEditor(currentSlideEditors[0].editor, payload)
}
function applyCursorPayloadToMatchingEditors(payload: LiveCursorPayload) {
  if (!monaco)
    return

  const payloadSlideNo = getSlideNoFromEditorKey(payload.editorKey)
  if (payloadSlideNo !== slideNo.value)
    return

  const payloadEditorIndex = getEditorIndexFromEditorKey(payload.editorKey)
  const standaloneEditors = monaco.editor.getEditors().filter(isStandaloneCodeEditor)
  const currentSlideEditors: { editor: StandaloneCodeEditor, editorKey: string }[] = []
  for (const editor of standaloneEditors) {
    const editorKey = getEditorKey(editor)
    if (!isEditorFromCurrentSlide(editorKey))
      continue
    currentSlideEditors.push({ editor, editorKey })
  }

  const exactMatches = currentSlideEditors
    .filter(({ editorKey }) => editorKey === payload.editorKey)
    .map(({ editor }) => editor)
  const exactMatch = pickPreferredEditor(exactMatches)
  if (exactMatch) {
    applyCursorPayloadToEditor(exactMatch, payload)
    return
  }

  const indexMatches = currentSlideEditors
    .filter(({ editorKey }) => getEditorIndexFromEditorKey(editorKey) === payloadEditorIndex)
    .map(({ editor }) => editor)
  const indexMatch = pickPreferredEditor(indexMatches)
  if (indexMatch) {
    applyCursorPayloadToEditor(indexMatch, payload)
    return
  }

  if (currentSlideEditors.length === 1)
    applyCursorPayloadToEditor(currentSlideEditors[0].editor, payload)
}

function canPublishEditorChanges(editor: StandaloneCodeEditor): boolean {
  if (!isConnectedEditor(editor))
    return false
  if (!isEditorInPresenterMainPane(editor))
    return false
  if (!editor.hasTextFocus())
    return false
  return true
}

function publishEditorChanges(editor: StandaloneCodeEditor) {
  if (!syncEnabled.value)
    return
  if (!canPublishEditorChanges(editor))
    return

  const editorId = editor.getId()
  if (applyingRemoteByEditorId.has(editorId))
    return

  const editorKey = getEditorKey(editor)
  if (!isEditorFromCurrentSlide(editorKey))
    return

  const existingTimer = publishTimers.get(editorId)
  if (existingTimer)
    clearTimeout(existingTimer)

  const timer = setTimeout(() => {
    publishTimers.delete(editorId)
    const payload: LiveCodeSyncPayload = {
      editorKey,
      content: editor.getValue(),
      sourceId,
      version: nextCodeVersion(),
    }
    publishCodePayload(payload)
  }, 100)

  publishTimers.set(editorId, timer)
}

function untrackEditor(editorId: string) {
  const editor = trackedEditors.get(editorId)
  if (editor)
    clearCursorDecorations(editor)
  const subscriptions = editorSubscriptions.get(editorId)
  subscriptions?.onChange.dispose()
  subscriptions?.onCursorPosition.dispose()
  subscriptions?.onCursorSelection.dispose()
  subscriptions?.onFocus.dispose()
  subscriptions?.onBlur.dispose()
  subscriptions?.onDispose.dispose()
  editorSubscriptions.delete(editorId)
  trackedEditors.delete(editorId)

  const timer = publishTimers.get(editorId)
  if (timer)
    clearTimeout(timer)
  publishTimers.delete(editorId)

  const cursorTimer = cursorPublishTimers.get(editorId)
  if (cursorTimer)
    clearTimeout(cursorTimer)
  cursorPublishTimers.delete(editorId)
  applyingRemoteByEditorId.delete(editorId)
  cursorDecorationIdsByEditorId.delete(editorId)
}

function trackEditor(editor: StandaloneCodeEditor) {
  const editorId = editor.getId()
  if (trackedEditors.has(editorId))
    return
  if (!isConnectedEditor(editor))
    return
  if (!isEditorInPresenterMainPane(editor))
    return

  const editorKey = getEditorKey(editor)
  if (!isEditorFromCurrentSlide(editorKey))
    return

  trackedEditors.set(editorId, editor)

  const onChange = editor.onDidChangeModelContent(() => publishEditorChanges(editor))
  const onCursorPosition = editor.onDidChangeCursorPosition(() => publishEditorCursorState(editor))
  const onCursorSelection = editor.onDidChangeCursorSelection(() => publishEditorCursorState(editor))
  const onFocus = editor.onDidFocusEditorText(() => publishEditorCursorState(editor, true, true))
  const onBlur = editor.onDidBlurEditorText(() => publishEditorCursorState(editor, false, true))
  const onDispose = editor.onDidDispose(() => untrackEditor(editorId))
  editorSubscriptions.set(editorId, { onChange, onCursorPosition, onCursorSelection, onFocus, onBlur, onDispose })

  const payload = cachedPayloadByEditorKey.get(editorKey)
  if (payload)
    applyPayloadToEditor(editor, payload)

  const cursorPayload = cachedCursorPayloadByEditorKey.get(editorKey)
  if (cursorPayload)
    applyCursorPayloadToEditor(editor, cursorPayload)
}

function handleRemotePayload(payload?: LiveCodeSyncPayload) {
  if (!payload)
    return
  if (payload.sourceId === sourceId)
    return
  if (getSlideNoFromEditorKey(payload.editorKey) !== slideNo.value)
    return

  const knownVersion = latestRemoteVersionByEditorKey.get(payload.editorKey) ?? 0
  if (payload.version <= knownVersion)
    return

  latestRemoteVersionByEditorKey.set(payload.editorKey, payload.version)
  cachedPayloadByEditorKey.set(payload.editorKey, payload)

  if (syncEnabled.value)
    applyPayloadToMatchingEditors(payload)
}
function handleRemoteCursorPayload(payload?: LiveCursorPayload) {
  if (!payload)
    return
  if (payload.sourceId === sourceId)
    return
  if (getSlideNoFromEditorKey(payload.editorKey) !== slideNo.value)
    return

  const knownVersion = latestRemoteCursorVersionByEditorKey.get(payload.editorKey) ?? 0
  if (payload.version <= knownVersion)
    return

  latestRemoteCursorVersionByEditorKey.set(payload.editorKey, payload.version)
  cachedCursorPayloadByEditorKey.set(payload.editorKey, payload)

  if (syncEnabled.value)
    applyCursorPayloadToMatchingEditors(payload)
}

function stopSync() {
  syncRunning = false
  createEditorListener?.dispose()
  createEditorListener = null
  for (const editor of trackedEditors.values())
    publishEditorCursorState(editor, false, true)

  for (const editorId of Array.from(trackedEditors.keys()))
    untrackEditor(editorId)
}

async function startSync() {
  if (syncRunning || typeof window === 'undefined')
    return

  syncRunning = true
  if (!monaco)
    monaco = await import('monaco-editor')
  if (!syncRunning)
    return

  for (const editor of monaco.editor.getEditors()) {
    if (isStandaloneCodeEditor(editor))
      trackEditor(editor)
  }

  createEditorListener = monaco.editor.onDidCreateEditor((editor) => {
    if (isStandaloneCodeEditor(editor))
      trackEditor(editor)
  })
}

const stopWatchingPayload = watch(
  () => syncState.liveCodeSync,
  payload => handleRemotePayload(payload),
  { immediate: true, deep: true },
)
const stopWatchingCursorPayload = watch(
  () => syncState.liveCursorSync,
  payload => handleRemoteCursorPayload(payload),
  { immediate: true, deep: true },
)

const stopWatchingSyncEnabled = watch(
  syncEnabled,
  (enabled) => {
    if (enabled)
      void startSync()
    else
      stopSync()
  },
  { immediate: true },
)

onBeforeUnmount(() => {
  stopWatchingPayload()
  stopWatchingCursorPayload()
  stopWatchingSyncEnabled()
  stopSync()
})
</script>

<style scoped>
:deep(.live-host-cursor-selection) {
  background-color: rgb(33 110 199 / 24%);
}

:deep(.live-host-cursor-caret) {
  border-left: 2px solid var(--slidev-theme-primary, #216ec7);
  margin-left: -1px;
  pointer-events: none;
  animation: live-host-cursor-blink 1s step-end infinite;
}

@keyframes live-host-cursor-blink {
  0%,
  49% {
    opacity: 1;
  }
  50%,
  100% {
    opacity: 0;
  }
}
</style>
