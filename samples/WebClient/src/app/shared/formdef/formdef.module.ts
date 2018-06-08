import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { FormdefRegistry } from './formdefRegistry.service';

import { ArraySlotComponent } from './arraySlot.component';
import { DateValueAccessorDirective } from './dateValueAccessor';
import { SlotComponent } from './slot.component';
import { EditorComponent } from './editor.component';
import { FormdefComponent } from './formdef.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  declarations: [
    EditorComponent,
    FormdefComponent,
    SlotComponent,
    ArraySlotComponent,
    DateValueAccessorDirective
  ],
  exports: [
    ReactiveFormsModule,
    FormdefComponent
  ],
  providers: [
    FormdefRegistry
  ]
})
export class FormdefModule {
}
