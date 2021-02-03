<template>
    <div :class="classname">
        <label v-if="prefix(field)" v-html="prefix(field)"></label>
        <input type="checkbox"
               v-on="eventHandler(field, 'handleChange')"
               class="radio"
               v-bind:class="fieldClass(field)"
               v-model="field.value"
               :name="field.propertyName"
               v-bind:disabled="field.disabled || field.readOnly"
               :id="field.propertyName"> <label :for="field.propertyName" v-html="field.inputPost"></label>
        <label v-if="suffix(field)" v-html="suffix(field)"></label>
    </div>
</template>
<script>
    import { shared } from './input';

    export default {
        props: ['field', 'classname'],
        mixins: [shared],
        methods: {
            handleChange($event) {
                console.log('checkbox');
                this.$parent.postFields($event);
            }
        },
        mounted() {
        },
        computed: {
            id() {
                return this.field.propertyName;
            },
        }
    }
</script>
<style>
    input[type=checkbox] {
        width: auto !important;
    }
</style>