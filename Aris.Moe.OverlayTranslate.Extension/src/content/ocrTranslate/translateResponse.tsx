import {MatchType} from "./matchType";
import {ImageInfo} from "./imageInfo";
import {TranslationViewModel} from "./translationViewModel";

export class TranslateResponse {
    match!: MatchType;
    image!: ImageInfo;
    machineTranslations!: TranslationViewModel[];
}