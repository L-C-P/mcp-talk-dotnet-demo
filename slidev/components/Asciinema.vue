<template>
  <RenderWhen context="main">
    <div ref="playerContainer"/>
  </RenderWhen>
</template>

<script setup>
import {ref, computed, onMounted, onBeforeUnmount} from "vue";
import RenderWhen from "@slidev/client/builtin/RenderWhen.vue";
import * as AsciinemaPlayer from "asciinema-player";

const props = defineProps({
  src: {type: String, required: true},
  props: {type: Object, default: () => ({})},
  markers: {type: Array, default: () => []},
});

const defaultProps = {
  autoPlay: true,
  fit: false,
  idleTimeLimit: 5,
  terminalFontFamily: 'Hack Nerd Font',
  terminalFontSize: '13px',
  controls: false,
  pauseOnMarkers: false,
};

const mergedPlayerProps = computed(() => ({
  ...defaultProps,
  ...props.props,
  ...(props.markers.length > 0 && {markers: props.markers})
}));

const playerContainer = ref(null);
const player = ref(null);
const visibilityObserver = ref(null);
const wasVisible = ref(false);
const playDelayHandler = ref(null);

const blurFocusedPlayerElement = () => {
  const activeElement = document.activeElement;
  const target = playerContainer.value;

  if (
    activeElement instanceof HTMLElement &&
    target &&
    target.contains(activeElement)
  ) {
    // activeElement.blur();
  }
};

const onPointerUp = () => {
  requestAnimationFrame(blurFocusedPlayerElement);
};

const onFocusIn = () => {
  requestAnimationFrame(blurFocusedPlayerElement);
};

onMounted(() => {
  const target = playerContainer.value;
  if (!(target instanceof HTMLElement)) return;

  player.value = AsciinemaPlayer.create(
    props.src,
    target,
    mergedPlayerProps.value
  );

  target.addEventListener("pointerup", onPointerUp);
  target.addEventListener("focusin", onFocusIn);

  visibilityObserver.value = new IntersectionObserver(
    ([entry]) => {
      const isVisible = entry.isIntersecting && entry.intersectionRatio > 0;

      if (isVisible && !wasVisible.value && player.value) {
        player.value.pause();

        if (mergedPlayerProps.value?.numberStartAt)
          player.value.seek(mergedPlayerProps.value.numberStartAt);
        else
          player.value.seek(0);

        if (mergedPlayerProps.value?.autoPlay) {
          if (mergedPlayerProps.value?.delayStart) {
            playDelayHandler.value = setTimeout(
              () => player.value.play(),
              mergedPlayerProps.value.delayStart * 1000
            );
          } else {
            player.value.play();
          }
        }
      } else if (!isVisible && wasVisible.value && player.value) {
        player.value.pause();
        player.value.seek(0);
      }

      wasVisible.value = isVisible;
    },
    {threshold: 0.1}
  );

  visibilityObserver.value.observe(target);
});

onBeforeUnmount(() => {
  if (playDelayHandler.value) {
    clearTimeout(playDelayHandler.value);
    playDelayHandler.value = null;
  }

  const target = playerContainer.value;
  if (target instanceof HTMLElement) {
    target.removeEventListener("pointerup", onPointerUp);
    target.removeEventListener("focusin", onFocusIn);
  }

  visibilityObserver.value?.disconnect();
  visibilityObserver.value = null;
});
</script>

<style>
@import 'asciinema-player/dist/bundle/asciinema-player.css';

.slidev-presenter .ap-player:focus-within {
  outline: 2px solid var(--brand-primary, #4c9aff);
  outline-offset: 3px;
  border-radius: 4px;
}
</style>
