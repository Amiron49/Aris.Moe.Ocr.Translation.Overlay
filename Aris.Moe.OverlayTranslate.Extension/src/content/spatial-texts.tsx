import * as React from "react";
import {SpatialTextViewModel} from "./ocrTranslate/spatialTextViewModel";
import {SpatialText} from "./spatial-text";

export class SpatialTexts extends React.Component<{ texts: SpatialTextViewModel[] }, any> {
    constructor(props: { texts: SpatialTextViewModel[] }) {
        super(props);
    }

    render() {
        return (
            <div>
                {this.props.texts.map((x,i) => <SpatialText key={i} spatialText={x}/>)}  
            </div>
        );
    }
}