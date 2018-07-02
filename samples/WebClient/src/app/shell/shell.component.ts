import { Component } from '@angular/core';

@Component({
  selector: 'tw-shell',
  template: `
  <tw-header></tw-header>

  <main role="main" class="container">
    <router-outlet></router-outlet>
  </main>`
})
export class ShellComponent {
}
