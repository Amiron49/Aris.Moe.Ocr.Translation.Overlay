import * as React from "react";
import {SpatialText} from "./spatial-text";
import {Overlay} from "./overlay";
import {Subscription} from "rxjs";
import {ErrorDisplay} from "./error-display";
import {ResultResponse} from "./ocrTranslate/backgroundOcrTranslateService";
import {OcrTranslateResponse} from "./ocrTranslate/ocrTranslateResponse";
import {LoadingSpinner} from "./loading/loading-spinner";
import "./image-overlay.css"

export class ImageOverlay extends React.Component<ImageOverlayProperties, ImageOverlayState> {
    private subscription: Subscription

    constructor(props: ImageOverlayProperties) {
        super(props);
        this.subscription = props.overlay.translationResult.subscribe(x => {
            this.setState({
                translateResult: x
            })
        })
    }

    componentWillUnmount() {
        this.subscription.unsubscribe();
    }


    render() {
        if (this.state?.loadingAnimationDone && this.state?.translateResult?.success === true) {
            return (
                <div className="honyaku-image-overlay">
                    {this.state.translateResult?.result?.machineTranslations[0].texts.map((x, i) => <SpatialText key={i}
                                                                                                                 spatialText={x}/>)}
                </div>
            );
        }
        if (this.state?.loadingAnimationDone && this.state?.translateResult?.success === false) {
            return (
                <div className="honyaku-image-overlay">
                    <ErrorDisplay result={this.state.translateResult!}/>
                </div>
            );
        }

        return (
            <div className="honyaku-image-overlay">
                <LoadingSpinner observable={this.props.overlay.translationResult} loadingAnimationDone={() => this.loadingDone()}/>
            </div>
        );
    }
    
    private loadingDone()
    {
        let state = {...this.state}
        state.loadingAnimationDone = true;
        this.setState(state)
    }
}


export class ImageOverlayProperties {
    constructor(
        public overlay: Overlay
    ) {
    }
}

class ImageOverlayState {
    translateResult?: ResultResponse<OcrTranslateResponse>
    loadingAnimationDone = false;
}

