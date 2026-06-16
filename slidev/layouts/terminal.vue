<script setup lang="ts">
import {computed, PropType} from 'vue'
import {resolveAssetUrl} from "../layoutHelper";

const props = defineProps({
  cast: {
    type: String as PropType<String>,
    required: true
  },
})

const file = computed(() => resolveAssetUrl(`/assets/casts/${props.cast}`));
</script>

<template>
  <div class="slidev-layout terminal">
    <Asciinema :src="file"
               :playerProps="{autoPlay: true, fit: false, idleTimeLimit: 5, terminalFontFamily: 'Hack Nerd Font', terminalFontSize: '13px', controls: false }"/>
  </div>
</template>
