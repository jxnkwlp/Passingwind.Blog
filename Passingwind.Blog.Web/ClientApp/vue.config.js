const buildVersion = new Date().getTime();

module.exports = {
    publicPath: "/admin",
    lintOnSave: false,
    productionSourceMap: false,
    runtimeCompiler: true,
    // devServer: {
    //     proxy: "https://localhost:44317/"
    // },
    css: {
        loaderOptions: {
            less: {
                javascriptEnabled: true
            }
        }
    }
    //configureWebpack: {
    //    output: {
    //        filename: `js/[name].${buildVersion}.js`,
    //        chunkFilename: `js/[name].${buildVersion}.js`
    //    }
    //}
};
