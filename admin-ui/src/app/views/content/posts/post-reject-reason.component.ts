import {
  Component,
  OnInit,
  EventEmitter,
  OnDestroy,
} from '@angular/core';
import {
  Validators,
  FormControl,
  FormGroup,
  FormBuilder,
} from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import {
  AdminApiPostApiClient,
} from '../../../api/admin-api.service.generated';
import { PostSharedModule } from './post-shared.module';
import { ToastService } from '../../../shared/services/toast.service';


@Component({
  templateUrl: 'post-reject-reason.component.html',
  standalone: true,
  imports: [
    PostSharedModule
],
})
export class PostRejectReasonComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  // Default
  public blockedPanelRejectReason: boolean = false;
  public form: FormGroup = new FormGroup({});
  public title: string = '';
  public btnDisabled = false;
  public saveBtnName: string = '';

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private postApiService: AdminApiPostApiClient,
    private fb: FormBuilder,
    public toastService: ToastService,
  ) {}

  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    reason: [{ type: 'required', message: 'You must enter a reason' }],
  };

  ngOnInit() {
    this.buildForm();
  }

  buildForm() {
    this.form = this.fb.group({
      reason: new FormControl(null, Validators.required),
    });
  }

  saveChange() {
    this.toggleBlockUI(true);
    this.saveData();
  }

  private saveData() {
    this.toggleBlockUI(true);
    this.postApiService
      .rejectPost(this.config.data.id, this.form.value)
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
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.btnDisabled = true;
      this.blockedPanelRejectReason = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.blockedPanelRejectReason = false;
      }, 1000);
    }
  }

  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
