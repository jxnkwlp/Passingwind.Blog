<template>
    <div>
        <el-tabs type="border-card" @tab-click="handleTabsChanges">
            <el-tab-pane label="Themes">
                <themes :actived="activeName == 'Themes'"></themes>
            </el-tab-pane>
            <el-tab-pane label="Widgets">
                <widgets :actived="activeName == 'Widgets'"></widgets>
            </el-tab-pane>
        </el-tabs>
    </div>
</template>

<script>
import Themes from "./custom_themes";
import Widgets from "./custom_widgets";

export default {
    components: { Themes, Widgets },
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
