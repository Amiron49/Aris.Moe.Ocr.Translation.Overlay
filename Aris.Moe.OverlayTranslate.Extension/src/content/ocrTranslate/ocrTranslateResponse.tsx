import {TranslateResponse} from "./translateResponse";
import {OcrViewModel} from "./ocrViewModel";

export class OcrTranslateResponse extends TranslateResponse {
    machineOcrs!: OcrViewModel[];
}