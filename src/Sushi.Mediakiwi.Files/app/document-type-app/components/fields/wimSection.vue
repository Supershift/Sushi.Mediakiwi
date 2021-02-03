<template>
    <v-container class="full long">
        <th colspan="2" class="sectionHeader">
            <span>
                <h3 v-html="field.title" @click="toggle($event)" v-bind:class="{'toggle' : field.canToggleSection}"></h3>
                <span>
                    <a href="" v-if="field.canDeleteSection" @click="removeFields($event)" class="list-btn icon-times"></a>
                    <a href="" v-if="field.canToggleSection" @click="toggle($event)" class="list-icon" v-bind:class="getToggleClass()"></a>
                </span>
            </span>
        </th>
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
            sortUp($event) {
                $event.preventDefault();
                console.log(`sortUp:${this.field.formSection}`);
                this.$parent.sortUp($event, this.field.formSection);
            },
            sortDown($event) {
                $event.preventDefault();
            },
            removeFields($event) {
                $event.preventDefault();
                if (this.field.canDeleteSection) {

                    this.$dialog.confirm('Are you sure you want to delete this section')
                        .then((dialog) => {
                            console.log(`remove:${this.field.formSection}`);
                            this.$parent.removeFields(this.field.formSection);
                        });;
                }
                return false;
            },
            toggle($event) {
                if ($event)
                    $event.preventDefault();

                if (this.field.canToggleSection) {
                    console.log(`toggle:${this.field.formSection}`);
                    this.$parent.toggle(this.field.formSection);
                }
            },
            getToggleClass() {
                // VISIBLE
                if (typeof (this.field.visible) === "undefined" || this.field.visible) {
                    return "icon-angle-up";
                }
                else {
                    return "icon-angle-down";
                }
            }
        },
        created() {
            if (this.field.toggleDefaultClosed) {
                this.toggle();
            }
        },
        mounted() {
        },
        computed: {
        }
    }
</script>
