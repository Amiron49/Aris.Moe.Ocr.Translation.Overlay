import * as ReactDOM from "react-dom";
import * as React from "react";
import {OcrTranslateResponse} from "./ocrTranslate/ocrTranslateResponse";
import {
    ForegroundOcrTranslateService, ResultResponse
} from "./ocrTranslate/backgroundOcrTranslateService";
import {Observable, Subject} from "rxjs";
import {ImageOverlay} from "./image-overlay";

export class Overlay {

    private imageInfo?: HtmlImageInfo | null;
    private translationResultSubject : Subject<ResultResponse<OcrTranslateResponse>> = new Subject<ResultResponse<OcrTranslateResponse>>()
    public translationResult : Observable<ResultResponse<OcrTranslateResponse>> = this.translationResultSubject;
    
    constructor(
        public readonly parentContainer: HTMLDivElement,
        public readonly overlayTarget: HTMLImageElement
    ) {
    }

    public async attach() {
        await this.waitForImageLoad(this.overlayTarget);

        let overlay = this.crateSingleImageOverlay(this.overlayTarget)
        this.parentContainer.appendChild(overlay);
        
        try {
            this.imageInfo = await HtmlImageInfo.create(this.overlayTarget)
        }
        catch (error) {
            this.translationResultSubject.next({
                result: null,
                success: false,
                message: "Extension encountered an error while loading the image:" + error.toString()
            })
        }
        
        if (this.imageInfo == null)
        {
            this.translationResultSubject.next({
                result: null,
                success: false,
                message: "Extension couldn't load the image :<"
            })
            return;
        }

        let imageHash = this.imageInfo.hash ? Array.from(this.imageInfo.hash) : null;

        ForegroundOcrTranslateService.translatePublicSync({
            imageHash: imageHash,
            imageUrl: this.imageInfo.url,
            height: this.imageInfo.height,
            width: this.imageInfo.width
        }, response => {
            this.translationResultSubject.next(response);
        })
    }

    private waitForImageLoad(elem: HTMLImageElement): Promise<void> {
        if (elem.complete)
            return Promise.resolve();

        return new Promise((resolve, reject) => {
            elem.addEventListener('onload', evt => {
                resolve();
            }, {once: true})
            elem.addEventListener('onerror', evt => {
                reject();
            }, {once: true})
        });
    }

    private crateSingleImageOverlay(targetElement: HTMLDivElement): HTMLDivElement {
        let container = document.createElement("div");
        container.className = 'honyaku-image-overlay-container'

        let boundingRectangle = targetElement.getBoundingClientRect();

        let scrollLeft = window.pageXOffset || document.documentElement.scrollLeft;
        let scrollTop = window.pageYOffset || document.documentElement.scrollTop;

        container.style.left = `${scrollLeft + boundingRectangle.x}px`;
        container.style.top = `${scrollTop + boundingRectangle.y}px`;
        container.style.width = `${boundingRectangle.width}px`;
        container.style.height = `${boundingRectangle.height}px`;

        ReactDOM.render(<ImageOverlay overlay={this}/>, container);

        return container;
    }
}


class HtmlImageInfo {
    private constructor(
        public url: string,
        public hash: Uint8Array | null,
        public width: number,
        public height: number,
        public htmlWidth: number,
        public htmlHeight: number,
    ) {
    }

    public static async create(element: HTMLImageElement): Promise<HtmlImageInfo | null> {

        let imageContent = await this.imageToByteArray1(element);

        let shaAsArray: Uint8Array | null = null;

        // If the image has no bytes then that means the js cannot access the image due to CORS restrictions.
        if (imageContent.byteLength != 0) {
            let sha256 = await crypto.subtle.digest('SHA-256', imageContent);
            shaAsArray = new Uint8Array(sha256);
        }

        //todo check currentSrc
        return new HtmlImageInfo(element.src, shaAsArray, element.naturalWidth, element.naturalHeight, element.width, element.height)
    }

    public static async imageToByteArray1(element: HTMLImageElement): Promise<ArrayBuffer> {
        return await (await fetch(element.src, {
            mode: "cors"
        })).arrayBuffer()
    }
}