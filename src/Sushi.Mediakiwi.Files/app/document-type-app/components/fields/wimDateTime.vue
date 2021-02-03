<template>
    <div :class="classname">
        <label v-if="prefix(field)" v-html="prefix(field)"></label>
        <input :name="id(field.propertyName, 'date')"
               :id="id(field.propertyName, 'date')"
               type="text"
               class="date datepicker"
               v-bind:class="fieldClass(field)"
               placeholder="dd-mm-yyyy"
               :value="date" 
               v-bind:disabled="field.disabled || field.readOnly" />

        <input :name="id(field.propertyName, 'time')"
               :id="id(field.propertyName, 'time')"
               type="text"
               class="time timepicker"
               v-bind:class="fieldClass(field)"
               placeholder="00:00"
               :value="time" 
               v-bind:disabled="field.disabled || field.readOnly" />
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

                //this.handleChange();
            },
            onCloseTime(time) {
                let date = undefined;
                if (this.field.value && this.field.value.split(' ')[0]) {
                    date = `${this.field.value.split(' ')[0]}`;
                }

                if (date)
                    this.field.value = `${date} ${time}`;
                else
                    this.field.value = `${time}`;

                //this.handleChange();
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
                $(`#${this.field.propertyName}_time`).timepicker({
                    dateFormat: 'HH-MM',
                    onClose: this.onCloseTime
                });
            }
        },
        mounted() {
            this.init();
        },
        computed: {
            date() {
                return this.field.value ? this.field.value.split(' ')[0] : undefined;
            },
            time() {
                return this.field.value ? this.field.value.split(' ')[1] : undefined;
            },
            value() {
                let v = `${this.date}`;

                if (this.time)
                    v += ` ${this.time}`;

                return v;
            }
        },
        watch: {
            field() {
                this.init();
            }
        }
    }
</script>