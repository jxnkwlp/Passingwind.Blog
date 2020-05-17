import http from "@/libs/httpclient";

export const getWidgets = () => {
    return http.request({
        method: "get",
        url: "/api/Widgets",
    });
};

export const getZones = () => {
    return http.request({
        method: "get",
        url: `/api/Widgets/zones`,
    });
};

export const getWidgetsFromZone = (zone) => {
    return http.request({
        method: "get",
        url: `/api/Widgets/zones/${zone}`,
    });
};

export const saveZoneWidgets = (payload) => {
    return http.request({
        method: "post",
        url: `/api/Widgets/zones`,
        data: payload
    });
};