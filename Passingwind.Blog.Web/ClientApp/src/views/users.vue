<template>
    <div>
        <el-row style="margin-bottom:10px;">
            <el-col :span="20">
                <el-button-group>
                    <el-button
                        type="primary"
                        icon="el-icon-plus"
                        @click="handleAdd"
                        v-permission="['user.create']"
                    >New</el-button>
                    <el-button
                        type="info"
                        icon="el-icon-unlock"
                        title="Unlock"
                        :disabled="tableRowSelect.length == 0"
                        v-permission="['user.setlock']"
                        @click="handleLockUser(false)"
                    ></el-button>
                    <el-button
                        type="warning"
                        icon="el-icon-lock"
                        title="Lock"
                        :disabled="tableRowSelect.length == 0"
                        v-permission="['user.setlock']"
                        @click="handleLockUser(true)"
                    ></el-button>
                    <el-button
                        type="danger"
                        icon="el-icon-delete"
                        title="Delete"
                        :disabled="tableRowSelect.length == 0"
                        v-permission="['user.delete']"
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
        <div>
            <el-table
                :data="tableData"
                v-loading="tableDataLoading"
                stripe
                border
                @selection-change="handleTableSelectChange"
                @sort-change="handleTableSortChange"
            >
                <el-table-column type="selection" width="50" align="center"></el-table-column>
                <el-table-column prop="userName" label="UserName" minWidth="150">
                    <template slot-scope="scope">
                        <el-link @click="handleEdit(scope.row)">{{scope.row.userName}}</el-link>
                    </template>
                </el-table-column>
                <el-table-column prop="email" label="Email" minWidth="150">
                    <template slot-scope="scope">
                        {{scope.row.email}}
                        <el-tag
                            type="warning"
                            size="small"
                            title="Unconfirmed"
                            v-if="!scope.row.emailConfirmed"
                        >N</el-tag>
                    </template>
                </el-table-column>
                <el-table-column prop="displayName" label="DisplayName" minWidth="150"></el-table-column>
                <el-table-column prop="userDescription" label="Description" minWidth="150"></el-table-column>
                <el-table-column prop="bio" label="Bio" minWidth="150"></el-table-column>
                <el-table-column
                    prop="lockoutEnabled"
                    label="Lockout"
                    width="110"
                    align="center"
                    sortable="custom"
                >
                    <template slot-scope="scope">
                        <el-tooltip
                            v-if="scope.row.lockoutEnd"
                            :content="scope.row.lockoutEnd"
                            placement="top"
                        >
                            <el-tag type="warning" disable-transitions>Y</el-tag>
                        </el-tooltip>
                        <span v-else>-</span>
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
        </div>

        <el-dialog
            :visible.sync="formModelShow"
            :title="formModelTitle"
            :close-on-click-modal="false"
        >
            <el-form
                ref="formModelData"
                :model="formModelData"
                label-position="right"
                label-width="120px"
            >
                <el-form-item
                    label="UserName"
                    prop="userName"
                    :rules="[{required:true, message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input v-model="formModelData.userName" placeholder :maxlength="16"></el-input>
                </el-form-item>
                <el-form-item
                    label="Email"
                    prop="email"
                    :rules="[{required:true, message:'The field is required', trigger: 'blur'}, { type:'email', message:'The field is must be email', trigger: 'blur'}]"
                >
                    <el-input v-model="formModelData.email" placeholder :maxlength="32"></el-input>
                </el-form-item>
                <el-form-item prop="emailConfirmed" :rules="[]">
                    <el-checkbox v-model="formModelData.emailConfirmed" label="Email confirmed"></el-checkbox>
                </el-form-item>
                <el-form-item
                    label="Display name"
                    prop="displayName"
                    :rules="[{required:true, message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input v-model="formModelData.displayName" placeholder :maxlength="16"></el-input>
                </el-form-item>
                <el-form-item label="Description" prop="userDescription">
                    <el-input
                        type="textarea"
                        v-model="formModelData.userDescription"
                        placeholder
                        :maxlength="128"
                    ></el-input>
                </el-form-item>
                <el-form-item label="Bio" prop="bio">
                    <el-input
                        type="textarea"
                        v-model="formModelData.bio"
                        placeholder
                        :maxlength="256"
                    ></el-input>
                </el-form-item>
                <!-- <el-form-item prop="lockoutEnabled" :rules="[]">
                    <el-checkbox v-model="formModelData.lockoutEnabled" label="Lockouted"></el-checkbox>
                </el-form-item>-->
                <el-form-item label="Password" prop="password" :rules="[]">
                    <el-input v-model="formModelData.password" :maxlength="32"></el-input>
                </el-form-item>
                <el-form-item label="Roles">
                    <el-checkbox-group v-model="formModelData.roleIds">
                        <el-checkbox
                            v-for="item in roleList"
                            :label="item.id"
                            :key="item.id"
                        >{{item.name}}</el-checkbox>
                    </el-checkbox-group>
                </el-form-item>
                <el-form-item v-permission="['user.update']">
                    <el-button type="primary" @click="handleFormDataSubmit">Save</el-button>
                </el-form-item>
            </el-form>
        </el-dialog>

        <el-dialog title="LockEnd" :visible.sync="LockEndDialogVisible" width="30%">
            <span>Lock expiration date</span>
            <el-date-picker v-model="LockEndDate" placeholder></el-date-picker>

            <span slot="footer" class="dialog-footer">
                <el-button @click="LockEndDialogVisible = false">取 消</el-button>
                <el-button type="primary" @click="handleSetLockEndDate">确 定</el-button>
            </span>
        </el-dialog>
    </div>
