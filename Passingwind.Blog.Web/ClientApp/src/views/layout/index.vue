<template>
    <el-container class="container">
        <left-nav></left-nav>
        <el-main class="main-content">
            <div class="content-header" v-if="pageHeaderShow">
                <el-page-header :content="pageTitle" @back="onBack"></el-page-header>
            </div>
            <router-view class="content"></router-view>
        </el-main>
    </el-container>
</template>
 
<script>
import LeftNav from "./left_nav";
import config from "@/config";
import * as userService from "@/services/userservice";

import { mapActions } from "vuex";

export default {
    components: { LeftNav },
    data() {
        return {
            pageTitle: "",
            pageHeaderShow: true
        };
    },
    created() {
        this.handleLoadIdentity()
            .then()
            .catch(() => {
                window.location.href =
                    config.loginUrl + "?returnUrl=" + encodeURI("/admin/");
            });
    },
    mounted() {
        this.pageHeaderShow = !(
            this.$route.name == "home" || this.$route.name == "403"
        );
        this.pageTitle =
            (this.$route.meta && this.$route.meta.title) || this.$route.name;
    },
    methods: {
        ...mapActions(["handleLoadIdentity"]),
        onBack() {
            this.$router.go(-1);
        }
    },
    watch: {
        $route() {
            // console.log(this.$route);
            this.pageTitle =
                (this.$route.meta && this.$route.meta.title) ||
                this.$route.name;
            this.pageHeaderShow = this.$route.name != "home";
        }
    }
};
</script>