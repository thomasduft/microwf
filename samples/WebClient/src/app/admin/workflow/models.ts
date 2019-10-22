import {
  Slot,
  SINGLE_SLOT,
  Editor,
  TEXT_EDITOR,
  NUMBER_EDITOR
} from '../../shared/formdef/index';

export class WorkflowSearchSlot implements Slot {
  public static KEY = 'workflowsearchslot';

  public key = WorkflowSearchSlot.KEY;
  public type = SINGLE_SLOT;
  public title = '';
  public editors: Editor[];

  public constructor() {
    this.editors = [
      {
        key: 'type',
        type: TEXT_EDITOR,
        label: 'Type',
        required: false
      },
      {
        key: 'correlationId',
        type: NUMBER_EDITOR,
        label: 'Correlation Id',
        required: false
      },
      {
        key: 'assignee',
        type: TEXT_EDITOR,
        label: 'Assignee',
        required: false
      }
    ];
  }
}
