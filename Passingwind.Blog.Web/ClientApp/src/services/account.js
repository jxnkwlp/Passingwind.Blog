import http from "@/libs/httpclient";

export const updateProfile = payload => {
    return http.request({
        url: "api/account/profile",
        method: "post",
        data: payload
    });
};

export const getLogins = () => {
    return http.request({
        url: "api/account/externallogin",
        method: "get"
    });
};

export const getExternalLoginSchemes = () => {
    return http.request({
        url: "api/account/ExternalAuthenticationSchemes",
        method: "get"
    });
};

export const removeLogin = payload => {
    return http.request({
        url: "api/account/externallogin",
        method: "delete",
        data: payload
    });
};
