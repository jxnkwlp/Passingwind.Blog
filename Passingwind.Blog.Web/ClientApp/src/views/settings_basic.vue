<template>
    <el-row>
        <el-col :span="12">
            <el-form ref="formData" :model="formData" label-position="top" label-width="150px">
                <el-form-item
                    label="Title"
                    prop="title"
                    :rules="[{required:true, message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input v-model="formData.title" placeholder :maxlength="32"></el-input>
                </el-form-item>
                <el-form-item
                    label="Description"
                    prop="description"
                    :rules="[{required:true, message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input
                        v-model="formData.description"
                        type="textarea"
                        placeholder
                        :maxlength="128"
                    ></el-input>
                </el-form-item>
                <el-form-item
                    label="Page Show Count"
                    prop="pageShowCount"
                    :rules="[{required:true, message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input-number
                        v-model="formData.pageShowCount"
                        :min="0"
                        :max="100"
                        placeholder
                    ></el-input-number>
                </el-form-item>
                <el-form-item>
                    <el-checkbox v-model="formData.showDescription" label="Show Description"></el-checkbox>
                </el-form-item>
                <el-form-item
                    v-if="formData.showDescription"
                    label="Show Description String Count"
                    :rules="[{required:true, message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input-number
                        v-model="formData.showDescriptionStringCount"
                        :min="0"
                        :max="1024"
                        placeholder
                    ></el-input-number>
                </el-form-item>
                <el-form-item label="Icp Number">
                    <el-input v-model="formData.icpNumber" placeholder :maxlength="16"></el-input>
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
            groupName: "basic"
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
                    }
                })
                .catch(err => {
                    console.log(err);
                    this.$message.error("Error");
                });
        },

        handleFormSave() {
            this.$refs["formData"].validate(valid => {
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