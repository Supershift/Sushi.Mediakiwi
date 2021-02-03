<template>
    <v-container :class="expressionClass(field.expression)">
        <th :class="expressCell(field.expression)"><label :for="field.propertyName"><abbr style="display: block;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;" :title="field.title" v-html="field.title"></abbr></label></th>
        <td :class="expressCell(field.expression)">
            <div :class="isHalfField(field.expression)">
                <label v-if="prefix(field)" v-html="prefix(field)"></label>
                <input type="checkbox"
                       v-on="eventHandler(field, 'handleChange')"
                       class="radio"
                       v-bind:class="fieldClass(field)"
                       v-model="field.value"
                       :name="field.propertyName"
                       :disabled="field.disabled == 1"
                       :id="field.propertyName"> <label :for="field.propertyName" v-html="field.inputPost"></label>
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