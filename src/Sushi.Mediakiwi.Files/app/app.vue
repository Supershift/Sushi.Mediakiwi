<template>
    <article id="homeArticle">
        <section id="pageHeaderV2" class="pageHeader">
            <header>
                <h1>
                    <v-container v-html="form.listTitle"></v-container>
                    <a v-if="form.listSettingsUrl" :href="form.listSettingsUrl" class="flaticon icon-gears"></a>
                </h1>
            </header>
            <article>
                <p v-html="form.listDescription"></p>
            </article>
        </section>
        <wim-form :form="form" :render="render"></wim-form>
    </article>
</template>
<script>
    import { shared } from './shared';
    import wimForm from './components/wimForm.vue';

    export default {
        data() {
            return {
                listTitle: "",
                form: {
                    fields: [],
                    buttons: [],
                },
                grid: {
                    rows: []
                },
                render: true,
            }
        },
        mixins: [shared],
        components: {
            wimForm,
        },
        methods: {
            renderForm(mediakiwiResponse) {
                if (typeof (mediakiwiResponse) !== "object") {
                    console.error(mediakiwiResponse); // response isn't right; log it as an error
                }

                if (mediakiwiResponse && mediakiwiResponse.fields) {
                    for (let field of mediakiwiResponse.fields) {
                        if (field) {
                            if (field.propertyName)
                                field.disabled = 0;
                            field.visible = true;
                            //field.formSection = "";
                            //field.canToggleSection = true;                            
                            //field.toggleDefaultClosed = true;
                            //field.canDeleteSection = true;
                            //field.canSort = true;
                            //field.isFixed = true; 
                        }
                    }

                    this.form = mediakiwiResponse;
                    //this.render = false;

                    setTimeout(() => {
                        if (this.form && this.form.fields) {
                            // manually (re)set the value of the RTEs
                            for (let rte of this.form.fields.filter((f) => { return f.vueType == 'wimRichText' })) {
                                if (tinymce.editors[rte.propertyName]) {
                                    tinymce.editors[rte.propertyName].setContent(rte.value ? rte.value : "");
                                }
                            }

                        }
                        // ellipsis the dirty way
                        $("abbr").tipTip({ maxWidth: "auto", edgeOffset: 10 });
                    });
                }

                this.$nextTick(function () {
                    // Code that will run only after the entire view has been rendered
                    this.render = false;
                });
            },
            async fetchFields() {
                let list = this.$route.query.list;
                if (list) {
                    let headers = this.headers;
                    await this.$http.get(this.uriForm, { headers }).then((r) => {
                        this.renderForm(r.data);
                    });
                }
            },
            async postFields($event, target) {
                if (!target)
                    target = $event.target.id;

                let request = {
                    Referrer: target,
                    FormFields: {}
                };

                // loop through all form fields and get the value
                for (let field of this.form.fields) {
                    if (field && field.propertyName) {
                        field.disabled = 1;
                        this.render = true;
                        request.FormFields[field.propertyName] = field.value;
                    }
                }

                let headers = this.headers;
                await this.$http.post(this.uriForm, JSON.stringify(request), { headers }).then((result) => {
                    this.renderForm(result.data);
                    // scroll to top?!
                }, (r) => {
                    if (r.status === 302) {
                        window.location.assign(r.body.redirectUrl);
                    }
                    else {
                        console.error(r);
                    }
                });
            },
            async removeFields(fields) {
                for (let f of fields) {
                    let index = this.form.fields.indexOf(f);
                    this.form.fields.splice(index, 1);
                }
            },
            async addFields(section) {
                let request = {
                    section: section
                };
                let headers = this.headers;
                await this.$http.post(this.uriForm, JSON.stringify(request), { headers }).then((result) => {
                    this.renderForm(result.data);
                }, (r) => {
                    if (r.status === 302) {
                        //
                    }
                    else {
                        console.error(r);
                    }
                });
            },
            toggle(section) {
                let sectionFields = this.form.fields.filter((f) => { return f.formSection === section });
                for (let field of sectionFields) {
                    field.visible = !field.visible;
                }
            },
            array_move(arr, old_index, new_index) {
                if (new_index >= arr.length) {
                    var k = new_index - arr.length + 1;
                    while (k--) {
                        arr.push(undefined);
                    }
                }
                arr.splice(new_index, 0, arr.splice(old_index, 1)[0]);
            },
            sortUp(section, index) {
                let sectionFields = this.form.fields.filter((f) => { return f.formSection === section });


                for (let sectionField of sectionFields) {
                    let i = this.form.fields.indexOf(sectionField);

                    this.array_move(this.form.fields, i, index);
                    index++;
                }

            },
            getSectionAboveIndex(section) {
                // find first field of the current section
                let firstSectionFieldIndex = this.form.fields.findIndex((f) => { return f.formSection === section });

                if (firstSectionFieldIndex > -1) {
                    // find all fields above the currenct section
                    let fieldsAbove = [...this.form.fields];
                    fieldsAbove = fieldsAbove.slice(0, firstSectionFieldIndex);

                    if (fieldsAbove.length) {
                        // get the last field of all fields above
                        let lastItem = fieldsAbove[fieldsAbove.length - 1];

                        if (lastItem) {
                            // find the first item of the section
                            let firstItem = this.form.fields.find((f) => { return f.formSection === lastItem.formSection });

                            // are we allowed to move over the item?
                            if (firstItem && firstItem.formSection && !firstItem.isFixed)
                                return this.form.fields.indexOf(firstItem);
                        }
                    }
                }
                return "";
            },
            sortDown(section, index) {
                let sectionFields = this.form.fields.filter((f) => { return f.formSection === section });

                if (index > -1) {
                    for (let sectionField of sectionFields) {
                        let i = this.form.fields.indexOf(sectionField);
                        this.array_move(this.form.fields, i, index);
                    }
                }
            },
            getSectionBelowIndex(section) {
                // find first field of the current section
                let firstSectionFieldIndex = this.form.fields.findIndex((f) => { return f.formSection === section });

                if (firstSectionFieldIndex > -1) {
                    // find the last item of the current section
                    while (this.form.fields[firstSectionFieldIndex].formSection === section && firstSectionFieldIndex < this.form.fields.length - 1) {
                        firstSectionFieldIndex++;
                    }

                    // get the first item of the nextsection
                    let firstItem = this.form.fields[firstSectionFieldIndex];

                    if (firstItem) {
                        if (firstSectionFieldIndex > -1) {
                            // find the index of the last item in the section
                            while (this.form.fields[firstSectionFieldIndex].formSection === firstItem.formSection && firstSectionFieldIndex < this.form.fields.length - 1) {
                                firstSectionFieldIndex++;
                            }
                            firstSectionFieldIndex--;

                            // find the last item of the section
                            let lastItem = this.form.fields[firstSectionFieldIndex];
                            // are we allowed to move over the item?

                            if (lastItem && lastItem.formSection && !lastItem.isFixed)
                                return firstSectionFieldIndex;
                        }
                    }

                }
                return "";
            },
            getUrl: function () {
                let list = this.$route.query.list;
                let item = this.$route.query.item;

                let url = '?list=' + list;
                if (item)
                    url += '&item=' + item;
                return url;
            },
        },
        mounted: function () {
            this.fetchFields();
        },
        computed: {
            uriForm() {
                return this.getUrl();
            },
            headers() {
                return {
                    'Content-Type': 'application/json'
                };
            }
        }
    }
</script>