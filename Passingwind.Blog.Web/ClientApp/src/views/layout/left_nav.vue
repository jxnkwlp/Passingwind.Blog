<template>
    <el-aside width="auto" class="left-nav">
        <el-menu
            default-active
            :collapse="true"
            :router="true"
            class="nav-item-top"
            @select="menuSelect"
        >
            <el-menu-item class="user-avator">
                <template>
                    <router-link :title="displayName" :to="{name:'profile'}">
                        <img :src="avatarUrl" />
                    </router-link>
                </template>
            </el-menu-item>

            <template v-for="(item, index) in menus">
                <template v-if="item.children && item.children.length > 0">
                    <el-submenu
                        :key="index"
                        :name="`${index}_${item.title}`"
                        :index="`${index}_${item.title}`"
                        v-show="checkMentPermission(item.permissionKey)"
                    >
                        <template slot="title">
                            <i :class="`${item.icon}`"></i>
                            <span>{{ item.title }}</span>
                        </template>
                        <el-menu-item
                            v-for="(item2, index2) in item.children"
                            :key="index2"
                            :name="`${item2.index}_${item2.title}`"
                            :index="item2.index"
                            v-show="checkMentPermission(item2.permissionKey)"
                        >{{ item2.title }}</el-menu-item>
                    </el-submenu>
                </template>
                <template v-else>
                    <el-menu-item
                        :key="index"
                        :name="`${item.index}_${item.title}`"
                        :index="item.index"
                        v-show="checkMentPermission(item.permissionKey)"
                    >
                        <i :class="`${item.icon}`"></i>
                        <span slot="title">{{ item.title }}</span>
                    </el-menu-item>
                </template>
            </template>
        </el-menu>
        <el-menu :collapse="true" class="nav-item-bottom" @select="footMenuSelect">
            <el-menu-item class index="goToWebsite">
                <i class="fa fa-globe"></i>
                <span slot="title">Web site</span>
            </el-menu-item>
            <el-menu-item class index="goToLogout">
                <i class="fa fa-sign-out"></i>
                <span slot="title">Logout</span>
            </el-menu-item>
        </el-menu>
    </el-aside>
</template>

<script>
import * as userService from "@/services/userservice";

export default {
    data() {
        return {
            menus: []
            // displayName: "",
            // avatarUrl: ""
        };
    },
    computed: {
        displayName() {
            return this.$store.state.displayName;
        },
        avatarUrl() {
            return this.$store.state.avatarUrl;
        },
        permissionKeys() {
            return this.$store.state.permissionKeys;
        }
    },
    created() {
        this.loadMenus();
    },
    mounted() {},
    methods: {
        loadMenus() {
            var source = [
                {
                    icon: "fa fa-home",
                    title: "Dashbord",
                    index: "/"
                },
                {
                    icon: "fa fa-plus",
                    title: "Add",
                    permissionKey: [
                        "post.edit",
                        "page.create",
                        "category.create"
                    ],
                    children: [
                        {
                            title: "Post",
                            index: "/post/edit",
                            permissionKey: ["post.edit"]
                        },
                        {
                            title: "Page",
                            index: "/page/edit",
                            permissionKey: ["page.create"]
                        },
                        {
                            title: "Category",
                            index: "/category?edit",
                            permissionKey: ["category.create"]
                        }
                    ]
                },
                {
                    icon: "el-icon-s-grid",
                    title: "List",
                    permissionKey: [
                        "post.list",
                        "page.list",
                        "category.list",
                        "tags.list"
                    ],
                    children: [
                        {
                            title: "Post",
                            index: "/post",
                            permissionKey: ["post.list"]
                        },
                        {
                            title: "Page",
                            index: "/page",
                            permissionKey: ["page.list"]
                        },
                        {
                            title: "Category",
                            index: "/category",
                            permissionKey: ["category.list"]
                        },
                        {
                            title: "Tags",
                            index: "/tags",
                            permissionKey: ["tags.list"]
                        }
                    ]
                },
                {
                    icon: "el-icon-chat-line-square",
                    title: "Comments",
                    index: "/comments",
                    permissionKey: ["comment.list"]
                },
                {
                    icon: "el-icon-s-operation",
                    title: "Custom",
                    index: "/custom",
                    permissionKey: ["custom"]
                },
                {
                    icon: "el-icon-user",
                    title: "Users",
                    index: "/users",
                    permissionKey: ["user.list", "role.list"]
                },
                {
                    icon: "el-icon-setting",
                    title: "Settings",
                    index: "/settings",
                    permissionKey: ["setting.load"]
                }
            ];

            this.menus = source;
        },
        menuSelect(index) {},
        footMenuSelect(index) {
            if (index == "goToWebsite") {
                location.href = "/";
            } else if (index == "goToLogout") {
                location.href = "/account/logout";
            }
        },
        checkMentPermission(keys) {
            if (keys) {
                if (keys.length == 1) {
                    return userService.hasPermission(keys[0]);
                } else {
                    return userService.hasAnyPermissions(keys);
                }
            } else return true;
        }
    },
    watch: {
        displayName() {
            //console.log("displayName");
        }
    }
};
</script>
