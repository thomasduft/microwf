import {
  Slot,
  SINGLE_SLOT,
  Editor,
  HIDDEN_EDITOR,
  TEXT_EDITOR,
  DATE_EDITOR
} from '../shared/formdef/index';

export interface HolidayMessage {
  author: string;
  message: string;
}

export interface Holiday {
  id: number;
  requester: string;
  superior: string;
  from: Date;
  to: Date;
  state: string;
  messages: HolidayMessage[];
}

export interface ApplyHoliday {
  from: Date;
  to: Date;
  message: string;
}

export interface ApproveHoliday {
  id: number;
  approved: boolean;
  message: string;
}

export class ApplyHolidayDetailSlot implements Slot {
  public static KEY = 'apply_holidaydetailslot';

  public key = ApplyHolidayDetailSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Apply for holidays';
  public editors: Editor[];

  public constructor() {
    this.editors = [
      {
        key: 'from',
        type: DATE_EDITOR,
        label: 'From',
        required: true
      },
      {
        key: 'to',
        type: DATE_EDITOR,
        label: 'To',
        required: true
      },
      {
        key: 'message',
        type: TEXT_EDITOR,
        label: 'Message'
      }
    ];
  }
}

export class ApproveHolidayDetailSlot implements Slot {
  public static KEY = 'approve_holidaydetailslot';

  public key = ApproveHolidayDetailSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Approve Holiday';
  public editors: Editor[];

  public constructor() {
    this.editors = [
      {
        key: 'id',
        type: HIDDEN_EDITOR,
        label: 'Id',
        required: true
      },
      {
        key: 'message',
        type: TEXT_EDITOR,
        label: 'Message',
        required: false
      }
    ];
  }
}
