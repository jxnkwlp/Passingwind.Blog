import http from "@/libs/httpclient";

export const getlist = payload => {
    return http.request({
        method: "get",
        url: "/api/page",
        params: payload
    });
};

export const get = payload => {
    return http.request({
        method: "get",
        url: "/api/page/" + payload.id
    });
};

export const create = payload => {
    return http.request({
        method: "post",
        url: "/api/page",
        data: payload
    });
};

export const update = payload => {
    return http.request({
        method: "put",
        url: "/api/page",
        data: payload
    });
};

export const del = payload => {
    return http.request({
        method: "delete",
        url: "/api/page",
        data: payload
    });
};
