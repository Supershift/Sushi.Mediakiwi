<template>
    <v-container :class="expressionClass(field.expression)">
        <th :class="expressCell(field.expression)"><label :for="field.propertyName"><abbr style="display: block;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;" :title="field.title" v-html="field.title"></abbr></label></th>
        <td :class="expressCell(field.expression)">
            <div>
                <label v-if="prefix(field)" v-html="prefix(field)"></label>
                <select :name="field.propertyName"
                        :id="field.propertyName"
                        v-model="field.value"
                        :value="field.value"
                        tabindex="-1"
                        aria-hidden="true"
                        :class="errorClass(field)"
                        :disabled="field.disabled == 1"
                        ref="dropdown">
                </select>
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
        data() {
            return {
                value: "",
                select2data: [],
                name: '',
                disabled: null,
                width: '100%',
            }
        },
        mounted: function () {
            // prep the options
            this.formatOptions();
            // bind the select2
            this.bind();
        },
        watch: {
            field() {
                this.rebind();
            },
            options: {
                handler: function (newValue, oldValue) {
                    if (newValue !== oldValue) {
                        this.rebind();
                    }
                },
                deep: true
            },
            value: function (newValue, oldValue) {
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
                    let select = $("select#" + this.field.propertyName);
                    select.val(newValue).trigger('change');
                    this.valueChanged();
                }
            },
        },
        methods: {
            formatOptions: function () {
                // empty the data array
                this.select2data = [];
                // loop through the options
                for (let option of this.field.options) {
                    // set disabled default to false
                    // add the option with the right value, text and disabled state
                    // let optionValue =  ? option.value : 0;
                    let item = { id: option.value, text: option.text, disabled: false, selected: option.value === this.field.value };
                    this.select2data.push(item);
                }
            },
            bind: function (value) {
                // get the element as a property
                let select = $("select#" + this.field.propertyName);
                // empty all options, otherwise updating the values won't remove them
                select.empty();
                // set the select2 on the element
                select.select2({
                    width: this.width,
                    data: this.select2data,
                    placeholder: '',
                    escapeMarkup: function (text) {
                        // this allows special characters in the option label
                        return text;
                    },
                    allowClear: true,
                }).on('change', () => {
                    this.value = select.val();
                });

                // if there is a disabled method preset, run it
                if (this.disabled) {
                    // if its a boolean, set the boolean value
                    if (typeof (this.disabled) === "boolean") {
                        select.prop("disabled", this.disabled);
                    }
                    else {
                        // run the method
                        select.prop("disabled", this.disabled());
                    }
                }
            },
            rebind: function (value) {
                //$("select#" + this.field.propertyName).off().select2('destroy')
                // prep the options
                this.formatOptions();
                // (re) bind the select2
                this.bind(value);
            },
            valueChanged() {
                //console.log('trigger:valueChanged');
                this.$emit('input', this.$refs.dropdown.value);
                if (this.field.event === 'none') return;
                this.$parent.postFields(undefined, this.field.propertyName);
            },
        },
        destroyed: function () {

        }
    }
</script>
