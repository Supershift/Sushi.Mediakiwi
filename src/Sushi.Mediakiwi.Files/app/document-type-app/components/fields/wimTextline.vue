<template>
    <div :class="classname">
        <label v-if="prefix(field)" v-html="prefix(field)"></label>
        <label :class="getClass(field)"
               :name="field.propertyName"
               :id="field.propertyName"
               v-html="value"></label>
        <label v-if="suffix(field)" v-html="suffix(field)"></label>
    </div>
</template>
<script>
    import { shared } from './input';

    export default {
        props: ['field', 'classname'],
        mixins: [shared],
        data() {
            return {
                value: "",
            }
        },
        methods: {
            getClass(field) {
                let expression = '';//this.expressCell(field.expression);
                if (typeof (field.className) !== 'undefined' && field.className) {
                    expression += ` ${field.className}`;
                }
                return expression;
            },
        },
        mounted() {
            this.value = this.field.value;
            if (!this.value)
                this.value = "&nbsp;";

            // TODO ENUM!?
            if (this.field.vueType === "wimChoiceCheckbox") {
                this.field.className += " half short";
                this.value = this.field.value ? 'Yes' : 'No';
            }
        },
        computed: {
            id() {
                return this.field.propertyName;
            },
        }
    }
</script>
