<template>
    <el-row>
        <el-col :span="12">
            <el-form ref="formData" :model="formData" label-position="top" label-width="150px">
                <el-form-item
                    label="Email"
                    prop="email"
                    :rules="[{required:false , message:'The field is required', trigger: 'blur'}, {type:'email' , message:'The field must be email', trigger: 'blur'}]"
                >
                    <el-input v-model="formData.email" placeholder :maxlength="32"></el-input>
                </el-form-item>
                <el-form-item
                    label="UserName"
                    prop="userName"
                    :rules="[{required:false , message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input v-model="formData.userName" placeholder :maxlength="32"></el-input>
                </el-form-item>
                <el-form-item
                    label="Password"
                    prop="password"
                    :rules="[{required:false , message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input v-model="formData.password" placeholder :maxlength="32"></el-input>
                </el-form-item>
                <el-form-item
                    label="SmtpHost"
                    prop="smtpHost"
                    :rules="[{required:false , message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input v-model="formData.smtpHost" placeholder :maxlength="32"></el-input>
                </el-form-item>
                <el-form-item
                    label="SmtpPort"
                    prop="smtpPort"
                    :rules="[{required:false , message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input-number v-model="formData.smtpPort" placeholder></el-input-number>
                </el-form-item>
                <el-form-item
                    label="DisplayName"
                    prop="displayName"
                    :rules="[{required:false , message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input v-model="formData.displayName" placeholder :maxlength="32"></el-input>
                </el-form-item>

                <el-form-item>
                    <el-checkbox v-model="formData.enableSsl" label="Enable ssl"></el-checkbox>
                </el-form-item>
            </el-form>
            <div class="form-bottom-action">
                <el-button type="primary" @click="handleFormSave">Save</el-button>
                <el-button @click="handleEmailTest">Test</el-button>
            </div>
        </el-col>
        <el-dialog title="Test Email" :visible.sync="testEmailDialogVisible" width="50%">
            <el-form
                ref="testEmailForm"
                :model="testEmailForm"
                label-position="left"
                label-width="auto"
            >
                <el-form-item label="Email" :rules="[ { required: true, message: ''} ]">
                    <el-input v-model="testEmailForm.email" />
                </el-form-item>
                <el-form-item label="Body" :rules="[ { required: true, message: ''} ]">
                    <el-input v-model="testEmailForm.body" type="textarea" :rows="5" />
                </el-form-item>
                <el-form-item label>
                    <el-button type="primary" @click="handleEmailTestSend">Send</el-button>
                </el-form-item>
            </el-form>
        </el-dialog>
    </el-row>
</template>

<script>
import * as settings from "@/services/settings";
import * as common from "@/services/common";

export default {
    data() {
        return {
            formData: {},
            groupName: "email",
            testEmailDialogVisible: false,

            testEmailForm: {
                subject: "",
                body: "This is test message."
            }
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
        },

        handleEmailTest() {
            this.testEmailDialogVisible = true;
        },

        handleEmailTestSend() {
            this.$refs["testEmailForm"].validate(valid => {
                if (valid) {
                    common
                        .sendTestEmail(this.testEmailForm)
                        .then(result => {
                            this.$message.success("Send successed.");
                            this.testEmailDialogVisible = false;
                        })
                        .catch(err => {
                            console.log(err);
                            this.$message.error(
                                (err && err.title) || "Send error."
                            );
                        });
                }
            });
        }
    }
};
</script>

<style lang="less" scoped>
</style>