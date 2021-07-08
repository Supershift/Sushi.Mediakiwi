<template>
    <div :class="classname">
        <label v-if="prefix(field)" v-html="prefix(field)"></label>
        <input :name="id(field.propertyName, 'date')"
               :id="id(field.propertyName, 'date')"
               type="text"
               class="date datepicker"
               v-bind:class="fieldClass(field)"
               placeholder="dd-mm-yyyy"
               v-bind:disabled="field.disabled || field.readOnly"
               :value="date" />
        <label v-if="suffix(field)" v-html="suffix(field)"></label>
    </div>
</template>
<script>
    import { shared } from './input';

    export default {
        props: ['field', 'classname'],
        components: {

        },
        data() {
            return {
                //nl: nl
            }
        },
        mixins: [shared],
        methods: {
            handleChange() {
                console.log('datetime');
                this.$parent.postFields(undefined, this.field.propertyName);
            },
            onCloseDate(date) {
                let time = undefined;
                if (this.field.value && this.field.value.split(' ')[1]) {
                    time = `${this.field.value.split(' ')[1]}`;
                }

                if (time)
                    this.field.value = `${date} ${time}`;
                else
                    this.field.value = `${date}`;

                this.handleChange();
            },
            id(propertyName, suffix) {
                return `${propertyName}_${suffix}`;
            },
            init() {
                $(`#${this.field.propertyName}_date`).datepicker({
                    dateFormat: 'dd-mm-yy',
                    onClose: this.onCloseDate,
                    changeMonth: true,
                    changeYear: true,
                });
            }
        },
        mounted() {
            this.init();
        },
        computed: {
            date() {
                return this.field.value ? this.field.value : undefined;
            },
            value() {
                return `${this.date}`;
            }
        },
        watch: {
            field() {
                $(`#${this.field.propertyName}_date`).datepicker("destroy");
                this.init();
            }
        }
    }
</script>