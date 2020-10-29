<template>
    <v-container :class="expressionClass(field.expression)">
        <th :class="expressCell(field.expression)"><label :for="field.propertyName"><abbr style="display: block;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;" :title="field.title" v-html="field.title"></abbr></label></th>
        <td :class="expressCell(field.expression)">
            <div :class="isHalfField(field.expression)">
                <label v-if="prefix(field)" v-html="prefix(field)"></label>
                <Select2 :id="field.propertyName"
                         :name="field.propertyName"
                         v-model="value"
                         :settings="select2Options"
                         :options="select2data"
                         @change="valueChanged($event)"
                         v-bind:disabled="field.disabled == 1"
                         ref="select2"></Select2>
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
        computed: {
            select2data() {
                return this.field.options.map((option) => {
                    return {
                        id: option.value,
                        text: option.text,
                        disabled: false,
                        selected: option.value === this.field.value
                    }
                });
            }
        },
        data() {
            return {
                value: [],
                name: '',
                disabled: null,
                select2Options: {
                    width: '100%',
                    placeholder: '',
                    escapeMarkup: function (text) {
                        // this allows special characters in the option label
                        return text;
                    },
                    searchable: false,
                    allowClear: true,
                    multiple: true,
                    containerCssClass: this.errorClass(this.field),
                }
            }
        },
        watch: {
            field(newValue, oldValue) {
                // change to undefined if empty or null
                if (!newValue)
                    newValue = undefined;
                // change to undefined if empty or null
                if (!oldValue)
                    oldValue = undefined;
                // stringify the value (when multiple)
                let jnew = JSON.stringify(newValue);
                let jold = JSON.stringify(oldValue);
                // compare if values are the same
                if (jnew !== jold) {
                    this.Init();
                }
            },
        },
        methods: {
            Init() {
                this.$forceUpdate();  // Notice we have to use a $ here

                if (!this.field.value)
                    this.value = [];
                else
                    this.value = this.field.value
            },
            valueChanged(val) {
                console.log(`triggerChange:wimtag ${this.field.propertyName}`);
                this.$emit('input', val);
                this.$refs["select2"].select2.select2('destroy');

                if (this.field.event === 'none')
                    return;

                this.$parent.postFields(undefined, this.field.propertyName);
            },
        },
        mounted: function () {
            this.Init();
        },
    }
</script>
