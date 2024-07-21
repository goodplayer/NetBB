import { h, render, Component } from "preact";

export class Page extends Component {
  render(props) {
    return (
      <div class="page-container mx-auto rounded-2 border">
        {props.children}
      </div>
    );
  }
}
