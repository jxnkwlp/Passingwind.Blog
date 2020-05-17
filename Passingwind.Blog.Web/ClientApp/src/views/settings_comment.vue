<template>
    <el-row>
        <el-col :span="12">
            <el-form ref="formData" :model="formData" label-position="top" label-width="150px">
                <el-form-item>
                    <el-checkbox v-model="formData.enableComments" label="Enable comments"></el-checkbox>
                </el-form-item>
                <el-form-item>
                    <el-checkbox
                        :disabled="!formData.enableComments"
                        v-model="formData.enableCommentsModeration"
                        label="Enable comments moderation"
                    ></el-checkbox>
                </el-form-item>
                <el-form-item>
                    <el-checkbox
                        :disabled="!formData.enableComments"
                        v-model="formData.trustAuthenticatedUsers"
                        label="Trust authenticated users"
                    ></el-checkbox>
                </el-form-item>
                <el-form-item>
                    <el-checkbox
                        :disabled="!formData.enableComments"
                        v-model="formData.commentNestingEnabled"
                        label="Comment nesting enabled"
                    ></el-checkbox>
                </el-form-item>
                <el-form-item>
                    <el-checkbox
                        v-model="formData.showComments"
                        :disabled="!formData.enableComments"
                        label="Show comments"
                    ></el-checkbox>
                </el-form-item>
                <el-form-item>
                    <el-checkbox
                        v-model="formData.sendEmail"
                        :disabled="!formData.enableComments"
                        label="Send email"
                    ></el-checkbox>
                </el-form-item>
                <el-form-item>
                    <el-checkbox
                        v-model="formData.enableFormVerificationCode"
                        :disabled="!formData.enableComments"
                        label="Enable form verification code"
                    ></el-checkbox>
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
            groupName: "comments"
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