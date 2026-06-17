<template>
  <component :is="'style'">{{ cssVariables }}</component>
</template>

<script setup>
import {cssVariables} from './styles/theme.ts'
</script>
