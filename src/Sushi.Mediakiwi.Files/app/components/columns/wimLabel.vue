<template>
    <label v-html="value"></label>
</template>
<script>
    export default {
        props: ["value"],
    }
</script>