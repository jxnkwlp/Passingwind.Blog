<template>
    <div>
        <el-row style="margin-bottom:10px;">
            <el-col :span="20">
                <el-button-group>
                    <el-button
                        type="primary"
                        icon="el-icon-plus"
                        @click="handleAdd"
                        v-permission="['settings.create']"
                    >New</el-button>
                    <el-button
                        type="danger"
                        icon="el-icon-delete"
                        :disabled="tableRowSelect.length == 0"
                        @click="handleDelete"
                        v-permission="['settings.delete']"
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
            >
                <el-table-column type="selection" width="50" align="center"></el-table-column>
                <el-table-column prop="key" label="Key" minWidth="200">
                    <template slot-scope="scope">
                        <el-link @click="handleEdit(scope.row)">{{ scope.row.key }}</el-link>
                    </template>
                </el-table-column>
                <el-table-column prop="value" label="Value" minWidth="150"></el-table-column>
            </el-table>
        </div>
        <div style="margin-top:15px;">
            <el-pagination
                :total="tableDataTotal"
                :current-page="tableDataPage"
                @current-change="handleTablePageChange"
            ></el-pagination>
        </div>

        <el-dialog
            :visible.sync="formModelShow"
            :title="formModelTitle"
            :close-on-click-modal="false"
        >
            <el-form ref="formModelData" :model="formModelData" label-position="left">
                <el-form-item
                    label="Key"
                    prop="key"
                    :rules="[
                        {
                            required: true,
                            message: 'The field is required',
                            trigger: 'blur'
                        }
                    ]"
                >
                    <el-input v-model="formModelData.key" placeholder :maxlength="32"></el-input>
                </el-form-item>
                <el-form-item
                    label="Value"
                    prop="value"
                    :rules="[
                        {
                            required: true,
                            message: 'The field is required',
                            trigger: 'blur'
                        }
                    ]"
                >
                    <el-input v-model="formModelData.value" placeholder :maxlength="128"></el-input>
                </el-form-item>
                <el-form-item v-permission="['category.edit']">
                    <el-button type="primary" @click="handleFormDataSubmit">Save</el-button>
                </el-form-item>
            </el-form>
        </el-dialog>
    </div>
</template>

<script>
import * as settings from "@/services/settings";

export default {
    data() {
        return {
            searchBar: {},

            tableColumns: [],
            tableData: [],
            tableDataLoading: false,
            tableDataTotal: 0,
            tableDataPage: 1,

            tableRowSelect: [],

            formModelShow: false,
            formModelTitle: "New",
            formModelData: {}
        };
    },
    mounted() {
        this.loadData();
    },
    computed: {},
    methods: {
        loadData() {
            var qs = {
                skip: (this.tableDataPage - 1) * 10,
                limit: 10
            };

            qs = Object.assign(qs, this.searchBar);

            this.tableRowSelect = [];
            this.tableDataLoading = true;
            settings
                .list(qs)
                .then(res => {
                    if (res.data) {
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
            this.formModelShow = true;
            this.formModelTitle = "New";
            this.formModelData = {
                id: 0
            };
            if (this.$refs["formModelData"])
                this.$refs["formModelData"].resetFields();
        },

        // handle add
        handleEdit(row) {
            this.formModelShow = true;
            this.formModelTitle = "Edit";
            this.formModelData = Object.assign({}, row);
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

            settings
                .del(items)
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
        },

        handleFormDataSubmit() {
            this.$refs["formModelData"].validate(valid => {
                if (valid) {
                    if (!this.formModelData.userId)
                        this.formModelData["userId"] = "";
                    var task = settings.addOrUpdate(this.formModelData);

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


