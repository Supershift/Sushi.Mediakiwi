<template>
    <article class="documentTypeArticle">
        <wimNotifications :notifications="notifications"></wimNotifications>
        <section class="centerContainer">
            <section class="leftContainer">
                <article class="dataBlock" v-if="fieldsCollection">
                    <draggable v-model="fieldsCollection" class="selections" @end="onMoved">
                        <transition-group>
                            <div class="field" v-for="field in fieldsCollection" :key="field.id" @click="selectField(field)" v-bind:class="{'active' : selectedFieldID === field.id}" :data-key="field.id">
                                <div class="long">
                                    <input type="text"
                                           :id="'field_text_' + field.id"
                                           v-model="field.title"
                                           @click.stop
                                           @focus="selectField(field)"
                                           class="long"
                                           v-bind:class="getClass(field, 'Title')">
                                </div>
                                <div>
                                    <select v-model="field.contentTypeID"
                                            :id="'field_type_' + field.id"
                                            class="selectX"
                                            style="width:100%"
                                            @click.stop
                                            @focus="selectField(field)"
                                            v-bind:class="getClass(field, 'TypeID')"
                                            @change="changedType(field)">
                                        <option value="10">TextField</option>
                                        <option value="11">TextArea</option>
                                        <option value="12">RichText</option>
                                        <option value="9">TextLine</option>
                                        <option value="13">Date</option>
                                        <option value="15">DateTime</option>
                                        <option value="18">Dropdown</option>
                                        <option value="19">Image</option>
                                        <option value="20">Document</option>
                                        <option value="21">Hyperlink</option>
                                        <option value="22">PageSelect</option>
                                        <option value="23">FolderSelect</option>
                                        <option value="24">ListItemSelect</option>
                                        <option value="25">SubListSelect</option>
                                        <option value="28">Choice_Checkbox</option>
                                        <option value="38">MultiField</option>
                                        <option value="39">Sourcecode</option>
                                    </select>
                                </div>
                                <div class=" half">
                                    <input type="checkbox" :id="'field_ismandatory_' + field.id" :name="'field_text_' + field.id" v-model="field.isMandatory" class="radio  half long" @click.stop @focus="selectField(field)">
                                    <label @click.stop :for="'field_ismandatory_' + field.id">Required</label>
                                </div>
                                <div class="status">
                                    <transition name="fade">
                                        <i class="fas fa-pen edit" title="Edited" v-if="field.status === fieldStatus.edited"></i>
                                        <i class="fas fa-spinner-third spin" title="Saving" v-if="field.status === fieldStatus.saving"></i>
                                        <i class="fas fa-check-circle check" title="Saved" v-if="field.status === fieldStatus.saved"></i>
                                        <i class="far fa-exclamation-triangle warning" title="Warning" v-if="field.status === fieldStatus.invalid"></i>
                                    </transition>
                                    <i class="fas fa-retweet sharedField" title="Shared field" v-if="field.isSharedField"></i>
                                    <i class="fas fa-trash delete" title="Delete" @click.stop="deleteField(field.id)"></i>
                                    <i class="fas fa-bars sort" title="Move"></i>
                                </div>
                            </div>
                        </transition-group>
                    </draggable>
                </article>
                <v-container style="width: inherit; display: flex; margin: 10px auto;">
                    <a href="" onclick="return false" @click="addField" class="plusBtn icon-plus" style="margin: 0 auto;"></a>
                </v-container>
            </section>
            <section class="rightContainer">
                <div v-if="selectedField">
                    <wim-form :render="false" :fields="selectedField.fields"></wim-form>
                </div>
            </section>
        </section>
        <section class="bottomContainer">
            <a href="" onclick="return false" @click="saveAll" class="action submit" title="Save" v-bind:disabled="isSaving" v-bind:class="{'disabled': isSaving} ">Save document</a>
        </section>
    </article>
