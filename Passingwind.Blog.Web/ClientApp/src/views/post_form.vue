<style lang="less">
.post-edit-page {
    //   .ivu-divider {
    //     margin-bottom: 10px;
    //   }
    //   .category-part {
    //     padding-left: 10px;

    .el-checkbox {
        display: block;
        margin-bottom: 10px;
    }
    //   }
}
</style>

<template>
    <div class="post-edit-page">
        <el-form ref="formData" :model="formData" label-position="top">
            <el-row :gutter="30">
                <el-col :span="18">
                    <el-card dis-hover>
                        <el-form-item
                            prop="title"
                            :rules="[
                                {
                                    required: true,
                                    message: 'The field is required',
                                    trigger: 'blur'
                                }
                            ]"
                        >
                            <el-input v-model="formData.title" placeholder="Please input title"></el-input>
                        </el-form-item>
                        <el-form-item
                            prop="content"
                            :rules="[
                                {
                                    required: true,
                                    message: 'The field is required',
                                    trigger: 'blur'
                                }
                            ]"
                        >
                            <template v-if="editorName == 'markdown'">
                                <mavon-editor
                                    ref="mavonEditor"
                                    v-model="formData.content"
                                    placeholder
                                    style="height: 500px"
                                    @imgAdd="handleMarkdownEditorImageUpload"
                                ></mavon-editor>
                            </template>
                            <template v-else>
                                <editor
                                    id="tinymce1"
                                    v-model="formData.content"
                                    :init="editorOption2"
                                    style="height:500px;"
                                ></editor>
                            </template>
                        </el-form-item>
                        <el-form-item
                            label="Slug"
                            prop="slug"
                            :rules="[{
                                    required: true,
                                    message: 'The field is required',
                                    trigger: 'blur'
                                }]"
                        >
                            <el-input v-model="formData.slug" placeholder :maxlength="128"></el-input>
                        </el-form-item>
                        <el-form-item label="Description">
                            <el-input v-model="formData.description" type="textarea" placeholder></el-input>
                        </el-form-item>
                    </el-card>
                </el-col>
                <el-col :span="6">
                    <el-card dis-hover>
                        <div>
                            <div v-if="formData.id">
                                <el-button
                                    type="info"
                                    plain
                                    class="block mb-10"
                                    @click="handlePostPreview"
                                >GO TO POST</el-button>
                                <!--  -->
                                <div v-if="formData.isDraft">
                                    <el-button
                                        type="primary"
                                        class="block mb-10"
                                        :loading="submitLoading"
                                        @click="handleSubmit(true)"
                                    >PUBLISH</el-button>
                                    <el-button
                                        type="primary"
                                        plain
                                        class="block mb-10"
                                        :loading="submitLoading"
                                        @click="handleSubmit(false)"
                                    >SAVE</el-button>
                                </div>
                                <div v-else>
                                    <el-button
                                        type="warning"
                                        class="block mb-10"
                                        :loading="submitLoading"
                                        @click="handleSubmit(false)"
                                    >UNPUBLISH</el-button>
                                    <el-button
                                        type="primary"
                                        plain
                                        class="block mb-10"
                                        :loading="submitLoading"
                                        @click="handleSubmit(true)"
                                    >SAVE</el-button>
                                </div>
                            </div>
                            <div v-else>
                                <el-button
                                    type="success"
                                    class="block mb-10"
                                    @click="handleSubmit(true)"
                                >PUBLISH</el-button>
                                <el-button
                                    type="primary"
                                    plain
                                    class="block mb-10"
                                    @click="handleSubmit(false)"
                                >SAVE</el-button>
                            </div>
                        </div>

                        <el-divider>CATEGORIES</el-divider>
                        <div class="category-part">
                            <el-checkbox-group v-model="formData.categories">
                                <el-checkbox
                                    v-for="(item, index) in allCategory"
                                    :key="index"
                                    :label="item.id"
                                >{{ item.name }}</el-checkbox>
                            </el-checkbox-group>
                            <template v-if="allCategory.length == 0">
                                <div class="text-align-center">
                                    <el-link disabled>NO CATEGORIES</el-link>
                                </div>
                            </template>
                        </div>

                        <el-divider>TAGS</el-divider>
                        <div>
                            <el-select
                                v-model="formData.tags"
                                multiple
                                filterable
                                allow-create
                                default-first-option
                                placeholder="请选择文章标签"
                                style="display:block;"
                            >
                                <el-option
                                    v-for="(item, index) in allTags"
                                    :key="index"
                                    :label="item"
                                    :value="item"
                                ></el-option>
                            </el-select>
                        </div>

                        <el-divider>DATE</el-divider>
                        <div>
                            <el-form-item
                                prop="publishedTime"
                                :rules="[
                                    {
                                        required: true,
                                        type: 'date',
                                        message: 'The field is required',
                                        trigger: 'change'
                                    }
                                ]"
                            >
                                <el-date-picker
                                    type="datetime"
                                    style="width:100%;"
                                    v-model="formData.publishedTime"
                                ></el-date-picker>
                            </el-form-item>
                        </div>

                        <el-divider>AUTHOR</el-divider>
                        <div>
                            <el-form-item
                                prop="userId"
                                :rules="[
                                    {
                                        required: true,
                                        message: 'The field is required',
                                        trigger: 'change'
                                    }
                                ]"
                            >
                                <el-select v-model="formData.userId" style="display:block;">
                                    <el-option
                                        v-for="(item, index) in allUsers"
                                        :key="index"
                                        :value="item.id"
                                        :label="item.name"
                                    ></el-option>
                                </el-select>
                            </el-form-item>
                        </div>

                        <el-divider>OPTIONS</el-divider>
                        <div style="padding-left:10px;">
                            <el-checkbox v-model="formData.enableComment">Enable Comments</el-checkbox>
                        </div>
                    </el-card>
                </el-col>
            </el-row>
        </el-form>
    </div>
