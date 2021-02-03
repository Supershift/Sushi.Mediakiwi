<template>
    <div :class="classname" class="error">
        <label v-if="prefix(field)" v-html="prefix(field)"></label>
        <Select2 v-model="field.value"
                 tabindex="-1"
                 :name="field.propertyName"
                 :id="field.propertyName"
                 :value="field.value"
                 :options="select2data"
                 :settings="select2Settings"
                 v-bind:class="errorClass(field)"
                 v-bind:disabled="field.disabled || field.readOnly"
                 ref="dropdown">
        </Select2>
        <label v-if="suffix(field)" v-html="suffix(field)"></label>
    </div>
</template>
<script>
    import { shared } from './input';
    import Select2 from 'v-select2-component';    

    export default {
        props: ['field', 'classname'],
        mixins: [shared],
        components: {
            Select2
        },
        data() {
            return {
                select2Settings: {
                    width: '100%',
                    placeholder: '',
                    allowClear: true,
                }
            }
        },
        computed: {
            select2data() {
                if (!this.field || !this.field.options || !this.field.options.length)
                    return [];

                return this.field.options.map((r) => {
                    return {
                        id: r.value,
                        text: r.text,
                        disabled: (typeof (r.disabled) !== "undefined" && r.disabled) ? r.disabled : false,
                    };
                });
            }
        },
        methods: {
            valueChanged() {
                //console.log('trigger:valueChanged');
                this.$emit('input', this.$refs.dropdown.value);
                if (this.field.event === 'none') return;
                this.$parent.postFields(undefined, this.field.propertyName);
            },
        },
        mounted() {

        },
        destroyed() {
        }
    }
</script>
<style>
    td [aria-hidden=true] {
        display: block;
    }
</style>