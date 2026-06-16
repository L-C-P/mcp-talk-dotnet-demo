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

    const source = this.src && (
      this.src.startsWith("/") ||
      this.src.startsWith("./") ||
      this.src.startsWith("../") ||
      this.src.startsWith("http://") ||
      this.src.startsWith("https://")
    )
      ? this.src
      : import.meta.env.BASE_URL + this.src;

    this.player = AsciinemaPlayer.create(
      source,
      targetElement,
      this.playerProps
    );

    this.visibilityObserver = new IntersectionObserver(
      ([entry]) => {
        const isVisible = entry.isIntersecting && entry.intersectionRatio > 0;

        if (isVisible && !this.wasVisible && this.player) {
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
