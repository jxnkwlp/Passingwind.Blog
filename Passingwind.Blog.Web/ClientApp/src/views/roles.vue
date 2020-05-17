<template>
    <div>
        <el-row style="margin-bottom:10px;">
            <el-col :span="20">
                <el-button-group>
                    <el-button
                        type="primary"
                        icon="el-icon-plus"
                        @click="handleAdd"
                        v-permission="['role.create']"
                    >New</el-button>
                    <el-button
                        type="danger"
                        icon="el-icon-delete"
                        :disabled="tableRowSelect.length == 0"
                        v-permission="['role.delete']"
                        @click="handleDelete"
                    ></el-button>
                </el-button-group>
            </el-col>
            <el-col :span="4">
                <el-input
                    placeholder
                    v-model="searchBar.searchTerm"
                    clearable
                    @change="handleSearchTextChanged"
                >
                    <i slot="suffix" class="el-input__icon el-icon-search"></i>
                </el-input>
            </el-col>
        </el-row>
        <el-table
            :data="tableData"
            v-loading="tableDataLoading"
            stripe
            border
            @selection-change="handleTableSelectChange"
        >
            <el-table-column type="selection" width="50" align="center"></el-table-column>
            <el-table-column prop="name" label="Name" minWidth="200">
                <template slot-scope="scope">
                    <el-link @click="handleEdit(scope.row)">{{scope.row.name}}</el-link>
                </template>
            </el-table-column>
        </el-table>
        <div style="margin-top:15px;">
            <el-pagination
                :total="tableDataTotal"
                :current-page="tableDataPage"
                @current-change="handleTablePageChange"
            />
        </div>

        <el-dialog
            :visible.sync="formModelShow"
            :title="formModelTitle"
            :close-on-click-modal="false"
        >
            <el-form ref="formModelData" :model="formModelData" label-position="top">
                <el-form-item
                    label="Name"
                    prop="name"
                    :rules="[{required:true, message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input v-model="formModelData.name" placeholder :maxlength="16"></el-input>
                </el-form-item>
                <el-form-item label="Permissions" :rules="[]">
                    <div style="margin-bottom:15px">
                        <el-collapse v-model="permissionsOpend">
                            <el-collapse-item name="Post" title="Post">
                                <el-checkbox-group v-model="formModelData.permissionKeys">
                                    <el-checkbox label="post.list">Post list</el-checkbox>
                                    <el-checkbox label="post.edit">Post edit</el-checkbox>
                                    <el-checkbox label="post.delete">Post delete</el-checkbox>
                                    <el-checkbox label="post.published">Post published</el-checkbox>
                                </el-checkbox-group>
                            </el-collapse-item>
                            <el-collapse-item name="Comment" title="Comment">
                                <el-checkbox-group v-model="formModelData.permissionKeys">
                                    <el-checkbox label="comment.list">Comment list</el-checkbox>
                                    <el-checkbox label="comment.setapprove">Comment approve</el-checkbox>
                                    <el-checkbox label="comment.replay">Comment replay</el-checkbox>
                                    <el-checkbox label="comment.delete">Comment delete</el-checkbox>
                                </el-checkbox-group>
                            </el-collapse-item>
                            <el-collapse-item name="Page" title="Page">
                                <el-checkbox-group v-model="formModelData.permissionKeys">
                                    <el-checkbox label="page.list">Page list</el-checkbox>
                                    <el-checkbox label="page.create">Page create</el-checkbox>
                                    <el-checkbox label="page.update">Page update</el-checkbox>
                                    <el-checkbox label="page.delete">Page delete</el-checkbox>
                                </el-checkbox-group>
                            </el-collapse-item>
                            <el-collapse-item name="Category" title="Category">
                                <el-checkbox-group v-model="formModelData.permissionKeys">
                                    <el-checkbox label="category.list">Category list</el-checkbox>
                                    <el-checkbox label="category.create">Category create</el-checkbox>
                                    <el-checkbox label="category.update">Category update</el-checkbox>
                                    <el-checkbox label="category.delete">Category delete</el-checkbox>
                                </el-checkbox-group>
                            </el-collapse-item>
                            <el-collapse-item name="Tags" title="Tags">
                                <el-checkbox-group v-model="formModelData.permissionKeys">
                                    <el-checkbox label="tags.list">Tags list</el-checkbox>
                                    <el-checkbox label="tags.create">Tags create</el-checkbox>
                                    <el-checkbox label="tags.delete">Tags delete</el-checkbox>
                                </el-checkbox-group>
                            </el-collapse-item>
                            <el-collapse-item name="Users" title="User">
                                <el-checkbox-group v-model="formModelData.permissionKeys">
                                    <el-checkbox label="user.list">User list</el-checkbox>
                                    <el-checkbox label="user.create">User create</el-checkbox>
                                    <el-checkbox label="user.update">User update</el-checkbox>
                                    <el-checkbox label="user.delete">User delete</el-checkbox>
                                    <el-checkbox label="user.setlock">User setlock</el-checkbox>
                                </el-checkbox-group>
                            </el-collapse-item>
                            <el-collapse-item name="Roles" title="Role">
                                <el-checkbox-group v-model="formModelData.permissionKeys">
                                    <el-checkbox label="role.list">Role list</el-checkbox>
                                    <el-checkbox label="role.create">Role create</el-checkbox>
                                    <el-checkbox label="role.update">Role update</el-checkbox>
                                    <el-checkbox label="role.delete">Role delete</el-checkbox>
                                </el-checkbox-group>
                            </el-collapse-item>
                            <el-collapse-item name="Settings" title="Setting">
                                <el-checkbox-group v-model="formModelData.permissionKeys">
                                    <el-checkbox label="setting.load">Settings load</el-checkbox>
                                    <el-checkbox label="setting.update">Settings update</el-checkbox>
                                </el-checkbox-group>
                            </el-collapse-item>
                        </el-collapse>
                    </div>
                </el-form-item>

                <div class="form-bottom-action" v-permission="['role.update']">
                    <el-button type="primary" @click="handleFormDataSubmit">Save</el-button>
                </div>
            </el-form>
        </el-dialog>
    </div>
