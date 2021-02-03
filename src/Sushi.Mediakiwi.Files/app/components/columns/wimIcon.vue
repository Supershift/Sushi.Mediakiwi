<template>
    <figure v-if="value" class="icon icon-check-circle-o pos"></figure>
    <figure v-else class="icon icon-times-circle-o neg"></figure>
</template>
<script>
    export default {
        props: ["value"],
    }
</script>
