import store from '@/store'
import * as userService from "@/services/userservice";

export default {
    bind(el, binding, vnode) {
        // var keys = binding.value;
        // const unwatch = store.watch(state => state.permissionKeys, role => {
        //     if (keys.filter(item => userService.hasPermission(item)).length == 0) {
        //         el.style.display = 'none';
        //     }
        // });
        // el.__role_unwatch__ = unwatch
        // console.log('bind');
    },
    inserted(el, binding, vnode, old) {
        // console.log('inserted');
        // var keys = binding.value;
        // const unwatch = store.watch(state => state.permissionKeys, role => {
        //     if (keys.filter(item => userService.hasPermission(item)).length == 0) {
        //         el.style.display = 'none';
        //     }
        // });
        // el.__role_unwatch__ = unwatch
        // const roles = store.getters && store.getters.roles;

        const { value } = binding;
        if (value.filter(item => userService.hasPermission(item)).length == 0) {
            el.parentNode && el.parentNode.removeChild(el);
        }
    },
    update: (el, binding) => {
    },
    unbind(el, binding) {
        // el.__role_unwatch__ && el.__role_unwatch__()
    }
}
