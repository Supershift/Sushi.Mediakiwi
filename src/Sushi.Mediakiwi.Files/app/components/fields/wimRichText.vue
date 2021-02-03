<template>
    <v-container :class="expressionClass(field.expression)">
        <th :class="expressCell(field.expression)"><label :for="field.propertyName"><abbr style="display: block;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;" v-html="field.title"></abbr></label></th>
        <td :class="expressCell(field.expression)">
            <div :class="isHalfField(field.expression)">
                <tinymce-editor v-model="field.value"
                                :name="field.propertyName"
                                :id="field.propertyName"
                                :init="tinymce"
                                :class="fieldClass(field)"
                                v-on="eventHandler(field, 'handleChange')"
                                ></tinymce-editor>
            </div>
        </td>
    </v-container>
</template>
<script>
    import { shared } from './../../shared';
    import Editor from '@tinymce/tinymce-vue';

    export default {
        props: ['field'],
        mixins: [shared],
        components: {
            'tinymce-editor': Editor
        },
        data() {
            return {
                tinymce: {
                    menubar: false,
                    statusbar: false,
                    plugins: "code link",
                    toolbar: "bold italic underline bullist numlist indent outdent link unlink removeformat subscript superscript code",
                }
            }
        },
        methods: {
            handleChange(editor) {
                console.log('triggerChange:RTE');
                console.log(this.field.propertyName);
                this.$parent.postFields(undefined, this.field.propertyName);
            }
        },
        mounted() {
        },
        computed: {
            id() {
                return this.field.propertyName;
            },
            value() {
                return this.field.value;
            }
        }
    }
</script>
