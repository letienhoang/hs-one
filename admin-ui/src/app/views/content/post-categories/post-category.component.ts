import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiPostCategoryApiClient, PostCategoryDto, PostCategoryDtoPagedResult } from '../../../api/admin-api.service.generated';
import { ToastService } from '../../../shared/services/toast.service';
import { MessageConstants } from '../../../shared/constants/messages.constants';
import { DialogService, DynamicDialogComponent } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { PostCategorySharedModule } from './post-category-shared.module';

@Component({
  selector: 'app-post-categoryies',
  templateUrl: './post-category.component.html',
  styleUrls: ['./post-category.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
  standalone: true,
  imports: [
    PostCategorySharedModule
  ],
})
export class PostCategoryComponent implements OnInit, OnDestroy {
  // System variables
  private ngUnsubcribe: Subject<void> = new Subject<void>();
  public isBlockUI: boolean = false;

  // Pagination variables
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalRecords: number = 0;

  // Buisness variables
  public items: PostCategoryDto[] = [];
  public selectedItems: PostCategoryDto[] = [];
  public keyword: string = '';

  constructor(
    public postCategoryApiService: AdminApiPostCategoryApiClient,
    public toastService: ToastService,
    public dialogService: DialogService,
    public confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadDatas();
  }

  loadDatas() {
    this.toggleBlockUI(true);
    this.postCategoryApiService
      .getPostCategoriesPaging(this.keyword, this.pageIndex, this.pageSize)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: (response: PostCategoryDtoPagedResult) => {
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
    // const ref = this.dialogService.open(PostCategoryDetailComponent, {
    //   header: 'Add Post Category',
    //   width: '70%',
    // });
    // const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    // const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    // const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    // dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    // ref.onClose.subscribe((data: PostCategoryDto) => {
    //   if (data) {
    //     this.toastService.showSuccess(MessageConstants.CREATED_OK_MSG);
    //     this.selectedItems = [];
    //     this.loadDatas();
    //   }
    // });
  }

  showEditModal() {
    // if (this.selectedItems.length == 0) {
    //   this.toastService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
    //   return;
    // }
    // var id = this.selectedItems[0].id;
    // const ref = this.dialogService.open(PostCategoryDetailComponent, {
    //   data: {
    //     id: id,
    //   },
    //   header: 'Update Post Category',
    //   width: '70%',
    // });
    // const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    // const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    // const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    // dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    // ref.onClose.subscribe((data: PostCategoryDto) => {
    //   if (data) {
    //     this.toastService.showSuccess(MessageConstants.UPDATED_OK_MSG);
    //     this.selectedItems = [];
    //     this.loadDatas();
    //   }
    // });
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

    this.postCategoryApiService.deletePostCategories(ids).subscribe({
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

  ngOnDestroy(): void {
    this.ngUnsubcribe.next();
    this.ngUnsubcribe.complete();
  }
}
