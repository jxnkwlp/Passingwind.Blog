<template>
    <el-row>
        <el-col :span="12">
            <div>
                <el-divider content-position="left">Import</el-divider>
                <!-- <div>
                    <el-checkbox label="IncludeComments">IncludeComment</el-checkbox>
                </div>-->
                <div style="margin-top:10px;">
                    <el-upload
                        ref="upload"
                        action="/api/blogml/import"
                        name="file"
                        :limit="1"
                        :with-credentials="true"
                        :show-file-list="false"
                        :on-success="handleBlogMLImportSuccess"
                        :on-error="handleBlogMLImportError"
                        :on-progress="handleBlogMLImportProgress"
                    >
                        <el-button size="small" type="primary" :disabled="!completed">Upload</el-button>
                    </el-upload>
                </div>

                <el-divider content-position="left">Export</el-divider>
                <div style="margin-top:10px;">
                    <el-button
                        size="small"
                        type="primary"
                        :disabled="!completed"
                        @click="handleBlogMLExport"
                    >Download</el-button>
                </div>
            </div>
        </el-col>
    </el-row>
</template>

<script>
import * as settings from "@/services/settings";
import { _export } from "@/services/blogml";

export default {
    data() {
        return {
            formData: {},
            completed: true
        };
    },
    mounted() {},
    methods: {
        handleBlogMLImportProgress() {
            this.completed = false;
        },
        handleBlogMLImportSuccess(response, file, fileList) {
            this.completed = true;
            this.$message({
                message:
                    "Import success." +
                    `Post:${response.postCount}, Page:${response.pageCount}, Category:${response.categoryCount}`,
                type: "success"
            });
            this.$refs["upload"].clearFiles();
        },
        handleBlogMLImportError(error, file, fileList) {
            this.completed = true;
            this.$message({
                message: error,
                type: "error"
            });
            this.$refs["upload"].clearFiles();
        },
        handleBlogMLExport() {
            this.completed = false;

            _export()
                .then(res => {
                    // console.log(res);
                    let blob = new Blob([res.data], {
                        type: "application/stream"
                    });
                    let fileName = "BlogML.xml";
                    if (window.navigator.msSaveOrOpenBlob) {
                        navigator.msSaveBlob(blob, fileName);
                    } else {
                        var link = document.createElement("a");
                        link.href = window.URL.createObjectURL(blob);
                        link.download = fileName;
                        link.click();
                        window.URL.revokeObjectURL(link.href);
                    }
                })
                .then(() => {
                    this.completed = true;
                });
        }
    }
};
</script>

<style lang="less" scoped>
</style>