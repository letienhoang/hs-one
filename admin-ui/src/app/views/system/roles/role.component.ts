import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {
  UtilitiesModule,
  GridModule,
  TooltipModule
  } from '@coreui/angular';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiRoleApiClient, RoleDto, RoleDtoPagedResult } from 'src/app/api/admin-api.service.generated';
import { ToastService } from 'src/app/shared/services/toast.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BlockUIModule } from 'primeng/blockui';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { PanelModule } from 'primeng/panel';
import { NgIf } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { MessageConstants } from 'src/app/shared/constants/messages.constants';
import { DialogService, DynamicDialogComponent } from 'primeng/dynamicdialog';
import { RolesDetailComponent } from './roles-detail.component';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { InputGroupModule } from 'primeng/inputgroup';

@Component({
  selector: 'app-roles',
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
  standalone: true,
  imports: [
    TableModule, 
    UtilitiesModule,
    GridModule,
    ProgressSpinnerModule,
    BlockUIModule,
    PaginatorModule,
    TooltipModule,
    PanelModule,
    NgIf,
    InputTextModule,
    ButtonModule,
    RippleModule,
    InputGroupModule
  ],
})
export class RoleComponent implements OnInit, OnDestroy {
  // System variables
  private ngUnsubcribe: Subject<void> = new Subject<void>();
  public isBlockUI: boolean = false;

  // Pagination variables
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalRecords: number = 0;

  // Buisness variables
  public items: RoleDto[] = [];
  public selectedItems: RoleDto[] = [];
  public keyword: string = '';

  constructor(
    public roleApiService: AdminApiRoleApiClient,
    public toastService: ToastService,
    public dialogService: DialogService,
    public confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadDatas();
  }

  loadDatas() {
    this.toggleBlockUI(true);
    this.roleApiService
      .getPaging(this.keyword, this.pageIndex, this.pageSize)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: (response: RoleDtoPagedResult) => {
          this.items = response.results;
          this.totalRecords = response.rowCount;
          this.toggleBlockUI(false);
        },
        error: (error) => {
          this.toastService.showError(error);
          this.toggleBlockUI(false);
        },
      });
  }

  pageChanged(event: any) {
    this.pageIndex = event.page;
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
    const ref = this.dialogService.open(RolesDetailComponent, {
      header: 'Add Role',
      width: '70%',
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: RoleDto) => {
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
    const ref = this.dialogService.open(RolesDetailComponent, {
      data: {
        id: id,
      },
      header: 'Update Role',
      width: '70%',
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: RoleDto) => {
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
    var ids = [];
    this.selectedItems.forEach((element) => {
        ids.push(element.id);
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

    this.roleApiService.delete(ids).subscribe({
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

  showPermissionModal(role: string) {
    console.log(role);
  }

  ngOnDestroy(): void {
    this.ngUnsubcribe.next();
    this.ngUnsubcribe.complete();
  }
}
