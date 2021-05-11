import {
    ActivateContentScriptCommand, ActivateContentScriptEvent,
    IEvent,
    InWhatTabAmIQuery,
    PublicTranslateQuery,
    PublicTranslateQueryResponse
} from "./events";
import {config, Helper} from "./helper";
import {BackgroundOcrTranslateService} from "./content/ocrTranslate/backgroundOcrTranslateService";

console.log("Background started")

let pendingContentScriptActivationResolves: {
    [key: number]: (response: any) => void
} = {};

chrome.runtime.onMessage.addListener((message: IEvent, sender, sendResponse) => {
    if (config.development)
        console.log("Background saw: " + message.identifier)
    switch (message.identifier) {
        case ActivateContentScriptCommand.identifier:
            let tabId = (message as ActivateContentScriptCommand).tabId;
            pendingContentScriptActivationResolves[tabId] = sendResponse;
            activateContentScriptForCurrentTab(tabId);
            return true;
        case ActivateContentScriptEvent.identifier:
            //Late answer the activate command
            pendingContentScriptActivationResolves[(message as ActivateContentScriptEvent).tabId](true);
            break;
        case PublicTranslateQuery.identifier:
            let asPublicTranslateQuery = <PublicTranslateQuery>message;
            BackgroundOcrTranslateService.translatePublic(asPublicTranslateQuery.request)
                .then((x: PublicTranslateQueryResponse) => {
                    sendResponse(x);
                });
            return true;
        case InWhatTabAmIQuery.identifier:
            sendResponse(sender.tab?.id!)
            break;

        default:
            return;
    }

    return false;
});


function activateContentScriptForCurrentTab(tabId: number) : Promise<void> {
    return new Promise<void>(async (resolve, reject) => {
        chrome.scripting.executeScript({
            files: ['content.js'],
            target: {
                tabId: tabId
            }
        },results => {
            resolve();
        });
    });
}