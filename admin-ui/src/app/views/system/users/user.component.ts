import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogComponent } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiUserApiClient, UserDto, UserDtoPagedResult } from '../../../api/admin-api.service.generated';
import { MessageConstants } from '../../../shared/constants/messages.constants';
import { ToastService } from '../../../shared/services/toast.service';
import { UserSharedModule } from './user-shared.module';
import { UserDetailComponent } from './user-detail.component';
import { SetPasswordComponent } from './set-password.component';
import { ChangeEmailComponent } from './change-email.component';
import { RoleAssignComponent } from './role-assign.component';

@Component({
    selector: 'app-users',
    templateUrl: './user.component.html',
    styleUrls: ['./user.component.scss'],
    changeDetection: ChangeDetectionStrategy.Default,
    standalone: true,
    imports: [
      UserSharedModule
    ]
})
export class UserComponent implements OnInit, OnDestroy {
  //System variables
  private ngUnsubscribe = new Subject<void>();
  public isBlockUI: boolean = false;

  //Paging variables
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalRecords: number = 0;

  //Business variables
  public items: UserDto[] = [];
  public selectedItems: UserDto[] = [];
  public keyword: string = '';


  constructor(
    private userApiService: AdminApiUserApiClient,
    public dialogService: DialogService,
    private alertService: ToastService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadDatas();
  }

  loadDatas(selectionId: string | null | undefined = null) {
    this.toggleBlockUI(true);
    this.userApiService
      .getUsersPaging(this.keyword, this.pageIndex, this.pageSize)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: UserDtoPagedResult) => {
          this.items = response.results || [];
          this.totalRecords = response.rowCount ?? 0;
          if (selectionId != null && this.items.length > 0) { 
            this.selectedItems = this.items.filter(x => x.id == selectionId);
          }
          this.toggleBlockUI(false);
        },
        error: (error: any) => {
          this.alertService.showError(MessageConstants.GET_DATA_FAIL);
          this.toggleBlockUI(false);
        }
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

  showAddModal(){
    const ref = this.dialogService.open(UserDetailComponent, {
      header: 'Create new user',
      width: '70%',
      modal: true,
      closable: true
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: UserDto) => {
        if (data) {
            this.alertService.showSuccess(
                MessageConstants.CREATED_OK_MSG
            );
            this.selectedItems = [];
            this.loadDatas();
        }
    });
  }

  showEditModal(){
    if (this.selectedItems.length == 0) {
      this.alertService.showError(
          MessageConstants.NOT_CHOOSE_ANY_RECORD
      );
      return;
    }
    var id = this.selectedItems[0].id;
    const ref = this.dialogService.open(UserDetailComponent, {
        data: {
            id: id,
        },
        header: 'Update user',
        width: '70%',
        modal: true,
        closable: true
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: UserDto) => {
        if (data) {
            this.alertService.showSuccess(
                MessageConstants.UPDATED_OK_MSG
            );
            this.selectedItems = [];
            this.loadDatas(data.id);
        }
    });
  }

  deleteItems(){
    if (this.selectedItems.length == 0) {
      this.alertService.showWarning(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    var ids: string[] = [];
    this.selectedItems.forEach((item) => {
      ids.push(item.id);
    });
    this.confirmationService.confirm({
      message: MessageConstants.CONFIRM_DELETE_MSG,
      accept: () => {
        this.deleteItemsComfirm(ids);
      }
    });
  }

  deleteItemsComfirm(ids: any[]){
    this.toggleBlockUI(true);
    this.userApiService.deleteUsers(ids).subscribe({
      next: () => {
        this.alertService.showSuccess(MessageConstants.DELETED_OK_MSG);
        this.loadDatas();
        this.selectedItems = [];
        this.toggleBlockUI(false);
      },
      error: () => {
        this.toggleBlockUI(false);
      }
    });
  }

  setPassword(id: string, username: string){
    const ref = this.dialogService.open(SetPasswordComponent, {
      data: {
          id: id,
      },
      header: 'Set password for ' + username,
      width: '70%',
      modal: true,
      closable: true
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((result: boolean) => {
        if (result) {
            this.alertService.showSuccess(
                MessageConstants.CHANGE_PASSWORD_SUCCCESS_MSG
            );
            this.selectedItems = [];
            this.loadDatas();
        }
    });
  }

  changeEmail(id: string){
    const ref = this.dialogService.open(ChangeEmailComponent, {
      data: {
          id: id,
      },
      header: 'Change email',
      width: '70%',
      modal: true,
      closable: true
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((result: boolean) => {
        if (result) {
            this.alertService.showSuccess(
                MessageConstants.CHANGE_EMAIL_SUCCCESS_MSG
            );
            this.selectedItems = [];
            this.loadDatas();
        }
    });
  }

  assignRole(id: string){
    const ref = this.dialogService.open(RoleAssignComponent, {
      data: {
          id: id,
      },
      header: 'Assign role',
      width: '70%',
      modal: true,
      closable: true
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((result: boolean) => {
        if (result) {
            this.alertService.showSuccess(
                MessageConstants.ROLE_ASSIGN_SUCCESS_MSG
            );
            this.loadDatas();
        }
    });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  
}
