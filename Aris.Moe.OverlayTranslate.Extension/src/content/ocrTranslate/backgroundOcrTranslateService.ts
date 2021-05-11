import {PublicOcrTranslationRequest} from "./publicOcrTranslationRequest";
import {OcrTranslateResponse} from "./ocrTranslateResponse";
import {PublicTranslateQuery, PublicTranslateQueryResponse} from "../../events";
import {config} from "../../helper";

export class BackgroundOcrTranslateService {
    static async translatePublic(request: PublicOcrTranslationRequest): Promise<ResultResponse<OcrTranslateResponse>> {
        let url = `${config.endpoint}TranslateOcr/public`;
        
        let body = JSON.stringify(request);
        let responseMessage: Response;

        try {
            responseMessage = await fetch(url, {
                method: 'POST',
                body: body,
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                }
            })
        } catch (e) {
            console.error(e);
            return {
                message: `Post failed` + e,
                success: false
            }
        }

        if (responseMessage.status != 200) {
            return {
                message: `Server returned unexpected status code: '${responseMessage.status}'`,
                success: false
            }
        }

        return responseMessage.json();
    }
}

export class ForegroundOcrTranslateService {
    static translatePublic(request: PublicOcrTranslationRequest): Promise<ResultResponse<OcrTranslateResponse>> {
        return new Promise((resolve, reject) => {
            chrome.runtime.sendMessage(new PublicTranslateQuery(request), (res: PublicTranslateQueryResponse) => {
                resolve(res)
            })
        })
    }

    static translatePublicSync(request: PublicOcrTranslationRequest, responseCallback: (response: ResultResponse<OcrTranslateResponse>) => void){
        chrome.runtime.sendMessage(new PublicTranslateQuery(request), (res: PublicTranslateQueryResponse) => {
            responseCallback(res)
        })
    }
}

export class ResultResponse<T> {
    result?: T | null;
    success: boolean = false;
    message?: string;
}