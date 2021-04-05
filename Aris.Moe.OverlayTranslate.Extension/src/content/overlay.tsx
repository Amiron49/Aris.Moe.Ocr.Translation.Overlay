import * as ReactDOM from "react-dom";
import * as React from "react";
import {OcrTranslateResponse} from "./ocrTranslate/ocrTranslateResponse";
import {SpatialTexts} from "./spatial-texts";
import {
    ForegroundOcrTranslateService
} from "./ocrTranslate/backgroundOcrTranslateService";

export class Overlay {

    private imageInfo?: HtmlImageInfo;

    constructor(
        public readonly parentContainer: HTMLDivElement,
        public readonly overlayTarget: HTMLImageElement
    ) {
    }

    async attach() {
        await this.waitForImageLoad(this.overlayTarget);
        this.imageInfo = await HtmlImageInfo.create(this.overlayTarget)
        let imageHash = Array.from(this.imageInfo.hash);
        
        console.log(imageHash);
        
        // let translation = await ForegroundOcrTranslateService.translatePublic({
        //     imageHash: imageHash,
        //     imageUrl: this.imageInfo.url
        // })
        //
        // if (!translation.success) {
        //     console.error(translation.message)
        //     return;
        // }
        //
        // let overlay = Overlay.crateSingleImageOverlay(this.overlayTarget, translation.result!)
        // this.parentContainer.appendChild(overlay);

       ForegroundOcrTranslateService.translatePublicSync({
            imageHash: imageHash,
            imageUrl: this.imageInfo.url
        }, response => {
            if (!response.success || response.result?.machineTranslations.length == 0) {
                console.error(response.message)
                return;
            }

            let overlay = Overlay.crateSingleImageOverlay(this.overlayTarget, response.result!)
            this.parentContainer.appendChild(overlay);
        })
    }

    waitForImageLoad(elem: HTMLImageElement): Promise<void> {
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

    private static crateSingleImageOverlay(targetElement: HTMLDivElement, translation: OcrTranslateResponse): HTMLDivElement {
        let container = document.createElement("div");
        container.className = 'honyaku-image-overlay-container'

        let boundingRectangle = targetElement.getBoundingClientRect();

        let scrollLeft = window.pageXOffset || document.documentElement.scrollLeft;
        let scrollTop = window.pageYOffset || document.documentElement.scrollTop;

        container.style.left = `${scrollLeft + boundingRectangle.x}px`;
        container.style.top = `${scrollTop + boundingRectangle.y}px`;
        container.style.width = `${boundingRectangle.width}px`;
        container.style.height = `${boundingRectangle.height}px`;

        ReactDOM.render(<SpatialTexts texts={translation.machineTranslations[0].texts}/>, container);

        return container;
    }
}


class HtmlImageInfo {
    private constructor(
        public url: string,
        public hash: Uint8Array,
        public width: number,
        public height: number,
        public htmlWidth: number,
        public htmlHeight: number,
    ) {
    }

    public static async create(element: HTMLImageElement): Promise<HtmlImageInfo> {

        let imageContent = await this.imageToByteArray1(element);
        let sha256 = await crypto.subtle.digest('SHA-256', imageContent);
        let asArray = new Uint8Array(sha256);
        //todo check currentSrc
        return new HtmlImageInfo(element.src, asArray, element.naturalWidth, element.naturalHeight, element.width, element.height)
    }

    public static async imageToByteArray1(element: HTMLImageElement): Promise<ArrayBuffer> {
        return await (await fetch(element.src)).arrayBuffer()
    }
}