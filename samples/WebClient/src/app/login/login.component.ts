import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { ResponseErrorHandler } from '../shared/services/models';
import { IToken } from '../shared/services/auth.service';

import { LoginService } from './login.service';

@Component({
  selector: 'tw-login',
  styleUrls: ['./login.component.css'],
  template: `
  <form class="form form--signin"
        [formGroup]="formGroup"
        (ngSubmit)="login()"
        novalidate>
    <div [ngClass]="{ 'has-error': formGroup.controls['username'].invalid }">
      <input type="email"
             id="inputEmail"
             placeholder="Username"
             formControlName="username"
             autofocus>
    </div>

    <div [ngClass]="{ 'has-error': formGroup.controls['password'].invalid }">
      <input type="password"
             id="inputPassword"
             placeholder="Password"
             formControlName="password">
    </div>

    <div *ngIf="error" class="alert alert-danger">
      Username or password is wrong!
    </div>

    <button type="submit"[disabled]="!formGroup.valid">Sign in</button>
  </form>`,
  providers: [
    LoginService
  ]
})
export class LoginComponent implements OnInit {
  public formGroup: FormGroup;
  public error: any;

  public constructor(
    private _loginService: LoginService,
    private _fb: FormBuilder
  ) { }

  public ngOnInit(): void {
    this.init();
  }

  public login(): void {
    delete this.error;

    this._loginService.login(
      this.formGroup.value.username,
      this.formGroup.value.password
    ).subscribe((token: IToken) => {
      this._loginService.authenticate(token);
    }, (error) => {
      this.error = ResponseErrorHandler.handleError(error);
    });
  }

  private init(): void {
    this.formGroup = this._fb.group({
      'username': ['', Validators.required],
      'password': ['', Validators.required]
    });
  }
}
