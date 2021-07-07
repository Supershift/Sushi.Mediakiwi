<template>
    <v-container :class="expressionClass(field.expression)">
        <th :class="expressCell(field.expression)"><label :for="field.propertyName"><abbr style="display: block;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;" :title="field.title" v-html="field.title"></abbr></label></th>
        <td :class="expressCell(field.expression)">
            <div :class="isHalfField(field.expression)">
                <label v-if="prefix(field)" v-html="prefix(field)"></label>
                <label :class="getClass(field)"
                       :name="field.propertyName"
                       :id="field.propertyName"
                       v-html="value"></label>
                <label v-if="suffix(field)" v-html="suffix(field)"></label>
            </div>
        </td>
    </v-container>
</template>
<script>
    import { shared } from '../../shared';

    export default {
        props: ['field'],
        mixins: [shared],
        data() {
            return {
                value: "",
            }
        },
        methods: { 
            getClass(field) {
                let expression = this.expressCell(field.expression);
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
