import http from "@/libs/httpclient";

export const getlist = payload => {
    return http.request({
        method: "get",
        url: "/api/category",
        params: payload
    });
};

export const create = payload => {
    return http.request({
        method: "post",
        url: "/api/category",
        data: payload
    });
};

export const update = payload => {
    return http.request({
        method: "put",
        url: "/api/category",
        data: payload
    });
};

export const del = payload => {
    return http.request({
        method: "delete",
        url: "/api/category",
        data: payload
    });
};
