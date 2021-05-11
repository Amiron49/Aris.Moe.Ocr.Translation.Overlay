import * as React from "react";
import "./Button.css";
import {ContentScriptControllerProvider, PopupContext} from "../../popup-context";
import {Subscription} from "rxjs";
import {Mode} from "../../content/mode";

export class ModeButton extends React.Component<any, { mode: Mode | null }> {
    
    private stateSubscription: Subscription | null = null;
    
    constructor(props: any) {
        super(props);
        this.state = {
            mode: null
        }
    }
    
    async componentDidMount() {
        this.stateSubscription = (await (this.context as ContentScriptControllerProvider).get()).stateChange.subscribe(x => {
            this.setState({
                mode: x?.mode!
            })
        })
    }

    render() {
        return (
            <div className="buttonContainer">
                <button className="button" onClick={async () => this.handleClick()}>
                    {this.state.mode == Mode.OnDemand ? "Activate auto-detection" : "Disable auto-detection"}
                </button>
            </div>
        );
    }

    private async handleClick() {
        let controller = await (this.context as ContentScriptControllerProvider).get()

        switch (this.state.mode) {
            case Mode.OnDemand:
                controller.setMode(Mode.ContinuousScan);
                this.setState({
                    mode: Mode.ContinuousScan
                })
                break;
            case Mode.ContinuousScan:
                controller.setMode(Mode.OnDemand);
                this.setState({
                    mode: Mode.OnDemand
                })
                break;
        }
    }
    
    componentWillUnmount() {
        this.stateSubscription?.unsubscribe();
    }

}

ModeButton.contextType = PopupContext;
