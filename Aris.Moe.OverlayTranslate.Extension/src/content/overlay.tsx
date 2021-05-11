import * as ReactDOM from "react-dom";
import * as React from "react";
import {OcrTranslateResponse} from "./ocrTranslate/ocrTranslateResponse";
import {
    ForegroundOcrTranslateService, ResultResponse
} from "./ocrTranslate/backgroundOcrTranslateService";
import {Observable, of, Subject} from "rxjs";
import {ImageOverlay} from "./image-overlay";

export class Overlay {

    private overlay: HTMLDivElement | null = null;
    private imageInfo?: HtmlImageInfo | null;
    private translationResultSubject: Subject<ResultResponse<OcrTranslateResponse>> = new Subject<ResultResponse<OcrTranslateResponse>>()
    public translationResult: Observable<ResultResponse<OcrTranslateResponse>> = this.translationResultSubject;
    private resizeSubject: Subject<{ xScale: number, yScale: number }> = new Subject<{ xScale: number; yScale: number }>();
    public onResize: Observable<{ xScale: number, yScale: number }> = this.resizeSubject;


    constructor(
        public readonly parentContainer: HTMLDivElement,
        public readonly overlayTarget: HTMLImageElement,
        public onTargetLoss: (overlay: Overlay) => void,
        public counter: number
    ) {
    }

    public async attach() {
        if (!this.overlayTarget.currentSrc) {
            await this.waitForCurrentSrcToBeSet(this.overlayTarget);
        }

        await this.waitForImageLoad(this.overlayTarget);

        if (this.overlayTarget.naturalWidth < 200 || this.overlayTarget.naturalHeight < 200) {
            return;
        }

        this.overlay = this.crateSingleImageOverlay(this.overlayTarget)
        this.parentContainer.appendChild(this.overlay);

        try {
            this.imageInfo = await HtmlImageInfo.create(this.overlayTarget)
        } catch (error) {
            this.translationResultSubject.next({
                result: null,
                success: false,
                message: "Extension encountered an error while loading the image:" + error.toString()
            })
        }

        if (this.imageInfo == null) {
            this.translationResultSubject.next({
                result: null,
                success: false,
                message: "Extension couldn't load the image :<"
            })
            return;
        }

        let imageHash = this.imageInfo.hash ? Array.from(this.imageInfo.hash) : null;

        this.recalculateRendering();
        this.startObserving();

        console.log(`${this.counter} requesting translation for ${this.imageInfo.url}`)
        
        ForegroundOcrTranslateService.translatePublicSync({
            imageHash: imageHash,
            imageUrl: this.imageInfo.url,
            height: this.imageInfo.height,
            width: this.imageInfo.width
        }, response => {
            this.translationResultSubject.next(response);
        })

    }

    private waitForCurrentSrcToBeSet(elem: HTMLImageElement): Promise<boolean> {
        return new Promise<boolean>(resolve => {
            setTimeout(async () => {
                if (elem.currentSrc) {
                    resolve(true)
                    return;
                }

                let waitAgain = await this.waitForCurrentSrcToBeSet(elem);
                return resolve(waitAgain);

            }, 25)
        });
    }

    private waitForImageLoad(elem: HTMLImageElement): Promise<void> {
        if (elem.complete)
            return Promise.resolve();

        return new Promise((resolve, reject) => {
            elem.addEventListener('onload', evt => {
                resolve();
            }, {once: true})
            elem.addEventListener('onerror', evt => {
                reject("Failed to load image?");
            }, {once: true})
        });
    }

    private crateSingleImageOverlay(targetElement: HTMLDivElement): HTMLDivElement {
        let container = document.createElement("div");
        container.className = 'honyaku-image-overlay-container'
        container.id = `honyaku-image-overlay-container-${this.counter}`

        ReactDOM.render(<ImageOverlay overlay={this}/>, container);

        return container;
    }

    private static setPosition(target: HTMLDivElement, position: DivRectangle) {
        target.style.left = `${position.x}px`;
        target.style.top = `${position.y}px`;
        target.style.width = `${position.width}px`;
        target.style.height = `${position.height}px`;
    }

