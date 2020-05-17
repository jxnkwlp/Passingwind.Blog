import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

import "font-awesome/css/font-awesome.css";

import ElementUI from "element-ui";
import "element-ui/lib/theme-chalk/index.css";

Vue.use(ElementUI);

import axios from 'axios'

Vue.prototype.$http = axios

import "@/assets/css/site.less";

import 'nprogress/nprogress.css'

import importDirective from '@/directive'
importDirective(Vue);

import httpClient from '@/libs/httpclient'

httpClient.when({
    401: function () {
        console.log('401');
    }
})

Vue.config.productionTip = false;

new Vue({
    router,
    store,
    render: h => h(App)
}).$mount("#app");

console.log(process.env.BASE_URL);
