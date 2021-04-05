import "./content.css";
import {
    ActivateContentScriptCommand,
    ActivateContentScriptEvent,
    ContentScriptUnloadEvent,
    DeactivateContentScriptCommand,
    DeactivateContentScriptEvent,
    IEvent
} from "./types";
import * as React from "react";
import {Overlay} from "./content/overlay";
import {ArrayHelper} from "./content/arrayHelper";

debugger;

class ContentScriptManager {
    private readonly overlays: Overlay[] = [];
    private readonly overlayContainer: HTMLDivElement;

    constructor() {
        this.overlayContainer = ContentScriptManager.createOverlayContainer();
        this.overlayAllImageTags();
    }

    Init() {
        if ((window as any).honyakuWasActive) {
            console.error("The honyaku extension was already here. Suppressing content script startup")
            return;
        }
        (window as any).honyakuWasActive = true
        console.log("content is alive")

        chrome.runtime.onMessage.addListener((message: IEvent, sender, sendResponse) => {
            if (message.identifier == ActivateContentScriptCommand.identifier) {
                this.activate();
            }
            if (message.identifier == DeactivateContentScriptCommand.identifier) {
                this.deactivate();
            }
        })

        window.addEventListener('pagehide', (event) => {
            // Note: if the browser is able to cache the page, `event.persisted`
            // is `true`, and the state is frozen rather than terminated.
            chrome.runtime.sendMessage(new ContentScriptUnloadEvent());
        }, {capture: true});


        this.activate();
        this.doStuffAgain();
    }

    private activate() {
        console.log("activating content script");
        const body = document.getElementsByTagName("body");
        body[0]?.prepend(this.overlayContainer);
        chrome.runtime.sendMessage(new ActivateContentScriptEvent());
    }

    private deactivate() {
        console.log("deactivating content script");
        this.overlayContainer.parentNode?.removeChild(this.overlayContainer);
        chrome.runtime.sendMessage(new DeactivateContentScriptEvent());
    }
    
    private overlayAllImageTags() {
        let images = document.getElementsByTagName('img');
        let allImages = ArrayHelper.toArray(images).filter(x => x.naturalHeight > 200 && x.naturalWidth > 200);
        let newImages = ArrayHelper.except(allImages, this.overlays.map(x => x.overlayTarget));

        for (const image of newImages) {
            let newOverlay = new Overlay(this.overlayContainer, image)
            newOverlay.attach();
            this.overlays.push(newOverlay);
        }
    }

    private doStuffAgain() {
        setTimeout(() => {
            console.log("I'm alive")
            this.doStuffAgain();
        }, 500)
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
        
        // let observer = new MutationObserver((mutations, observer) => {
        //     for(const mutation of mutations) {
        //         console.log('mutation');
        //         if (mutation.type === 'childList') {
        //             console.log('A child node has been added or removed.');
        //         }
        //         else if (mutation.type === 'attributes') {
        //             console.log('The ' + mutation.attributeName + ' attribute was modified.');
        //         }
        //         console.log(mutation);
        //     }
        // })

        // observer.observe(container, { attributes: true, childList: true, subtree: true });

        return container as HTMLDivElement;
    }
}

let honyakuContentScriptManager = new ContentScriptManager()
honyakuContentScriptManager.Init();

