import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { ApiService } from '../shared/services/api.service';
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
             placeholder="Username"
             formControlName="username"
             required autofocus>
    </div>

    <div class="form-label-group"
         [ngClass]="{ 'has-error': formGroup.controls['password'].invalid }">
      <input type="password"
             id="inputPassword"
             class="form-control"
             placeholder="Password"
             formControlName="password"
             required>
    </div>

    <div *ngIf="error" class="alert alert-danger">
      Username or password is wrong!
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
  public error: any;

  public constructor(
    private _loginService: LoginService,
    private _fb: FormBuilder,
    private _api: ApiService
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
      this.error = this._api.handleError(error);
    });
  }

  private init(): void {
    this.formGroup = this._fb.group({
      'username': ['', Validators.required],
      'password': ['', Validators.required]
    });
  }
}
