import { Component } from '@angular/core';
import { NgStyle } from '@angular/common';
import { IconDirective } from '@coreui/icons-angular';
import {
  ContainerComponent,
  RowComponent,
  ColComponent,
  CardGroupComponent,
  TextColorDirective,
  CardComponent,
  CardBodyComponent,
  FormDirective,
  InputGroupComponent,
  InputGroupTextDirective,
  FormControlDirective,
  ButtonDirective,
} from '@coreui/angular';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  AdminApiAuthApiClient,
  AuthenticatedResult,
  LoginRequest,
} from 'src/app/api/admin-api.service.generated';
import { ToastService } from 'src/app/shared/services/toast.service';
import { Router } from '@angular/router';
import { UrlConstants } from 'src/app/shared/constants/url.constants';
import { TokenStorageService } from 'src/app/shared/services/token-storage.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [
    ContainerComponent,
    RowComponent,
    ColComponent,
    CardGroupComponent,
    TextColorDirective,
    CardComponent,
    CardBodyComponent,
    FormDirective,
    InputGroupComponent,
    InputGroupTextDirective,
    IconDirective,
    FormControlDirective,
    ButtonDirective,
    NgStyle,
    ReactiveFormsModule,
  ],
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AdminApiAuthApiClient,
    private toastService: ToastService,
    private router: Router,
    private tokenStorageService: TokenStorageService
  ) {
    this.loginForm = this.fb.group({
      username: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
    });
  }

  login() {
    var request: LoginRequest = new LoginRequest({
      userName: this.loginForm.get('username')?.value,
      password: this.loginForm.get('password')?.value,
    });

    this.authService.login(request).subscribe({
      next: (response: AuthenticatedResult) => {
        // Save Token and refesh token to local storage
        this.tokenStorageService.saveToken(response.token);
        this.tokenStorageService.saveRefreshToken(response.refreshToken);
        this.tokenStorageService.saveUser(response);
        // Redirect to dashboard
        this.router.navigate([UrlConstants.DASHBOARD]);
      },
      error: (error: any) => {
        if (error.status === 400) {
          this.toastService.showError('Invalid username or password');
          return;
        } else if (error.status === 401) {
          this.toastService.showError('Unauthorized');
          return;
        } else {
          this.toastService.showError(error.message);
        }
      },
    });
  }
}
