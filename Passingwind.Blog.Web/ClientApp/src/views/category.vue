<template>
    <div>
        <el-card dis-hover>
            <el-row style="margin-bottom:10px;">
                <el-col :span="20">
                    <el-button-group>
                        <el-button
                            type="primary"
                            icon="el-icon-plus"
                            @click="handleAdd"
                            v-permission="['category.create']"
                        >New</el-button>
                        <el-button
                            type="danger"
                            icon="el-icon-delete"
                            :disabled="tableRowSelect.length == 0"
                            @click="handleDelete"
                            v-permission="['category.delete']"
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
                    <el-table-column prop="name" label="Name" minWidth="200">
                        <template slot-scope="scope">
                            <el-link @click="handleEdit(scope.row)">{{ scope.row.name }}</el-link>
                        </template>
                    </el-table-column>
                    <el-table-column prop="description" label="Description" minWidth="150"></el-table-column>
                    <el-table-column prop="displayOrder" label="Display Order" minWidth="150"></el-table-column>
                    <el-table-column prop="postCount" label="Post Count" minWidth="150"></el-table-column>
                </el-table>
            </div>
        </el-card>

        <el-dialog
            :visible.sync="formModelShow"
            :title="formModelTitle"
            :close-on-click-modal="false"
        >
            <el-form ref="formModelData" :model="formModelData" label-position="top">
                <el-form-item
                    label="Name"
                    prop="name"
                    :rules="[
                        {
                            required: true,
                            message: 'The field is required',
                            trigger: 'blur'
                        }
                    ]"
                >
                    <el-input v-model="formModelData.name" placeholder :maxlength="16"></el-input>
                </el-form-item>
                <el-form-item
                    label="Slug"
                    prop="slug"
                    :rules="[
                        {
                            required: true,
                            message: 'The field is required',
                            trigger: 'blur'
                        }
                    ]"
                >
                    <el-input v-model="formModelData.slug" placeholder :maxlength="128"></el-input>
                </el-form-item>
                <el-form-item label="Description" prop="description" :rules="[]">
                    <el-input
                        type="textarea"
                        v-model="formModelData.description"
                        placeholder
                        :maxlength="128"
                    ></el-input>
                </el-form-item>
                <el-form-item
                    label="DisplayOrder"
                    prop="displayOrder"
                    :rules="[
                        {
                            required: true,
                            type: 'number',
                            message: 'The field is required',
                            trigger: 'blur'
                        }
                    ]"
                >
                    <el-input-number :max="100000" :min="0" v-model="formModelData.displayOrder"></el-input-number>
                </el-form-item>
                <el-form-item v-permission="['category.edit']">
                    <el-button type="primary" @click="handleFormDataSubmit">Save</el-button>
                </el-form-item>
            </el-form>
        </el-dialog>
    </div>
</template>

<script>
import * as category from "@/services/category";

export default {
    mounted() {
        this.loadData();
    },
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
            formModelData: {}
        };
    },
    computed: {},
    methods: {
        loadData() {
            var qs = Object.assign({}, this.searchBar);

            this.tableRowSelect = [];
            this.tableDataLoading = true;
            category
                .getlist(qs)
                .then(res => {
                    if (res.data) {
                        this.tableData = res.data;
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
                id: 0,
                displayOrder: 100
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

            this.$confirm("Are you sure ?", "Confirm", { type: "warning" })
                .then(() => {
                    category
                        .del(items)
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
                        task = category.update(this.formModelData);
                    else task = category.create(this.formModelData);

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
