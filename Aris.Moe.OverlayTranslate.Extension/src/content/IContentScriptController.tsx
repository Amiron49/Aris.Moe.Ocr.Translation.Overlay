import {Observable} from "rxjs";
import {IContentScriptState} from "./IContentScriptState";
import {Mode} from "./mode";

export interface IContentScriptController {
    stateChange: Observable<IContentScriptState | null>;

    init(): Promise<void>;

    getState(): Promise<IContentScriptState | null>;

    translateAllImages(): Promise<void>;

    setOverlayVisibility(visible: boolean): void;

    setMode(mode: Mode): void;
}