</template>

<script>
import * as role from "@/services/role";
import * as userService from "@/services/userservice";

export default {
    data() {
        return {
            searchBar: {},

            tableData: [],
            tableDataLoading: false,
            tableDataTotal: 0,
            tableDataPage: 1,

            tableRowSelect: [],

            formModelShow: false,
            formModelTitle: "New",
            formModelData: {
                id: "",
                name: "",
                displayOrder: 100,
                permissionKeys: []
            },

            permissionsOpend: []
        };
    },
    computed: {},
    mounted() {
        this.loadData();
    },
    methods: {
        loadData() {
            //console.log(userService.hasPermission("role.list"));
            if (!userService.hasPermission("role.list")) {
                return;
            }

            var qs = {
                skip: (this.tableDataPage - 1) * 10,
                limit: 10,
                includePermissionKeys: true
            };

            qs = Object.assign(qs, this.searchBar);

            this.tableRowSelect = [];
            this.tableDataLoading = true;
            role.getlist(qs)
                .then(res => {
                    if (res.data && res.data.value) {
                        this.tableData = res.data.value;
                        this.tableDataTotal = res.data.count;
                    }
                })
                .catch(() => {
                    this.$message.error("Error");
                })
                .then(() => {
                    this.tableDataLoading = false;
                });
        },

        handleTablePageChange(page) {
            this.tableDataPage = page;
            this.loadData();
        },

        handleTableSelectChange(selection, row) {
            this.tableRowSelect = selection;
        },

        handleSearchTextChanged(txt) {
            this.loadData();
        },

        // handle add
        handleAdd() {
            this.permissionsOpend = [];
            this.formModelShow = true;
            this.formModelTitle = "New";
            this.formModelData = {
                id: "",
                name: "",
                displayOrder: 100,
                permissionKeys: []
            };
            if (this.$refs["formModelData"])
                this.$refs["formModelData"].resetFields();
        },

        // handle add
        handleEdit(row) {
            this.permissionsOpend = [];
            this.formModelShow = true;
            this.formModelTitle = "Edit";
            row.permissionKeys = row.permissionKeys ?? [];
            this.formModelData = Object.assign({}, this.formModelData, row);
            this.$refs["formModelData"]?.resetFields();
        },

        // row delete
        handleDelete() {
            if (this.tableRowSelect.length == 0) {
                this.$message.info("Unselect");
                return;
            }

            var items = this.tableRowSelect.map(item => {
                return item.id;
            });

            this.$confirm("Are you sure ?", "Confirm", { type: "warning" })
                .then(() => {
                    role.del(items)
                        .then(res => {
                            if (res.data && !res.data.result) {
                                this.$message.error(
                                    res.data.message || "Error"
                                );
                            } else {
                                this.$message.success("Success");
                                this.loadData();
                            }
                        })
                        .catch(() => {
                            this.$message.error("Error");
                        });
                })
                .catch();
        },

        handleFormDataSubmit() {
            //console.log(this.formModelData);
            this.$refs["formModelData"].validate(valid => {
                if (valid) {
                    var task;
                    if (this.formModelData.id)
                        task = role.update(this.formModelData);
                    else task = role.create(this.formModelData);

                    task.then(res => {
                        if (res.data && !res.data.result) {
                            this.$message.error(res.data.message || "Error");
                        } else {
                            this.$message.success("Success");
                            this.formModelShow = false;
                            this.loadData();
                        }
                    }).catch(() => {
                        this.$message.error("Error");
                    });
                }
            });
        }
    }
};
</script>
