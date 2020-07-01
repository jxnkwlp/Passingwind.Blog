 
<template>
    <div class="post-edit-page">
        <el-card dis-hover>
            <el-form ref="formData" :model="formData" label-position="top">
                <el-form-item
                    label="Title"
                    prop="title"
                    :rules="[
						{
							required: true,
							message: 'The field is required',
							trigger: 'blur'
						}
					]"
                >
                    <el-input v-model="formData.title" placeholder></el-input>
                </el-form-item>
                <el-form-item
                    label
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
                    :rules="[
						{
							required: true,
							message: 'The field is required',
							trigger: 'blur'
						}
					]"
                >
                    <el-input v-model="formData.slug" placeholder :maxlength="128"></el-input>
                </el-form-item>
                <el-form-item label="keywords">
                    <el-input v-model="formData.keywords" placeholder></el-input>
                </el-form-item>
                <el-form-item label="Description">
                    <el-input type="textarea" v-model="formData.description" placeholder></el-input>
                </el-form-item>
                <el-form-item label="Description">
                    <el-input-number :max="10000" :min="0" v-model="formData.displayOrder"></el-input-number>
                </el-form-item>
                <el-form-item>
                    <el-checkbox v-model="formData.published">Published</el-checkbox>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" :loading="submitLoading" @click="handleSubmit">SAVE</el-button>
                </el-form-item>
            </el-form>
        </el-card>
    </div>
</template>
<script>
import { mavonEditor } from "mavon-editor";
import "mavon-editor/dist/css/index.css";

// tinymce
import tinymce from "tinymce/tinymce";
import "tinymce/themes/silver";
import Editor from "@tinymce/tinymce-vue";

import dayjs from "dayjs";
import * as page from "@/services/page";
import * as settings from "@/services/settings";
import * as file from "@/services/file";

export default {
    components: {
        mavonEditor,
        Editor
    },
    data() {
        return {
            pageId: 0,
            formData: {
                published: true,
                displayOrder: 100
            },

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
        this.pageId = this.$route.params.id;

        if (this.pageId) {
            this.formData["id"] = this.pageId;
            this.loadData();
        }
    },
    methods: {
        loadBasicData() {
            settings.load({ name: "advanced" }).then(result => {
                if (result.data && result.data.editorName) {
                    this.editorName = result.data.editorName;
                }
            });
        },

        loadData() {
            page.get({ id: this.pageId })
                .then(res => {
                    if (res.data) this.formData = Object.assign({}, res.data);

                    this.editorName =
                        (this.formData.isMarkdownText ? "markdown" : "") &&
                        "markdown";
                })
                .then(() => {});
        },

        handleSubmit(published) {
            this.$refs["formData"].validate(valid => {
                if (valid) {
                    this.submitLoading = true;
                    this.formData["isDraft"] = !published;
                    var task;
                    if (this.formData.id) task = page.update(this.formData);
                    else task = page.create(this.formData);

                    task.then(res => {
                        if (res.data) {
                            this.$message.success("Success");
                            if (!this.formData.id)
                                this.$router.push({
                                    name: "pageEdit",
                                    params: {
                                        id: res.data.id
                                    }
                                });
                        } else {
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
                this.pageId = params.id;
            } else {
                this.pageId = "";
            }

            this.formData["id"] = this.pageId;
        }
    }
};
</script>

