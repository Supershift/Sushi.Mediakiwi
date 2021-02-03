<template>
    <div :class="classname">
        <tinymce-editor v-model="field.value"
                        :name="field.propertyName"
                        :id="field.propertyName"
                        :init="tinymce"
                        :class="fieldClass(field)"
                        :disabled="field.disabled || field.readOnly" 
                        v-on="eventHandler(field, 'handleChange')"
                        ></tinymce-editor>
    </div>
</template>
<script>
    import { shared } from './input';
    import Editor from '@tinymce/tinymce-vue';

    export default {
        props: ['field', 'classname'],
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
