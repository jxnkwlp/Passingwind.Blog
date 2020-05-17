import http from "@/libs/httpclient";

export const getlist = payload => {
    return http.request({
        method: "get",
        url: "/api/comment",
        params: payload
    });
};

export const setApproved = payload => {
    return http.request({
        method: "put",
        url: "/api/comment/approved",
        data: payload
    });
};

export const setSpam = payload => {
    return http.request({
        method: "put",
        url: "/api/comment/spam",
        data: payload
    });
};

export const del = payload => {
    return http.request({
        method: "delete",
        url: "/api/comment",
        data: payload
    });
};

export const replay = payload => {
    return http.request({
        method: "post",
        url: "/api/comment/replay",
        data: payload
    });
};