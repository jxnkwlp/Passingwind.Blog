<template>
    <div>
        <div style="margin-bottom:10px;">
            <el-select v-model="currentZone" @change="loadZoneWidgets">
                <el-option
                    v-for="(item, index) in allZones"
                    :key="index"
                    :label="item.value"
                    :value="item.name"
                ></el-option>
            </el-select>
            <el-button type="primary" style="margin-left:5px;" @click="handleSubmit">Save</el-button>
        </div>

        <el-row class="widgets">
            <el-col :span="6">
                <draggable
                    class="config-widgets-list widgets"
                    v-bind:class="installed.length==0?`empty`:``"
                    v-model="installed"
                    :group="{name:'w',}"
                >
                    <div
                        class="item"
                        v-for="(element, index) in installed"
                        :key="index"
                        :data-id="element.id"
                    >
                        <i class="icon el-icon-sort"></i>
                        <div class="name">
                            <span
                                v-if="!(element.isNameEdit||false)"
                                v-on:click="handleNameEdit(index)"
                            >{{element.name || element.widgetName}}</span>
                            <el-input
                                v-model="element.name"
                                v-else
                                placeholder
                                size="mini"
                                @blur="handleNameEditComplete(index)"
                            ></el-input>
                        </div>
                        <div class="action">
                            <el-button
                                type="text"
                                size="mini"
                                icon="el-icon-edit"
                                @click="handleEdit(element,index)"
                                v-if="element.id && element.adminConfigureUrl"
                            ></el-button>
                            <el-button
                                type="text"
                                size="mini"
                                icon="el-icon-close"
                                @click="handleRemove(element,index)"
                            ></el-button>
                        </div>
                    </div>
                </draggable>
            </el-col>
            <el-col :span="6" :offset="1">
                <draggable
                    class="all-widgets widgets list-group"
                    v-model="allWidgets"
                    :group="{name:'w', pull: 'clone', put: false}"
                >
                    <div class="item" v-for="element in allWidgets" :key="element.widgetId">
                        <i class="icon el-icon-rank"></i>
                        <div class="name">{{element.widgetName}}</div>
                    </div>
                </draggable>
            </el-col>
        </el-row>
        <!-- widget edit dialog -->
        <el-dialog title="Configure" :visible.sync="widgetConfigureVisible">
            <iframe
                id="configureWindow"
                ref="configureWindow"
                :src="widgetConfigureUrl"
                frameborder="0"
                scrolling="no"
                style="width:100%;"
            ></iframe>
        </el-dialog>

        <div id="widgetView"></div>
    </div>
</template>

<script>
import draggable from "vuedraggable";

import * as widgetService from "@/services/widgets";

