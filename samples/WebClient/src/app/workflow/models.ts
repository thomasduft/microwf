import { PagingModel } from '../shared/services/models';

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

export interface WorkflowSearchModel {
  type: string;
  correlationId: number;
  assignee: string;
}

export class WorkflowPagingModel extends PagingModel {
  public type: string;
  public correlationId: number;
  public assignee: string;

  public static createWorkflowPagingModel(
    page: PagingModel,
    searchModel?: WorkflowSearchModel
  ): WorkflowPagingModel {
    const model = new WorkflowPagingModel();
    model.pageIndex = page.pageIndex;
    model.pageSize = page.pageSize;

    if (!searchModel) {
      searchModel = {
        type: null,
        correlationId: null,
        assignee: null
      };
    }
    if (searchModel.type) {
      model.type = searchModel.type;
    }
    if (searchModel.correlationId) {
      model.correlationId = searchModel.correlationId;
    }
    if (searchModel.assignee) {
      model.assignee = searchModel.assignee;
    }

    return model;
  }
}

export interface QueueItem {
  id: number;
  triggerName: string;
  entityId: number;
  workflowType: string;
  retries: number;
  error: string;
  dueDate: Date;
}
