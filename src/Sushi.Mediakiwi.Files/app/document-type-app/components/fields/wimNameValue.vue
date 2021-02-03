<template>
    <div :class="classname">
        <label v-if="prefix(field)" v-html="prefix(field)"></label>
        <section class="name-value" v-for="(nameValue, index) of field.value">
            <input type="text"
                   v-model="nameValue.text"
                   :name="field.propertyName + index"
                   :id="field.propertyName + index"
                   v-bind:disabled="field.disabled || field.readOnly"
                   class="long short">
            <input type="text"
                   v-model="nameValue.value"
                   :name="field.propertyName + index"
                   :id="field.propertyName + index"
                   v-bind:disabled="field.disabled || field.readOnly"
                   class="long short">

            <i class="delete far fa-times" @click="removeNameValuePair(nameValue)"></i>
        </section>
        <div style="width: inherit; display: flex; margin: 10px 0;">
            <a href="#" class="plusBtn small icon-plus" @click="addNameValuePair" style="margin: 0 auto;" :id="field.propertyName + '_add'"></a>
        </div>
        <label v-if="suffix(field)" v-html="suffix(field)"></label>        
    </div>
</template>
<script>
    import { shared } from './input';
    
    export default {
        name: 'wimNameValue',
        props: ['field', 'classname'],
        mixins: [shared],
        components: {
            
        },
        data() {
            return {

            }
        },
        methods: {
            addNameValuePair() {
                this.field.value.push({ text: '', value: '' });
            },
            removeNameValuePair(nameValuePair) {
                let index = this.field.value.findIndex(r => r.text === nameValuePair.text && r.value === nameValuePair.value);
                this.field.value.splice(index, 1);
            }
        },
        mounted() {
            
            if (!this.field.value) {
                this.field.value = [];
                this.addNameValuePair();
                
            }
        }
    }
</script>