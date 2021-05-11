import * as React from "react";
import "./Button.css";
import {ContentScriptControllerProvider, PopupContext} from "../../popup-context";
import {Subscription} from "rxjs";

export class VisibilityButton extends React.Component<any, { visible: boolean }> {

    private stateSubscription: Subscription | null = null;
    
    constructor(props: any) {
        super(props);
        this.state = {
            visible: true
        }
    }

    async componentDidMount() {
        this.stateSubscription = (await (this.context as ContentScriptControllerProvider).get()).stateChange.subscribe(x => {
            this.setState({
                visible: x?.visible!
            })
        })
    }

    render() {
        return (
            <div className="buttonContainer">
                <button className="button" onClick={async () => this.handleClick()}>
                    {this.state.visible ? "Hide overlays" : "Show overlays"}
                </button>
            </div>
        );
    }

    private async handleClick() {
        let controller = await (this.context as ContentScriptControllerProvider).get()

        if (this.state.visible) {
            controller.setOverlayVisibility(false);
            this.setState({
                visible: false
            })
        }
        else {
            controller.setOverlayVisibility(true);
            this.setState({
                visible: true
            })
        }
    }

}
VisibilityButton.contextType = PopupContext;
