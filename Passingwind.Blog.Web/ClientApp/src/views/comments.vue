<template>
    <div>
        <el-card dis-hover>
            <div style="margin-bottom:10px;">
                <el-button-group>
                    <el-button
                        type="danger"
                        icon="el-icon-delete"
                        :disabled="tableRowSelect.length == 0"
                        @click="handleDelete"
                        v-permission="['comment.delete']"
                    ></el-button>
                    <el-button
                        type="warning"
                        :disabled="tableRowSelect.length == 0"
                        @click="handleUpdateApproved(false)"
                        v-permission="['comment.setapprove']"
                    >UnApprove</el-button>
                    <el-button
                        type="success"
                        :disabled="tableRowSelect.length == 0"
                        @click="handleUpdateApproved(true)"
                        v-permission="['comment.setapprove']"
                    >Approve</el-button>
                </el-button-group>
            </div>
            <div class="table-filter-list" style="margin-bottom:15px" v-if="tableFilterList.length">
                <el-tag
                    v-for="tag in tableFilterList"
                    :key="tag.value"
                    closable
                    type="info"
                    @close="handleListFilterRemove(tag)"
                >{{tag.title}}</el-tag>
            </div>
            <div>
                <el-table
                    :data="tableData"
                    v-loading="tableDataLoading"
                    stripe
                    border
                    @selection-change="handleTableSelectChange"
                    @filter-change="handleTableFilterChange"
                >
                    <el-table-column type="selection" width="50" align="center"></el-table-column>
                    <el-table-column width="60" align="center">
                        <el-image
                            class="avatar"
                            style="width:45px; height:45px;"
                            src="https://www.gravatar.com/avatar/7797c9c44ae36f74153460f59ccff34d?d=mm"
                            fit="fill"
                        ></el-image>
                    </el-table-column>
                    <el-table-column
                        prop="name"
                        label="Content"
                        minWidth="200"
                        class-name="content"
                        :show-overflow-tooltip="true"
                    >
                        <template slot-scope="scope">
                            <a
                                class="el-tooltip"
                                @click="handleEdit(scope.row)"
                            >{{scope.row.content}}</a>
                            <!-- <el-link class="el-tooltip" style="width:100%;" @click="handleEdit(scope.row)"></el-link> -->
                        </template>
                    </el-table-column>
                    <el-table-column prop="post.title" label="Post" minWidth="150">
                        <template slot-scope="scope">
                            <a
                                class="el-tooltip"
                                @click="handleListFilter('postId',scope.row.postId,scope.row.post.title)"
                            >{{scope.row.post.title}}</a>
                        </template>
                    </el-table-column>
                    <el-table-column prop="creationTime" label="Create on" minWidth="160"></el-table-column>
                    <el-table-column prop="author" label="Author" minWidth="120">
                        <template slot-scope="scope">
                            <a
                                class="el-tooltip"
                                @click="handleListFilter('author',scope.row.author,scope.row.author)"
                            >{{scope.row.author}}</a>
                        </template>
                    </el-table-column>
                    <el-table-column prop="email" label="Email" minWidth="120">
                        <template slot-scope="scope">
                            <a
                                class="el-tooltip"
                                @click="handleListFilter('email',scope.row.email,scope.row.email)"
                            >{{scope.row.email}}</a>
                        </template>
                    </el-table-column>
                    <el-table-column
                        column-key="approved"
                        prop="isApproved"
                        label="Approved"
                        width="120"
                        align="center"
                        :filters="[{value:'true', text:'Y'}, {value:'false', text:'N'}]"
                        :filter-multiple="false"
                    >
                        <template slot-scope="scope">
                            <el-tag
                                :type="scope.row.isApproved ? 'success' : 'info'"
                                disable-transitions
                            >{{scope.row.isApproved? 'Y':'N'}}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column
                        column-key="spam"
                        prop="isSpam"
                        label="Spam"
                        width="120"
                        align="center"
                        :filters="[{value:'true', text:'Y'}, {value:'false', text:'N'}]"
                        :filter-multiple="false"
                    >
                        <template slot-scope="scope">
                            <el-tag
                                :type="scope.row.isSpam ? 'success' : 'warning'"
                                disable-transitions
                            >{{scope.row.isSpam? 'Y':'N'}}</el-tag>
                        </template>
                    </el-table-column>
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

        <el-dialog :visible.sync="detailModelShow" title="Detail" :close-on-click-modal="false">
            <el-form
                ref="detailModelData"
                :model="detailModelData"
                label-position="left"
                v-if="detailModelData"
            >
                <blockquote>{{detailModelData.content}}</blockquote>

                <el-form-item label="Author">{{detailModelData.author}}</el-form-item>
                <el-form-item label="Email">{{detailModelData.email}}</el-form-item>
                <el-form-item label="Website">{{detailModelData.website}}</el-form-item>
                <el-form-item label="IP">{{detailModelData.ip}}</el-form-item>
                <el-form-item label="Country">{{detailModelData.country}}</el-form-item>
                <el-form-item label="Create on">{{detailModelData.creationTime}}</el-form-item>
                <el-form-item label="Post">{{detailModelData.post.title}}</el-form-item>

                <el-form-item
                    label="Reply Content"
                    prop="replyContent"
                    v-permission="['comment.replay']"
                    :rules="[{required:true, message:'The field is required', trigger: 'blur'}]"
                >
                    <el-input
                        type="textarea"
                        v-model="detailModelData.replyContent"
                        placeholder
                        :maxlength="256"
                        :rows="5"
                    ></el-input>
                </el-form-item>

                <el-form-item v-permission="['comment.replay']">
                    <el-button type="primary" @click="handleFormDataSubmit">Reply</el-button>
                </el-form-item>
            </el-form>
        </el-dialog>
    </div>
