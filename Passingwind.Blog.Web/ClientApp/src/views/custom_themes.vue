<template>
    <div>
        <el-row>
            <el-col
                :span="5"
                v-for="(item, index) in list"
                :key="index"
                :offset="index % 4 != 0 ? 1 : 0"
                class="theme-item"
            >
                <el-card :body-style="{ padding: '0px' }">
                    <img
                        :src="item.previewUrl"
                        class="image"
                        v-if="item.previewUrl"
                    />
                    <div class="image" v-else>
                        <i class="el-icon-picture"></i>
                    </div>
                    <div style="padding: 14px;">
                        <span>{{ item.name }}</span>
                        <div class="clearfix action">
                            <el-button
                                type="primary"
                                size="mini"
                                class="pull-left"
                                :disabled="item.id == current"
                                @click="handleSetTheme(item.id)"
                                >Apply</el-button
                            >
                            <el-button
                                icon="el-icon-info"
                                size="mini"
                                class="pull-right"
                            ></el-button>
                        </div>
                    </div>
                </el-card>
            </el-col>
        </el-row>
    </div>
</template>

<script>
import * as theme from "@/services/theme";

export default {
    data() {
        return {
            list: [],
            current: ""
        };
    },
    mounted() {
        this.loadData();
    },
    methods: {
        loadData() {
            theme
                .getList()
                .then(result => {
                    this.list = result.data.themes;
                    this.current = result.data.name;
                })
                .catch(err => {});
        },

        handleSetTheme(value) {
            theme
                .setTheme(value)
                .then(result => {
                    this.current = value;
                    this.$message.success("Saved successfully");
                })
                .catch(() => {
                    this.$message.error("Error");
                });
        }
    }
};
</script>

<style lang="less" scoped>
.theme-item {
    margin-bottom: 20px;

    .action {
        margin-top: 15px;
    }

    .image {
        width: 100%;
        height: 260px;
        text-align: center;
        background: #eee;

        .el-icon-picture {
            font-size: 30px;
            line-height: 260px;

            display: inline-block;
        }
    }
}
</style>
