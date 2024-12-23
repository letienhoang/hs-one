import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiPostApiClient, AdminApiPostCategoryApiClient, PostCategoryDto, PostDto, PostInListDto, PostInListDtoPagedResult } from '../../../api/admin-api.service.generated';
import { ToastService } from '../../../shared/services/toast.service';
import { MessageConstants } from '../../../shared/constants/messages.constants';
import { DialogService, DynamicDialogComponent } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { PostSharedModule } from './post-shared.module';
import { PostDetailComponent } from './post-detail.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
  standalone: true,
  imports: [
    PostSharedModule,
    FormsModule
  ],
})
export class PostComponent implements OnInit, OnDestroy {
  // System variables
  private ngUnsubscribe: Subject<void> = new Subject<void>();
  public isBlockUI: boolean = false;

  // Pagination variables
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalRecords: number = 0;

  // Buisness variables
  public items: PostInListDto[] = [];
  public selectedItems: PostInListDto[] = [];
  public keyword: string = '';
  public categoryId: any = null;
  public postCategories: any[] = [];

  constructor(
    public postApiService: AdminApiPostApiClient,
    public postCategoryApiService: AdminApiPostCategoryApiClient,
    public toastService: ToastService,
    public dialogService: DialogService,
    public confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadDatas();
    this.loadPostCategories();
  }

  loadDatas() {
    this.toggleBlockUI(true);
    this.postApiService
      .getPostsPaging(this.keyword, this.categoryId,this.pageIndex, this.pageSize)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostInListDtoPagedResult) => {
          this.items = response.results || [];
          this.totalRecords = response.rowCount || 0;
          this.toggleBlockUI(false);
        },
        error: (error) => {
          this.toastService.showError(error);
          this.toggleBlockUI(false);
        },
      });
  }

  loadPostCategories() {
    this.postCategoryApiService
      .getAllPostCategories()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostCategoryDto[]) => {
          response.forEach((element) => {
            this.postCategories.push({
              label: element.name,
              value: element.id,
            });
          });
        },
        error: (error) => {
          this.toastService.showError(error);
        },
      });
  }

  pageChanged(event: any) {
    this.pageIndex = event.page + 1;
    this.pageSize = event.rows;
    this.loadDatas();
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

  showAddModal() {
    const ref = this.dialogService.open(PostDetailComponent, {
      header: 'Add Post',
      width: '70%',
      modal: true,
      closable: true
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: PostDto) => {
      if (data) {
        this.toastService.showSuccess(MessageConstants.CREATED_OK_MSG);
        this.selectedItems = [];
        this.loadDatas();
      }
    });
  }

  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.toastService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    var id = this.selectedItems[0].id;
    const ref = this.dialogService.open(PostDetailComponent, {
      data: {
        id: id,
      },
      header: 'Update Post',
      width: '70%',
      modal: true,
      closable: true
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: PostDto) => {
      if (data) {
        this.toastService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadDatas();
      }
    });
  }

  deleteItems() {
    if (this.selectedItems.length == 0) {
      this.toastService.showError(
          MessageConstants.NOT_CHOOSE_ANY_RECORD
      );
      return;
    }
    var ids: string[] = [];
    this.selectedItems.forEach((element) => {
            if (element.id !== undefined) {
                ids.push(element.id);
            }
        });
    this.confirmationService.confirm({
        message: MessageConstants.CONFIRM_DELETE_MSG,
        accept: () => {
            this.deleteItemsConfirm(ids);
        },
    });
  }

  deleteItemsConfirm(ids: any[]) {
    this.toggleBlockUI(true);

    this.postApiService.deletePosts(ids).subscribe({
        next: () => {
            this.toastService.showSuccess(
                MessageConstants.DELETED_OK_MSG
            );
            this.loadDatas();
            this.selectedItems = [];
            this.toggleBlockUI(false);
        },
        error: () => {
            this.toggleBlockUI(false);
        },
    });
  }

  addToSeries(id: string) {}

  approve(id: string) {}

  sendForApproval(id: string) {}

  reject(id: string) {}

  showLogs(id: string) {}

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
