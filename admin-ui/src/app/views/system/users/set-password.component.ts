import { Component, OnInit, EventEmitter, OnDestroy } from '@angular/core';
import {
  Validators,
  FormControl,
  FormGroup,
  FormBuilder,
  ValidatorFn,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { catchError, Subject, takeUntil } from 'rxjs';
import {
  AdminApiUserApiClient,
  ChangeMyPasswordRequest,
  RoleDto,
} from '../../../api/admin-api.service.generated';
import { UserSharedModule } from './user-shared.module';

@Component({
  templateUrl: 'set-password.component.html',
  standalone: true,
  imports: [
    UserSharedModule
  ],
})
export class SetPasswordComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  // Default
  public blockedPanelDetail: boolean = false;
  public form: FormGroup;
  public title: string;
  public btnDisabled = false;
  public saveBtnName: string;
  public closeBtnName: string;
  selectedEntity = {} as RoleDto;

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private userService: AdminApiUserApiClient,
    private fb: FormBuilder
  ) {}

  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit() {
    this.buildForm();
    this.saveBtnName = 'Update';
    this.closeBtnName = 'Cancel';
  }

  // Validate
  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    newPassword: [
      { type: 'required', message: 'You must enter a password' },
      {
        type: 'pattern',
        message:
          'Password must be at least 8 characters, at least 1 number, 1 special character, and 1 uppercase letter',
      },
    ],
    confirmNewPassword: [
      { type: 'required', message: 'Confirm password is incorrect' },
      { type: 'notMatched', message: 'Passwords do not match' }
    ],
  };

  saveChange() {
    this.toggleBlockUI(true);
    this.saveData();
  }

  private saveData() {
    this.userService
      .setPassword(this.config.data.id, this.form.value)
      .pipe(takeUntil(this.ngUnsubscribe)
        ,catchError((error) => {
        this.toggleBlockUI(false);
        return error;
      }))
      .subscribe(() => {
        this.toggleBlockUI(false);
        this.ref.close(this.form.value);
      });
  }

  buildForm() {
    this.form = this.fb.group(
      {
        newPassword: new FormControl(
          null,
          Validators.compose([
            Validators.required,
            Validators.pattern(
              '^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{8,}$'
            ),
          ])
        ),
        confirmNewPassword: new FormControl(null, Validators.required),
      },
      { validators: passwordMatchingValidatior }
    );
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.btnDisabled = true;
      this.blockedPanelDetail = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.blockedPanelDetail = false;
      }, 1000);
    }
  }
}
export const passwordMatchingValidatior: ValidatorFn = (
  control: AbstractControl
): ValidationErrors | null => {
  const password = control.get('newPassword');
  const confirmPassword = control.get('confirmNewPassword');

  if (!password || !confirmPassword) {
    return null; 
  }

  return password.value === confirmPassword.value
    ? null
    : { notMatched: true };
};
