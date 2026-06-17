<template>
  <RenderWhen context="main">
    <div ref="playerContainer"/>
  </RenderWhen>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from "vue";
import RenderWhen from "@slidev/client/builtin/RenderWhen.vue";
import * as AsciinemaPlayer from "asciinema-player";

const props = defineProps(["src", "playerProps"]);

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
    activeElement.blur();
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
    props.playerProps
  );

  target.addEventListener("pointerup", onPointerUp);
  target.addEventListener("focusin", onFocusIn);

  visibilityObserver.value = new IntersectionObserver(
    ([entry]) => {
      const isVisible = entry.isIntersecting && entry.intersectionRatio > 0;

      if (isVisible && !wasVisible.value && player.value) {
        if (props.playerProps?.delayStart)
          player.value.pause();

        if (props.playerProps?.numberStartAt)
          player.value.seek(props.playerProps.numberStartAt);
        else
          player.value.seek(0);

        if (props.playerProps?.autoPlay) {
          if (props.playerProps?.delayStart)
            playDelayHandler.value = setTimeout(
              () => player.value.play(),
              props.playerProps.delayStart * 1000
            );
          else
            player.value.play();
        }
      } else if (!isVisible && wasVisible.value && player.value) {
        player.value.pause();
        player.value.seek(0);
      }
      wasVisible.value = isVisible;
    },
    { threshold: 0.1 }
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
</style>
