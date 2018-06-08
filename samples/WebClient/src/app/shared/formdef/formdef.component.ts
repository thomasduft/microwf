import {
  Component,
  OnInit,
  Input,
  EventEmitter,
  Output
} from '@angular/core';
import { FormGroup } from '@angular/forms';

import { Slot } from './models';
import { FormdefService } from './formdef.service';

@Component({
  selector: 'tw-formdef',
  template: `
  <form [formGroup]="form"
        (ngSubmit)="onSubmit()">
    <tw-slot [slot]="slot"
             [parentForm]="form">
    </tw-slot>
    <div class="btn-group" role="group">
      <button type="submit"
              class="btn btn-primary"
              [disabled]="!form.valid"
              i18n>Save</button>
      <button type="button"
              class="btn btn-secondary"
              (click)="onReset()"
              i18n>Cancel</button>
      <button type="button"
              class="btn btn-secondary"
              (click)="onDelete()"
              i18n>Delete</button>
    </div>
  </form>`,
  providers: [
    FormdefService
  ]
})
export class FormdefComponent implements OnInit {
  @Input()
  public key: string;

  @Input()
  public viewModel: any;

  @Output()
  public submitted: EventEmitter<any> = new EventEmitter<any>();

  @Output()
  public resetted: EventEmitter<any> = new EventEmitter<any>();

  @Output()
  public deleted: EventEmitter<any> = new EventEmitter<any>();

  public form: FormGroup;
  public slot: Slot;

  public constructor(
    private _formdefService: FormdefService
  ) { }

  public ngOnInit(): void {
    this.form = this._formdefService.toGroup(this.key, this.viewModel);
    this.slot = this._formdefService.getSlot(this.key);
  }

  public onSubmit(): void {
    this.submitted.next(this.form.value);
  }

  public onReset(): void {
    this.resetted.next(this.form.value);
  }

  public onDelete(): void {
    this.deleted.next(this.form.value);
  }
}
