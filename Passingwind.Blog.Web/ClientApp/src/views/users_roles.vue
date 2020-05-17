<template>
    <div>
        <el-tabs type="border-card" @tab-click="handleTabsChanges">
            <el-tab-pane label="Users" v-if="showUserPane">
                <user :actived="activeName == 'Users'"></user>
            </el-tab-pane>
            <el-tab-pane label="Roles" v-if="showRolePane">
                <role :actived="activeName == 'Roles'"></role>
            </el-tab-pane>
        </el-tabs>
    </div>
</template>

<script>
import User from "./users";
import Role from "./roles";
import * as userService from "@/services/userservice";

export default {
    components: { User, Role },
    data() {
        return {
            activeName: "",
            showUserPane: false,
            showRolePane: false
        };
    },
    computed: {
        permissionKeys() {
            return this.$store.state.permissionKeys;
        }
    },
    mounted() {
        this.showUserPane = userService.hasPermission("user.list");
        this.showRolePane = userService.hasPermission("role.list");
    },
    methods: {
        handleTabsChanges(pane) {
            // console.log(e);
            this.activeName = pane.label;
        }
    }
};
</script>
