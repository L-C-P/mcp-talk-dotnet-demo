<template>
  <span class="code-block-sync-activator" />
</template>

<script setup lang="ts">
import './codeblocksync.css'
import type { SharedState } from '@slidev/client'
import { sharedState, useIsSlideActive, useSlideContext } from '@slidev/client'
import { computed, onBeforeUnmount, onMounted, watch } from 'vue'

interface LiveCodeBlockScrollPayload {
  codeBlockKey: string
  sourceId: string
  version: number
  scrollTop: number
  scrollLeft: number
}

interface LiveCodeBlockSelectionPayload {
  codeBlockKey: string
  sourceId: string
  version: number
  visible: boolean
  anchorOffset: number
  focusOffset: number
}

type SharedStateWithCodeBlockSync = SharedState & {
  liveCodeBlockScroll?: LiveCodeBlockScrollPayload
  liveCodeBlockSelection?: LiveCodeBlockSelectionPayload
  $patch?: (patch: Partial<SharedStateWithCodeBlockSync>) => Promise<boolean>
}

const syncState = sharedState as SharedStateWithCodeBlockSync
const sourceId = typeof crypto !== 'undefined' && 'randomUUID' in crypto
  ? crypto.randomUUID()
  : `code-block-sync-${Math.random().toString(36).slice(2)}`

const { $page, $renderContext } = useSlideContext()
const isSlideActive = useIsSlideActive()
const slideNo = computed(() => String($page.value))
const isPresenter = computed(() => $renderContext.value === 'presenter')

let scrollVersionCounter = 0
let selectionVersionCounter = 0
let pollTimer: ReturnType<typeof setInterval> | null = null
const applyingRemoteScroll = new Set<string>()
const applyingRemoteSelection = new Set<string>()
const latestRemoteScrollVersionByKey = new Map<string, number>()
const cachedScrollPayloadByKey = new Map<string, LiveCodeBlockScrollPayload>()
const latestRemoteSelectionVersionByKey = new Map<string, number>()
const cachedSelectionPayloadByKey = new Map<string, LiveCodeBlockSelectionPayload>()

function getWrappersOnSlide(): HTMLElement[] {
  const slideNode = document.querySelector(`.slidev-page[data-slidev-no="${slideNo.value}"]`)
  if (!slideNode)
    return []
  return Array.from(slideNode.querySelectorAll<HTMLElement>('.slidev-code-wrapper'))
}

function getCodeBlockKey(wrapper: HTMLElement): string {
  const wrappers = getWrappersOnSlide()
  const index = wrappers.indexOf(wrapper)
  return `${slideNo.value}:${index >= 0 ? index : 0}`
}

function getSlideNoFromKey(key: string): string {
  return key.split(':')[0] ?? ''
}

function nextScrollVersion(): number {
  scrollVersionCounter += 1
  return scrollVersionCounter
}

function nextSelectionVersion(): number {
  selectionVersionCounter += 1
  return selectionVersionCounter
}

function getScrollStateForWrapper(wrapper: HTMLElement): { scrollTop: number, scrollLeft: number } {
  const scrollTop = wrapper.scrollTop
  let scrollLeft = wrapper.scrollLeft

  const codeEls = wrapper.querySelectorAll<HTMLElement>('.slidev-code')
  for (const codeEl of codeEls) {
    if (codeEl.scrollLeft !== 0) {
      scrollLeft = codeEl.scrollLeft
      break
    }
  }

  return { scrollTop, scrollLeft }
}

function publishScrollPayload(payload: LiveCodeBlockScrollPayload): void {
  if (typeof syncState.$patch === 'function') {
    void syncState.$patch({ liveCodeBlockScroll: payload })
    return
  }
  syncState.lastUpdate = {
    id: sourceId,
    type: 'presenter',
    time: Date.now(),
  }
  const currentSlideNo = Number.parseInt(slideNo.value, 10)
  if (Number.isFinite(currentSlideNo))
    syncState.page = currentSlideNo
  syncState.liveCodeBlockScroll = payload
}

function publishSelectionPayload(payload: LiveCodeBlockSelectionPayload): void {
  if (typeof syncState.$patch === 'function') {
    void syncState.$patch({ liveCodeBlockSelection: payload })
    return
  }
  syncState.lastUpdate = {
    id: sourceId,
    type: 'presenter',
    time: Date.now(),
  }
  const currentSlideNo = Number.parseInt(slideNo.value, 10)
  if (Number.isFinite(currentSlideNo))
    syncState.page = currentSlideNo
  syncState.liveCodeBlockSelection = payload
}

function getTextContentFromCodeBlock(wrapper: HTMLElement): string {
  const codeEl = wrapper.querySelector('.slidev-code') as HTMLElement | null
  if (!codeEl)
    return ''
  return codeEl.textContent || ''
}

