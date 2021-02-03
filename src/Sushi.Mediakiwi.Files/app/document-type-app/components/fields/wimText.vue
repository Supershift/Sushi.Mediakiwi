<template>
    <div :class="classname">
        <label v-if="prefix(field)" v-html="prefix(field)"></label>
        <input type="text"
               v-model="field.value"
               v-on="eventHandler(field, 'handleChange')"
               :class="fieldClass(field)"
               :name="field.propertyName"
               :id="field.propertyName"
               v-bind:disabled="field.disabled || field.readOnly">
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
                console.log('triggerChange:wimText');
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
