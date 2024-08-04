import { h, render, Component } from "preact";

export function ObtainRenderJsonFromHtmlPage() {
    // get object from the html page
    return ____renderJson____
}

export function DefaultRenderObject() {
  return (ObtainRenderJsonFromHtmlPage() && ObtainRenderJsonFromHtmlPage().render_object) ?? {}
}

export function ObtainAspNetCoreRequestVerificationTokenFromHtmlPage() {
    return ObtainRenderJsonFromHtmlPage().request_verification_token || '';
}

export class AspNetCoreRequestVerificationTokenFieldComponent extends Component {
    render(props) {
        if (!props.token) {
            props.token = ObtainAspNetCoreRequestVerificationTokenFromHtmlPage()
        }
        return <input type='hidden' name='__RequestVerificationToken' value={props.token}></input>
    }
}

export function GetErrorInfoList() {
    let renderObject = ObtainRenderJsonFromHtmlPage();
    let errorInfoObj = renderObject['error_info']
    if (errorInfoObj) {
      var itemList = [];
      let allKeys = Object.keys(errorInfoObj).sort();
      for (let i = 0; i < allKeys.length; i++) {
        itemList.push({
          "error_key": allKeys[i],
          "error_value": errorInfoObj[allKeys[i]]
        });
      }
      return itemList;
    }
    return [];
}

export function UserIsLogin() {
  let renderObject = ObtainRenderJsonFromHtmlPage();
  if (renderObject && renderObject.login) {
    let logined = renderObject.login.is_logined ? true : false;
    return logined;
  }
  return false;
}

export function UserNickName() {
  let renderObject = ObtainRenderJsonFromHtmlPage();
  if (renderObject && renderObject.login) {
    return renderObject.login.nickname || "";
  }
  return "";
}
