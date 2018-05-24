import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'tw-login',
  styleUrls: ['./login.component.css'],
  template: `
  <form class="form-signin">
    <div class="form-label-group">
      <input type="email" id="inputEmail" class="form-control" placeholder="Email address" required autofocus>
      <label for="inputEmail">Email address</label>
    </div>

    <div class="form-label-group">
      <input type="password" id="inputPassword" class="form-control" placeholder="Password" required>
      <label for="inputPassword">Password</label>
    </div>

    <div class="checkbox mb-3">
      <label>
        <input type="checkbox" value="remember-me"> Remember me
      </label>
    </div>
    <button class="btn btn-lg btn-primary btn-block" type="submit">Sign in</button>
  </form>`
})
export class LoginComponent implements OnInit {

  public constructor() { }

  public ngOnInit(): void {
  }
}
