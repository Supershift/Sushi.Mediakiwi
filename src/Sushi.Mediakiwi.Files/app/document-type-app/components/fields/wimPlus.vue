<template>
    <v-container style="width: inherit; display: flex; margin: 10px 0;">
        <a href="#" class="plusBtn icon-plus" @click="addSection($event)" style="margin: 0 auto;" :id="field.propertyName"></a>
    </v-container>
</template>
<script>
    import { shared } from './input';

    export default {
        props: ['field'],
        mixins: [shared],
        data() {
            return {
                value: "",
            }
        },
        methods: {
            valueChanged($event) {
                console.log('triggerChange:wimPlus');
                this.$parent.postFields($event);                
            },
            addSection($event) {
                // what element to add
                this.$parent.addFields($event, this.field); 
            },
        },
        mounted() {
            this.value = this.field.value;
        },
        computed: {
            isHalf() {
                return this.field.expression !== this.outputExpression.FullWidth;
            },
            id() {
                return this.field.propertyName;
            },
        }
    }
</script>
