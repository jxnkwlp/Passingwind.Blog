import Vue from "vue";
import VueRouter from "vue-router";
import NProgress from "nprogress"

Vue.use(VueRouter);

const routes = [
    {
        path: "/",
        name: "_home",
        redirect: "/home",
        component: () => import("@/views/layout/index.vue"),
        children: [
            {
                path: "/home",
                name: "home",
                component: () => import("@/views/home.vue")
            },
            {
                path: "/403",
                name: "403",
                component: () => import("@/views/403.vue")
            },
            {
                path: "/profile",
                name: "profile",
                meta: {
                    title: "Profile",
                },
                component: () => import("@/views/profile.vue")
            },

            {
                path: "/category",
                name: "category",
                meta: {
                    title: "Category",
                },
                component: () => import("@/views/category.vue")
            },
            {
                path: "/tags",
                name: "tags",
                meta: {
                    title: "Tags",
                },
                component: () => import("@/views/tags.vue")
            },
            {
                path: "/page",
                name: "page",
                meta: {
                    title: 'Page'
                },
                component: () => import("@/views/page.vue")
            },
            {
                path: "/page/edit/:id?",
                name: "pageEdit",
                meta: {
                    title: "Page edit",
                },
                component: () => import("@/views/page_edit.vue")
            },
            {
                path: "/post",
                name: "post",
                meta: {
                    title: "Post",
                },
                component: () => import("@/views/post.vue")
            },
            {
                path: "/post/edit/:id?",
                name: "postEdit",
                meta: {
                    title: "Post edit",
                },
                component: () => import("@/views/post_edit.vue")
            },
            {
                path: "/comments",
                name: "comments",
                meta: {
                    title: "Comments",
                },
                component: () => import("@/views/comments.vue")
            },
            {
                path: "/users",
                name: "users",
                meta: {
                    title: "Users",
                },
                component: () => import("@/views/users_roles.vue")
            },
            {
                path: "/settings",
                name: "settings",
                meta: {
                    title: "Settings",
                },
                component: () => import("@/views/settings.vue")
            },
            {
                path: "/custom",
                name: "Custom",
                meta: {
                    title: "Custom",
                },
                component: () => import("@/views/custom.vue")
            }
        ]
    },

    {
        path: "*",
        name: "404",
        component: () => import("@/views/404.vue")
    },
];

const router = new VueRouter({
    base: process.env.BASE_URL,
    routes
});

router.beforeEach((to, from, next) => {
    NProgress.start();
    next();
});

router.afterEach((to, from) => {
    NProgress.done();
});

export default router;
