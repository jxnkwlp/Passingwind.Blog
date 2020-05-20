<template>
    <el-row>
        <el-col :span="12">
            <el-form ref="formData" :model="formData" label-position="top" label-width="150px">
                <el-form-item label="Header html" prop="headerHtml">
                    <el-input
                        v-model="formData.headerHtml"
                        type="textarea"
                        placeholder 
                    ></el-input>
                </el-form-item>
                <el-form-item label="Footer html" prop="footerHtml">
                    <el-input
                        v-model="formData.footerHtml"
                        type="textarea"
                        placeholder
                    ></el-input>
                </el-form-item>

                <el-form-item>
                    <el-checkbox v-model="formData.enableOpenSearch" label="Enable open search"></el-checkbox>
                </el-form-item>
                <el-form-item>
                    <el-checkbox v-model="formData.enableRegister" label="Enable register"></el-checkbox>
                </el-form-item>

                <el-form-item label="Editor ">
                    <el-select v-model="formData.editorName" style="width:100%;">
                        <el-option value="auto" label="Auto">Auto</el-option>
                        <el-option value="markdown" label="Markdown"></el-option>
                        <el-option value="tinymce" label="Tinymce"></el-option>
                    </el-select>
                </el-form-item>
            </el-form>
            <div class="form-bottom-action">
                <el-button type="primary" @click="handleFormSave">Save</el-button>
            </div>
        </el-col>
    </el-row>
</template>

<script>
import * as settings from "@/services/settings";

export default {
    data() {
        return {
            formData: {},
            groupName: "advanced"
        };
    },
    mounted() {
        this.loadData();
    },
    methods: {
        loadData() {
            settings
                .load({ name: this.groupName })
                .then(result => {
                    if (result.data) {
                        this.formData = result.data;
                        if (!this.formData.editorName)
                            this.formData.editorName = "auto";
                    }
                })
                .catch(err => {
                    console.log(err);
                    this.$message.error("Error");
                });
        },

        handleFormSave() {
            this.$refs["formData"].validate(valid => {
                if (this.formData.editorName == "auto")
                    this.formData.editorName = "";
                if (valid) {
                    settings
                        .save(this.groupName, this.formData)
                        .then(result => {
                            this.$message.success("Success");
                            this.loadData();
                        })
                        .catch(err => {
                            console.log(err);
                            this.$message.error("Error");
                        });
                }
            });
        }
    }
};
</script>

<style lang="less" scoped>
</style>