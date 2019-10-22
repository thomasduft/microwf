import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { IconModule } from '../../shared/icon/icon.module';
import { ListModule } from '../../shared/list/list.module';
import { FormdefModule } from '../../shared/formdef/index';
import { WorkflowModule } from '../../workflow/workflow.module';
import { ScrollerModule } from '../../shared/scroller/scroller.module';

import { AdministratorClaimGuard } from './../administratorClaimGuard';

import { SnapshotsComponent } from './snapshots.component';
import { QueueDashboardComponent } from './queue-dashboard.component';
import { QueueListItemComponent } from './queue-list-item.component';

const ROUTES: Routes = [
  {
    path: 'queue',
    component: QueueDashboardComponent,
    canActivate: [AdministratorClaimGuard],
    children: [
      { path: 'snapshots', component: SnapshotsComponent },
      { path: 'upcommings', component: SnapshotsComponent },
      { path: 'failed', component: SnapshotsComponent }
    ]
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
    ScrollerModule
  ],
  declarations: [
    SnapshotsComponent,
    QueueDashboardComponent,
    QueueListItemComponent
  ]
})
export class QueueAdminModule {
}
