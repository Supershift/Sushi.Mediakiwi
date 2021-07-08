<template>
    <v-container :class="expressionClass(field.expression)">
        <th :class="expressCell(field.expression)"><label :for="field.propertyName"><abbr style="display: block;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;" :title="field.title" v-html="field.title"></abbr></label></th>
        <td :class="expressCell(field.expression)">
            <div :class="isHalfField(field.expression)">
                <textarea cols="32"
                          rows="3"
                          type="text"
                          v-model="field.value"
                          v-on="eventHandler(field, 'handleChange')"
                          :class="fieldClass(field)"
                          :name="field.propertyName"
                          :id="field.propertyName"
                          :disabled="field.disabled == 1"
                          ref="textArea">{{field.value}}</textarea>
            </div>
        </td>
    </v-container>
</template>
<script>
    import { shared } from './../../shared';

    export default {
        props: ['field'],
        mixins: [shared],
        data() {
            return {
                offset: undefined,
            }
        },
        methods: {
            handleChange($event) {
                console.log('triggerChange:wimTextAre');
                this.$parent.postFields($event);
            },
            autoResize(element) {
                element.style.height = 'auto';
                element.style.height = element.scrollHeight + this.offset + 'px';
            },
        },
        mounted() {
            // get the element
            let element = this.$refs.textArea;
            // set original offset 
            this.offset = element.offsetHeight - element.clientHeight;
            // add event listener for the the content changes
            element.addEventListener('input', (event) => this.autoResize(event.target));
            // set the size for the inital state
            this.autoResize(element);
        },
        computed: {
            id() {
                return this.field.propertyName;
            },
        }
    }
</script>
