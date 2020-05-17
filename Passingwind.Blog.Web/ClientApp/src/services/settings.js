import http from "@/libs/httpclient";

export const list = payload => {
    return http.request({
        method: "get",
        url: "/api/setting/",
        params: payload
    });
};

export const addOrUpdate = payload => {
    return http.request({
        method: "post",
        url: "/api/setting",
        data: payload
    });
};

export const del = payload => {
    return http.request({
        method: "delete",
        url: "/api/setting",
        data: payload
    });
};

export const load = payload => {
    return http.request({
        method: "get",
        url: "/api/setting/load",
        params: payload
    });
};

export const save = (name, payload) => {
    return http.request({
        method: "post",
        url: "/api/setting/save/" + name,
        data: payload
    });
};