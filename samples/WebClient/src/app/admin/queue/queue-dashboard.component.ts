import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-queue-dashboard',
  template: `
  <tw-tabs>
    <tw-tab i18n-title title='Snapshot'>
      <tw-snapshots></tw-snapshots>
    </tw-tab>
    <tw-tab i18n-title title='Upcommings'>
      <tw-upcommings></tw-upcommings>
    </tw-tab>
    <tw-tab i18n-title title='Failed'>
      <tw-failed></tw-failed>
    </tw-tab>
  </tw-tabs>
  `
})
export class QueueDashboardComponent {
  @HostBinding('class')
  public workspace = 'pane';
}
