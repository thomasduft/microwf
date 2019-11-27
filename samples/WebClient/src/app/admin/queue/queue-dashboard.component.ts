import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-queue-dashboard',
  template: `
  <tw-tabs>
    <tw-tab i18n-title title='Snapshot'>
      <tw-snapshots></tw-snapshots>
    </tw-tab>
    <tw-tab i18n-title title='Upcommings'>
      <p>Some upcommings<p>
    </tw-tab>
    <tw-tab i18n-title title='Failed'>
      <p>Some failed<p>
    </tw-tab>
  </tw-tabs>
  `
})
export class QueueDashboardComponent {
  @HostBinding('class')
  public workspace = 'pane';
}
