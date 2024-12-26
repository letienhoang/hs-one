import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiRoyaltyApiClient, TransactionDto, TransactionDtoPagedResult } from '../../../api/admin-api.service.generated';
import { ToastService } from '../../../shared/services/toast.service';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { Panel } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { InputGroup } from 'primeng/inputgroup';
import { ButtonModule } from 'primeng/button';
import { BadgeModule } from 'primeng/badge';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUI } from 'primeng/blockui';
import { ProgressSpinner } from 'primeng/progressspinner';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { DatePicker } from 'primeng/datepicker';
import { FloatLabel } from 'primeng/floatlabel';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  standalone: true,
  imports: [
    CommonModule,
    Panel,
    TableModule,
    InputTextModule,
    InputGroup,
    ButtonModule,
    BadgeModule,
    PaginatorModule,
    BlockUI,
    ProgressSpinner,
    FormsModule,
    DatePicker,
    FloatLabel
  ],
})
export class TransactionComponent implements OnInit, OnDestroy {
  // System variables
  private ngUnsubscribe: Subject<void> = new Subject<void>();
  public isBlockUI: boolean = false;

  // Pagination variables
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalRecords: number = 0;

  // Buisness variables
  public items: TransactionDto[] = [];
  public selectedItems: TransactionDto[] = [];
  public userName: string = '';
  public fromMonth: number = 1;
  public fromYear: number = new Date().getFullYear();
  public toMonth: number = 12;
  public toYear: number = new Date().getFullYear();
  public rangeDate: Date = new Date();

  constructor(
    public royaltyApiService: AdminApiRoyaltyApiClient,
    public toastService: ToastService,
    public dialogService: DialogService,
    public confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadDatas();
  }

  loadDatas() {
    this.toggleBlockUI(true);
    this.royaltyApiService
      .getTransactionHistory(
        this.userName,
        this.fromMonth,
        this.fromYear,
        this.toMonth,
        this.toYear,
        this.pageIndex, 
        this.pageSize)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: TransactionDtoPagedResult) => {
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

  dateChanged(event: any) {
    if (event[0] != null) {
        this.fromMonth = event[0].getMonth() + 1;
        this.fromYear = event[0].getFullYear();
    }
    if (event[1] != null) {
        this.toMonth = event[1].getMonth() + 1;
        this.toYear = event[1].getFullYear();
    }
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

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
