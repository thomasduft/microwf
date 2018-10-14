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
  <ng-container *ngIf="slot.editors && slot.editors.length > 0">
    <fieldset>
      <legend (click)="toggle()">{{ slot.title }}</legend>
      <ng-container *ngIf="!collapsed">
        <tw-editor *ngFor="let editor of slot.editors"
                  [editor]="editor"
                  [parentForm]="parentForm">
        </tw-editor>
      </ng-container>
    </fieldset>
  </ng-container>
  <ng-container *ngIf="slot.children && slot.children.length > 0">
    <ng-container *ngFor="let child of slot.children">
      <tw-slot *ngIf="child.type === 'single'"
               [slot]="child"
               [parentForm]="parentForm.get(child.key)">
      </tw-slot>
      <tw-arrayslot *ngIf="child.type === 'array'"
                    [slot]="child"
                    [parentForm]="parentForm">
      </tw-arrayslot>
    </ng-container>
  </ng-container>`
})
export class SlotComponent {
  private collapsed = false;

  @Input()
  public slot: Slot;

  @Input()
  public parentForm: FormGroup;

  public toggle(): void {
    this.collapsed = !this.collapsed;
  }
}
