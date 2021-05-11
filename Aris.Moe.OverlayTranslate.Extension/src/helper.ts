import * as prodConfig from "./config.json";
import * as devConfig from "./config.dev.json";

export class Helper {
    static getCurrentActiveTabId(): Promise<number> {
        return new Promise((resolve) => {
            chrome.tabs.query({active: true, currentWindow: true}, result => {
                let number = result[0].id;

                if (number == undefined)
                    throw Error("Couldn't determine current tab Id")

                resolve(number);
            });
        });
    }
}

let c = prodConfig;

if (typeof(process) != undefined && process.env.NODE_ENV === "development") {
    c = devConfig;
}

export const config = c;