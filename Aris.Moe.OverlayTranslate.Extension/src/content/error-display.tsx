import * as React from "react";
import {ResultResponse} from "./ocrTranslate/backgroundOcrTranslateService";

export class ErrorDisplay extends React.Component<{ result: ResultResponse<any> }> {
    constructor(props: { result: ResultResponse<any> }) {
        super(props);
    }

    render() {
        return (
            <div className="error-message">
                {this.props.result.message}
            </div>
        );
    }
}