import * as React from "react";
import {Helper} from "../helper";
import {IsActiveInTabQuery, IsActiveInTabQueryResponse} from "../types";

export class Snow extends React.Component<SnowProperties, any> {
    constructor(props: SnowProperties) {
        super(props);
    }

    render() {
        return (
            <div className="snowflakes" aria-hidden="true">
                <div className="snowflake">
                    {this.createFlakes()}
                </div>
            </div>
        );
    }
    
    private createFlakes() {
        let result = [];

        for (let i = 0; i < this.props.amount; i++) {
            result.push(<div className="snowflake">❆</div>)
        }

        return result;
    }
}

export class SnowProperties {
    public amount: number = 12;
}