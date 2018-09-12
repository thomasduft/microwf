import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-workspace',
  template: `
  <div class="workspace__header"></div>
  <tw-statusbar></tw-statusbar>
  <div class="workspace__content">
    <router-outlet></router-outlet>
  </div>
  `
})
export class WorkspaceComponent {
  @HostBinding('class')
  public workspace = 'workspace';
}