</template>

<script>
import * as comments from "@/services/comments";

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

            tableFilterList: [],
            tableColumnFilter: {},

            detailModelShow: false,
            detailModelTitle: "New",
            detailModelData: null
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
            comments
                .getlist(qs)
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

        handleTableFilterChange(e) {
            // console.log(e);
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

        // handle add
        handleAdd() {
            this.detailModelShow = true;
            this.detailModelTitle = "New";
            this.detailModelData = {
                id: "",
                displayOrder: 100
            };
            if (this.$refs["detailModelData"])
                this.$refs["detailModelData"].resetFields();
        },

        // handle add
        handleEdit(row) {
            this.detailModelShow = true;
            this.detailModelTitle = "Edit";
            this.detailModelData = Object.assign({}, row);
            if (this.$refs["detailModelData"])
                this.$refs["detailModelData"].resetFields();
        },

        handleUpdateApproved(approved) {
            if (this.tableRowSelect.length == 0) {
                this.$message.info("Unselect");
                return;
            }

            var items = this.tableRowSelect.map(item => {
                return item.id;
            });

            items.forEach(element => {
                comments
                    .setApproved({
                        value: approved,
                        id: element
                    })
                    .then(res => {
                        if (res.data && !res.data.result) {
                            this.$message.error(res.data.message || "Error");
                        } else {
                            this.$message.success("Success");
                        }
                    })
                    .catch(() => {
                        this.$message.error("Error");
                    });
            });

            this.handleTablePageChange(this.tableDataPage);
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
                    comments
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
            this.$refs["detailModelData"].validate(valid => {
                if (valid) {
                    comments
                        .replay({
                            commentId: this.detailModelData.id,
                            content: this.detailModelData.replyContent
                        })
                        .then(res => {
                            if (res.data && !res.data.result) {
                                this.$message.error(
                                    res.data.message || "Error"
                                );
                            } else {
                                this.$message.success("Success");
                                this.detailModelShow = false;
                                this.loadData();
                            }
                        })
                        .catch(() => {
                            this.$message.error("Error");
                        });
                }
            });
        }
    }
};
</script> 

<style lang="less" scoped>
.content {
    // .avatar {
    //     float: left;
    //     margin-right: 10px;
    // }

    a {
        &:hover {
            text-decoration: underline;
            color: #409eff;
            cursor: pointer;
        }
    }
}

blockquote {
    padding: 10px 20px;
    margin: 0 0 20px;
    font-size: 17.5px;
    border-left: 5px solid #eee;
}
</style>
