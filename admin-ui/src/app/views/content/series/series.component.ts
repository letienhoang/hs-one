import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiSeriesApiClient, SeriesDto, SeriesInListDto, SeriesInListDtoPagedResult } from '../../../api/admin-api.service.generated';
import { ToastService } from '../../../shared/services/toast.service';
import { MessageConstants } from '../../../shared/constants/messages.constants';
import { DialogService, DynamicDialogComponent } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { SeriesSharedModule } from './series-shared.module';
import { SeriesDetailComponent } from './series-detail.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-series',
  templateUrl: './series.component.html',
  styleUrls: ['./series.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
  standalone: true,
  imports: [
    SeriesSharedModule,
    FormsModule
  ],
})
export class SeriesComponent implements OnInit, OnDestroy {
  // System variables
  private ngUnsubscribe: Subject<void> = new Subject<void>();
  public isBlockUI: boolean = false;

  // Pagination variables
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalRecords: number = 0;

  // Buisness variables
  public items: SeriesInListDto[] = [];
  public selectedItems: SeriesInListDto[] = [];
  public keyword: string = '';

  constructor(
    public seriesApiService: AdminApiSeriesApiClient,
    public toastService: ToastService,
    public dialogService: DialogService,
    public confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadDatas();
  }

  loadDatas() {
    this.toggleBlockUI(true);
    this.seriesApiService
      .getSeriesPaging(this.keyword, this.pageIndex, this.pageSize)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: SeriesInListDtoPagedResult) => {
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
    const ref = this.dialogService.open(SeriesDetailComponent, {
      header: 'Add Series',
      width: '70%',
      modal: true,
      closable: true
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: SeriesDto) => {
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
    const ref = this.dialogService.open(SeriesDetailComponent, {
      data: {
        id: id,
      },
      header: 'Update Series',
      width: '70%',
      modal: true,
      closable: true
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: SeriesDto) => {
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

    this.seriesApiService.deleteSeries(ids).subscribe({
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
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
