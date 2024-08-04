import { h, render, Component } from "preact";

export class CopyrightFooter extends Component {
    render() {
        return (
            <footer style="margin-top: 10px; margin-bottom: 10px;">
                <div class="row">
                    <div class="col-12 text-center">
                        <div>Powered by NetBB® Forum Software © NetBB Limited</div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-12 text-center">
                        <div>Privacy | Terms</div>
                    </div>
                </div>
            </footer>
        );
    }
}
