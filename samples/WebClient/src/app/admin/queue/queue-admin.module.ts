import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { IconModule } from '../../shared/icon/icon.module';
import { ListModule } from '../../shared/list/list.module';
import { FormdefModule } from '../../shared/formdef/index';
import { WorkflowModule } from '../../workflow/workflow.module';
import { ScrollerModule } from '../../shared/scroller/scroller.module';
import { TabModule } from '../../shared/tab/tab.module';

import { AdministratorClaimGuard } from './../administratorClaimGuard';

import { SnapshotsComponent } from './snapshots.component';
import { UpcommingsComponent } from './upcommings.component';
import { FailedComponent } from './failed.component';
import { QueueDashboardComponent } from './queue-dashboard.component';
import { QueueListItemComponent } from './queue-list-item.component';

const ROUTES: Routes = [
  {
    path: 'queue',
    component: QueueDashboardComponent,
    canActivate: [AdministratorClaimGuard]
  }
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
    TabModule
  ],
  declarations: [
    SnapshotsComponent,
    UpcommingsComponent,
    FailedComponent,
    QueueDashboardComponent,
    QueueListItemComponent
  ]
})
export class QueueAdminModule {
}