</template>
<script>
    import VuejsDialog from 'vuejs-dialog';
    import VuejsDialogMixin from 'vuejs-dialog/dist/vuejs-dialog-mixin.min.js'; // only needed in custom components
    import 'vuejs-dialog/dist/vuejs-dialog.min.css';
    import draggable from 'vuedraggable'

    import wimForm from './components/wimForm.vue';
    import wimNotifications from './components/wimNotifications.vue';

    export default {
        name: 'document-types',
        components: {
            wimForm,
            draggable,
            wimNotifications,
            VuejsDialog
        },
        data() {
            return {
                rootPath: rootPath,
                documentTypeID: documentTypeID,
                headers: {
                    'Content-Type': 'application/json'
                },
                fieldsCollection: [],
                selectedFieldID: undefined,

                dialogOptionsMessage: 'Are you sure?',
                dialogOptions: {
                    okText: 'Yes',
                    cancelText: 'No',
                },
                notifications: [],

                FormMap_Title: "Title",
                FormMap_IsMandatory: "IsMandatory",
                FormMap_TypeID: "ContentTypeID",

                fieldStatus: {
                    default: 0,
                    edited: 1,
                    saved: 2,
                    invalid: 3,
                    saving: 4,
                },

                loading: false,
                saveLoading: false,
                saveFieldLoading: false,
            }
        },
        computed: {
            isSaving() {
                return (this.saveLoading || this.saveFieldLoading);
            },
            formUri() {
                return window.location.href.split("#")[0];
            },
            //FIELD
            selectedField() {
                if (this.selectedFieldID === undefined || this.selectedFieldID === null)
                    return;

                return this.fieldsCollection.find(r => r.id === this.selectedFieldID);
            },
            // Left side fields
            leftFields() {
                if (!this.fieldsCollection || !this.fieldsCollection.length)
                    return [];

                return this.fieldsCollection.map(r => {
                    return {
                        id: r.id,
                        contentTypeID: r.contentTypeID,
                        title: r.title,
                        isMandatory: r.isMandatory,
                        sortOrder: r.sortOrder,
                        isSharedField: r.isSharedField,
                        fieldName: r.fieldName
                    }
                });
            },
            // Right side items
            rightFields() {
                if (!this.fieldsCollection || !this.fieldsCollection.length)
                    return [];

                let f = this.fieldsCollection.filter(r => r.fields && r.fields.length).map(r => r.fields);
                if (f)
                    f = [].concat.apply([], f);;

                if (!f || !f.length)
                    return [];

                return f.map(r => {
                    return {
                        id: r.id,
                        propertyName: r.propertyName,
                        value: JSON.stringify(r.value) // for the watch
                    }
                });
            },
        },
        watch: {
            // Left side items
            leftFields(value, oldvalue) {
                if (value && oldvalue) {
                    if (value.length !== oldvalue.length)
                        return;

                    for (let item of value) {
                        // fetch oldItem
                        let old = oldvalue.find(r => (r.id === item.id));
                        if (old) {
                            if (JSON.stringify(item) !== JSON.stringify(old)) {
                                let field = this.fieldsCollection.find(r => r.id == item.id);
                                if (field) {
                                    field.status = this.fieldStatus.edited;
                                }
                            }
                        }
                    }
                }
            },
            // Right side fields
            rightFields(value, oldvalue) {
                if (value && oldvalue) {
                    if (value.length !== oldvalue.length || !oldvalue.length)
                        return;

                    for (let item of value) {
                        // fetch oldItem
                        let old = oldvalue.find(r => (r.id === item.id && r.propertyName === item.propertyName));
                        if (old) {
                            if (JSON.stringify(item) !== JSON.stringify(old)) {
                                let field = this.fieldsCollection.find(r => r.id == item.id);
                                if (field && field.status === this.fieldStatus.default) {
                                    field.status = this.fieldStatus.edited;
                                }
                            }
                        }
                    }
                }
            },
        },
        methods: {
            // UTIL
            async changedType(field) {
                let newContent = await this.postContent(this.selectedField);
                this.fetchFieldPropertiesDone(newContent);
            },
            getClass(field, propertyName) {
                let _class = '';

                var cfield = this.fieldsCollection.find(r => r.id === field.id)
                if (cfield) {
                    let f = cfield.fields.find(r => r.id === field.id && r.propertyName === propertyName)
                    if (f)
                        _class = f.className;
                }
                return _class;
            },
            onMoved(evt) {
                console.log(evt);

                let movedItemID = evt.item.getAttribute('data-key');

                let newItemID = undefined;
                if (evt.item.nextSibling) {
                    newItemID = evt.item.nextSibling.getAttribute('data-key');
                    console.log(newItemID);
                }

                this.saveSortOrder(movedItemID, newItemID);
            },
            parseForm(form, existingFieldID) {
                if (form && form.fields) {        
                    for (let field of form.fields) {
                        if (field) {
                            // add field ID to content for future reference
                            if (existingFieldID)
                                field.id = existingFieldID;

                            if (field.propertyName)
                                field.disabled = false;

                            if (typeof (field.value) === "undefined")
                                field.value = "";

                            field.visible = true;

                            if (field.propertyName === this.FormMap_Title || field.propertyName === this.FormMap_TypeID || field.propertyName === this.FormMap_IsMandatory)
                                field.visible = false;
                        }
                    }
                }
                return form;
            },
            camelCase(str) {
                return str.replace(/(?:^\w|[A-Z]|\b\w)/g, function (word, index) {
                    return index == 0 ? word.toLowerCase() : word.toUpperCase();
                }).replace(/\s+/g, '');
            },
            getDialogMessage($event) {
                let options = Object.assign({}, this.dialogOptions);
                let message = this.dialogOptionsMessage;
                if ($event && $event.target) {
                    message = $event.target.getAttribute('data-confirm') ? $event.target.getAttribute('data-confirm') : this.dialogOptionsMessage;
                    options.okText = $event.target.getAttribute('data-confirm-y') ? $event.target.getAttribute('data-confirm-y') : options.okText;
                    options.cancelText = $event.target.getAttribute('data-confirm-n') ? $event.target.getAttribute('data-confirm-n') : options.cancelText;
                }

                return {
                    message: message,
                    options: options,
                }
            },
            // FIELD
            selectField(field, $event) {
                console.log('selectfield');
                this.selectedFieldID = field.id;

                // only fetch if not yet fetched
                if (!field.fields || !field.fields.length)
                    this.fetchFieldProperties();
            },
            getNewID() {
                // get all existing "new" items
                let existingNewItems = this.fieldsCollection.map(r => r.id);;//.filter(r => r.id < 0);

                let lowestID = 0;

                // get the lowest value
                if (existingNewItems && existingNewItems.length)
                    lowestID = Math.min.apply(Math, existingNewItems);

                if (lowestID > 0)
                    lowestID = 0;

                // decrese by 1
                lowestID--;
                return lowestID;
            },
            addField() {
                this.fieldsCollection.push({
                    id: this.getNewID(),
                    title: '',
                    contentTypeID: '',
                    isMandatory: false,
                    fields: [],
                    notifications: [],
                    status: this.fieldStatus.edited,
                    isSharedField: false,
                    fieldName: ''
                });
            },
            deleteField(fieldID) {
                // Get content
                let dialogContent = this.getDialogMessage();

                this.$dialog.confirm(dialogContent.message, dialogContent.options)
                    .then((dialog) => {
                        if (fieldID > 0) {
                            let uri = `${this.formUri}&field=${fieldID}`;
                            let headers = this.headers;
                            this.$http.delete(uri, { headers })
                                .then((r) => {
                                    this.deleteFieldDone(r);
                                }, (r) => {
                                    this.deleteFieldDone(r);
                                });
                        }
                        let index = this.fieldsCollection.findIndex(r => r.id === fieldID);
                        this.fieldsCollection.splice(index, 1);
                    });
            },
            deleteFieldDone(result) {
                if (result) {
                    if (typeof (result.redirectUrl) !== "undefined" && result.redirectUrl) {
                        window.location = result.redirectUrl;
                    }
                    this.notifications = result.notifications;
                }
            },
            // API
            // GENERAL
            async saveSortOrder(sortF, sortT) {
                let base = window.location;

                let uri = `${base}&sortF=${sortF}&sortT=${sortT}`
                await this.$http.get(uri).then(response => {
                    console.log('sorting applied');
                }, response => {
                    console.error(response);
                    console.log('sorting failed');
                });
            },
            async postContent(field) {
                this.loading = true;

                let request = {
                    Referrer: "",
                    FormFields: {}
                };

                // loop through all form fields and get the value

                for (let fieldProperty of field.fields) {
                    if (fieldProperty && fieldProperty.propertyName) {
                        request.FormFields[fieldProperty.propertyName] = fieldProperty.value;
                    }
                }

                // Add order
                let fieldID = (field.id < 0) ? 0 : field.id;
                request.FormFields["ID"] = fieldID;
                request.FormFields["SortOrder"] = field.sortOrder;

                request.FormFields["Title"] = field.title;
                request.FormFields["ContentTypeID"] = parseInt(field.contentTypeID);
                request.FormFields["IsMandatory"] = field.isMandatory;

                request.FormFields["FieldName"] = this.camelCase(request.FormFields["Title"]);

                let headers = this.headers;
                let uri = window.location.href;
                if (fieldID !== undefined)
                    uri = `${this.formUri}&field=${fieldID}`;

                let result = undefined;
                await this.$http.post(`${uri}`, request, { headers }, {
                    before(request) {
                        if (this.previousFecthContentRequest) {
                            this.previousFecthContentRequest.abort();
                        }
                        this.previousFecthContentRequest = request;
                    }
                }).then(response => {
                    // success
                    if (typeof (response.data) === "string")
                        result = JSON.parse(response.data);
                    else
                        result = response.data;

                    this.loading = false;
                }, response => {
                    // error
                    if (response && response.status)
                        this.loading = false;
                });
                return result;
            },

            async fetchContent(fieldID) {
                this.loading = true;

                let result = undefined;
                let uri = window.location.href;

                if (fieldID) {
                    if (fieldID < 0)
                        fieldID = 0;
                }

                if (fieldID !== undefined)
                    uri = `${this.formUri}&field=${fieldID}`;

                let headers = this.headers;
                await this.$http.get(uri, { headers }, {
                    before(request) {
                        if (this.previousFecthContentRequest) {
                            this.previousFecthContentRequest.abort();
                        }
                        this.previousFecthContentRequest = request;
                    }
                }).then(response => {
                    // success
                    if (typeof (response.data) === "string")
                        result = JSON.parse(response.data);
                    else
                        result = response.data;

                    this.loading = false;
                }, response => {
                    // error
                    if (response && response.status)
                        this.loading = false;
                });
                return result;
            },
            async saveAll() {
                // save the fields that have been changed
                let allSuccess = true;

                for (let field of this.fieldsCollection.filter(r => (r.status === this.fieldStatus.edited || r.id <= 0))) {
                    let succuess = await this.saveFieldProperties(field);
                    if (!succuess)
                        allSuccess = false;
                }

                if (allSuccess) {
                    // save at the end
                    await this.saveDocumentType();

                    // fetch all the fields
                    let openAtIndex = this.fieldsCollection.findIndex(r => r.id === this.selectedFieldID);
                    this.init(openAtIndex);
                }
            },
            // document type
            async saveDocumentType() {
                let target = "Save";

                let request = {
                    Referrer: target,
                    FormFields: {}
                };

                let success = undefined;
                let headers = this.headers;
                this.saveLoading = true;
                await this.$http.post(`${this.formUri}`, JSON.stringify(request), { headers })
                    .then((r) => {
                        this.saveLoading = false;
                        success = this.saveDocumentTypeDone(r.data);
                    }, (r) => {
                        this.saveLoading = false;
                        success = this.saveDocumentTypeDone(r.data);
                    });
                return success;
            },
            saveDocumentTypeDone(result) {
                if (this.notifications)
                    return this.notifications.filter(r => r.isError).length === 0
                return true;
            },
            // field
            async fetchAllFields() {
                this.loading = true;

                let request = {
                    documentTypeID: this.documentTypeID
                };
                let result = undefined;
                await this.$http.post(`${this.rootPath}/api/documentype/getfields`, request, {
                    before(request) {
                        if (this.previousFetchFieldsRequest) {
                            this.previousFetchFieldsRequest.abort();
                        }
                        this.previousFetchFieldsRequest = request;
                    }
                }).then(response => {
                    // success
                    if (typeof (response.data) === "string")
                        result = JSON.parse(response.data);
                    else
                        result = response.data;

                    this.loading = false;
                }, response => {
                    // error
                    if (response && response.status)
                        this.loading = false;
                });
                return result;
            },
            async fetchFieldProperties() {
                let contentResult = await this.fetchContent(this.selectedFieldID);
                this.fetchFieldPropertiesDone(contentResult);
            },
            fetchFieldPropertiesDone(contentResult) {
                let i = this.fieldsCollection.findIndex(r => r.id === this.selectedFieldID);
                let form = this.parseForm(contentResult, this.selectedFieldID);

                this.fieldsCollection[i].fields = form.fields;
                this.fieldsCollection[i].notifications = form.notifications;
            },
            async checkSharedField(isChecked) {
                this.loading = true;

                let request = {
                    fieldName: this.selectedField.fieldName,
                    isChecked: isChecked
                };
                let result = undefined;

                await this.$http.post(`${this.rootPath}/api/documentype/checksharedfield`, request, {
                    before(request) {
                        if (this.previousFetchFieldsRequest) {
                            this.previousFetchFieldsRequest.abort();
                        }
                        this.previousFetchFieldsRequest = request;
                    }
                }).then(response => {
                    // success
                    if (typeof (response.data) === "string")
                        result = JSON.parse(response.data);
                    else
                        result = response.data;

                    // Check if we have an impact on other pages,
                    // if so, display a dialog box about that
                    if (result.pages && result.pages.length > 0) {
                        console.log(result.pages);
                        // Create page list
                        var ul = $('<ul/>');
                        for (var i = 0; i < result.pages.length; i++) {
                            var li = $('<li/>').html(result.pages[i].pagePath);
                            ul.append(li);
                        }

                        // Enabling shared field
                        if (isChecked) {
                            this.$dialog.alert(`<h2>Caution</h2>When enabling shared field '<i> ${this.selectedField.fieldName}</i>', the following pages will also be updated: ${ul[0].outerHTML}`, { html: true });
                        }
                        else {
                            this.$dialog.alert(`<h2>Caution</h2>When disabling shared field '<i> ${this.selectedField.fieldName}</i>', the following pages will also be updated: ${ul[0].outerHTML}`, { html: true });
                        }
                    }

                    this.loading = false;
                }, response => {
                    // error
                    if (response && response.status)
                        this.loading = false;
                });

            },
            async saveFieldProperties(field) {
                let target = "Save";

                let request = {
                    Referrer: target,
                    FormFields: {} 
                };

                // loop through all form fields and get the value
                for (let fieldProperty of field.fields) {
                    if (fieldProperty && fieldProperty.propertyName) {
                        request.FormFields[fieldProperty.propertyName] = fieldProperty.value;
                    }
                }

                // Log what we're posting
                console.log('posting content:');
                console.dir(field.fields);

                // Add order
                let fieldID = (field.id < 0) ? 0 : field.id;
                request.FormFields["ID"] = fieldID;
                request.FormFields["SortOrder"] = field.sortOrder;

                request.FormFields["Title"] = field.title;
                request.FormFields["ContentTypeID"] = parseInt(field.contentTypeID);
                request.FormFields["IsMandatory"] = field.isMandatory;

                request.FormFields["FieldName"] = this.camelCase(request.FormFields["Title"]);
                //console.log('ohjajoh');
                //console.log('isSharedField:' + field.isSharedField);
                //console.dir(field);
                //request.FormFields["IsSharedField"] = field.isSharedField; 

                let headers = this.headers;
                let success = undefined;

                this.saveFieldLoading = true;
                field.status = this.fieldStatus.saving;
                await this.$http.post(`${this.formUri}&field=${fieldID}`, JSON.stringify(request), { headers })
                    .then((r) => {
                        this.saveFieldLoading = false;
                        success = this.saveFieldDone(field, r.data);
                    }, (r) => {
                        this.saveFieldLoading = false;
                        success = this.saveFieldDone(field, r.data);
                    });

                return success;
            },
            saveFieldDone(field, result) {
                this.notifications = result.notifications;

                // bind fields!!
                field.fields = this.parseForm(result, field.id).fields;

                if (result.notifications.filter(r => r.isError).length > 0) {
                    // invalid
                    field.status = this.fieldStatus.invalid;
                }
                else {
                    field.status = this.fieldStatus.saved;
                    setTimeout(() => {
                        if (field.status === this.fieldStatus.saved)
                            field.status = this.fieldStatus.default;
                    }, 3000)
                }

                // return IsSuccess?
                return this.notifications.filter(r => r.isError).length === 0;
            },
            //
            async init(openAtIndex) {
                if (!openAtIndex)
                    openAtIndex = 0;

                // Fetch fields
                let fieldResult = await this.fetchAllFields();
                this.fieldsCollection = fieldResult.fields.map((r) => {
                    return {
                        id: r.id,
                        title: r.title,
                        contentTypeID: r.contentTypeID,
                        isMandatory: r.isMandatory,
                        sortOrder: r.sortOrder,
                        className: r.className,
                        fields: [],
                        notifications: [],
                        status: 0,
                        isSharedField: r.isSharedField,
                        fieldName: r.fieldName
                    }
                }).sort((a, b) => (a.sortOrder > b.sortOrder) ? 1 : -1);


                // If no items, add one blank
                if (!this.fieldsCollection || !this.fieldsCollection.length) {
                    this.fieldsCollection = [];
                    this.fieldsCollection.push({
                        id: -1,
                        title: '',
                        contentTypeID: 0,
                        isMandatory: false,
                        sortOrder: -1,
                        className: '',
                        fields: [],
                        notifications: [],
                        status: 0,
                        isSharedField: false,
                        fieldName: ''
                    });
                }

                if (this.fieldsCollection && this.fieldsCollection.length) {
                    // this.selectField(this.fieldsCollection[0]);
                    // focus
                    this.$nextTick(r => {
                        let elm = document.getElementById(`field_text_${this.fieldsCollection[openAtIndex].id}`)
                        if (!elm)
                            elm = document.getElementById(`field_text_${this.fieldsCollection[0].id}`);

                        if (elm) {
                            elm.focus();
                        }
                    });
                }
            }
        },
        async mounted() {
            this.init();
        }
    }
</script>