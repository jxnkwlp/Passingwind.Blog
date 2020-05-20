// globel configs
export default {
    title: "admin",

    baseUrl: '/',
    apiBaseUrl: (process.env.NODE_ENV == "development" ? 'https://localhost:44327/' : '/'),

    loginUrl: '/account/login'
};
