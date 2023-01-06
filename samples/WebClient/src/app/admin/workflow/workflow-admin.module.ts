import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule, Routes } from "@angular/router";

import { IconModule } from "../../shared/icon/icon.module";
import { ListModule } from "../../shared/list/list.module";
import { FormdefModule, FormdefRegistry } from "../../shared/formdef/index";
import { WorkflowModule } from "../../workflow/workflow.module";
import { ScrollerModule } from "../../shared/scroller/scroller.module";

import { AdministratorClaimGuard } from "./../administratorClaimGuard";

import { WorkflowDashboardComponent } from "./workflow-dashboard.component";
import { WorkflowComponent } from "./workflow.component";
import { WorkflowListItemComponent } from "./workflow-list-item.component";

import { WorkflowSearchComponent } from "./workflow-search.component";
import { WorkflowSearchSlot } from "./models";

const ROUTES: Routes = [
  {
    path: "workflows",
    component: WorkflowDashboardComponent,
    canActivate: [AdministratorClaimGuard],
    children: [{ path: "detail/:id", component: WorkflowComponent }],
  },
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(ROUTES),
    FormdefModule,
    WorkflowModule,
    IconModule,
    ListModule,
    ScrollerModule,
  ],
  declarations: [
    WorkflowDashboardComponent,
    WorkflowComponent,
    WorkflowListItemComponent,
    WorkflowSearchComponent,
  ],
})
export class WorkflowAdminModule {
  public constructor(private _slotRegistry: FormdefRegistry) {
    this._slotRegistry.register(new WorkflowSearchSlot());
  }
}
