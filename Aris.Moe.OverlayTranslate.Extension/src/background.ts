import {
    ActivateContentScriptCommand, ActivateContentScriptEvent, ContentScriptUnloadEvent, DeactivateContentScriptEvent,
    IEvent,
    IsActiveInTabQuery,
    IsActiveInTabQueryResponse,
} from "./types";
import {Helper} from "./helper";

let contentScriptState: {
    [key: number]: boolean;
} = {}

chrome.runtime.onMessage.addListener((message: IEvent, sender, sendResponse) => {
    console.log(`background saw event ${message.identifier}`)
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
        default:
            break;
    }
});

function isAlreadyActiveForTab(tabId: number):boolean {
    return contentScriptState[tabId];
}

function isAlreadyInjectedInTab(tabId: number):boolean {
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

    chrome.tabs.executeScript({
        file: 'content.js'
    })
    
    contentScriptState[currentTabId] = true;
}