import * as React from "react";
import {
    ActivateContentScriptCommand,
    DeactivateContentScriptCommand,
    IsActiveInTabQuery,
    IsActiveInTabQueryResponse
} from "../../types";
import "./Button.css";
import {Helper} from "../../helper";

export class Button extends React.Component<any, ButtonState> {
    constructor(props: any) {
        super(props);
        this.state = { 
            snowing: false,
            contentScriptActive: false 
        }
    }
    
    async componentDidMount() {
        let currentTab = await Helper.getCurrentActiveTabId();
        chrome.runtime.sendMessage(new IsActiveInTabQuery(currentTab), (response: IsActiveInTabQueryResponse) => {
            this.setState({
                snowing: response.state,
                contentScriptActive: response.active
            })         
        });
    }

    render() {
        const onClick = async () => {
            console.log("buttoan");
            let currentTab = await Helper.getCurrentActiveTabId();
            
            if (!this.state.contentScriptActive) {
                chrome.runtime.sendMessage(new ActivateContentScriptCommand(currentTab))
                this.setState({
                    snowing: true,
                    contentScriptActive: true
                })
                return;
            }
            
            if (!this.state.snowing) {
                chrome.tabs.sendMessage(currentTab, new ActivateContentScriptCommand(currentTab));
                this.setState({
                    snowing: true,
                    contentScriptActive: true
                })
            }
            else {
                chrome.tabs.sendMessage(currentTab, new DeactivateContentScriptCommand(currentTab));
                this.setState({
                    snowing: false,
                    contentScriptActive: true
                })
            }
        };

        return (
            <div className="buttonContainer">
                <button className="snowButton" onClick={async () => onClick()}>
                    {this.state.snowing ? "Disable the snow 🥶" : "Let it snow! ❄️"}
                </button>
            </div>
        );
    }
}

export interface ButtonState {
    snowing: boolean;
    contentScriptActive: boolean;
}

// export class Button extends React.Component {
//   const [snowing, setSnowing] = React.useState(true);
//
//   React.useEffect(() => {
//   chrome.runtime.sendMessage({ type: "REQ_SNOW_STATUS" });
//
//   chrome.runtime.onMessage.addListener((message: MessageTypes) => {
//   switch (message.type) {
//   case "SNOW_STATUS":
//       setSnowing(message.snowing);
//   break;
//   default:
//   break;
// }
// });
// }, []);
//
// const onClick = () => {
//   chrome.runtime.sendMessage(new ActivateContentScriptEvent());
// };
//
// redner
// return (
//     <div className="buttonContainer">
//       <button className="snowButton" onClick={onClick}>
//         {snowing ? "Disable the snow 🥶" : "Let it snow! ❄️"}
//       </button>
//     </div>
// );
// };
