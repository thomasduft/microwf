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
