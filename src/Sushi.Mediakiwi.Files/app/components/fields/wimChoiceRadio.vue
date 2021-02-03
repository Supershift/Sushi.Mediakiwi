<template>
    <v-container :class="expressionClass(field.expression)">
        <th :class="expressCell(field.expression)"><label :for="field.propertyName"><abbr style="display: block;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;" :title="field.title" v-html="field.title"></abbr></label></th>
        <td :class="expressCell(field.expression)">
            <div :class="isHalfField(field.expression)">
                <label v-if="prefix(field)" v-html="prefix(field)"></label>
                <v-container v-for="option in field.options" :key="fieldID(option)">
                    <input type="radio"
                           v-on="eventHandler(field, 'handleChange')"
                           v-model="field.value"
                           class="radio"
                           v-bind:class="errorClass(field)"
                           :name="field.groupName"
                           :value="option.value"
                           :disabled="field.disabled == 1"
                           :id="fieldID(option)" />
                    <label :for="fieldID(option)" v-html="option.text"></label>
                </v-container>
                <label v-if="suffix(field)" v-html="suffix(field)"></label>
            </div>
        </td>
    </v-container>
</template>
<script>
    import { shared } from './../../shared';


    export default {
        props: ['field'],
        mixins: [shared],
        methods: {
            handleChange($event) {
                console.log('radio');
                this.$parent.postFields($event);
            },
            fieldID(option) {
                return `${this.field.propertyName}_${option.value}`;
            },
            init() {
                // custom
                if (typeof (this.field.value) === "string") {
                    if (this.field.value.toLowerCase() === "false") {
                        this.field.value = 0;
                    }
                    else if (this.field.value.toLowerCase() === "true") {
                        this.field.value = 1;
                    }
                }
            }
        },
        watch: {
            field() {
                this.init();
            }
        },
        mounted() {
            this.init();
        },
        computed: {
            id() {
                return this.field.propertyName;
            },
        }
    }
</script>