function getTextOffsetFromSelection(selection: Selection, wrapper: HTMLElement): { anchor: number, focus: number } {
  if (selection.rangeCount === 0)
    return { anchor: 0, focus: 0 }

  const range = selection.getRangeAt(0)
  const codeEl = wrapper.querySelector('.slidev-code')
  if (!codeEl)
    return { anchor: 0, focus: 0 }

  if (!codeEl.contains(range.commonAncestorContainer))
    return { anchor: 0, focus: 0 }

  // Calculate text offset by walking all text nodes
  let anchorOffset = 0
  let focusOffset = 0
  let currentOffset = 0
  let foundAnchor = false
  let foundFocus = false

  const walker = document.createTreeWalker(codeEl, NodeFilter.SHOW_TEXT)
  let node: Node | null = walker.currentNode

  while (node) {
    const nodeLength = node.textContent?.length || 0

    if (!foundAnchor && node === range.startContainer) {
      anchorOffset = currentOffset + range.startOffset
      foundAnchor = true
    }
    if (!foundFocus && node === range.endContainer) {
      focusOffset = currentOffset + range.endOffset
      foundFocus = true
    }

    currentOffset += nodeLength
    node = walker.nextNode()
  }

  return { anchor: anchorOffset, focus: focusOffset }
}

function applySelectionFromOffsets(wrapper: HTMLElement, anchorOffset: number, focusOffset: number): void {
  const codeEl = wrapper.querySelector('.slidev-code') as HTMLElement | null
  if (!codeEl)
    return

  const startOffset = Math.min(anchorOffset, focusOffset)
  const endOffset = Math.max(anchorOffset, focusOffset)

  if (startOffset === 0 && endOffset === 0)
    return

  let currentOffset = 0
  let startNode: Node | null = null
  let startNodeOffset = 0
  let endNode: Node | null = null
  let endNodeOffset = 0

  const walker = document.createTreeWalker(codeEl, NodeFilter.SHOW_TEXT)
  let node: Node | null = walker.currentNode

  while (node) {
    const nodeLength = node.textContent?.length || 0

    if (!startNode && currentOffset + nodeLength >= startOffset) {
      startNode = node
      startNodeOffset = startOffset - currentOffset
    }
    if (!endNode && currentOffset + nodeLength >= endOffset) {
      endNode = node
      endNodeOffset = endOffset - currentOffset
      break
    }

    currentOffset += nodeLength
    node = walker.nextNode()
  }

  if (!startNode || !endNode)
    return

  try {
    const range = document.createRange()
    range.setStart(startNode, startNodeOffset)
    range.setEnd(endNode, endNodeOffset)

    const selection = window.getSelection()
    if (selection) {
      selection.removeAllRanges()
      selection.addRange(range)
    }
  }
  catch {
    // Ignore range errors during transitions
  }
}

function onWrapperScroll(wrapper: HTMLElement, key: string): void {
  if (applyingRemoteScroll.has(key))
    return
  if (!isPresenter.value)
    return

  const scrollState = getScrollStateForWrapper(wrapper)
  const payload: LiveCodeBlockScrollPayload = {
    codeBlockKey: key,
    sourceId,
    version: nextScrollVersion(),
    scrollTop: scrollState.scrollTop,
    scrollLeft: scrollState.scrollLeft,
  }
  publishScrollPayload(payload)
}

function applyRemoteScroll(wrapper: HTMLElement, payload: LiveCodeBlockScrollPayload): void {
  if (isPresenter.value)
    return

  const key = getCodeBlockKey(wrapper)
  if (key !== payload.codeBlockKey)
    return
  if (applyingRemoteScroll.has(key))
    return

  applyingRemoteScroll.add(key)

  wrapper.scrollTop = payload.scrollTop
  wrapper.scrollLeft = payload.scrollLeft

  const codeEls = wrapper.querySelectorAll<HTMLElement>('.slidev-code')
  for (const codeEl of codeEls) {
    if (codeEl.scrollLeft !== payload.scrollLeft)
      codeEl.scrollLeft = payload.scrollLeft
  }

  setTimeout(() => applyingRemoteScroll.delete(key), 10)
}

function onSelectionChange(): void {
  if (!isPresenter.value)
    return
  if (!syncState.liveCodeBlockSelection && !syncState.liveCodeBlockScroll)
    return

  const selection = window.getSelection()
  if (!selection || selection.isCollapsed) {
    // Clear selection
    const key = `${slideNo.value}:0`
    const payload: LiveCodeBlockSelectionPayload = {
      codeBlockKey: key,
      sourceId,
      version: nextSelectionVersion(),
      visible: false,
      anchorOffset: 0,
      focusOffset: 0,
    }
    publishSelectionPayload(payload)
    return
  }

  const wrappers = getWrappersOnSlide()
  for (const wrapper of wrappers) {
    const codeEl = wrapper.querySelector('.slidev-code')
    if (codeEl && selection.rangeCount > 0) {
      const range = selection.getRangeAt(0)
      if (codeEl.contains(range.commonAncestorContainer)) {
        const key = getCodeBlockKey(wrapper)
        const offsets = getTextOffsetFromSelection(selection, wrapper)
        const payload: LiveCodeBlockSelectionPayload = {
          codeBlockKey: key,
          sourceId,
          version: nextSelectionVersion(),
          visible: true,
          anchorOffset: offsets.anchor,
          focusOffset: offsets.focus,
        }
        publishSelectionPayload(payload)
        break
      }
    }
  }
}

