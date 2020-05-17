import axios from "axios";
import qs from "qs";

import router from "@/router";

import config from "@/config";
var baseUrl = config.apiBaseUrl;

axios.defaults.timeout = 30000; // 30s
axios.defaults.withCredentials = true;

class HttpRequest {
    config() {
        return {
            baseURL: baseUrl,
            headers: {}
        };
    }
    interceptors(instance, url) {
        instance.interceptors.response.use(
            res => {
                const { data, status } = res;
                return {
                    data,
                    status
                };
            },
            error => {
                let errorInfo = error.response;
                console.log(errorInfo);

                if (Object.keys(this.responseCodeConfig).indexOf(errorInfo.status) >= 0) {
                    this.responseCodeConfig[errorInfo.status]();
                }
                // if (errorInfo.status == 403) {
                //     router.replace({ name: '403' });
                //     return;
                // }
                // if (!errorInfo) {
                //   const {
                //     request: { statusText, status },
                //     config
                //   } = JSON.parse(JSON.stringify(error));
                //   errorInfo = {
                //     statusText,
                //     status,
                //     request: {
                //       responseURL: config.url
                //     }
                //   };
                // }
                // addErrorLog(errorInfo);
                return Promise.reject(error);
            }
        );
    }
    request(options) {
        const instance = axios.create();
        options = Object.assign(this.config(), options);
        if (options.method == 'get' && options.params) {
            options.paramsSerializer = function (params) {
                return qs.stringify(params, { arrayFormat: 'indices', allowDots: true })
            }
        }
        this.interceptors(instance, options.url);
        return instance(options);
    }
    when(responseCodeConfig) {
        this.responseCodeConfig = responseCodeConfig;
    }
}

const http = new HttpRequest();

export default http;
