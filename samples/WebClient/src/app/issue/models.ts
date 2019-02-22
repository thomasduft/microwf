import {
  Slot,
  SINGLE_SLOT,
  Editor,
  HIDDEN_EDITOR,
  TEXT_EDITOR,
  TEXT_AREA_EDITOR,
  MULTI_SELECT_EDITOR
} from '../shared/formdef/index';

import { VALUE_BINDING_BEHAVIOR } from '../shared/formdef/multi-select';

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
  public title = '';
  public editors: Editor[];

  public constructor(assigness: Array<string>) {
    const options = assigness.map((a: string) => {
      return { key: a, value: a };
    });

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
      // {
      //   key: 'assignee',
      //   type: MULTI_SELECT_EDITOR,
      //   label: 'Assignee',
      //   required: false,
      //   options: options,
      //   singleSelection: true,
      //   bindingBehaviour: VALUE_BINDING_BEHAVIOR
      // }
    ];
  }
}
