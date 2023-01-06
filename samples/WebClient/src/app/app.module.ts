import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { HttpClientModule } from "@angular/common/http";
import { PreloadAllModules, RouterModule, Routes } from "@angular/router";

import { OAuthModule } from "angular-oauth2-oidc";

import { SharedModule } from "./shared/shared.module";
import { ShellModule } from "./shell/shell.module";
import { WorkflowModule } from "./workflow/workflow.module";

import { httpInterceptorProviders } from "./shared/services/interceptors";

import { AppComponent } from "./app.component";
import { HomeComponent } from "./shell/home.component";
import { ForbiddenComponent } from "./shell/forbidden.component";
import { PageNotFoundComponent } from "./shell/page-not-found.component";

import { DispatchWorkflowComponent } from "./workflow/dispatch-workflow.component";

const ROUTES: Routes = [
  { path: "home", component: HomeComponent },
  { path: "", redirectTo: "home", pathMatch: "full" },
  { path: "dispatch/:assignee/:goto", component: DispatchWorkflowComponent },
  {
    path: "admin",
    loadChildren: () =>
      import("./admin/admin.module").then((m) => m.AdminModule),
  },
  {
    path: "holiday",
    loadChildren: () =>
      import("./holiday/holiday.module").then((m) => m.HolidayModule),
  },
  {
    path: "issue",
    loadChildren: () =>
      import("./issue/issue.module").then((m) => m.IssueModule),
  },
  { path: "forbidden", component: ForbiddenComponent },
  { path: "**", component: PageNotFoundComponent },
];

@NgModule({
  imports: [
    BrowserModule,
    HttpClientModule,
    RouterModule.forRoot(ROUTES, { preloadingStrategy: PreloadAllModules }),
    OAuthModule.forRoot(),
    SharedModule,
    ShellModule,
    WorkflowModule,
  ],
  declarations: [AppComponent, HomeComponent],
  providers: [httpInterceptorProviders],
  bootstrap: [AppComponent],
})
export class AppModule {}
