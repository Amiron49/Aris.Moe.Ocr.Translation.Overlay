import * as React from "react";
import "./Button.css";
import {ContentScriptControllerProvider, PopupContext} from "../../popup-context";

export class TranslateButton extends React.Component<any, any> {
    constructor(props: any) {
        super(props);
    }

    render() {
        return (
            <div className="buttonContainer">
                <button className="button" onClick={async () => this.handleClick()}>
                    Translate all images
                </button>
            </div>
        );
    }

    private async handleClick() {
        let controller = await (this.context as ContentScriptControllerProvider).get()
        await controller.translateAllImages();
    }

}

TranslateButton.contextType = PopupContext;