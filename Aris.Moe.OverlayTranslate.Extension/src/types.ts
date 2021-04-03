export type MessageTypes = "ActivateContentScript" | "DeactivateContentScript";

export interface IEvent {
    identifier: string;
}

export abstract class BaseEvent implements IEvent {
    protected constructor(public identifier: string) {
        console.log(`Created ${this.identifier} event`);
    }
}

export class ActivateContentScriptCommand extends BaseEvent {
    static identifier: string = "ActivateContentScriptCommand";

    constructor(public currentTab: number) {
        super(ActivateContentScriptCommand.identifier);
    }
}

export class ActivateContentScriptEvent extends BaseEvent {
    static identifier: string = "ActivateContentScriptEvent";

    constructor() {
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

export class IsActiveInTabQuery extends BaseEvent {
    static identifier: string = "IsActiveInTabQuery";

    constructor(public tabId: number) {
        super(IsActiveInTabQuery.identifier);
    }
}

export type IsActiveInTabQueryResponse = {active: boolean, state: boolean}