<template>
    <el-row>
        <el-col :span="12">
            <el-form ref="formData" :model="formData" label-position="top" label-width="150px">
                <el-form-item label>
                    <el-checkbox v-model="formData.enabled" label="Enabled"></el-checkbox>
                </el-form-item>
                <el-form-item
                    label="Limit output "
                    :rules="[{required:true, message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input-number
                        v-model="formData.limit"
                        :disabled="!formData.enabled"
                        :min="0"
                        :max="999999"
                        placeholder
                    ></el-input-number>
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
            groupName: "feed"
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