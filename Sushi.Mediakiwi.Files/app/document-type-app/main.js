import Vue from 'vue';
//import Vuex from 'vuex';
//import VueRouter from 'vue-router';
import VueResource from 'vue-resource';
import Select2 from 'v-select2-component';
import app from './app.vue';
import VuejsDialog from 'vuejs-dialog';
import 'vuejs-dialog/dist/vuejs-dialog.min.css';

//Vue.use(Vuex);
//Vue.use(VueRouter);
Vue.use(VueResource);
Vue.component('Select2', Select2);
Vue.use(VuejsDialog);

Vue.config.ignoredElements = ['v-container'];

Vue.config.errorHandler = (err, vm, info) => {
    // err: error trace
    // vm: component in which error occured
    // info: Vue specific error information such as lifecycle hooks, events etc.

    // TODO: Perform any custom logic or log to server
    console.error('-----');
    console.error(err);
    console.error(info);
    console.error('!-----!');
};


document.addEventListener("DOMContentLoaded", function () {
    // bind the app to to window for infinite scroll
    new Vue({
        el: '#app',
        render: h => h(app)
    });
}, false);