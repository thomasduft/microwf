import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { MultiSelectModule } from './multi-select/multi-select.module';

import { FormdefRegistry } from './formdefRegistry.service';

import { SINGLE_SLOT, ARRAY_SLOT } from './models';
import { ArraySlotComponent } from './arraySlot.component';
import { DateValueAccessorDirective } from './dateValueAccessor';
import { SlotComponent } from './slot.component';
import { EditorComponent } from './editor.component';
import { FormdefComponent } from './formdef.component';
import { SlotComponentRegistry, SlotComponentMetaData } from './slotComponentRegistry.service';
import { SlotHostComponent } from './slotHost.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MultiSelectModule
  ],
  declarations: [
    EditorComponent,
    FormdefComponent,
    SlotHostComponent,
    SlotComponent,
    ArraySlotComponent,
    DateValueAccessorDirective
  ],
  exports: [
    ReactiveFormsModule,
    FormdefComponent,
    EditorComponent
  ],
  entryComponents: [
    SlotComponent,
    ArraySlotComponent
  ],
  providers: [
    FormdefRegistry,
    SlotComponentRegistry
  ]
})
export class FormdefModule {
  public constructor(
    private _registry: SlotComponentRegistry
  ) {
    this._registry.register(new SlotComponentMetaData(SINGLE_SLOT, SlotComponent));
    this._registry.register(new SlotComponentMetaData(ARRAY_SLOT, ArraySlotComponent));
  }
}
