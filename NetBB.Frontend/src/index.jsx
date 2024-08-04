import { h, render, Component } from "preact";

import {
  UserRegister,
  UserRegistered,
  UserLogin,
  UserLogined,
} from "./user";

import {
  AdminPostsNewPost,
  AdminPostsNewPostCreated,
  DisplayAdminPosts,
  AdminPostsEditPost,
} from "./admin/posts";

function renderElements(definition) {
  for (let renderId in definition) {
    let element = document.getElementById(renderId);
    if (element === undefined || element === null) {
      continue;
    }
    definition[renderId](element);
  }
}

renderElements({
  app_user_register: function (elem) {
    render(<UserRegister />, elem);
  },
  app_user_registered: function (elem) {
    render(<UserRegistered />, elem);
  },
  app_user_login: function (elem) {
    render(<UserLogin />, elem);
  },
  app_user_logined: function (elem) {
    render(<UserLogined />, elem);
  },
  app_admin_posts_new_post: function (elem) {
    render(<AdminPostsNewPost />, elem);
  },
  app_admin_posts_new_post_created: function (elem) {
    render(<AdminPostsNewPostCreated />, elem);
  },
  app_admin_posts: function (elem) {
    render(<DisplayAdminPosts />, elem);
  },
  app_admin_posts_edit_post: function (elem) {
    render(<AdminPostsEditPost />, elem);
  }
});
