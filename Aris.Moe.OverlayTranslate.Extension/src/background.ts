import {
    ActivateContentScriptCommand, ActivateContentScriptEvent, ContentScriptUnloadEvent, DeactivateContentScriptEvent,
    IEvent,
    IsActiveInTabQuery,
    IsActiveInTabQueryResponse, PublicTranslateQuery, PublicTranslateQueryResponse,
} from "./types";
import {Helper} from "./helper";
import {PublicOcrTranslationRequest} from "./content/ocrTranslate/publicOcrTranslationRequest";
import {BackgroundOcrTranslateService} from "./content/ocrTranslate/backgroundOcrTranslateService";
import MessageSender = chrome.runtime.MessageSender;

let contentScriptState: {
    [key: number]: boolean;
} = {}

chrome.runtime.onMessage.addListener((message: IEvent, sender, sendResponse) => {
    switch (message.identifier) {
        case ActivateContentScriptCommand.identifier:
            activateContentScriptForCurrentTab();
            break;
        case IsActiveInTabQuery.identifier:
            let asActiveTabQuery = <IsActiveInTabQuery>message;
            let response: IsActiveInTabQueryResponse = {
                active: isAlreadyInjectedInTab(asActiveTabQuery.tabId),
                state: isAlreadyActiveForTab(asActiveTabQuery.tabId)
            }
            sendResponse(response);
            break;
        case ContentScriptUnloadEvent.identifier:
            delete contentScriptState[sender.tab!.id!];
            break;
        case DeactivateContentScriptEvent.identifier:
            contentScriptState[sender.tab!.id!] = false;
            break;
        case ActivateContentScriptEvent.identifier:
            contentScriptState[sender.tab!.id!] = true;
            break;
        case PublicTranslateQuery.identifier:
            let asPublicTranslateQuery = <PublicTranslateQuery>message;
            BackgroundOcrTranslateService.translatePublic(asPublicTranslateQuery.request)
                .then((x: PublicTranslateQueryResponse) => {
                sendResponse(x);
            });
            return true;
        default:
            return false;
    }
    
    return false;
});

chrome.runtime.onMessage.addListener((message: IEvent, sender, sendResponse) => {
    switch (message.identifier) {
      
        default:
            return false;
    }
});

function isAlreadyActiveForTab(tabId: number): boolean {
    return contentScriptState[tabId];
}

function isAlreadyInjectedInTab(tabId: number): boolean {
    return contentScriptState[tabId] != null;
}

async function activateContentScriptForCurrentTab() {
    let currentTabId = await Helper.getCurrentActiveTabId();

    if (currentTabId == undefined)
        throw new Error("current tab id is undefined");

    if (isAlreadyInjectedInTab(currentTabId)) {
        console.warn("ContentScript is already active in tab")
        return;
    }

    chrome.scripting.executeScript({
        files: ['content.js'],
        target: {
            tabId: currentTabId
        }
    })
    
    contentScriptState[currentTabId] = true;
}