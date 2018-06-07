import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'tw-holiday-dashboard',
  template: `
  <h1 i18n>Holiday</h1>
  <hr />
  <h2 i18n>My work</h2>
  <div class="table-responsive-md">
    <table class="table table-hover">
      <thead>
        <tr>
          <th scope="col" i18n>Title</th>
          <th scope="col" i18n>Description</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td> title </td>
          <td> description </td>
          <td>
            <a href="javascript:void(0)" i18n>open</a>
          </td>
        </tr>
      </tbody>
    </table>
  </div>`
})
export class HolidayDashboardComponent implements OnInit {
  public ngOnInit(): void {

  }
}
