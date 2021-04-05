import "./spatial-text.css";
import * as React from "react";
import {SpatialTextViewModel} from "./ocrTranslate/spatialTextViewModel";
import {CSSProperties} from "react";

export class SpatialText extends React.Component<{ spatialText: SpatialTextViewModel }, any> {
    constructor(props: { spatialText: SpatialTextViewModel }) {
        super(props);
    }

    render() {
        return (
            <div className="spatial-text" style={this.createStyle()}>
                {this.props.spatialText.text}
            </div>
        );
    }
    
    createStyle(): CSSProperties{
        return {
            position: "absolute",
            top: this.props.spatialText.position.topLeft.y,
            left: this.props.spatialText.position.topLeft.x,
            width: this.props.spatialText.position.bottomRight.x - this.props.spatialText.position.topLeft.x,
            height: this.props.spatialText.position.bottomRight.y - this.props.spatialText.position.topLeft.y
        }
    }
}