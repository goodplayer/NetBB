import { h, render, Component } from "preact";

import {
  UserNickName
} from "./aspnetcore_utils"

export class Header extends Component {
  render() {
    return (
      <nav class="navbar bg-body-tertiary border rounded-2 header-container">
        <div class="container-fluid">
          <a class="navbar-brand" href="/index">
            <img
              src="/favicon.ico"
              alt="Logo"
              width="30"
              height="24"
              class="d-inline-block align-text-top"
            />
            NetBB
          </a>
        </div>
      </nav>
    );
  }
}

export class GuestNavBar extends Component {
  render() {
    return (
      <div class="hstack gap-3 border rounded-2 header-container">
        <div class="p-2">Quick Menu</div>
        <div class="p-2 ms-auto">
          <a href="/user/register">Register</a>
        </div>
        <div class="p-2">
          <a href="/user/login">Login</a>
        </div>
      </div>
    );
  }
}

export class LoginedNavBar extends Component {
  render(props) {
    let nickname = UserNickName();

    return (
      <div class="hstack gap-3 border rounded-2 header-container">
        <div class="p-2">Quick Menu</div>
        <div class="p-2 ms-auto">
          <a href="#">{nickname}</a>
        </div>
      </div>
    );
  }
}
