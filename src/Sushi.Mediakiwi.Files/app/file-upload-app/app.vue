<template>
    <article id='file-upload' class='OrderingField'>
        <article class='error' v-if='errorMessage' v-html='errorMessage'></article>
        <section class='highlight-block'>
            <vue-dropzone ref='myVueDropzone' id='dropzone' :options='dropzoneOptions'
                          @vdropzone-error-multiple='uploadFailed'
                          @vdropzone-queue-complete='queueCompleted'
                          @vdropzone-success-multiple='uploadSuccess'
                          @vdropzone-processing-multiple='processing'></vue-dropzone>
        </section>
    </article>
</template>
<script>
    import vue2Dropzone from 'vue2-dropzone';
    import 'vue2-dropzone/dist/vue2Dropzone.min.css';

    export default {
        name: 'add-stock',
        components: {
            vueDropzone: vue2Dropzone
        },
        data() {
            return {
                addingCertificateLoading: false,
                uploadErrors: 0,
                errorMessage: '',
                dropzoneOptions: {
                    url: dzOptions.uploadUrl,
                    success: function (file, response) {
                        console.log(response);
                        var u = window.location.search + '&asset=' + response;
                        location.replace(u);
                    },
                    paramName: 'file', // The name that will be used to transfer the file
                    maxFilesize: 3, // MB
                    // parallelUploads: 1,
                    maxFiles: 1,
                    uploadMultiple: false,
                    autoProcessQueue: false, // disables auto upload
                    addRemoveLinks: true,
                    //maxFiles: 10,
                    acceptedFiles: dzOptions.acceptedFiles,
                    dictDefaultMessage: `<i class='drop-icon far fa-inbox-in'></i> ${dzOptions.uploadMessage}`,
                    dictRemoveFile: 'Remove',
                    dictCancelUpload: '',
                }
            }
        },
        computed: {
        },
        watch: {
            addingCertificateLoading(newValue) {
                let btnUpload = document.querySelector('#BtnUpload');
                if (newValue)
                    btnUpload.classList.add('disabled');
                else
                    btnUpload.classList.remove('disabled');
            }
        },
        methods: {
            uploadFailed(file, mes, xhr) {
                if (xhr && xhr.responseText) {
                    let messages = '';
                    var xhrResponse = JSON.parse(xhr.responseText);
                    if (xhrResponse) {
                        try {
                            var response = JSON.parse(xhrResponse.message);
                            if (response && typeof (response.Errors) !== 'undefined')
                                messages = response.Errors.join(', ');
                        }
                        catch (e) {
                            messages = xhrResponse.message;
                        }
                    }
                    this.errorMessage = messages;
                }
                this.uploadErrors++;
                this.$refs.myVueDropzone.removeAllFiles();
                this.addingCertificateLoading = false;
            },
            uploadSuccess(file, response) {
                this.addingCertificateLoading = false;
                console.log(response);
                console.log(file);
            },
            queueCompleted() {
                if (!this.uploadErrors) {
                    // do somethign with the resutl!?!?
                    console.log(vue2Dropzone);

                    //$('#save').trigger('click');

                }
                this.addingCertificateLoading = false;
            },
            vFileCanceled() {
                alert('vFileCanceled');
            },
            processing() {
                // add extra values
                this.$refs.myVueDropzone.setOption('url', `${dzOptions.uploadUrl}`);
            },
            isValidForm() {
                let $dropzone = document.querySelector('#dropzone');

                $dropzone.classList.remove('error');

                let errors = 0;

                if (!this.$refs.myVueDropzone.dropzone.files.length) {
                    $dropzone.classList.add('error');
                    errors++;
                }

                return errors === 0;
            },
            post(e) {
                e.preventDefault();
                e.stopPropagation();
                this.addingCertificateLoading = true;
                this.uploadErrors = 0;

                if (!this.isValidForm()) {
                    this.addingCertificateLoading = false;
                    return false;
                }

                this.$refs.myVueDropzone.processQueue();
                // always return false; 'cause input button
                return false;
            }
        },
        mounted() {
            let old_element = document.querySelector('#BtnUpload');
            let new_element = old_element.cloneNode();
            new_element.type = 'button';
            new_element.addEventListener('click', (e) => this.post(e));
            old_element.parentNode.replaceChild(new_element, old_element);

            let cancel = document.querySelector("#BtnCloseLayer");
            if (cancel) {
                cancel.className = cancel.className.replace('postBack', '');
                cancel.addEventListener('click', e => {
                    e.preventDefault();
                    parent.$.colorbox.close();
                });

            }
        },
        destroyed() {

        },
    }
</script>
<style>
    #file-upload + hr {
        display: none;
    }

    #file-upload .highlight-block {
        height: 100%;
        margin: 0 !important;
    }

        #file-upload .highlight-block #dropzone {
            height: 100%;
        }


    #dropzone .dz-default.dz-message {
        position: relative;
        top: 50%;
        margin: 0;
        transform: translateY(-50%);
    }

    #dropzone .dz-preview {
        margin-top: 0 !important;
    }
</style>