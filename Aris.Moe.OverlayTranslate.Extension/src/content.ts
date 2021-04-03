import "./content.css";
import {
    ActivateContentScriptCommand,
    ActivateContentScriptEvent,
    ContentScriptUnloadEvent,
    DeactivateContentScriptCommand,
    DeactivateContentScriptEvent,
    IEvent
} from "./types";

debugger;

class ContentScriptManager {
    private readonly overlayedImageTags: HTMLImageElement[] = [];
    private readonly overlayContainer: HTMLDivElement;

    constructor() {
        this.overlayContainer = ContentScriptManager.createOverlayContainer();
        this.hookUntoAllImageTags();
    }

    Init() {
        if ((<any>window).honyakuWasActive) {
            console.error("The honyaku extension was already here. Suppressing content script startup")
            return;
        }
        (<any>window).honyakuWasActive = true
        console.log("content is alive")

        chrome.runtime.onMessage.addListener((message: IEvent, sender, sendResponse) => {
            console.log("EVENT BRO");
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
    
    private hookUntoAllImageTags() {
        let allImages = this.toArray(document.getElementsByTagName('img'));
        let newImages = this.except(allImages, this.overlayedImageTags);

        for (const image of newImages) {
            let singleImageOverlay = ContentScriptManager.crateSingleImageOverlay(image)
            this.overlayContainer.appendChild(singleImageOverlay);
        }
    }

    private except<T>(something: T[], except: T[]): T[] {
        let result: T[] = [];

        return something.filter(x => !except.find(y => y == x));
    }


    private toArray<T extends Element>(something: HTMLCollectionOf<T>): T[] {
        let array: T[] = [];

        for (let i = 0; i < something.length; i++) {
            array.push(something.item(i)!);
        }

        return array;
    }



    private doStuffAgain() {
        setTimeout(() => {
            console.log("I'm alive")
            this.doStuffAgain();
        }, 500)
    }

    private static crateSingleImageOverlay(targetElement: HTMLImageElement):HTMLDivElement {
        let container = document.createElement("div");
        container.className = 'honyaku-image-overlay-container'
        
        let boundingRectangle = targetElement.getBoundingClientRect();

        let scrollLeft = window.pageXOffset || document.documentElement.scrollLeft;
        let scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        
        container.style.left = `${scrollLeft + boundingRectangle.x}px`;
        container.style.top = `${scrollTop + boundingRectangle.y}px`;
        container.style.width = `${boundingRectangle.width}px`;
        container.style.height = `${boundingRectangle.height}px`;
        
        let snowContainer = this.createSnowflakesContainer();
        
        container.appendChild(snowContainer);
        
        console.log(`x:${boundingRectangle.x} y:${boundingRectangle.y} w:${boundingRectangle.width} h:${boundingRectangle.height}`)
        
        return container;
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

        return <HTMLDivElement>container;
    }
    
    private static createSnowflakesContainer(): HTMLDivElement {
        let container = document.createElement("div");
        container.className = "snowflakes";
        container.setAttribute("aria-hidden", "true");

        const snowflake = document.createElement("div");
        snowflake.className = "snowflake";
        snowflake.innerHTML = "❆";

        for (let i = 0; i < 12; i++) {
            container.appendChild(snowflake.cloneNode(true));
        }
        
        return container;
    }
}

let honyakuContentScriptManager = new ContentScriptManager()
honyakuContentScriptManager.Init();
