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

    // render() {
    //     return (
    //         <div>
    //             {this.props.texts.map((x,i) => <div key={i}>lel</div>)}
    //         </div>
    //     );
    // }

    // render() {
    //     return (
    //         <div>
    //             {this.createFlakes()}
    //         </div>
    //     );
    // }
    //
    // private createFlakes() {
    //     let result = [];
    //
    //     for (const text of this.props.texts) {
    //         //result.push(<SpatialText text={text}/>)
    //         result.push(<SpatialText text={text}/>)
    //     }
    //    
    //     return result;
    // }
}