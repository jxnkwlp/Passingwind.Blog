<template>
    <component :is="viewName" v-bind="$props"></component>
</template>

<script>
import axios from "axios";
import less from "less";

export default {
    props: {
        url: {
            type: String,
            default() {
                return null;
            }
        }
    },
    data() {
        return {
            cssId: null,
            viewTemplate: null
        };
    },
    computed: {
        viewName() {
            if (!this.viewTemplate)
                return { template: "<div class='view-null'></div>" };
            else return this.renderView();
        }
    },
    mounted() {
        this.getTemplate();
    },
    methods: {
        renderView() {
            const tplData = this.resolveTemplate(this.viewTemplate);
            let ponentObj = new Function(
                `return ${tplData.sctipts.slice(
                    tplData.sctipts.indexOf("{"),
                    tplData.sctipts.lastIndexOf("}") + 1
                )}`
            )();
            ponentObj.template = tplData.templates;
            this.$el.setAttribute("class", `remote css${this.cssId}`);
            if (!document.querySelector(`style[id=css${this.cssId}]`)) {
                //防止重复创建
                let cssStr = `
                    .css${this.cssId}{
                        ${tplData.styles}
                    }
                `;
                this.resolveCss(cssStr);
            }
            return ponentObj;
        },

        getId() {
            var d = new Date().getTime();
            var uid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(
                /[xy]/g,
                function(c) {
                    var r = (d + Math.random() * 16) % 16 | 0;
                    d = Math.floor(d / 16);
                    return (c == "x" ? r : (r & 0x3) | 0x8).toString(16);
                }
            );
            return uid;
        },

        resolveCss(lessInput) {
            less.render(lessInput).then(
                function(output) {
                    let style = document.createElement("style");
                    style.setAttribute("type", "text/css");
                    style.setAttribute("id", "css" + this.cssId);
                    if (style.styleSheet)
                        // IE
                        style.styleSheet.cssText = output.css;
                    else {
                        // w3c
                        var cssText = document.createTextNode(output.css);
                        style.appendChild(cssText);
                    }
                    var heads = document.getElementsByTagName("head");
                    if (heads.length) heads[0].appendChild(style);
                    else document.documentElement.appendChild(style);
                }.bind(this)
            );
        },

        resolveTemplate(str) {
            return {
                templates: str.match(/<template>([\s\S]*)<\/template>/)[1],
                sctipts: str.match(/<script.*>([\s\S]*)<\/script>/)[1],
                styles: str.match(/<style.*>([\s\S]*)<\/style>/)[1]
            };
        },

        getTemplate() {
            return this.$http
                .get(this.$props.url)
                .then(res => {
                    if (res.data) {
                        this.cssId = this.getId();
                        this.viewTemplate = res.data;
                    }
                })
                .catch(() => {
                    this.resData = null;
                });
        }
    },
    watch: {
        url() {
            this.getTemplate();
        }
    }
};
</script>

<style lang="less" scoped>
</style>