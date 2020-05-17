<template>
    <div>
        <el-card dis-hover>
            <el-row style="margin-bottom:10px;">
                <el-col :span="20">
                    <el-button-group>
                        <el-button
                            type="danger"
                            icon="el-icon-delete"
                            :disabled="tableRowSelect.length == 0"
                            @click="handleDelete"
                            v-permission="['tags.delete']"
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
                    <el-table-column prop="name" label="Name" minWidth="200"></el-table-column>
                    <el-table-column prop="postCount" label="Post Count" minWidth="150"></el-table-column>
                </el-table>
            </div>
            <div style="margin-top:15px;">
                <el-pagination
                    :total="tableDataTotal"
                    :current-page="tableDataPage"
                    @current-change="handleTablePageChange"
                ></el-pagination>
            </div>
        </el-card>
    </div>
</template>

<script>
import * as tags from "@/services/tags";
export default {
    mounted() {
        this.loadData();
    },
    data() {
        return {
            searchBar: {},

            tableColumns: [
                {
                    type: "selection",
                    width: 50
                },
                {
                    title: "Name",
                    minWidth: 200,
                    key: "name"
                },
                {
                    title: "Post Count",
                    minWidth: 100,
                    key: "postCount"
                }
            ],
            tableData: [],
            tableDataLoading: false,
            tableDataTotal: 0,
            tableDataPage: 1,

            tableRowSelect: []
        };
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
            tags.getlist(qs)
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

        // row delete
        handleDelete() {
            if (this.tableRowSelect.length == 0) {
                this.$message.info("Unselect");
                return;
            }

            var items = this.tableRowSelect.map(item => {
                return item.name;
            });

            tags.del(items)
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
        }
    }
};
</script>


