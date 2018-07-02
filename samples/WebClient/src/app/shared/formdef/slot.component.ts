import {
  Component,
  Input
} from '@angular/core';
import {
  FormGroup
} from '@angular/forms';

import { Slot } from './models';

@Component({
  selector: 'tw-slot',
  template: `
  <h3 *ngIf="slot.title">{{ slot.title }}</h3>
  <ng-container *ngIf="slot.editors && slot.editors.length > 0">
    <tw-editor *ngFor="let editor of slot.editors"
               [editor]="editor"
               [parentForm]="parentForm">
    </tw-editor>
  </ng-container>

  <ng-container *ngIf="slot.children && slot.children.length > 0">
    <div *ngFor="let child of slot.children">
      <tw-slot *ngIf="child.type === 'single'"
               [slot]="child"
               [parentForm]="parentForm.get(child.key)">
      </tw-slot>
      <tw-arrayslot *ngIf="child.type === 'array'"
                    [slot]="child"
                    [parentForm]="parentForm">
      </tw-arrayslot>
    </div>
  </ng-container>`
})
export class SlotComponent {
  @Input()
  public slot: Slot;

  @Input()
  public parentForm: FormGroup;
}
