import http from "@/libs/httpclient";

export const upload = payload => {
    return http.request({
        method: "post",
        url: "/api/file",
        data: payload
    });
};