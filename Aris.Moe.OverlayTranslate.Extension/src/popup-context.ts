import {config, Helper} from "./helper";
import {Context} from "react";
import * as React from "react";
import {
    ActivateContentScriptCommand,
    ContentScriptStateQuery,
    SetOverlayModeCommand,
    SetOverlayVisibilityCommand, TranslateAllImagesCommand
} from "./events";
import {BehaviorSubject, Observable, ReplaySubject} from "rxjs";
import {IContentScriptController} from "./content/IContentScriptController";
import {IContentScriptState} from "./content/IContentScriptState";
import {Mode} from "./content/mode";

export class ContentScriptControllerProvider {
    private cache: IContentScriptController | null = null;

    async get(): Promise<IContentScriptController> {
        if (this.cache != null)
            return this.cache

        let currentTabId = await Helper.getCurrentActiveTabId();
        return this.cache = new BackgroundFriendlyContentScriptController(currentTabId);
    }
}

export class BackgroundFriendlyContentScriptController implements IContentScriptController {

    private stateChangeSubject: ReplaySubject<IContentScriptState | null> = new ReplaySubject<IContentScriptState | null>(1)
    stateChange: Observable<IContentScriptState | null> = this.stateChangeSubject.asObservable();
    lastState: IContentScriptState | null = null;

    constructor(private tabId: number) {
        this.stateChange.subscribe(x => {
            this.lastState = x ?? null;
        })
    }

    getState(): Promise<IContentScriptState | null> {
        if (config.development)
            console.log("popup sending" + ContentScriptStateQuery.identifier)

        return new Promise<IContentScriptState | null>((resolve) => {

            try {
                chrome.tabs.sendMessage(this.tabId, new ContentScriptStateQuery(), response => {
                    this.stateChangeSubject.next(response);
                    if (!response) {
                        resolve(null);
                        return;
                    }
                    resolve(response);
                });
            } catch (e) {
                resolve(null);
            }
        });
    }

    async init(): Promise<void> {
        if (this.lastState)
            return;

        let currentState = await this.getState();
        if (currentState == null) {
            await this.initInternal();
            await this.getState();
        }
    }

    initInternal(): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            chrome.runtime.sendMessage(new ActivateContentScriptCommand(this.tabId), (response) => {
                resolve();
            });
        });
    }

    setMode(mode: Mode): void {
        if (config.development)
            console.log("popup sending" + SetOverlayModeCommand.identifier)

        chrome.tabs.sendMessage(this.tabId, new SetOverlayModeCommand(mode));

        let changedState = {...this.lastState!}
        changedState.mode = mode;
        this.stateChangeSubject.next(changedState)
    }

    setOverlayVisibility(visible: boolean): void {
        if (config.development)
            console.log("popup sending" + SetOverlayVisibilityCommand.identifier)
        
        chrome.tabs.sendMessage(this.tabId, new SetOverlayVisibilityCommand(visible));

        let changedState = {...this.lastState!}
        changedState.visible = visible;
        this.stateChangeSubject.next(changedState)
    }

    translateAllImages(): Promise<void> {
        if (config.development)
            console.log("popup sending" + TranslateAllImagesCommand.identifier)
        
        return new Promise<void>(resolve => {
            chrome.tabs.sendMessage(this.tabId, new TranslateAllImagesCommand(), response => {
                resolve();
            });
        });
    }

}

let contentScriptControllerProvider = new ContentScriptControllerProvider();

export const PopupContext: Context<ContentScriptControllerProvider> = React.createContext(contentScriptControllerProvider);


