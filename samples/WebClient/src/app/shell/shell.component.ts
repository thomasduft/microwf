import { Component } from '@angular/core';

@Component({
  selector: 'tw-shell',
  template: `
  <tw-header></tw-header>

  <main role="main">
    <router-outlet class="container"></router-outlet>
  </main>`
})
export class ShellComponent {
}
