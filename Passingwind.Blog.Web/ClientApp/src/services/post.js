import http from "@/libs/httpclient";

export const getlist = payload => {
    return http.request({
        method: "get",
        url: "/api/post",
        params: payload
    });
};

export const get = payload => {
    const id = payload.id;
    delete payload.id;
    return http.request({
        method: "get",
        url: "/api/post/" + id,
        params: payload
    });
};

export const edit = payload => {
    return http.request({
        method: "post",
        url: "/api/post",
        data: payload
    });
};

export const del = payload => {
    return http.request({
        method: "delete",
        url: "/api/post",
        data: payload
    });
};

export const setPublished = payload => {
    return http.request({
        method: "post",
        url: "/api/post/published",
        data: payload
    });
};