import "./spatial-text.css";
import * as React from "react";
import {SpatialTextViewModel} from "./ocrTranslate/spatialTextViewModel";
import {CSSProperties} from "react";

export class SpatialText extends React.Component<{ spatialText: SpatialTextViewModel, xScale: number, yScale: number }, any> {
    constructor(props: { spatialText: SpatialTextViewModel, xScale: number, yScale: number }) {
        super(props);
    }

    render() {
        return (
            <div className="spatial-text" style={this.createStyle()}>
                <span>{this.props.spatialText.text}</span>
            </div>
        );
    }

    shittyAutoFontSize(): number {
        let measurements = this.createMeasurements();
        let fontSize = 80;

        let div = document.createElement("div");
        div.style.position = 'absolute';
        div.style.left = '-99vh';
        div.style.top = '-99vh';
        div.style.height = `${measurements.height}px`;
        div.style.width = `${measurements.width}px`;
        div.style.fontSize = `${fontSize}px`;
        div.innerText = this.props.spatialText.text;

        document.body.appendChild(div)

        while (this.isOverflown(div) && fontSize > 1) {
            fontSize = fontSize - 1;
            div.style.fontSize = `${fontSize}px`;
        }

        document.body.removeChild(div)

        return fontSize;
    }

    isOverflown(element: HTMLElement): boolean {
        return element.scrollHeight > element.clientHeight || element.scrollWidth > element.clientWidth;
    }

    createStyle(): CSSProperties {
        let measurements = this.createMeasurements();
        let fontSize = this.shittyAutoFontSize();
        return {
            position: "absolute",
            top: measurements.y,
            left: measurements.x,
            width: measurements.width,
            height: measurements.height,
            fontSize: fontSize
        }
    }

    private createMeasurements(): {
        x: number,
        y: number,
        width: number,
        height: number
    } {
        let y = this.props.spatialText.position.topLeft.y;
        let x = this.props.spatialText.position.topLeft.x;
        let width = this.props.spatialText.position.bottomRight.x - x;
        let height = this.props.spatialText.position.bottomRight.y - y;
        return {
            y: y * this.props.yScale,
            x: x * this.props.xScale,
            width: width * this.props.xScale,
            height: height * this.props.yScale
        }
    }
}