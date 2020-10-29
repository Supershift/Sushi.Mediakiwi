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
                       :disabled="field.disabled == 1"
                       :value="date" />
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