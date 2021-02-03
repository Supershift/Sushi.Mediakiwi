<template>
    <div :class="classname">
        <label v-if="prefix(field)" v-html="prefix(field)"></label>
        <vue-tags-input v-model="tag"
                        :tags="tags"
                        :autocomplete-min-length="0"
                        :autocomplete-items="filteredItems"
                        @tags-changed="newTags => tags = newTags"
                        v-bind:disabled="field.disabled || field.readOnly"  />
        <label v-if="suffix(field)" v-html="suffix(field)"></label>
    </div>
</template>
<script>
    import { shared } from './input';
    import VueTagsInput from '@johmun/vue-tags-input';


    export default {
        props: ['field', 'classname'],
        mixins: [shared],
        components: {
            VueTagsInput
        },
        data() {
            return {
                tag: "",
                tags: [],
                render: false
            }
        },
        created: function () {
            this.setTags();
            this.setValue(this.tags);

            // start watching after initial set
            this.$watch('tags', (newValue, oldValue) => {
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
                    //
                    this.setValue(newValue);
                };
                this.valueChanged();
            });
        },
        watch: {
        },
        methods: {
            valueChanged() {
                console.log('triggerChange:wimTagVue');
                this.$emit('input', this.field.value);
                this.$parent.postFields(undefined, this.field.propertyName);
            },
            setValue(tags) {
                this.field.value = tags.map((tag) => {
                    return tag.text;
                });
            },
            setTags() {
                this.tags = this.field.value.map((v) => {
                    let option = this.field.options.find((o) => o.value === v);
                    return {
                        text: option.text,
                        tiClasses: ["ti-valid"]
                    };
                });
            }
        },
        computed: {
            filteredItems() {
                return this.autocompleteItems.filter(i => {
                    return i.text.toLowerCase().indexOf(this.tag.toLowerCase()) !== -1;
                });
            },
            autocompleteItems() {
                return this.field.options.filter((o) => o.text).map((option) => {
                    return {
                        text: option.text
                    };
                });
            },
        }
    }
</script>
