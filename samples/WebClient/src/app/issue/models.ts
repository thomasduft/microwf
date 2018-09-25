import {
  Slot,
  SINGLE_SLOT,
  Editor,
  HIDDEN_EDITOR,
  TEXT_EDITOR,
  TEXT_AREA_EDITOR
} from '../shared/formdef/index';

export interface Issue {
  id: number;
  state: string;
  assignee: string;
  title: string;
  description: string;
}

export interface IssueViewmodel {
  id: number;
  trigger: string;
  assignee: string;
  title: string;
  description: string;
}

export class IssueDetailSlot implements Slot {
  public static KEY = 'issuedetailslot';

  public key = IssueDetailSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Issue';
  public editors: Editor[];

  public constructor() {
    this.editors = [
      {
        key: 'id',
        type: HIDDEN_EDITOR,
        label: 'Id',
        required: false
      },
      {
        key: 'title',
        type: TEXT_EDITOR,
        label: 'Message',
        required: true,
        minLength: 3,
        maxLength: 140
      },
      {
        key: 'description',
        type: TEXT_AREA_EDITOR,
        label: 'Description',
        required: false,
        maxLength: 500
      }
    ];
  }
}
