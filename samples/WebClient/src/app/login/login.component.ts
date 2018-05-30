import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { IToken } from '../shared/services/auth.service';

import { LoginService } from './login.service';

@Component({
  selector: 'tw-login',
  styleUrls: ['./login.component.css'],
  template: `
  <form class="form-signin"
        [formGroup]="formGroup"
        (ngSubmit)="login()"
        novalidate>
    <div class="form-label-group"
         [ngClass]="{ 'has-error': formGroup.controls['username'].invalid }">
      <input type="email"
             id="inputEmail"
             class="form-control"
             placeholder="Email address"
             formControlName="username"
             required autofocus>
      <label for="inputEmail">Email address</label>
    </div>

    <div class="form-label-group"
         [ngClass]="{ 'has-error': formGroup.controls['password'].invalid }">
      <input type="password"
             id="inputPassword"
             class="form-control"
             placeholder="Password"
             formControlName="password"
             required>
      <label for="inputPassword">Password</label>
    </div>

    <div class="checkbox mb-3">
      <label>
        <input type="checkbox" value="remember-me" i18n>Remember me
      </label>
    </div>
    <button class="btn btn-lg btn-primary btn-block"
            type="submit"
            [disabled]="!formGroup.valid">Sign in</button>
  </form>`,
  providers: [
    LoginService
  ]
})
export class LoginComponent implements OnInit {
  public formGroup: FormGroup;

  public constructor(
    private _loginService: LoginService,
    private _fb: FormBuilder
  ) { }

  public ngOnInit(): void {
    this.init();
  }

  public login(): void {
    this._loginService.login(
      this.formGroup.value.username,
      this.formGroup.value.password
    ).subscribe((token: IToken) => {
      this._loginService.authenticate(token);
    }, (error) => {
      console.log(error);
    });
  }

  private init(): void {
    this.formGroup = this._fb.group({
      'username': ['', Validators.required],
      'password': ['', Validators.required]
    });
  }
}
