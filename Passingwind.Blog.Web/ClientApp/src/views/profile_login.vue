<template>
    <div>
        <el-table :data="tableData" v-loading="tableDataLoading" stripe border>
            <el-table-column prop="key" label="Provider" minWidth="200">
                <template slot-scope="scope">
                    <span
                        :class="'fa fa-'+ scope.row.name.toLocaleLowerCase() "
                        style="margin-right:3px"
                    ></span>
                    <span>{{scope.row.name}}</span>
                </template>
            </el-table-column>
            <el-table-column prop="value" label="Action" minWidth="150">
                <template slot-scope="scope">
                    <el-button
                        type="primary"
                        size="mini"
                        v-if="scope.row.providerKey"
                        @click="handleUnlink(scope.row.name, scope.row.providerKey)"
                    >UnLink</el-button>
                    <el-button
                        type="primary"
                        size="mini"
                        v-else
                        @click="handleLink(scope.row.name)"
                    >Link</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
</template>
 
<script>
import * as account from "@/services/account";

export default {
    data() {
        return {
            tableData: [],
            tableDataLoading: false,

            loginSchemes: [],
            userLogins: []
        };
    },
    mounted() {
        this.loadData();
    },
    methods: {
        loadData() {
            var task1 = account.getExternalLoginSchemes().then(result => {
                this.loginSchemes = result.data;
            });
            var task2 = account.getLogins().then(result => {
                this.userLogins = result.data;
            });

            Promise.all([task1, task2])
                .then(result => {
                    this.tableDataLoading = true;
                    this.tableData = this.loginSchemes.map(item => {
                        var find = this.userLogins.find(
                            i => i.loginProvider == item.name
                        );
                        return {
                            name: item.name,
                            displayName: item.displayName,
                            providerKey: (find && find.providerKey) || ""
                        };
                    });
                })
                .catch(err => {})
                .then(() => {
                    this.tableDataLoading = false;
                });
        },

        handleUnlink(name, key) {
            this.$confirm(
                `Are you sure you want to remove '${name}' login ?`,
                "Confirm"
            )
                .then(result => {
                    account
                        .removeLogin({ loginProvider: name, providerKey: key })
                        .then(result => {
                            this.$message.success("Success");
                            this.loadData();
                        })
                        .catch(err => {});
                })
                .catch(err => {});
        },

        handleLink(name) {
            window.open("/account/LinkLogin?provider=" + name);
        }
    }
};
</script>

<style lang="less" scoped>
</style>