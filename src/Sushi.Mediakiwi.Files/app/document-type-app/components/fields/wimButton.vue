<template>
    <div>
        <a href="#"
           :id="field.propertyName"
           :title="field.title"
           :data-title="field.title"
           :class="buttonClass(field)"
           v-html="field.title"
           :disabled="field.disabled || field.readOnly" 
           v-on="eventHandler(field, 'Event')"></a>
    </div>
</template>
<script>
    import { shared } from './input';

    export default {
        props: ['field'],
        mixins: [shared],
        data() {
            return {

            }
        },
        methods: {
            buttonClass(button) {
                let _class = "abbr submit";
                if (button.className)
                    _class += ` ${button.className}`;
                return _class;
            },
            Event($event) {
                $event.preventDefault();
                this.$parent.postFields($event);
                // scroll top
                document.querySelector("#bodySection").scrollTop = 0
                return false;
            },
        },
        computed: {
        }
    }
</script>