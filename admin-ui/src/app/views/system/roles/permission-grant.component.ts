import { Component, OnInit, EventEmitter, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiRoleApiClient, PermissionDto, RoleClaimsDto } from '../../../api/admin-api.service.generated';
import { PanelModule } from 'primeng/panel';
import { CheckboxModule } from 'primeng/checkbox';
import { NgFor } from '@angular/common';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BlockUIModule } from 'primeng/blockui';

@Component({
    templateUrl: 'permission-grant.component.html',
    standalone: true,
    imports: [
        PanelModule,
        ReactiveFormsModule,
        CheckboxModule,
        NgFor,
        ProgressSpinnerModule,
        BlockUIModule,
        FormsModule
    ]
})
export class PermissionGrantComponent implements OnInit, OnDestroy {
    private ngUnsubscribe = new Subject<void>();

    // Default
    public blockedPanelDetail: boolean = false;
    public form: FormGroup;
    public title: string;
    public btnDisabled = false;
    public saveBtnName: string;
    public closeBtnName: string;
    public permissions: RoleClaimsDto[] = [];
    public selectedPermissions: RoleClaimsDto[] = [];
    public id: string;
    formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

    constructor(
        public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private roleService: AdminApiRoleApiClient,
        private fb: FormBuilder
    ) { }

    ngOnDestroy(): void {
        if (this.ref) {
            this.ref.close();
        }
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    ngOnInit() {
        this.buildForm();
        this.loadDetail(this.config.data.id);
        this.saveBtnName = 'Update';
        this.closeBtnName = 'Cancel';
    }

    loadDetail(roleId: string) {
        this.toggleBlockUI(true);
        this.roleService
            .getPermissions(roleId)
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe({
                next: (response: PermissionDto) => {
                    this.permissions = response.roleClaims || [];
                    this.buildForm();
                    this.toggleBlockUI(false);
                },
                error: () => {
                    this.toggleBlockUI(false);
                },
            });
    }
    saveChange() {
        this.toggleBlockUI(true);
        this.saveData();
    }

    private saveData() {
        var roleClaims: RoleClaimsDto[] = [];
        for (let index = 0; index < this.permissions.length; index++) {
            const isGranted = this.selectedPermissions.filter((x) => x.value == this.permissions[index].value).length > 0;

            roleClaims.push(new RoleClaimsDto({
                type: this.permissions[index].type,
                isSelected: isGranted,
                value: this.permissions[index].value
            }));
        }
        var updateValues = new PermissionDto({
            roleId: this.config.data.id,
            roleClaims: roleClaims,
        });
        this.roleService
            .updatePermissions(updateValues)
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(() => {
                this.toggleBlockUI(false);
                this.ref.close(this.form.value);
            });
    }

    buildForm() {
        this.form = this.fb.group({});
        //Fill value
        for (let index = 0; index < this.permissions.length; index++) {
            const permission = this.permissions[index];
            if (permission.isSelected) {
                this.selectedPermissions.push(new RoleClaimsDto({
                    isSelected: true,
                    displayName: permission.displayName,
                    type: permission.type,
                    value: permission.value
                }));
            }
        }
    }

    private toggleBlockUI(enabled: boolean) {
        if (enabled == true) {
            this.btnDisabled = true;
            this.blockedPanelDetail = true;
        } else {
            setTimeout(() => {
                this.btnDisabled = false;
                this.blockedPanelDetail = false;
            }, 1000);
        }
    }
}
