import http from "@/libs/httpclient";

export const sendTestEmail = payload => {
    return http.request({
        method: "post",
        url: "/api/common/sendTestEmail",
        data: payload
    });
};