    private static getPosition(target: HTMLDivElement): DivRectangle {
        let boundingRectangle = target.getBoundingClientRect();

        let scrollLeft = window.pageXOffset || document.documentElement.scrollLeft;
        let scrollTop = window.pageYOffset || document.documentElement.scrollTop;

        return new DivRectangle(
            scrollLeft + boundingRectangle.x,
            scrollTop + boundingRectangle.y,
            boundingRectangle.width,
            boundingRectangle.height);
    }

    private lastScale: { xScale: number, yScale: number } | null = null;
    private lastPosition: DivRectangle | null = null;
    private detaching = false;

    private recalculateRendering() {
        if (this.detaching)
            return;

        if (this.overlayTarget.src != this.imageInfo?.url) {
            console.log("detected image loss. detaching")
            this.detach();
            return;
        }

        let newPosition = Overlay.getPosition(this.overlayTarget!);

        if (this.lastPosition == null || !this.lastPosition.AreSame(newPosition)) {
            Overlay.setPosition(this.overlay!, newPosition)
            this.lastPosition = newPosition;
        }

        let newScale = Overlay.determineScale(this.overlayTarget, this.imageInfo!);

        if (this.lastScale == null || this.lastScale.xScale != newScale.xScale || this.lastScale.yScale != newScale.yScale) {
            this.resizeSubject.next(newScale);
            this.lastScale = newScale;
        }
    }


    private static determineScale(target: HTMLDivElement, imageInfo: HtmlImageInfo): { xScale: number, yScale: number } {
        let boundingRectangle = target.getBoundingClientRect();
        let xScale = boundingRectangle.width / imageInfo?.width;
        let yScale = boundingRectangle.height / imageInfo?.height;

        return {
            xScale: xScale,
            yScale: yScale
        }
    }

    private mutationObserver: MutationObserver | null = null;

    private startObserving() {
        this.mutationObserver = new MutationObserver(mutations => {
            this.recalculateRendering();
        });

        this.mutationObserver.observe(this.overlayTarget, {
            attributes: true
        });
        this.pollForChanges();
    }

    private timeoutId: number | null = null;

    private pollForChanges() {
        this.timeoutId = window.setTimeout(() => {
            this.recalculateRendering();
            this.checkForTargetLoss();
            if (!this.detaching)
                this.pollForChanges();
        }, 1000);
    }

    private checkForTargetLoss() {
        let isTargetLost = this.isTargetLost();
        if (isTargetLost) {
            console.info("target loss detected")
            this.onTargetLoss(this);
        }
    }

    private isTargetLost(): boolean {
        if (this.overlayTarget.src != this.imageInfo?.url) {
            return true;
        }

        if (this.overlayTarget.width == 0 && this.overlayTarget.height == 0) {
            return true;
        }

        if (this.overlayTarget.parentNode == null) {
            return true;
        }

        if (!this.overlayTarget.parentNode.contains(this.overlayTarget)) {
            return true;
        }

        if (!document.getElementsByTagName("body")[0].contains(this.overlayTarget)) {
            return true;
        }

        return false;
    }

    public detach() {
        if (this.detaching)
            return;

        this.detaching = true;

        if (this.timeoutId != null)
            window.clearTimeout(this.timeoutId)

        if (this.mutationObserver != null)
            this.mutationObserver.disconnect();

        if (this.overlay != null && this.parentContainer.contains(this.overlay)) {
            this.parentContainer.removeChild(this.overlay);
        }
    }
}


export class DivRectangle {
    constructor(
        public x: number,
        public y: number,
        public width: number,
        public height: number,
    ) {
    }

    public AreSame(other: DivRectangle): boolean {
        return this.x == other.x &&
            this.y == other.y &&
            this.width == other.width &&
            this.height == other.height;
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
        try {
            let response = await fetch(element.src, {
                mode: "cors"
            })

            return await (response).arrayBuffer()
        } catch (error) {
            return new ArrayBuffer(0)
        }

    }
}