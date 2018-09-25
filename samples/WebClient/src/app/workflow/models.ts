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

export interface Workflow {
  id: number;
  correlationId: number;
  type: string;
  state: string;
  title: string;
  description: string;
  assignee: string;
  route: string;
  started: Date;
  completed?: Date;
}

export interface WorkflowHistory {
  id: number;
  created: Date;
  fromState: string;
  toState; string;
  userName: string;
  workflowId: number;
}

export interface WorkflowVariable {
  id: number;
  type: string;
  content: string;
  workflowId: number;
}

export interface TriggerInfo {
  succeeded: boolean;
  triggers: Array<string>;
  errors: Array<string>;
}

export interface WorkflowResult<TEntity, TViewModel> {
  triggerInfo: TriggerInfo;
  entity: TEntity;
  viewModel: TViewModel;
}

export interface AssigneeWorkflowResult {
  assignee: string;
  message: string;
}
