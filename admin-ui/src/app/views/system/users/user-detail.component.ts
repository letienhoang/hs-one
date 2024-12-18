import {
  Component,
  OnInit,
  EventEmitter,
  OnDestroy,
  ChangeDetectorRef,
} from '@angular/core';
import {
  Validators,
  FormControl,
  FormGroup,
  FormBuilder,
} from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { DomSanitizer } from '@angular/platform-browser';
import { formatDate } from '@angular/common';
import {
  AdminApiRoleApiClient,
  AdminApiUserApiClient,
  RoleDto,
  UserDto,
} from '../../../api/admin-api.service.generated';
import { UserSharedModule } from './user-shared.module';
import { UtilityService } from '../../../shared/services/utility.service';


@Component({
  templateUrl: 'user-detail.component.html',
  standalone: true,
  imports: [
    UserSharedModule,
],
})
export class UserDetailComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  // Default
  public blockedPanelDetail: boolean = false;
  public form: FormGroup = new FormGroup({});
  public title: string = '';
  public btnDisabled = false;
  public saveBtnName: string = '';
  public roles: any[] = [];
  selectedEntity = {} as UserDto;
  public avatarImage = '';

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private roleApiService: AdminApiRoleApiClient,
    private userApiService: AdminApiUserApiClient,
    private utilService: UtilityService,
    private fb: FormBuilder,
    private cd: ChangeDetectorRef,
    private sanitizer: DomSanitizer
  ) {}
  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  // Validate
  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    fullName: [{ type: 'required', message: 'You must enter a name' }],
    email: [{ type: 'required', message: 'You must enter an email' }],
    userName: [{ type: 'required', message: 'You must enter a username' }],
    password: [
      { type: 'required', message: 'You must enter a password' },
      {
        type: 'pattern',
        message:
          'The password must be at least 8 characters long, include at least one number, one special character, and one uppercase letter',
      },
    ],
    phoneNumber: [
      { type: 'required', message: 'You must enter a phone number' },
    ],
  };

  ngOnInit() {
    //Init form
    this.buildForm();
    //Load data to form
    var roles = this.roleApiService.getAllRoles();
    this.toggleBlockUI(true);
    forkJoin({
      roles,
    })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (repsonse: any) => {
          //Push categories to dropdown list
          var roles = repsonse.roles as RoleDto[];
          roles.forEach((element) => {
            this.roles.push({
              value: element.id,
              label: element.name,
            });
          });

          if (this.utilService.isEmpty(this.config.data?.id) == false) {
            this.loadFormDetails(this.config.data?.id);
          } else {
            this.setMode('create');
            this.toggleBlockUI(false);
          }
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }
  loadFormDetails(id: string) {
    this.userApiService
      .getUser(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: UserDto) => {
          this.selectedEntity = response;
          this.buildForm();
          this.setMode('update');

          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  onFileChange(event: any) {
    const reader = new FileReader();

    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.form.patchValue({
          avatarFileName: file.name,
          avatarFileContent: reader.result,
        });

        // need to run CD since file load runs outside of zone
        this.cd.markForCheck();
      };
    }
  }
  saveChange() {
    this.toggleBlockUI(true);
    this.saveData();
  }

  private saveData() {
    this.toggleBlockUI(true);
    console.log(this.form.value);
    if (this.utilService.isEmpty(this.config.data?.id)) {
      this.userApiService
        .createUser(this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.ref.close(this.form.value);
            this.toggleBlockUI(false);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    } else {
      this.userApiService
        .updateUser(this.config.data?.id, this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.toggleBlockUI(false);

            this.ref.close(this.form.value);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    }
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

  setMode(mode: string) {
    if (mode == 'update') {
      this.form.controls['userName'].clearValidators();
      this.form.controls['userName'].disable();
      this.form.controls['email'].clearValidators();
      this.form.controls['email'].disable();
      this.form.controls['password'].clearValidators();
      this.form.controls['password'].disable();
    } else if (mode == 'create') {
      this.form.controls['userName'].addValidators(Validators.required);
      this.form.controls['userName'].enable();
      this.form.controls['email'].addValidators(Validators.required);
      this.form.controls['email'].enable();
      this.form.controls['password'].addValidators(Validators.required);
      this.form.controls['password'].enable();
    }
  }
  buildForm() {
    this.form = this.fb.group({
      firstName: new FormControl(
        this.selectedEntity.firstName || null,
        Validators.required
      ),
      lastName: new FormControl(
        this.selectedEntity.lastName || null,
        Validators.required
      ),
      userName: new FormControl(
        this.selectedEntity.userName || null,
        Validators.required
      ),
      email: new FormControl(
        this.selectedEntity.email || null,
        Validators.required
      ),
      phoneNumber: new FormControl(
        this.selectedEntity.phoneNumber || null,
        Validators.required
      ),
      password: new FormControl(
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern(
            '^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{8,}$'
          ),
        ])
      ),
      dob: new FormControl(
        this.selectedEntity.dob
          ? formatDate(this.selectedEntity.dob, 'yyyy-MM-dd', 'en')
          : null
      ),
      avatarFile: new FormControl(null),
      avatar: new FormControl(this.selectedEntity.avatar || null),
      isActive: new FormControl(this.selectedEntity.isActive || true),
    });
  }
}
