import { h, render, Component } from "preact";

import { Page } from "./layout/shared";
import { Header, GuestNavBar, LoginedNavBar } from "./layout/header";
import { CopyrightFooter } from "./layout/footer"
import { StandardPage } from "./layout/standardpage"

import {
  AspNetCoreRequestVerificationTokenFieldComponent,
  GetErrorInfoList,
  UserIsLogin,
  DefaultRenderObject
} from "./layout/aspnetcore_utils"

import './styles/user.css'

export class UserLogined extends Component {
  render() {
    // redirect url handling
    var redirectInfo = "/"
    if (DefaultRenderObject().redirect != undefined) {
      redirectInfo = DefaultRenderObject().redirect
    }

    return (
      <StandardPage>
        <div class="user-register-form border rounded-2">
          <h2 style="margin: 10px">Information</h2>
          <hr style="margin: 10px" />
          <p style="margin: 10px">You have logged in successfully.</p>
          <p style="margin: 10px"><a href={redirectInfo}>Return to the index page</a></p>
        </div>
      </StandardPage>
    );
  }
}

export class UserLogin extends Component {
  render() {
    var errorField = [];
    var itemList = GetErrorInfoList();
    if (itemList.length > 0) {
      for (let obj of itemList) {
        errorField.push(<li>{obj.error_value}</li>)
      }
    }

    // redirect url handling
    var redirectInfo = ""
    if (DefaultRenderObject().redirect != undefined) {
      redirectInfo = "?redirect=" + DefaultRenderObject().redirect
    }
    var fullPostUrl = "/user/login" + redirectInfo

    return (
      <StandardPage>
        <form method="post" action={fullPostUrl}>
          <div class="user-register-form border rounded-2">
            <AspNetCoreRequestVerificationTokenFieldComponent />
            <div class="row">
              <div>Login</div>
            </div>
            {errorField.length > 0 &&
              <div class="row">
                <div class="col-3">&nbsp;</div>
                <div class="col-9">
                  <ul class="user-register-error-info">
                    {errorField}
                  </ul>
                </div>
              </div>
            }
            <div class="row">
              <div class="col-3">Username:</div>
              <div class="col-9"><input type="text" name="username" autoComplete='off'></input></div>
            </div>
            <div class="row">
              <div class="col-3">Password:</div>
              <div class="col-9"><input type="password" name="password" autoComplete='off'></input></div>
            </div>
          </div>
          <div class="user-register-form border rounded-2">
            <div class="row">
              <div class="col-1 mx-auto"><button type="submit">提交</button></div>
            </div>
          </div>
        </form>
      </StandardPage>
    )
  }
}

export class UserRegistered extends Component {
  render() {
    return (
      <StandardPage>
        <div class="user-register-form border rounded-2">
          <h2 style="margin: 10px">Information</h2>
          <hr style="margin: 10px" />
          <p style="margin: 10px">Thank you for registering, your account has been created.</p>
          <p style="margin: 10px"><a href="/index">Return to the index page</a></p>
          <p style="margin: 10px"><a href="/user/login">Jump to login page</a></p>
        </div>
      </StandardPage>
    );
  }
}

export class UserRegister extends Component {

  render() {
    var errorField = [];
    var itemList = GetErrorInfoList();
    if (itemList.length > 0) {
      for (let obj of itemList) {
        errorField.push(<li>{obj.error_value}</li>)
      }
    }

    return (
      <StandardPage>
        <form method="post" action="/user/register">
          <div class="user-register-form border rounded-2">
            <AspNetCoreRequestVerificationTokenFieldComponent />
            <div class="row">
              <div>Registration</div>
            </div>
            {errorField.length > 0 &&
              <div class="row">
                <div class="col-3">&nbsp;</div>
                <div class="col-9">
                  <ul class="user-register-error-info">
                    {errorField}
                  </ul>
                </div>
              </div>
            }
            <div class="row">
              <div class="col-3">Username:</div>
              <div class="col-9"><input type="text" name="username" autoComplete='off'></input></div>
            </div>
            <div class="row">
              <div class="col-3">Password:</div>
              <div class="col-9"><input type="password" name="password" autoComplete='off'></input></div>
            </div>
            <div class="row">
              <div class="col-3">Confirm password:</div>
              <div class="col-9"><input type="password" name="confirmpassword" autoComplete='off'></input></div>
            </div>
            <div class="row">
              <div class="col-3">Email address:</div>
              <div class="col-9"><input type="email" name="email" autoComplete='off'></input></div>
            </div>
            <div class="row">
              <div class="col-3">Nickname:</div>
              <div class="col-9"><input type="text" name="nickname" autoComplete='off'></input></div>
            </div>
          </div>
          <div class="user-register-form border rounded-2">
            <div class="row">
              <div class="col-1 mx-auto"><button type="submit">提交</button></div>
            </div>
          </div>
        </form>
      </StandardPage>
    );
  }
}