export default {
    components: {
        draggable
    },

    data() {
        return {
            allZones: [],
            currentZone: "",

            allWidgets: [],
            installed: [],

            widgetConfigureVisible: false,
            widgetConfigureUrl: "",

            timer1: null,

            remote: null
        };
    },
    mounted() {
        this.loadData();

        // this.timer1 = window.setInterval(
        //     this.handleIFrameWindowAutoHeight,
        //     200
        // );
    },
    beforeDestroy() {
        if (this.timer1) window.clearInterval(this.timer1);
    },
    methods: {
        loadData() {
            widgetService
                .getWidgets()
                .then(res => {
                    this.allWidgets = res.data.map(item => {
                        return {
                            widgetId: item.widgetId,
                            widgetName: item.widgetName,
                            name: item.widgetName,
                            adminConfigureUrl: item.adminConfigureUrl
                        };
                    });
                })
                .then(() => {
                    widgetService.getZones().then(res => {
                        this.allZones = res.data;
                        this.currentZone = res.data[0].name;

                        this.loadZoneWidgets(this.currentZone);
                    });
                });
        },

        loadZoneWidgets(value) {
            widgetService.getWidgetsFromZone(value).then(res => {
                this.installed = res.data || [];
                this.installed.forEach(item => {
                    var w = this.allWidgets.find(
                        w => w.widgetId == item.widgetId
                    );
                    if (w && w.adminConfigureUrl) {
                        item.adminConfigureUrl = w.adminConfigureUrl;
                    }
                });
            });
        },

        handleNameEdit(index) {
            this.$nextTick(() => {
                // this.installed[index].isNameEdit = true;
                this.$set(this.installed[index], "isNameEdit", true);
            });
        },

        handleNameEditComplete(index) {
            this.$nextTick(() => {
                this.$set(this.installed[index], "isNameEdit", false);
            });
        },

        handleSubmit() {
            var data = {
                zone: this.currentZone,
                widgets: this.installed.map((item, index) => {
                    return {
                        id: item.id,
                        widgetId: item.widgetId,
                        Name: item.name || item.widgetName,
                        Order: index + 1
                    };
                })
            };

            widgetService
                .saveZoneWidgets(data)
                .then(res => {
                    this.$message.success("Saved successfully");

                    this.loadZoneWidgets(this.currentZone);
                })
                .catch(() => {
                    this.$message.error("Error");
                });
        },

        handleEdit(ele, i) {
            var url = ele.adminConfigureUrl + "?id=" + ele.id;
            window.open(url);
            // this.widgetConfigureVisible = true;
            // this.widgetConfigureUrl = ele.adminConfigureUrl + "?id=" + ele.id;

            // Vue.component("MyExternalComponent", () =>
            //     fetch("/test.json", { method: "get" })
            //         .then(res => res.json())
            //         .then(definition => ({
            //             ...definition,
            //             data() {
            //                 return definition.data;
            //             }
            //         }))
            // );

            // Vue.component("Test", () => {
            //     fetch("/test.vue").then(definition => ({
            //         ...definition,
            //         data() {
            //             return definition.data;
            //         }
            //     }));
            // }).$mount("widgetView");
        },

        handleRemove(ele, i) {
            this.installed.splice(i, 1);
        },

        handleIFrameWindowAutoHeight() {
            var iframe = document.getElementById("configureWindow");
            try {
                var bHeight = iframe.contentWindow.document.body.scrollHeight;
                var dHeight =
                    iframe.contentWindow.document.documentElement.scrollHeight;
                var height = Math.max(bHeight, dHeight);
                iframe.height = height;
                console.log(height);
            } catch (ex) {}
        }
    }
};
</script>
 
<style lang="less" scoped>
.widgets {
    .highlight {
        background-color: #d9edf7;
    }
    .handle {
        cursor: grab;
    }

    .item {
        position: relative;
        display: block;
        padding: 10px 15px;
        margin-bottom: -1px;
        background-color: #fff;
        border: 1px solid #ddd;
        display: flex;
        align-items: center;

        &:first-child {
            border-top-left-radius: 4px;
            border-top-right-radius: 4px;
        }
        &:last-child {
            margin-bottom: 0;
            border-bottom-right-radius: 4px;
            border-bottom-left-radius: 4px;
        }

        .icon {
            flex: 0 0 auto;
            margin-right: 4px;
        }
        .name {
            flex: 1 1 auto;
            width: 100%;

            text-overflow: ellipsis;
            overflow: hidden;
        }

        .action {
            flex: 0 0 auto;
            width: 40px;
            text-align: right;

            button {
                padding: 0;

                i {
                    font-size: 14px;
                }
            }
        }
    }
}
.config-widgets-list {
    min-height: 200px;
    position: relative;
    border: 1px dashed #aaa;
    padding: 10px;
    border-radius: 4px;

    &:empty,
    &.empty {
        background: #f1f1f1;
        border-radius: 4px;
    }
    &.empty:after {
        display: block;
        content: "Drop to here.";
        font-size: 20px;
        text-align: center;
        line-height: 200px;
        color: #808080;
    }
}
.all-widgets .pull-right {
    display: none;
}
</style>