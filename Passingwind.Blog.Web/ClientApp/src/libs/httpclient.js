import axios from "axios";
import qs from "qs";

import router from "@/router";

import { Notification } from "element-ui";

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
                let response = error.response;
                console.log(response);

                // var codeHandle = this.responseCodeConfig[errorInfo.status];
                // if (codeHandle) {
                //     codeHandle(errorInfo);
                // } else {
                //     Notification.error({
                //         message: "您的网络发生异常，无法连接服务器",
                //         title: "网络异常"
                //     });
                // }

                if (response && response.status) {
                    const errorText =
                        codeMessage[response.status] || response.statusText;

                    Notification.error({
                        title: `请求错误${response.status} : ${response.config.url}`,
                        message: errorText
                    });
                } else {
                    Notification.error({
                        title: "网络异常",
                        message: "您的网络发生异常，无法连接服务器"
                    });
                }

                return Promise.reject(error);
            }
        );
    }
    request(options) {
        const instance = axios.create();
        options = Object.assign(this.config(), options);
        if (options.method == "get" && options.params) {
            options.paramsSerializer = function(params) {
                return qs.stringify(params, {
                    arrayFormat: "indices",
                    allowDots: true
                });
            };
        }
        this.interceptors(instance, options.url);
        return instance(options);
    }
    when(responseCodeConfig) {
        console.log(responseCodeConfig);
        this.responseCodeConfig = responseCodeConfig;
    }
}

const http = new HttpRequest();

const codeMessage = {
    200: "服务器成功返回请求的数据。",
    201: "新建或修改数据成功。",
    202: "一个请求已经进入后台排队（异步任务）。",
    204: "删除数据成功。",
    400: "发出的请求有错误，服务器没有进行新建或修改数据的操作。",
    401: "用户没有权限（令牌、用户名、密码错误）。",
    403: "用户得到授权，但是访问是被禁止的。",
    404: "发出的请求针对的是不存在的记录，服务器没有进行操作。",
    406: "请求的格式不可得。",
    410: "请求的资源被永久删除，且不会再得到的。",
    422: "当创建一个对象时，发生一个验证错误。",
    500: "服务器发生错误，请检查服务器。",
    502: "网关错误。",
    503: "服务不可用，服务器暂时过载或维护。",
    504: "网关超时。"
};

// http.when({
//     401: function() {
//         Notification.error({
//             title: codeMessage[401],
//             message: `请求错误${error.status} : ${error.config.url}`
//         });
//     },
//     404: function(error) {
//         Notification.error({
//             title: codeMessage[404],
//             message: `请求错误${error.status} : ${error.config.url}`
//         });
//     },
//     500: function(error) {
//         Notification.error({
//             title: codeMessage[500],
//             message: `请求错误${error.status} : ${error.config.url}`
//         });
//     }
// });

export default http;
