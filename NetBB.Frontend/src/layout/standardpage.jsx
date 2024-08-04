import { h, render, Component } from "preact";

import { Page } from "./shared";
import { Header, GuestNavBar, LoginedNavBar } from "./header";
import { CopyrightFooter } from "./footer"

import {
    UserIsLogin
} from "./aspnetcore_utils"

export class StandardPage extends Component {
    render(props) {
        let isLogined = UserIsLogin();
        return (
            <Page>
                <Header />
                {!isLogined &&
                    <GuestNavBar />
                }
                {isLogined &&
                    <LoginedNavBar />
                }

                {props.children}

                <CopyrightFooter />
            </Page>
        );
    }
}