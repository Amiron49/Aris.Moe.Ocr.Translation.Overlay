import * as React from "react";
import {OverlayControls} from "./components/Button/OverlayControls";
import {ContentScriptControllerProvider, PopupContext} from "./popup-context";

export class PopupComponent extends React.Component<any, { ready: boolean }> {
    constructor(props: any) {
        super(props);
        this.state = {
            ready: false
        }
    }
    
    async componentDidMount() {
        let controls = await (this.context as ContentScriptControllerProvider).get();
        await controls.init();
        this.setState({
            ready: true
        });
    }

    render() {
        if (!this.state.ready)
        return (
            <div>loading</div>
        );
        return (
            <OverlayControls/>
        );
    }
}
PopupComponent.contextType = PopupContext;




