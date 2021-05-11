import {PublicOcrTranslationRequest} from "./content/ocrTranslate/publicOcrTranslationRequest";
import {OcrTranslateResponse} from "./content/ocrTranslate/ocrTranslateResponse";
import {ResultResponse} from "./content/ocrTranslate/backgroundOcrTranslateService";
import {IContentScriptState} from "./content/IContentScriptState";
import {Mode} from "./content/mode";
import {config} from "./helper";

export interface IEvent {
    identifier: string;
}

export abstract class BaseEvent implements IEvent {
    protected constructor(public identifier: string) {
        if (config.development)
            console.log(`Created ${this.identifier} event`);
    }
}

export class ActivateContentScriptCommand extends BaseEvent {
    static identifier: string = "ActivateContentScriptCommand";

    constructor(public tabId: number) {
        super(ActivateContentScriptCommand.identifier);
    }
}

export class ActivateContentScriptEvent extends BaseEvent {
    static identifier: string = "ActivateContentScriptEvent";

    constructor(public tabId: number) {
        super(ActivateContentScriptEvent.identifier);
    }
}

export class DeactivateContentScriptCommand extends BaseEvent {
    static identifier: string = "DeactivateContentScriptCommand";

    constructor(public currentTab: number) {
        super(DeactivateContentScriptCommand.identifier);
    }
}

export class DeactivateContentScriptEvent extends BaseEvent {
    static identifier: string = "DeactivateContentScriptEvent";

    constructor() {
        super(DeactivateContentScriptEvent.identifier);
    }
}

export class ContentScriptUnloadEvent extends BaseEvent {
    static identifier: string = "ContentScriptUnloadEvent";

    constructor() {
        super(ContentScriptUnloadEvent.identifier);
    }
}

export class IsActiveQuery extends BaseEvent {
    static identifier: string = "IsActiveInTabQuery";

    constructor() {
        super(IsActiveQuery.identifier);
    }
}

export type IsActiveInTabQueryResponse = { active: boolean, state: boolean }

export class PublicTranslateQuery extends BaseEvent {
    static identifier: string = "PublicTranslateQuery";

    constructor(public request: PublicOcrTranslationRequest) {
        super(PublicTranslateQuery.identifier);
    }
}

export type PublicTranslateQueryResponse = ResultResponse<OcrTranslateResponse>

export class ContentScriptStateQuery extends BaseEvent {
    static identifier: string = "ContentScriptStateQuery";

    constructor() {
        super(ContentScriptStateQuery.identifier);
    }
}

export type ContentScriptStateQueryResponse = IContentScriptState

export class TranslateAllImagesCommand extends BaseEvent {
    static identifier: string = "TranslateAllImagesCommand";

    constructor() {
        super(TranslateAllImagesCommand.identifier);
    }
}

export class SetOverlayVisibilityCommand extends BaseEvent {
    static identifier: string = "SetOverlayVisibilityCommand";

    constructor(public visible: boolean) {
        super(SetOverlayVisibilityCommand.identifier);
    }
}

export class SetOverlayModeCommand extends BaseEvent {
    static identifier: string = "SetOverlayModeCommand";

    constructor(public mode: Mode) {
        super(SetOverlayModeCommand.identifier);
    }
}

export class InWhatTabAmIQuery extends BaseEvent {
    static identifier: string = "InWhatTabAmIQuery";

    constructor() {
        super(InWhatTabAmIQuery.identifier);
    }
}

export type InWhatTabAmIQueryResponse = number;