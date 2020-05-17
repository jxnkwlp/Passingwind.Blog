<template>
    <div>
        <el-tabs type="border-card" @tab-click="handleTabsChanges">
            <el-tab-pane label="Basic">
                <basic :actived="activeName == 'Basic'"></basic>
            </el-tab-pane>
            <el-tab-pane label="Advanced">
                <advanced :actived="activeName == 'Advanced'"></advanced>
            </el-tab-pane>
            <el-tab-pane label="Comments">
                <comment :actived="activeName == 'Comments'"></comment>
            </el-tab-pane>
            <el-tab-pane label="Feed">
                <feed :actived="activeName == 'Feed'"></feed>
            </el-tab-pane>
            <el-tab-pane label="Email">
                <email :actived="activeName == 'Email'"></email>
            </el-tab-pane>
            <el-tab-pane label="BlogML">
                <blogml :actived="activeName == 'BlogML'"></blogml>
            </el-tab-pane>
            <el-tab-pane label="Custom">
                <custom :actived="activeName == 'Custom'"></custom>
            </el-tab-pane>
        </el-tabs>
    </div>
</template>

<script>
import Basic from "./settings_basic";
import Advanced from "./settings_advanced";
import Feed from "./settings_feed";
import Comment from "./settings_comment";
import Email from "./settings_email";
import Blogml from "./settings_blogml";
import Custom from "./settings_custom";

export default {
    components: { Basic, Advanced, Feed, Comment, Email, Blogml, Custom },
    data() {
        return {
            activeName: ""
        };
    },
    mounted() {},
    methods: {
        handleTabsChanges(pane) {
            this.activeName = pane.label;
        }
    }
};
</script>
