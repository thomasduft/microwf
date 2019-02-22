import { Component } from '@angular/core';
import {
  FormGroup,
  FormArray,
  FormControl,
  FormBuilder
} from '@angular/forms';

import { FormdefValidator, Editor, Slot } from './models';
import { SlotComponent } from './slot.component';

@Component({
  selector: 'tw-arrayslot',
  template: `
  <fieldset>
    <legend (click)="toggle()">{{ slot.title }}</legend>
    <ng-container *ngIf="!collapsed">
      <div>
        <button type="button"
                class="button--secondary"
                (click)="add()">+</button>
      </div>
      <div>
        <table [formGroup]="parentForm">
          <tbody>
            <tr *ngFor="let row of rows.controls; let idx = index">
              <td *ngFor="let editor of slot.editors">
                <tw-editor
                  [hideLabel]="true"
                  [editor]="editor"
                  [parentForm]="rows.at(idx)">
                </tw-editor>
              </td>
              <td>
                <button type="button"
                        class="button--secondary"
                        (click)="remove(idx)">-</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </ng-container>
  </fieldset>`
})
export class ArraySlotComponent extends SlotComponent {
  public get rows(): FormArray {
    return this.parentForm.get(this.slot.key) as FormArray;
  }

  public constructor(
    private _fb: FormBuilder
  ) {
    super();
  }

  public add(): void {
    const row = this.createRow(this.slot);
    this.rows.push(row);
  }

  public remove(idx: number): void {
    this.rows.removeAt(idx);
  }

  protected createRow(arraySlot: Slot): FormGroup {
    const row = this._fb.group({});

    arraySlot.editors.forEach((e: Editor) => {
      row.addControl(e.key, new FormControl(undefined, FormdefValidator.getValidators(e)));
    });

    return row;
  }
}
