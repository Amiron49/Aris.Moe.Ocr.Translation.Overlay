export class Helper {
    static getCurrentActiveTabId(): Promise<number> {
        return new Promise((resolve) => {
            chrome.tabs.query({active: true, currentWindow: true}, result => {
                let number = result[0].id;
                console.log(number)

                if (number == undefined)
                    throw Error("Couldn't determine current tab Id")

                resolve(number);
            });
        });
    }

    // static async getCurrentActiveTabId(): Promise<number> {
    //     let number = (await chrome.tabs.query({active: true, currentWindow: true}))[0].id;
    //
    //     if (number == undefined)
    //         throw Error("Couldn't determine current tab Id")
    //
    //     return number;
    // }
}