</template>
<script>
import { mavonEditor } from "mavon-editor";
import "mavon-editor/dist/css/index.css";
// tinymce
import tinymce from "tinymce/tinymce";
import "tinymce/themes/silver";
import Editor from "@tinymce/tinymce-vue";

import * as post from "@/services/post";
import * as category from "@/services/category";
import * as tags from "@/services/tags";
import * as user from "@/services/user";
import * as settings from "@/services/settings";
import * as file from "@/services/file";

import dayjs from "dayjs";

export default {
    components: {
        mavonEditor,
        Editor
    },
    data() {
        return {
            postId: "",
            formData: {
                // publishedTime: dayjs().format("YYYY-MM-dd HH:mm:ss"),
                publishedTime: new Date(),
                enableComment: true,
                categories: [],
                tags: []
            },
            allCategory: [],
            allTags: [],
            tags: [],
            allUsers: [],

            editorName: "markdown",

            submitLoading: false,

            editorOption2: {
                // language: "zh_CN",
                plugins:
                    "print preview fullpage searchreplace autolink directionality visualblocks visualchars fullscreen image link media  template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount  imagetools textpattern help code  ",
                toolbar:
                    "formatselect | bold italic strikethrough forecolor backcolor permanentpen formatpainter | link image media pageembed | alignleft aligncenter alignright alignjustify  | numlist bullist outdent indent | removeformat | codesample",
                image_advtab: true,
                template_cdate_format: "[CDATE: %m/%d/%Y : %H:%M:%S]",
                template_mdate_format: "[MDATE: %m/%d/%Y : %H:%M:%S]",
                image_caption: true,
                spellchecker_dialog: true,
                relative_urls: false,
                images_upload_url: "/api/file/editor/tinymce"
            }
        };
    },
    created() {
        //  tinymce localpath
        window.tinymce.baseURL = process.env.BASE_URL + "/static/tinymce";
    },
    mounted() {
        this.loadBasicData();
        this.postId = this.$route.params.id;
        this.formData["id"] = this.postId;
        if (this.postId) {
            this.loadData();
        }
    },
    methods: {
        loadBasicData() {
            category.getlist().then(res => {
                if (res.data) this.allCategory = res.data;
            });

            tags.getlist({ allowpage: false }).then(res => {
                if (res.data && res.data.value)
                    this.allTags = res.data.value.map(item => {
                        return item.name;
                    });
            });

            user.getlist({ limit: 1024 }).then(res => {
                if (res.data && res.data.value)
                    this.allUsers = res.data.value.map(item => {
                        var name = item.userName;
                        if (item.displayName)
                            name = `${item.userName}(${item.displayName})`;

                        return {
                            name: name,
                            id: item.id
                        };
                    });
            });

            settings.load({ name: "advanced" }).then(result => {
                if (result.data && result.data.editorName) {
                    this.editorName = result.data.editorName;
                }
            });
        },

        loadData() {
            post.get({
                id: this.postId,
                includeUser: true,
                includeCategory: true,
                includeTags: true
            })
                .then(res => {
                    if (res.data)
                        this.formData = Object.assign(
                            {},
                            this.formData,
                            res.data
                        );

                    this.editorName =
                        (this.formData.isMarkdownText ? "markdown" : "") &&
                        "markdown";

                    if (!res.data.categories) this.formData.categories = [];
                    else {
                        this.formData.categories = res.data.categories.map(
                            item => {
                                return item.id;
                            }
                        );
                    }
                    if (this.formData.publishedTime)
                        this.formData["publishedTime"] = dayjs(
                            this.formData.publishedTime
                        ).toDate();
                })
                .catch(() => {
                    this.$message.error("Can't load post.");
                    this.$router.go(-1);
                })
                .then(() => {});
        },

        handleSubmit(published) {
            //console.log(this.formData);
            this.$refs["formData"].validate(valid => {
                if (valid) {
                    this.submitLoading = true;
                    this.formData["isDraft"] = !published;
                    var task;
                    post.edit(this.formData)
                        .then(res => {
                            if (res.data) {
                                this.$message.success("Success");
                                if (res.data.id && !this.postId)
                                    this.$router.push({
                                        name: "postEdit",
                                        params: {
                                            id: res.data.id
                                        }
                                    });
                                else {
                                    // this.$router.push({ name: "post" });
                                }
                            } else {
                                this.$router.push({ name: "post" });
                            }
                        })
                        .catch(() => {
                            this.$message.error("Error");
                        })
                        .then(() => {
                            this.submitLoading = false;
                        });
                }
            });
        },

        handlePostPreview() {
            window.open("/post/" + this.formData.slug);
        },

        handleMarkdownEditorImageUpload(pos, $file) {
            let formdata = new FormData();
            formdata.append("file", $file);
            file.upload(formdata)
                .then(res => {
                    // console.log(res);
                    if (res.data) {
                        this.$refs.mavonEditor.$img2Url(pos, res.data.uriPath);
                    } else {
                        this.$message.error(res.data.message || "Upload error");
                    }
                })
                .catch(err => {
                    console.log(err);
                });
        }
    },
    watch: {
        $route(from, to) {
            var params = this.$route.params;
            if (params && params.id) {
                this.postId = params.id;
            } else {
                this.postId = 0;
            }

            this.formData["id"] = this.postId;
            this.$forceUpdate();
        }
    }
};
</script>
