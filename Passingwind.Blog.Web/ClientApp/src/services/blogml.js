import http from "@/libs/httpclient";

export const _export = payload => {
    return http.request({
        method: "post",
        url: "/api/blogml/export",
        params: payload
    });
};