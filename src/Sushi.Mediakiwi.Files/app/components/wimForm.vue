<template>
    <v-container>
        <section id="iconBarz" class="component iconBar">
            <div class="footer" v-bind:class="hide(form.buttons)">
                <ul>
                    <li class="laster">
                        <ul>
                            <li v-for="button in form.buttons" v-if="button.section == 0" :key="button.id">
                                <component v-bind:is="button.vueType"
                                           v-bind:field="button"></component>
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
        </section>
        <wim-notifications v-if="form.notifications" :notifications="form.notifications"></wim-notifications>
        <section id="formStylesv2" class="component forms">
            <div class="loader" id="loader" style="display:block" v-show="render"></div>
            <div class="container">
                <div class="OrderingField">
                    <table class="formTable">
                        <tbody>
                            <tr v-for="row in rows" v-if="form.isEditMode" :key="row.id">
                                <template v-for="field in row.fields">
                                    <!--<wim-plus :field="dummyPlusField" v-if="field.vueType === 'wimSection'"></wim-plus>-->
                                    <component v-show="showField(field)"
                                               :key="field.propertyName"
                                               v-bind:is="field.vueType === 'undefined' ? 'wimTextline' : field.vueType"
                                               v-bind:field="field"
                                               v-model="field.value"></component>
                                </template>
                            </tr>
                            <tr v-for="row in rows" v-if="!form.isEditMode" :key="row.id">
                                <v-container v-for="field in row.fields" :key="field.id">
                                    <wim-section v-if="field.vueType === 'wimSection'"
                                                 v-show="!field.hidden"
                                                 v-bind:field="field"></wim-section>
                                    <wim-textline v-else
                                                  v-show="!field.hidden"
                                                  v-bind:field="field"></wim-textline>
                                </v-container>
                            </tr>
                        </tbody>
                    </table>
                    <footer>
                        <span class=""> </span>
                        <v-container v-for="button in form.buttons" v-if="button.section == 1" :key="button.id">
                            <component v-bind:is="button.vueType"
                                       v-bind:field="button"></component>
                        </v-container>
                    </footer>
                    <br class="clear">
                </div>
            </div>
        </section>
    </v-container>
</template>
<script>
    import { shared } from './../shared';
    import wimNotifications from './wimNotifications.vue';
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

    export default {
        props: ["form", "render"],
        mixins: [shared],
        components: {
            wimNotifications,
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
        },
        data() {
            return {
                dummyPlusField: {
                    propertyName: 'tjesting',
                }
            }
        },
        methods: {
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
                this.$parent.postFields($event, target);
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
                let sectionFields = this.form.fields.filter((f) => { return f.formSection === section });
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
                if (!this.form || !this.form.fields || !this.form.fields.length)
                    return rows;

                let currentFormSection = 'baseSection';
                for (let field of this.form.fields) {
                    if (!field.formSection) {
                        if (field.vueType === 'wimSection') {
                            currentFormSection = `${this.clean(field.title)}_${this.form.fields.indexOf(field)}`;
                        }
                        field.formSection = currentFormSection;
                    }

                    // Check if there is a notification for this field
                    if (this.form.notifications && this.form.notifications.length) {
                        let notification = this.form.notifications.find((f) => { return f.propertyName === field.propertyName })
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
