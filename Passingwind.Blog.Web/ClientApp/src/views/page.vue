<template>
    <div>
        <el-card dis-hover>
            <el-row>
                <el-col :xs="12" :sm="18" :md="20">
                    <el-button-group>
                        <el-button
                            type="primary"
                            icon="el-icon-plus"
                            @click="handleNew"
                            v-permission="['page.create']"
                        >New</el-button>
                        <el-button
                            type="danger"
                            icon="el-icon-delete"
                            :disabled="tableRowSelect.length == 0"
                            @click="handleDelete"
                            v-permission="['page.delete']"
                        ></el-button>
                    </el-button-group>
                </el-col>
                <el-col :xs="12" :sm="6" :md="4">
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

            <div style="margin-top:15px;">
                <el-table
                    :data="tableData"
                    v-loading="tableDataLoading"
                    stripe
                    border
                    @selection-change="handleTableSelectChange"
                >
                    <el-table-column type="selection" width="50" align="center"></el-table-column>
                    <el-table-column prop="title" label="Title" minWidth="200">
                        <template slot-scope="scope">
                            <router-link
                                class="el-link el-link--default is-underline"
                                v-if="canEdit"
                                :to="{
									name: 'pageEdit',
									params: { id: scope.row.id }
								}"
                            >{{ scope.row.title }}</router-link>
                            <span v-else>{{ scope.row.title }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column prop="displayOrder" label="Display order" minWidth="150"></el-table-column>
                </el-table>
            </div>
            <div style="margin-top:15px;">
                <el-pagination
                    :total="tableDataTotal"
                    :current-page="tableDataPage"
                    @current-change="handleTablePageChange"
                />
            </div>
        </el-card>
    </div>
</template>

<script>
import * as page from "@/services/page";
import * as identity from "@/services/identity";

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

            tableRowSelect: []
        };
    },
    computed: {
        canEdit() {
            return identity.hasPermission("page.update");
        }
    },
    methods: {
        loadData() {
            var qs = {
                skip: (this.tableDataPage - 1) * 10,
                limit: 10
            };

            qs = Object.assign(qs, this.searchBar);

            this.tableRowSelect = [];
            this.tableDataLoading = true;
            page.getlist(qs)
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

        handleSearchTextChanged(txt) {
            this.loadData();
        },

        handleTablePageChange(page) {
            this.tableDataPage = page;
            this.loadData();
        },

        handleTableSelectChange(selection, row) {
            this.tableRowSelect = selection;
        },

        // new
        handleNew() {
            this.$router.push({
                name: "pageCreate"
            });
        },
        // row delete
        handleDelete() {
            if (this.tableRowSelect.length == 0) {
                this.$message.info("Unselect");
                return;
            }

            var idList = this.tableRowSelect.map(item => {
                return item.id;
            });

            this.$confirm("Are you sure you want to delete?", "Confirm")
                .then(result => {
                    page.del(idList)
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
                .catch(err => {
                    this.$message.error("Delete faild.");
                });
        }
    },
    watch: {
        tableRowSelect() {
            // console.log(this.tableRowSelect)
        }
    }
};
</script>


