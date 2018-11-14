import { MessageBase } from '../../../shared/services/models';

export enum StatusLevel {
  Success,
  Info,
  Warning,
  Danger
}

export class StatusMessage extends MessageBase {
  public static KEY = 'StatusMessage';
  private _hasAction = false;
  private _viewed = false;

  public get viewed(): boolean {
    return this._viewed;
  }
  public set viewed(v: boolean) {
    this._viewed = v;

    if (this._viewed && this.hasAction) {
      delete this.action;
    }
  }

  public action: Function = undefined;
  public get hasAction(): boolean {
    return this._hasAction;
  }

  public constructor(
    public title: string,
    public message: string,
    public level?: StatusLevel,
    action?: Function) {
    super();

    this.level = level !== undefined ? level : StatusLevel.Info;

    if (action) {
      this._hasAction = true;
      this.action = action;
    }
  }

  public getType(): string {
    return StatusMessage.KEY;
  }
}
