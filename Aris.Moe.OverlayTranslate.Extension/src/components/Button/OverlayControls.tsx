import * as React from "react";
import {TranslateButton} from "./TranslateButton";
import {ModeButton} from "./ModeButton";
import {VisibilityButton} from "./VisibilityButton";

export class OverlayControls extends React.Component<any, any> {
    constructor(props: any) {
        super(props);
    }

    render() {
        return (
            <div>
                <TranslateButton/>
                <ModeButton/>
                <VisibilityButton/>
            </div>
        );
    }
}