function setupScrollSync(wrapper: HTMLElement): void {
  const key = getCodeBlockKey(wrapper)
  if (!key)
    return

  const handler = () => onWrapperScroll(wrapper, key)
  wrapper.addEventListener('scroll', handler, { passive: true })

  const codeEls = wrapper.querySelectorAll<HTMLElement>('.slidev-code')
  for (const codeEl of codeEls) {
    codeEl.addEventListener('scroll', handler, { passive: true })
  }

  const cached = cachedScrollPayloadByKey.get(key)
  if (cached && !isPresenter.value)
    applyRemoteScroll(wrapper, cached)
}

function setupSelectionSync(wrapper: HTMLElement): void {
  const cached = cachedSelectionPayloadByKey.get(getCodeBlockKey(wrapper))
  if (cached && !isPresenter.value && cached.visible) {
    applySelectionFromOffsets(wrapper, cached.anchorOffset, cached.focusOffset)
  }
}

function trackAllWrappers(): void {
  const wrappers = getWrappersOnSlide()
  for (const wrapper of wrappers) {
    setupScrollSync(wrapper)
    setupSelectionSync(wrapper)
  }
}

function startPolling(): void {
  stopPolling()
  trackAllWrappers()
  pollTimer = setInterval(() => {
    trackAllWrappers()
  }, 100)
}

function stopPolling(): void {
  if (pollTimer) {
    clearInterval(pollTimer)
    pollTimer = null
  }
}

function cleanupAll(): void {
  stopPolling()
  document.removeEventListener('selectionchange', onSelectionChange as EventListener)
}

function handleRemoteScrollPayload(payload?: LiveCodeBlockScrollPayload): void {
  if (!payload || payload.sourceId === sourceId)
    return
  if (getSlideNoFromKey(payload.codeBlockKey) !== slideNo.value)
    return

  const knownVersion = latestRemoteScrollVersionByKey.get(payload.codeBlockKey) ?? 0
  if (payload.version <= knownVersion)
    return

  latestRemoteScrollVersionByKey.set(payload.codeBlockKey, payload.version)
  cachedScrollPayloadByKey.set(payload.codeBlockKey, payload)

  if (isPresenter.value)
    return

  const wrappers = getWrappersOnSlide()
  for (const wrapper of wrappers) {
    const key = getCodeBlockKey(wrapper)
    if (key === payload.codeBlockKey) {
      applyRemoteScroll(wrapper, payload)
      return
    }
  }
}

function handleRemoteSelectionPayload(payload?: LiveCodeBlockSelectionPayload): void {
  if (!payload || payload.sourceId === sourceId)
    return
  if (getSlideNoFromKey(payload.codeBlockKey) !== slideNo.value)
    return

  const knownVersion = latestRemoteSelectionVersionByKey.get(payload.codeBlockKey) ?? 0
  if (payload.version <= knownVersion)
    return

  latestRemoteSelectionVersionByKey.set(payload.codeBlockKey, payload.version)
  cachedSelectionPayloadByKey.set(payload.codeBlockKey, payload)

  if (isPresenter.value)
    return

  const wrappers = getWrappersOnSlide()
  for (const wrapper of wrappers) {
    const key = getCodeBlockKey(wrapper)
    if (key === payload.codeBlockKey) {
      if (payload.visible) {
        applySelectionFromOffsets(wrapper, payload.anchorOffset, payload.focusOffset)
      } else {
        window.getSelection()?.removeAllRanges()
      }
      return
    }
  }
}

const stopWatchingScroll = watch(
  () => syncState.liveCodeBlockScroll,
  payload => handleRemoteScrollPayload(payload),
  { immediate: true, deep: true },
)

const stopWatchingSelection = watch(
  () => syncState.liveCodeBlockSelection,
  payload => handleRemoteSelectionPayload(payload),
  { immediate: true, deep: true },
)

const stopWatchingSlideActive = watch(
  isSlideActive,
  (active) => {
    if (active) {
      startPolling()
      document.addEventListener('selectionchange', onSelectionChange as EventListener)
    } else {
      cleanupAll()
    }
  },
  { immediate: true },
)

onMounted(() => {
  if (isSlideActive.value) {
    startPolling()
    document.addEventListener('selectionchange', onSelectionChange as EventListener)
  }
})

onBeforeUnmount(() => {
  stopWatchingScroll()
  stopWatchingSelection()
  stopWatchingSlideActive()
  cleanupAll()
})
</script>

<style scoped>
.code-block-sync-activator {
  display: none;
}
</style>
