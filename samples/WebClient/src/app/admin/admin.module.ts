import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdministratorClaimGuard } from './administratorClaimGuard';

import { WorkflowAdminModule } from './workflow/workflow-admin.module';
import { QueueAdminModule } from './queue/queue-admin.module';

@NgModule({
  imports: [
    CommonModule,
    WorkflowAdminModule,
    QueueAdminModule
  ],
  providers: [
    AdministratorClaimGuard
  ]
})
export class AdminModule {
}
