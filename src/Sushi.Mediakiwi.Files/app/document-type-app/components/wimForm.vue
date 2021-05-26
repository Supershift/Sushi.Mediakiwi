<template>
    <section>
        <table class="formTable">
            <tbody>
                <tr v-for="row in rows" :key="row.id">
                    <template v-for="field in row.fields">                        
                        <v-container v-if="field.vueType === 'wimSection'">
                            <component :key="field.propertyName"
                                       v-bind:is="field.vueType === 'undefined' ? 'wimTextline' : field.vueType"
                                       v-bind:field="field"
                                       v-model="field.value"></component>
                        </v-container>
                        <v-container v-else
                                     :class="expressionClass(field.expression)"
                                     v-show="showField(field)">
                            <th :class="expressCell(field.expression)"><label :for="field.propertyName"><abbr style="display: block;white-space: nowrap;overflow: hidden;text-overflow: ellipsis;" :title="field.title" v-html="field.title"></abbr></label></th>
                            <td :class="expressCell(field.expression)">
                                <component :key="field.propertyName"
                                           v-bind:is="field.vueType === 'undefined' ? 'wimTextline' : field.vueType"
                                           v-bind:field="field"
                                           v-bind:classname="isHalfField(field.expression)"
                                           v-model="field.value"></component>
                            </td>
                        </v-container>
                    </template>
                </tr>
            </tbody>
        </table>
    </section>
</template>
<script>
    import { shared } from './../utils/shared';

    import wimText from './fields/wimText.vue';
    import wimChoiceDropdown from './fields/wimChoiceDropdown.vue';
    import wimButton from './fields/wimButton.vue';
    import wimRichText from './fields/wimRichText.vue';
    import wimPlus from './fields/wimPlus.vue';
    import wimTextline from './fields/wimTextline.vue';
    import wimTag from './fields/wimTag.vue';
    import wimChoiceCheckbox from './fields/wimChoiceCheckbox.vue';
    import wimTagVue from './fields/wimTagVue.vue';
    import wimTextArea from './fields/wimTextArea.vue';
    import wimChoiceRadio from './fields/wimChoiceRadio.vue';
    import wimDateTime from './fields/wimDateTime.vue';
    import wimDate from './fields/wimDate.vue';
    import wimSection from './fields/wimSection.vue';
    import wimNameValue from './fields/wimNameValue.vue';

    export default {
        props: ["fields", "render", "notifications"],
        mixins: [shared],
        components: {
            wimText,
            wimChoiceDropdown,
            wimButton,
            wimRichText,
            wimPlus,
            wimTextline,
            wimTag,
            wimTagVue,
            wimChoiceCheckbox,
            wimTextArea,
            wimChoiceRadio,
            wimDateTime,
            wimDate,
            wimSection,
            wimNameValue,
        },
        data() {
            return {
                // Corresponding to the mediakiwi enum
                outputExpression: Object.freeze({
                    FullWidth: 0,
                    Left: 1,
                    Right: 2,
                    Alternating: 3
                }),
                dummyPlusField: {
                    propertyName: 'tjesting',
                }
            }
        },
        methods: {
            isHalfField(expression) {
                let _class = "";
                if (expression !== this.outputExpression.FullWidth)
                    _class += " half";
                else
                    _class += " long";

                return _class;
            },
            expressCell(expression) {
                let _class = "";
                if (expression !== this.outputExpression.FullWidth)
                    _class += "vhalf";
                else
                    _class += "full";

                return _class;
            },
            expressionClass(expression) {
                let _class = this.isHalfField(expression);

                switch (expression) {
                    case 1:
                        _class += " left";
                        break;
                    case 2:
                        _class += " right";
                        break;
                }
                return _class;
            },

            showField(field) {
                if (field.hidden)
                    return false;

                if (field.vueType === "wimSection")
                    return true;

                if (!field.visible)
                    return false;

                return true;
            },
            postFields: function ($event, target) {
                if ($event.target.id && $event.target.id === 'IsSharedField') {
                    if (this.$parent.checkSharedField) {
                        this.$parent.checkSharedField($event.target.checked);
                    }
                }
                if (this.$parent.postFields) {
                    this.$parent.postFields($event, target);
                }
            },
            hide(buttons) {
                for (let button of buttons) { 
                    if (button.section == 0)
                        return "";
                }
                return 'hidden';
            },
            clean(label) {
                return label.replace(/(?:^\w|[A-Z]|\b\w)/g, function (word, index) {
                    return index == 0 ? word.toLowerCase() : word.toUpperCase();
                }).replace(/\s+/g, '');
            },
            removeFields(section) {
                let sectionFields = this.fields.filter((f) => { return f.formSection === section });
                this.$parent.removeFields(sectionFields);
            },
            addFields(section) {
                this.$parent.addFields(section);
            },
            toggle(section) {
                this.$parent.toggle(section);
            },
        },
        computed: {
            rows() {
                let rows = [];
                // rows have an array of fields with max. 2 items
                // one left one right
                let row = {
                    fields: []
                };
                if (!this.fields || !this.fields.length)
                    return rows;

                let currentFormSection = 'baseSection';
                for (let field of this.fields) {
                    if (!field.formSection) {
                        if (field.vueType === 'wimSection') {
                            currentFormSection = `${this.clean(field.title)}_${this.fields.indexOf(field)}`;
                        }
                        field.formSection = currentFormSection;
                    }

                    // Check if there is a notification for this field
                    if (this.notifications && this.notifications.length) {
                        let notification = this.notifications.find((f) => { return f.propertyName === field.propertyName })
                        field.error = notification;
                    }

                    // is this a full width field?
                    if (field.expression === this.outputExpression.FullWidth) {
                        // add the current row object to the list
                        if (row.fields.length)
                            rows.push(row);
                        // create a new row
                        row = {
                            fields: []
                        };
                        // add push it to the list
                        row.fields.push(field);
                    }
                    else {
                        // add to 'current' row
                        row.fields.push(field);
                    }
                    // add row to rows
                    if (field.expression === this.outputExpression.FullWidth || row.fields.length === 2) {
                        if (row.fields.length)
                            rows.push(row);
                        row = {
                            fields: []
                        };
                    }
                }
                // add last row
                if (row.fields.length > 0)
                    rows.push(row);

                return rows;
            },
        }
    }
</script>
