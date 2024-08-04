import { h, render, Component } from "preact";
import { useState, useEffect } from "preact/hooks";
import Vditor from 'vditor';
import "vditor/dist/index.css";

import {
    StandardPage,
} from "../layout/standardpage"

import {
    AspNetCoreRequestVerificationTokenFieldComponent,
    GetErrorInfoList,
    DefaultRenderObject,
} from "../layout/aspnetcore_utils"

import "../styles/admin.css"

export class AdminPostsEditPost extends Component {
    render() {
        var content = DefaultRenderObject().content;
        var title = DefaultRenderObject().title;
        var postId = DefaultRenderObject().post_id;

        var errorField = [];
        var itemList = GetErrorInfoList();
        if (itemList.length > 0) {
            for (let obj of itemList) {
                errorField.push(<li>{obj.error_value}</li>)
            }
        }

        const [titleVal, setTitleValue] = useState(title);
        const onTitleInput = (e) => {
            setTitleValue(e.currentTarget.value)
        }

        var mainSection
        if (errorField.length > 0) {
            mainSection =
                <div class="row">
                    <div class="col-12" style="margin: 10px">
                        <ul class="post-page-error-info">
                            {errorField}
                        </ul>
                    </div>
                </div>
        } else if (content && title && postId) {
            const [mdval, setMdval] = useState();
            const [vd, setVd] = useState();
            useEffect(() => {
                const vditor = new Vditor("vditor", {
                    after: () => {
                        vditor.setValue(content)
                        setVd(vditor)
                    },
                    minHeight: 500,
                    mode: "sv",
                })
                // Clear the effect
                return () => {
                    vd?.destroy()
                    setVd(undefined)
                }
            }, []);
            let submitButton = () => {
                setMdval(vd.getValue() ?? "");
                return true;
            };
            mainSection =
                <div>
                    <input class="form-control" type="text" name="title" placeholder="Title" autocomplete="off" onInput={onTitleInput} value={titleVal}></input>
                    <div id="vditor" className="vditor" style="margin: 5px 0;" />
                    <input type="hidden" name="content" value={mdval}></input>
                    <input type="hidden" name="content_type" value="markdown_v1"></input>
                    <hr></hr>
                    <div class="row">
                        <div class="col-1 mx-auto" onClick={submitButton}><button class="btn btn-primary" type="submit">提交</button></div>
                    </div>
                </div>
        } else {
            mainSection =
                <div class="row">
                    <div class="col-12" style="margin: 10px">
                        <ul class="post-page-error-info">
                            <li>文章不存在</li>
                        </ul>
                    </div>
                </div>
        }

        return (
            <StandardPage>
                <div class="post-page border rounded-2" style="padding: 10px;">
                    <a href="/admin/posts">返回文章列表</a>
                    <h2>Edit Post</h2>
                    <form method="post" action={"/admin/post/" + postId}>
                        <AspNetCoreRequestVerificationTokenFieldComponent />
                        {mainSection}
                    </form>
                </div>
            </StandardPage>
        );
    }
}

export class DisplayAdminPosts extends Component {
    render() {
        let renderObject = DefaultRenderObject();
        // sample data
        // let listOfPosts = {
        //     "posts": [
        //         { "post_id": 1, "title": "title001svabwb", "last_update_time": 1722337258000 },
        //         { "post_id": 2, "title": "title002svawbrew", "last_update_time": 1722337258000 },
        //         { "post_id": 3, "title": "title03safbwrbw", "last_update_time": 1722337258000 },
        //         { "post_id": 4, "title": "title004svafwef", "last_update_time": 1722207449000 },
        //         { "post_id": 5, "title": "title005sabwarbabr", "last_update_time": 1722293849000 },
        //     ]
        // }
        let listOfPosts = renderObject;

        let hasPosts = listOfPosts && listOfPosts.posts && listOfPosts.posts.length && listOfPosts.posts.length > 0
        var postContent
        if (hasPosts) {
            postContent = <div class="list-group">
                {listOfPosts.posts.map(item =>
                    // <a href={"#" + item.post_id} class="list-group-item list-group-item-action">{item.title}  {new Date(item.last_update_time).toLocaleDateString()}</a>
                    <a href={"/admin/post/" + item.post_id} class="list-group-item list-group-item-action d-flex justify-content-between align-items-center"><span>{item.title}</span><span class="badge text-bg-primary rounded-pill">{new Date(item.last_update_time).toLocaleDateString()}</span></a>
                )}
            </div>
        } else {
            postContent =
                <div class="list-group">
                    <a class="list-group-item list-group-item-action disabled" aria-disabled="true">无文章</a>
                </div>
        }

        return (
            <StandardPage>
                <div class="post-page border rounded-2" style="padding: 10px;">
                    <h1>Post列表</h1>
                    <hr />
                    <div class="d-flex flex-row-reverse" style="margin: 5px 0px">
                        <a href="/admin/posts/new_post" class="btn btn-primary">写文章</a>
                    </div>
                    {postContent}
                </div>
            </StandardPage>
        );
    }
}

export class AdminPostsNewPost extends Component {
    render() {
        const [mdval, setMdval] = useState();
        const [vd, setVd] = useState();
        useEffect(() => {
            const vditor = new Vditor("vditor", {
                after: () => {
                    vditor.setValue("")
                    setVd(vditor)
                },
                minHeight: 500,
                mode: "sv",
            })
            // Clear the effect
            return () => {
                vd?.destroy()
                setVd(undefined)
            }
        }, []);
        let submitButton = () => {
            setMdval(vd.getValue() ?? "");
            return true;
        };

        var errorField = [];
        var itemList = GetErrorInfoList();
        if (itemList.length > 0) {
            for (let obj of itemList) {
                errorField.push(<li>{obj.error_value}</li>)
            }
        }

        return (
            <StandardPage>
                <div class="post-page border rounded-2" style="padding: 10px;">
                    <h2>New Post</h2>
                    <form method="post" action="/admin/posts/new_post">
                        <AspNetCoreRequestVerificationTokenFieldComponent />
                        {errorField.length > 0 &&
                            <div class="row">
                                <div class="col-12" style="margin: 10px">
                                    <ul class="post-page-error-info">
                                        {errorField}
                                    </ul>
                                </div>
                            </div>
                        }
                        <input class="form-control" type="text" name="title" placeholder="Title" autocomplete="off"></input>
                        <div id="vditor" className="vditor" style="margin: 5px 0;" />
                        <input type="hidden" name="content" value={mdval}></input>
                        <input type="hidden" name="content_type" value="markdown_v1"></input>
                        <hr></hr>
                        <div class="row">
                            <div class="col-1 mx-auto" onClick={submitButton}><button class="btn btn-primary" type="submit">提交</button></div>
                        </div>
                    </form>
                </div>
            </StandardPage>
        );
    }
}

export class AdminPostsNewPostCreated extends Component {
    render() {
        return (
            <StandardPage>
                <div class="post-page">TBD new post</div>
            </StandardPage>
        );
    }
}
