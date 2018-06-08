import {
  ValidatorFn,
  Validators
} from '@angular/forms';

export const HIDDEN_EDITOR = 'hidden';
export const TEXT_EDITOR = 'text';
export const PASSWORD_EDITOR = 'password';
export const RADIO_EDITOR = 'radio';
export const CHECKBOX_EDITOR = 'checkbox';
export const DATE_EDITOR = 'date';
export const NUMBER_EDITOR = 'number';
export const RANGE_EDITOR = 'range';
export const TIME_EDITOR = 'time';
export const SELECT_EDITOR = 'select';

export interface Editor {
  key: string;
  type: string;
  label: string;
  value?: any;
  options?: Array<{ key: string | number, value: string }>;
  required?: boolean;
  size?: number;
  valueMin?: number;
  valueMax?: number;
}

export const SINGLE_SLOT = 'single';
export const ARRAY_SLOT = 'array';

export interface Slot {
  key: string;
  type: string;
  title: string;
  editors: Array<Editor>;
  children?: Array<Slot>;
}

export class FormdefValidator {
  public static getValidators(editor: Editor): ValidatorFn {
    const validators: Array<ValidatorFn> = new Array<ValidatorFn>();

    if (editor.required) {
      validators.push(Validators.required);
    }
    if (editor.size) {
      validators.push(Validators.maxLength(editor.size));
    }
    if (editor.valueMin) {
      validators.push(Validators.min(editor.valueMin));
    }
    if (editor.valueMax) {
      validators.push(Validators.max(editor.valueMax));
    }

    return Validators.compose(validators);
  }
}
