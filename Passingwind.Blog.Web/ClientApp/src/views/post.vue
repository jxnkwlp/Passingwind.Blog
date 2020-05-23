<template>
    <div>
        <el-card dis-hover>
            <el-row style="margin-bottom: 10px;">
                <el-col :xs="12" :sm="18" :md="20">
                    <el-button-group>
                        <el-button
                            type="primary"
                            icon="el-icon-plus"
                            @click="handleNew"
                            v-permission="['post.edit']"
                        >New</el-button>
                        <el-button
                            type="danger"
                            icon="el-icon-delete"
                            :disabled="tableRowSelect.length == 0"
                            @click="handleDelete"
                            v-permission="['post.delete']"
                        ></el-button>
                        <el-button
                            type="success"
                            icon="el-icon-top"
                            :disabled="tableRowSelect.length == 0"
                            @click="handleSetPublished(true)"
                            v-permission="['post.published']"
                        ></el-button>
                        <el-button
                            type="warning"
                            icon="el-icon-bottom"
                            :disabled="tableRowSelect.length == 0"
                            @click="handleSetPublished(false)"
                            v-permission="['post.published']"
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

            <div class="table-filter-list" style="margin-bottom:15px" v-if="tableFilterList.length">
                <el-tag
                    v-for="tag in tableFilterList"
                    :key="tag.value"
                    closable
                    type="info"
                    @close="handleListFilterRemove(tag)"
                >{{tag.title}}</el-tag>
            </div>

            <div style="margin-top:15px;">
                <el-table
                    :data="tableData"
                    v-loading="tableDataLoading"
                    stripe
                    border
                    @selection-change="handleTableSelectChange"
                    @sort-change="handleTableSortChange"
                    @filter-change="handleTableFilterChange"
                >
                    <el-table-column type="selection" width="50" align="center"></el-table-column>
                    <el-table-column prop="title" label="Title" minWidth="200">
                        <template slot-scope="scope">
                            <router-link
                                class="el-link el-link--default is-underline"
                                v-if="canEdit"
                                :to="{
									name: 'postEdit',
									params: { id: scope.row.id }
								}"
                            >{{ scope.row.title }}</router-link>
                            <span v-else>{{scope.row.title}}</span>
                        </template>
                    </el-table-column>
                    <el-table-column
                        prop="user.userName"
                        label="Author"
                        sortable="custom"
                        minWidth="150"
                    >
                        <template slot-scope="scope">
                            <a
                                href="javascript:;"
                                class="el-link el-link--default is-underline"
                                @click="handleListFilter('userId', scope.row.user.id, scope.row.user.userName)"
                            >
                                <template
                                    v-if="scope.row.user.displayName"
                                >{{ scope.row.user.userName }}({{scope.row.user.displayName}})</template>
                                <template v-else>{{ scope.row.user.userName }}</template>
                            </a>
                        </template>
                    </el-table-column>
                    <el-table-column
                        prop="categoryId"
                        label="Category"
                        minWidth="120"
                        class-name="category"
                    >
                        <template slot-scope="scope">
                            <el-tag
                                class="category"
                                effect="plain"
                                size="small"
                                v-for="(item, index) in scope.row.categories"
                                :key="index"
                                @click="handleListFilter('categoryId', item.id, item.name)"
                            >{{ item.name }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column
                        prop="commentsCount"
                        label="Comments"
                        width="120"
                        sortable="custom"
                        align="center"
                    ></el-table-column>
                    <el-table-column
                        prop="viewsCount"
                        label="Views"
                        width="100"
                        sortable="custom"
                        align="center"
                    ></el-table-column>
                    <el-table-column
                        prop="draft"
                        label="Published"
                        width="150"
                        sortable="custom"
                        align="center"
                        column-key="isDraft"
                        :filters="[{text:'Y', value:false}, {text:'N', value:true}]"
                        :filter-multiple="false"
                    >
                        <template slot-scope="scope">
                            <el-tag
                                :type="
									scope.row.isDraft ? 'info' : 'success'
								"
                                disable-transitions
                            >{{ scope.row.isDraft ? "N" : "Y" }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column
                        prop="publishedTime"
                        label="Published time"
                        sortable="custom"
                        width="160"
                    ></el-table-column>
                    <!-- <el-table-column label="Action" width="160" align="center">
                        <template slot-scope="scope">
                            <el-button size="mini" plain type="primary" @click="handleEdit(scope.row)">Edit</el-button>
                            <el-button size="mini" plain type="danger" @click="handleDelete(scope.row)">Delete</el-button>
                        </template>
                    </el-table-column>-->
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
import * as post from "@/services/post";
import * as identity from "@/services/identity";

export default {
    data() {
        return {
            searchBar: {},

            tableData: [],
            tableDataLoading: false,
            tableDataTotal: 0,
            tableDataPage: 1,
            tableOrders: [],
            tableRowSelect: [],
            tableQueryFilter: {},

            tableColumnFilter: {},
            tableFilterList: []
        };
    },
    computed: {
        canEdit() {
            return identity.hasPermission("page.update");
        }
    },
    mounted() {
        this.loadData();
    },
    methods: {
        loadData() {
            var qs = {
                skip: (this.tableDataPage - 1) * 10,
                limit: 10,
                includeUser: true,
                includeCategory: true,
                orders: this.tableOrders
            };
            qs = Object.assign(qs, this.searchBar);
            qs = Object.assign(qs, this.tableColumnFilter);
            qs = Object.assign(
                qs,
                this.tableFilterList.reduce(function(m, v) {
                    m[v.key] = v.value;
                    return m;
                }, {})
            );

            this.tableRowSelect = [];
            this.tableDataLoading = true;

            post.getlist(qs)
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

        handleTableSelectChange(selection) {
            this.tableRowSelect = selection;
        },

        handleListFilter(key, value, title) {
            var current = this.tableFilterList.find(item => item.key == key);
            if (current) {
                current.value = value;
                current.title = title;
            } else {
                this.tableFilterList.push({
                    key: key,
                    value: value,
                    title: title
                });
            }

            this.handleTablePageChange(1);
        },

        handleListFilterRemove(t) {
            var current = this.tableFilterList.find(item => item.key == t.key);
            var i = this.tableFilterList.indexOf(current);
            if (i >= 0) {
                this.tableFilterList.splice(i, 1);
                this.handleTablePageChange(1);
            }
        },

        handleTableSortChange(e) {
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

        handleTableFilterLink(filter) {
            var q = Object.assign(this.tableQueryFilter, filter);

            if (JSON.stringify(q) != JSON.stringify(this.$route.query))
                this.$router.push({ name: "post", query: q });
        },

        handleTableFilterChange(e) {
            for (const key in e) {
                const element = e[key];
                if (element.length == 0) {
                    this.tableColumnFilter[key] = null;
                } else {
                    this.tableColumnFilter[key] = element[0];
                }
            }

            this.handleTablePageChange(1);
        },

        // new
        handleNew() {
            this.$router.push({
                name: "postEdit"
            });
        },

        // new
        handleEdit(item) {
            this.$router.push({
                name: "postEdit",
                params: {
                    id: item.id
                }
            });
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
                    // remove filter
                    this.tableQueryFilter = {};

                    post.del(items)
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

        // update is publish
        handleSetPublished(value) {
            if (this.tableRowSelect.length == 0) {
                this.$message.info("Unselect");
                return;
            }

            var idList = this.tableRowSelect.map(item => {
                return item.id;
            });

            post.setPublished({ ids: idList, value: value })
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
    },
    watch: {
        tableRowSelect() {
            // console.log(this.tableRowSelect)
        },
        $route() {
            // if (this.$route.query.userId) {
            //     this.filterUserId = this.$route.query.userId;
            // }
            this.loadData();
        }
    }
};
</script>
<style lang="less" scoped>
td.category {
    span + span {
        margin-left: 4px;
    }
}
</style>