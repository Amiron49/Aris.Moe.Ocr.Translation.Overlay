import "./content.css";
import {
    ActivateContentScriptEvent,
    ContentScriptStateQuery,
    ContentScriptUnloadEvent,
    IEvent,
    InWhatTabAmIQuery,
    InWhatTabAmIQueryResponse,
    SetOverlayModeCommand,
    SetOverlayVisibilityCommand,
    TranslateAllImagesCommand,
} from "./events";
import * as React from "react";
import {Overlay} from "./content/overlay";
import {ArrayHelper} from "./content/arrayHelper";
import {Observable} from "rxjs";
import {IContentScriptController} from "./content/IContentScriptController";
import {IContentScriptState} from "./content/IContentScriptState";
import {Mode} from "./content/mode";
import {config} from "./helper";

if (config.development)
    debugger;

class ContentScriptManager implements IContentScriptController {
    private readonly overlays: Overlay[] = [];
    private readonly overlayContainer: HTMLDivElement;
    private readonly state: IContentScriptState;
    private counter: number = 1;
    stateChange: Observable<IContentScriptState | null> = new Observable<IContentScriptState | null>();
    
    constructor(private tabId: number) {
        this.state = {
            visible: false,
            mode: Mode.OnDemand
        }

        this.overlayContainer = ContentScriptManager.createOverlayContainer();
    }

    public async init() {
        if ((window as any).honyakuWasActive) {
            console.error("The honyaku extension was already here. Suppressing content script startup")
            return;
        }
        (window as any).honyakuWasActive = true
        console.info("honyaku is alive")

        window.addEventListener('pagehide', (event) => {
            // Note: if the browser is able to cache the page, `event.persisted`
            // is `true`, and the state is frozen rather than terminated.
            chrome.runtime.sendMessage(new ContentScriptUnloadEvent());
        }, {capture: true});
        
        this.registerCommunicationEvents();
        this.setOverlayVisibility(true);
        this.notifyOfActivation();
    }

    private static createOverlayContainer(): HTMLDivElement {
        let container = document.createElement("div");

        container.className = "honyaku-overlay-container";

        container.innerHTML = `
<style>
    .honyaku-overlay-container {
        pointer-events: none
    }
    
    .honyaku-image-overlay-container {
        position: absolute;
        pointer-events: none;
        /*background: firebrick;*/
        overflow: hidden;
    }
</style>
`;
        return container as HTMLDivElement;
    }

    private registerCommunicationEvents() {
        chrome.runtime.onMessage.addListener((message: IEvent, sender, sendResponse): boolean => {
            console.log("Contentscript saw" + message.identifier);

            switch (message.identifier) {
                case ContentScriptStateQuery.identifier:
                    this.getState().then(x => {
                        sendResponse(x);
                    });
                    return true;
                case TranslateAllImagesCommand.identifier:
                    this.translateAllImages().then(() => {
                        sendResponse(true);
                    })
                    return true;
                case SetOverlayModeCommand.identifier:
                    this.setMode((message as SetOverlayModeCommand).mode)
                    break;
                case SetOverlayVisibilityCommand.identifier:
                    this.setOverlayVisibility((message as SetOverlayVisibilityCommand).visible)
                    break;
            }

            return false;
        })
    }

    private notifyOfActivation() {
        chrome.runtime.sendMessage(new ActivateContentScriptEvent(this.tabId!));
    }

    async getState(): Promise<IContentScriptState> {
        console.log(this.state);
        return this.state;
    }

    setMode(mode: Mode): void {
        switch (mode) {
            case Mode.OnDemand:
                this.deactivateContinuousScan();
                break;
            case Mode.ContinuousScan:
                this.activateContinuousScan();
                break;
        }
    }
    
    private imageWatcher: MutationObserver | null = null;
    
    private activateContinuousScan(){
        if (this.imageWatcher != null)
            return;
        
        this.imageWatcher = new MutationObserver(async mutations => {
            await this.translateAllImages();
        });
        
        this.imageWatcher.observe(document.getElementsByTagName("body")[0], {
            attributes: false,
            childList: true,
            subtree: true
        });

        this.state.mode = Mode.ContinuousScan;
    }

    private deactivateContinuousScan(){
        if (this.imageWatcher == null)
            return;

        this.imageWatcher.disconnect();
        this.imageWatcher = null;
        this.state.mode = Mode.OnDemand;
    }

    setOverlayVisibility(shouldBeVisible: boolean): void {
        if (shouldBeVisible && !this.state.visible) {
            const body = document.getElementsByTagName("body");
            body[0]?.prepend(this.overlayContainer);
            this.state.visible = true;
        }
        else if (!shouldBeVisible && this.state.visible) {
            this.overlayContainer.parentNode?.removeChild(this.overlayContainer);
            this.state.visible = false;
        }
    }

    async translateAllImages(): Promise<void> {
        let images = document.getElementsByTagName('img');
        let allImages = ArrayHelper.toArray(images).filter(x => (!x.className || x.className.indexOf("honyaku") < 0));

        let existingImageTargets = this.overlays.map(x => x.overlayTarget);
        let newImages = ArrayHelper.except(allImages, existingImageTargets, element => element.src);
        for (const image of newImages) {
            let newOverlay = new Overlay(this.overlayContainer, image, overlay => {
                let index = this.overlays.indexOf(overlay);
                this.overlays.splice(index, 1);
            }, this.counter++)
            this.overlays.push(newOverlay);
            newOverlay.attach();
        }
    }

}



let honyakuContentScriptManager: IContentScriptController;

chrome.runtime.sendMessage(new InWhatTabAmIQuery(), async (x:InWhatTabAmIQueryResponse) => {
    honyakuContentScriptManager = new ContentScriptManager(x);
    honyakuContentScriptManager.init();
})




