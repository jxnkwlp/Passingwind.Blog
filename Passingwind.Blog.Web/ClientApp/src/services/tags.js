import http from "@/libs/httpclient";

export const getlist = payload => {
    return http.request({
        method: "get",
        url: "/api/tags",
        params: payload
    });
};

export const del = payload => {
    return http.request({
        method: "delete",
        url: "/api/tags",
        data: payload
    });
};
