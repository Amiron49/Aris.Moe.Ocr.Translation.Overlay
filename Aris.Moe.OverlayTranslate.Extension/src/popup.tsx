import * as React from "react";
import * as ReactDOM from "react-dom";
import "./popup.css";
import {PopupComponent} from "./popup.component";

let mountNode = document.getElementById("popup");
ReactDOM.render(<PopupComponent />, mountNode);
