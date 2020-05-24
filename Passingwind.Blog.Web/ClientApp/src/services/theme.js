import http from "@/libs/httpclient";

export const getList = () => {
    return http.request({
        method: "get",
        url: "/api/theme",
    });
};

export const setTheme = (name) => {
    return http.request({
        method: "post",
        url: `/api/theme/default?name=` + name,
    });
};
