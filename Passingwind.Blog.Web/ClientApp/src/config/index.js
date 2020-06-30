// globel configs
export default {
    title: "admin",

    baseUrl: "/",
    apiBaseUrl:
        process.env.NODE_ENV == "development"
            ? "https://localhost:44317/"
            : "/",

    loginUrl: "/account/login"
};
