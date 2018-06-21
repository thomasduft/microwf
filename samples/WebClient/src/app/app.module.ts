import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { SharedModule } from './shared/shared.module';
import { LoginModule } from './login/login.module';
import { WorkflowModule } from './workflow/workflow.module';

import { httpInterceptorProviders } from './shared/services/interceptors';

import { AppComponent } from './app.component';
import { ShellComponent } from './shell/shell.component';
import { HomeComponent } from './shell/home.component';
import { PageNotFoundComponent } from './shell/page-not-found.component';
import { AuthGuard } from './shared/services/models';

const ROUTES: Routes = [
  {
    path: '',
    component: ShellComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'home', component: HomeComponent },
      {
        path: 'holiday',
        canActivate: [AuthGuard],
        loadChildren: './holiday/holiday.module#HolidayModule'
      },
      { path: '', redirectTo: 'home', pathMatch: 'full' }
    ]
  },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [
    BrowserModule,
    HttpClientModule,
    RouterModule.forRoot(ROUTES),
    NgbModule.forRoot(),
    SharedModule,
    LoginModule,
    WorkflowModule
  ],
  declarations: [
    AppComponent,
    ShellComponent,
    HomeComponent,
    PageNotFoundComponent
  ],
  providers: [
    httpInterceptorProviders
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
