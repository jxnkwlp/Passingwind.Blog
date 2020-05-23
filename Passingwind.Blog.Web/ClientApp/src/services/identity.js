import http from "@/libs/httpclient";
import store from '@/store'

export const identity = () => {
    return http.request({
        url: "api/identity",
        method: "get",
    });
};
 
export const hasPermission = (key) => {
    var allKeys = store.state.permissionKeys;
    var roles = store.state.roles;
    if (roles.filter(r => r == "Administrator").length > 0)
        return true;
    return (allKeys.filter(item => item == key).length > 0)
}

export const hasAnyPermissions = (keys) => {
    var allKeys = store.state.permissionKeys;
    var roles = store.state.roles;
    if (roles.filter(r => r == "Administrator").length > 0)
        return true;
    return allKeys.filter(item => keys.indexOf(item) >= 0).length > 0;
}