import {
  Component,
  OnInit,
  OnDestroy,
  Input,
  ComponentRef,
  ViewChild,
  ViewContainerRef,
  ComponentFactoryResolver
} from '@angular/core';
import {
  FormGroup
} from '@angular/forms';

import { Slot, SINGLE_SLOT } from './models';
import { SlotComponentRegistry } from './slotComponentRegistry.service';

@Component({
  selector: 'tw-slothost',
  template: `<ng-template #slotContent></ng-template>`
})
export class SlotHostComponent implements OnInit, OnDestroy {
  private _componentRef: ComponentRef<any>;
  private _slot: Slot;
  private _parentForm: FormGroup;

  @Input()
  public set slot(slot: Slot) {
    // if (this._componentRef) {
    //   this._componentRef.instance.slot = slot;
    // }

    this._slot = slot;
  }

  @Input()
  public set parentForm(form: FormGroup) {
    // if (this._componentRef) {
    //   this._componentRef.instance.myForm = form;
    // }

    this._parentForm = form;
  }

  @ViewChild('slotContent', { read: ViewContainerRef })
  protected slotContent: ViewContainerRef;

  public constructor(
    private _registry: SlotComponentRegistry
  ) { }

  public ngOnInit(): void {
    if (this._slot) {
      const slotType = this._slot.type ? this._slot.type : SINGLE_SLOT;
      const context = this._registry.get(slotType);

      if (context) {
        const factory = this.slotContent.injector
          .get(ComponentFactoryResolver)
          .resolveComponentFactory(context.component);

        this._componentRef = this.slotContent.createComponent(factory, this.slotContent.length);

        // @Input bindings
        this._componentRef.instance.slot = this._slot;
        this._componentRef.instance.parentForm = this._parentForm;
      }
    }
  }

  public ngOnDestroy(): void {
    if (this._componentRef) {
      delete this._componentRef.instance.slot;
      delete this._componentRef.instance.parentForm;

      this._componentRef.destroy();
    }
  }
}
