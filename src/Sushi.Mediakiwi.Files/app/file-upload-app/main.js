import Vue from 'Vue';
import app from './app.vue';

Vue.config.errorHandler = (err, vm, info) => {
    // err: error trace
    // vm: component in which error occured
    // info: Vue specific error information such as lifecycle hooks, events etc.

    // TODO: Perform any custom logic or log to server
    console.log('-----');
    console.error(err);
    console.error(info);
    console.log('!-----!');
};

document.addEventListener('DOMContentLoaded', (event) => {
    // dirty hack
    $("#app").insertAfter("#iconBarz");
    $("#BtnNext").addClass("hidden");

    new Vue({
        el: "#app",
        render: h => h(app)
    });
});