import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-shell',
  template: `
  <tw-sidebar></tw-sidebar>
  <tw-workspace></tw-workspace>
  `
})
export class ShellComponent {
  @HostBinding('class')
  public workspace = 'shell';
}
