<template>
    <v-container :class="expressionClass(field.expression)">
        <th :class="expressCell(field.expression)"><label :for="field.propertyName"><abbr style="display: block;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;" :title="field.title" v-html="field.title"></abbr></label></th>
        <td :class="expressCell(field.expression)">
            <div :class="isHalfField(field.expression)">
                <label v-if="prefix(field)" v-html="prefix(field)"></label>
                <input :name="id(field.propertyName, 'date')"
                       :id="id(field.propertyName, 'date')"
                       type="text"
                       class="date datepicker"
                       v-bind:class="fieldClass(field)"
                       placeholder="dd-mm-yyyy"
                       :value="date" :disabled="field.disabled == 1" />

                <input :name="id(field.propertyName, 'time')"
                       :id="id(field.propertyName, 'time')"
                       type="text"
                       class="time timepicker"
                       v-bind:class="fieldClass(field)"
                       placeholder="00:00"
                       :value="time" :disabled="field.disabled == 1" />
                <label v-if="suffix(field)" v-html="suffix(field)"></label>
            </div>
        </td>
    </v-container>

</template>
<script>
    import { shared } from './../../shared';

    export default {
        props: ['field'],
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