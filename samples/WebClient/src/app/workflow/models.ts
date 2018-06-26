export class WorkflowArea {
  public constructor(
    public key: string,
    public title: string,
    public description: string,
    public route: string
  ) { }
}

export interface WorkflowDefinition {
  type: string;
  title: string;
  description: string;
  route: string;
}

export interface TriggerInfo {
  succeeded: boolean;
  triggers: Array<string>;
  errors: Array<string>;
}

export interface WorkflowResult<T> {
  triggerInfo: TriggerInfo;
  viewModel: T;
}