</template>

<script>
import * as user from "@/services/user";
import * as role from "@/services/role";
import * as identity from "@/services/identity";

export default {
    props: ["actived"],
    data() {
        return {
            searchBar: {},

            tableData: [],
            tableDataLoading: false,
            tableDataTotal: 0,
            tableDataPage: 1,
            tableOrders: [],

            roleList: [],

            tableRowSelect: [],

            formModelShow: false,
            formModelTitle: "New",
            formModelData: { id: "", displayOrder: 100, roleIds: [] },

            LockEndDialogVisible: false,
            LockEndDate: null
        };
    },
    computed: {},
    activated() {},
    mounted() {
        this.load();
    },
    methods: {
        load() {
            if (identity.hasPermission("user.list")) {
                this.loadBasicData();
                this.loadData();
            }
        },

        loadBasicData() {
            role.getlist({ limit: 1024 })
                .then(res => {
                    if (res.data && res.data.value) {
                        this.roleList = res.data.value;
                    }
                })
                .catch(err => {});
        },

        loadData() {
            var qs = {
                skip: (this.tableDataPage - 1) * 10,
                limit: 10,
                includeRoles: true,
                orders: this.tableOrders
            };

            qs = Object.assign(qs, this.searchBar);

            this.tableRowSelect = [];
            this.tableDataLoading = true;
            user.getlist(qs)
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

        handleTableSortChange(e) {
            // console.log(e);
            this.tableOrders = [];
            if (e.order) {
                var orders = [
                    {
                        field: e.prop,
                        order: e.order == "ascending" ? "asc" : "desc"
                    }
                ];
                this.tableOrders = orders;
            }
            this.loadData();
        },

        // handle add
        handleAdd() {
            this.formModelShow = true;
            this.formModelTitle = "New";
            this.formModelData = {
                id: "",
                displayOrder: 100,
                roleIds: []
            };
            if (this.$refs["formModelData"])
                this.$refs["formModelData"].resetFields();
        },

        // handle add
        handleEdit(row) {
            this.formModelShow = true;
            this.formModelTitle = "Edit";
            this.formModelData = Object.assign({}, { roleIds: [] }, row);
            this.formModelData["roleIds"] =
                row.roles.map(item => {
                    return item.id;
                }) || [];
            if (this.$refs["formModelData"])
                this.$refs["formModelData"].resetFields();
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
                    user.del(items)
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
            this.$refs["formModelData"].validate(valid => {
                if (valid) {
                    var task;
                    if (this.formModelData.id)
                        task = user.update(this.formModelData);
                    else task = user.create(this.formModelData);

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
        },

        handleLockUser(value) {
            if (this.tableRowSelect.length == 0) {
                this.$message.info("Unselect");
                return;
            }

            var items = this.tableRowSelect.map(item => {
                return item.id;
            });

            this.$confirm("Are you sure Lock/Unlock those user?", "Confirm", {
                type: "warning"
            }).then(() => {
                var endDate = null;
                if (value) {
                    this.LockEndDialogVisible = true;
                    return;
                }

                user.setLock({
                    userIds: items,
                    value: value,
                    lockEnd: endDate
                })
                    .then(res => {
                        if (res.data && !res.data.result) {
                            this.$message.error(res.data.message || "Error");
                        } else {
                            this.$message.success("Success");
                            this.loadData();
                        }
                    })
                    .catch(() => {
                        this.$message.error("Error");
                    });
            });
        },

        handleSetLockEndDate() {
            var items = this.tableRowSelect.map(item => {
                return item.id;
            });

            user.setLock({
                userIds: items,
                value: true,
                lockEnd: this.LockEndDate
            })
                .then(res => {
                    if (res.data && !res.data.result) {
                        this.$message.error(res.data.message || "Error");
                    } else {
                        this.$message.success("Success");
                        this.loadData();
                    }
                })
                .catch(() => {
                    this.$message.error("Error");
                })
                .then(() => {
                    this.LockEndDialogVisible = false;
                });
        }
    },
    watch: {
        actived() {
            // console.log(this.actived);
            if (this.actived) this.loadBasicData();
        }
    }
};
</script>
