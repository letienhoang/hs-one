import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiPostApiClient, PostActivityLogDto } from '../../../api/admin-api.service.generated';
import { ToastService } from '../../../shared/services/toast.service';
import { MessageConstants } from '../../../shared/constants/messages.constants';
import { DialogService, DynamicDialogComponent, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { PostSharedModule } from './post-shared.module';

@Component({
  templateUrl: './post-activity-logs.component.html',
  standalone: true,
  imports: [
    PostSharedModule,
  ],
})
export class PostActivityLogsComponent implements OnInit, OnDestroy {
  // System variables
  private ngUnsubscribe: Subject<void> = new Subject<void>();
  public isBlockUI: boolean = false;

  // Buisness variables
  public items: PostActivityLogDto[] = [];
  public selectedItems: PostActivityLogDto[] = [];
  public keyword: string = '';

  constructor(
    public postApiService: AdminApiPostApiClient,
    public toastService: ToastService,
    public dialogService: DialogService,
    public confirmationService: ConfirmationService,
    public config: DynamicDialogConfig,
  ) {}

  ngOnInit(): void {
    this.loadDatas();
  }

  loadDatas() {
    this.toggleBlockUI(true);
    this.postApiService
      .getActivityLogs(this.config.data.id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostActivityLogDto[]) => {
          this.items = response || [];
          this.toggleBlockUI(false);
        },
        error: (error) => {
          this.toastService.showError(error);
          this.toggleBlockUI(false);
        },
      });
  }

  private toggleBlockUI(enable: boolean) {
    if (enable) {
      this.isBlockUI = true;
    } else {
      setTimeout(() => {
        this.isBlockUI = false;
      }, 1000);
    }
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
