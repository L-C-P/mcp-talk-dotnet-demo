<template>
  <div ref="playerContainer"/>
</template>

<script>
import * as AsciinemaPlayer from "asciinema-player";

export default {
  props: ["src", "playerProps"],
  data() {
    return {
      player: null,
      visibilityObserver: null,
      wasVisible: false,
    };
  },
  mounted() {
    const targetElement = this.$refs.playerContainer;

    if (!(targetElement instanceof HTMLElement)) {
      return;
    }

    this.player = AsciinemaPlayer.create(
      this.src,
      targetElement,
      this.playerProps
    );

    this.visibilityObserver = new IntersectionObserver(
      ([entry]) => {
        const isVisible = entry.isIntersecting && entry.intersectionRatio > 0;

        if (isVisible && !this.wasVisible && this.player) {
          if (this.playerProps?.delayStart)
            this.player.pause()

          if (this.playerProps?.numberStartAt)
            this.player.seek(this.playerProps.numberStartAt);
          else
            this.player.seek(0);

          if (this.playerProps?.autoPlay) {
            if (this.playerProps?.delayStart)
              setTimeout(() => this.player.play(), this.playerProps.delayStart * 1000);
            else
              this.player.play()
          }
        } else if (!isVisible && this.wasVisible && this.player) {
          this.player.pause();
          this.player.seek(0);
        }
        this.wasVisible = isVisible;
      },
      {threshold: 0.1}
    );

    this.visibilityObserver.observe(targetElement);
  },
  beforeUnmount() {
    this.visibilityObserver?.disconnect();
    this.visibilityObserver = null;
  },
};
</script>

<style>
@import 'asciinema-player/dist/bundle/asciinema-player.css';
</style>
