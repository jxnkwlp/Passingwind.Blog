<template>
    <div>
        <el-tabs type="border-card">
            <el-tab-pane label="Profile">
                <info></info>
            </el-tab-pane>
            <el-tab-pane label="Change Password">
                <el-button type="text" @click="handleOpenChangePasswordPage">
                    Go
                    <i class="el-icon-link el-icon--right"></i>
                </el-button>
            </el-tab-pane>
            <el-tab-pane label="Logins">
                <login></login>
            </el-tab-pane>
        </el-tabs>
    </div>
</template>

<script>
import Info from "./profile_info";
import Login from "./profile_login";

import { identity } from "@/services/identity";
import * as account from "@/services/account";

export default {
    components: { Info, Login },
    data() {
        return {};
    },
    mounted() {},
    methods: {
        handleOpenChangePasswordPage() {
            window.open("/account/changepassword");
        }
    }
};
</script>