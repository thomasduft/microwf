import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-queue-dashboard',
  template: `
  <tw-accordion>
    <tw-accordion-group i18n-title title='Snapshots'>
      <tw-snapshots></tw-snapshots>
    </tw-accordion-group>
    <tw-accordion-group i18n-title title='Upcommings'>
      <p>Some upcommings<p>
    </tw-accordion-group>
    <tw-accordion-group i18n-title title='Failed'>
      <p>Some failed<p>
    </tw-accordion-group>
  </tw-accordion>
  `
})
export class QueueDashboardComponent {
  @HostBinding('class')
  public workspace = 'pane';
}
