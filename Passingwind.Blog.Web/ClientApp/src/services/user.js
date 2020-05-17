import http from "@/libs/httpclient";

export const getlist = payload => {
    return http.request({
        method: "get",
        url: "/api/user",
        params: payload
    });
};

export const del = payload => {
    return http.request({
        method: "delete",
        url: "/api/user",
        data: payload
    });
};

export const create = payload => {
    return http.request({
        method: "post",
        url: "/api/user",
        data: payload
    });
};

export const update = payload => {
    return http.request({
        method: "put",
        url: "/api/user",
        data: payload
    });
};

export const setLock = payload => {
    return http.request({
        method: "post",
        url: "/api/user/lock",
        data: payload
    });
};

