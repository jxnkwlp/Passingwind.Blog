<template>
    <div>
        <el-tabs type="border-card">
            <el-tab-pane label="Profile">
                <el-form
                    ref="profileFormData"
                    :model="profileFormData"
                    label-position="left"
                    label-width="150px"
                >
                    <el-form-item
                        label="Email"
                        prop="email"
                        :rules="[{required:true, message:'The field is required', trigger: 'blur'}]"
                    >
                        <el-input v-model="profileFormData.email" placeholder :maxlength="32"></el-input>
                    </el-form-item>
                    <el-form-item
                        label="Phone"
                        prop="phoneNumber"
                        :rules="[{required:false, message:'The field is required', trigger: 'blur'}]"
                    >
                        <el-input v-model="profileFormData.phoneNumber" placeholder :maxlength="32"></el-input>
                    </el-form-item>
                    <el-form-item
                        label="Display name"
                        prop="displayName"
                        :rules="[{required:false, message:'The field is required', trigger: 'blur'}]"
                    >
                        <el-input v-model="profileFormData.displayName" placeholder :maxlength="32"></el-input>
                    </el-form-item>
                    <el-form-item
                        label="Bio"
                        prop="bio"
                        :rules="[{required:false, message:'The field is required', trigger: 'blur'}]"
                    >
                        <el-input
                            v-model="profileFormData.bio"
                            placeholder
                            type="textarea"
                            :maxlength="32"
                        ></el-input>
                    </el-form-item>
                    <el-form-item label>
                        <el-button type="primary" @click="handleFormSave">Submit</el-button>
                    </el-form-item>
                </el-form>
            </el-tab-pane>
            <el-tab-pane label="Change Password">
                <el-button type="text" @click="handleOpenChangePasswordPage">
                    Go
                    <i class="el-icon-link el-icon--right"></i>
                </el-button>
            </el-tab-pane>
        </el-tabs>
    </div>
</template>

<script>
import { identity, updateProfile } from "@/services/userservice";

export default {
    data() {
        return {
            profileFormData: {}
        };
    },
    mounted() {
        this.loadProfile();
    },
    methods: {
        loadProfile() {
            identity().then(res => {
                this.profileFormData = Object.assign({}, res.data);
            });
        },

        handleFormSave() {
            this.$refs["profileFormData"].validate(valid => {
                if (valid) {
                    updateProfile(this.profileFormData)
                        .then(result => {
                            this.$message.success("Success");
                            this.loadProfile();
                        })
                        .catch(err => {
                            console.log(err);
                            this.$message.error("Error");
                        });
                }
            });
        },

        handleOpenChangePasswordPage() {
            window.open("/account/changepassword");
        }
    }
};
</script>