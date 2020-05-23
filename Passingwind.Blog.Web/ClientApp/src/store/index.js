import Vue from "vue";
import Vuex from "vuex";
import { identity } from "@/services/identity";

Vue.use(Vuex);
  
export default new Vuex.Store({
    state: {
        email: '',
        displayName: '',
        avatarUrl: '',
        roles: [],
        permissionKeys: []
    },
    mutations: {
        setEmail(state, value) {
            state.email = value;
        },
        setDisplayName(state, value) {
            state.displayName = value;
        },
        setAvatarUrl(state, value) {
            state.avatarUrl = value;
        },
        setRoles(state, value) {
            state.roles = value;
        },
        setPermissionKeys(state, value) {
            state.permissionKeys = value;
        },
    },
    actions: {
        handleLoadIdentity({ state,
            commit }) {
            return new Promise((resolve, reject) => {
                identity().then(res => {
                    if (res.status == 401) {
                        reject(res);
                    } else {
                        var data = res.data;
                        commit("setEmail", data.email);
                        commit("setDisplayName", data.name);
                        commit("setAvatarUrl", data.avatarUrl);
                        commit("setRoles", data.roles);
                        commit("setPermissionKeys", data.permissions);
                        resolve(data);
                    }
                }).catch(() => {
                    reject();
                })
            });
        }
    },
    modules: {}
});
