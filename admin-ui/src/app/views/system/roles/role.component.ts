import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {
  UtilitiesModule,
  GridModule,
  ButtonModule,
  TooltipModule
  } from '@coreui/angular';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiRoleApiClient, RoleDto, RoleDtoPagedResult } from 'src/app/api/admin-api.service.generated';
import { ToastService } from 'src/app/shared/services/toast.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BlockUIModule } from 'primeng/blockui';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { IconDirective } from '@coreui/icons-angular';
import { PanelModule } from 'primeng/panel';
import { NgIf } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';

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
    ButtonModule,
    IconDirective,
    TooltipModule,
    PanelModule,
    NgIf,
    InputTextModule
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
  public roles: RoleDto[] = [];
  public selectedRoles: RoleDto[] = [];
  public keyword: string = '';

  constructor(
    public roleApiService: AdminApiRoleApiClient,
    public toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loadDatas();
  }

  loadDatas() {
    this.toggleBlockUi(true);
    this.roleApiService
      .getPaging(this.keyword, this.pageIndex, this.pageSize)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: (response: RoleDtoPagedResult) => {
          this.roles = response.results;
          this.totalRecords = response.rowCount;
          this.toggleBlockUi(false);
        },
        error: (error) => {
          this.toastService.showError(error);
          this.toggleBlockUi(false);
        },
      });
  }

  pageChanged(event: any) {
    this.pageIndex = event.page;
    this.pageSize = event.rows;
    this.loadDatas();
  }

  private toggleBlockUi(enable: boolean) {
    if (enable) {
      this.isBlockUI = true;
    } else {
      setTimeout(() => {
        this.isBlockUI = false;
      }, 1000);
    }
  }

  showAddModal() {
    console.log('Add new role');
  }

  showEditModal() {
    console.log('Edit role');
  }

  deleteItems() {
    console.log('Delete items');
  }

  showPermissionModal(role: string) {
    console.log(role);
  }

  ngOnDestroy(): void {
    this.ngUnsubcribe.next();
    this.ngUnsubcribe.complete();
  }
}
