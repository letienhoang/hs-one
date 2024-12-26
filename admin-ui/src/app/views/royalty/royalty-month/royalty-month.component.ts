import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiRoyaltyApiClient, AdminApiUserApiClient, RoyaltyReportByMonthDto, UserDto } from '../../../api/admin-api.service.generated';
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
import { Select } from 'primeng/select';

@Component({
  selector: 'app-royalty-month',
  templateUrl: './royalty-month.component.html',
  standalone: true,
  imports: [
    CommonModule,
    Panel,
    TableModule,
    InputTextModule,
    ButtonModule,
    BadgeModule,
    PaginatorModule,
    BlockUI,
    ProgressSpinner,
    FormsModule,
    DatePicker,
    FloatLabel,
    Select
  ],
})
export class RoyaltyReportMonthComponent implements OnInit, OnDestroy {
  // System variables
  private ngUnsubscribe: Subject<void> = new Subject<void>();
  public isBlockUI: boolean = false;

  // Buisness variables
  public items: RoyaltyReportByMonthDto[] = [];
  public selectedItems: RoyaltyReportByMonthDto[] = [];
  public userId: string = '';
  public fromMonth: number = 1;
  public fromYear: number = new Date().getFullYear();
  public toMonth: number = 12;
  public toYear: number = new Date().getFullYear();
  public rangeDate: Date = new Date();

  public users: any[] = [];

  constructor(
    public royaltyApiService: AdminApiRoyaltyApiClient,
    public userApiService: AdminApiUserApiClient,
    public toastService: ToastService,
    public dialogService: DialogService,
    public confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadDatas();
    this.loadUsers(); 
  }

  loadDatas() {
    this.toggleBlockUI(true);
    this.royaltyApiService
      .getRoyaltyReportByMonth(
        this.userId,
        this.fromMonth,
        this.fromYear,
        this.toMonth,
        this.toYear)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: RoyaltyReportByMonthDto[]) => {
          this.items = response || [];
          this.toggleBlockUI(false);
        },
        error: (error) => {
          this.toastService.showError(error);
          this.toggleBlockUI(false);
        },
      });
  }

  loadUsers() {
      this.userApiService
        .getAllUsers()
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: (response: UserDto[]) => {
            response.forEach((element) => {
              this.users.push({
                label: element.userName,
                value: element.id,
              });
            });
          },
          error: (error) => {
            this.toastService.showError(error);
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
