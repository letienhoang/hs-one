import { Component, OnInit, EventEmitter, OnDestroy } from '@angular/core';
import {
    Validators,
    FormControl,
    FormGroup,
    FormBuilder,
} from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiRoleApiClient, RoleDto } from '../../../api/admin-api.service.generated';
import { UtilityService } from '../../../shared/services/utility.service';
import { KeyFilterModule } from 'primeng/keyfilter';
import { HSOneSharedModule } from '../../../shared/modules/hs-one-shared.module';
import { RolesSharedModule } from './roles-shared.module';

@Component({
    templateUrl: 'roles-detail.component.html',
    standalone: true,
    providers: [],
    imports: [
        RolesSharedModule,
        HSOneSharedModule,
        KeyFilterModule
    ]
})
export class RolesDetailComponent implements OnInit, OnDestroy {
    private ngUnsubscribe = new Subject<void>();

    // Default
    public blockedPanelDetail: boolean = false;
    public form: FormGroup;
    public title: string;
    public btnDisabled = false;
    public saveBtnName: string;
    public closeBtnName: string;
    selectedEntity = {} as RoleDto;

    formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

    constructor(
        public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private roleService: AdminApiRoleApiClient,
        private utilService: UtilityService,
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
        if (this.utilService.isEmpty(this.config.data?.id) == false) {
            this.loadDetail(this.config.data.id);
            this.saveBtnName = 'Update';
            this.closeBtnName = 'Cancel';
        } else {
            this.saveBtnName = 'Create';
            this.closeBtnName = 'Close';
        }
    }

    // Validate
    noSpecial: RegExp = /^[^<>*!_~]+$/;
    validationMessages = {
        name: [
            { type: 'required', message: 'You must enter a name' },
            { type: 'minlength', message: 'You must enter at least 3 characters' },
            { type: 'maxlength', message: 'You cannot enter more than 255 characters' },
        ],
        displayName: [{ type: 'required', message: 'You must display name' }],
    };

    loadDetail(id: any) {
        this.toggleBlockUI(true);
        this.roleService
            .get(id)
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe({
                next: (response: RoleDto) => {
                    this.selectedEntity = response;
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
        if (this.utilService.isEmpty(this.config.data?.id)) {
            this.roleService
                .create(this.form.value)
                .pipe(takeUntil(this.ngUnsubscribe))
                .subscribe(() => {
                    this.ref.close(this.form.value);
                    this.toggleBlockUI(false);
                });
        } else {
            this.roleService
                .update(this.config.data.id, this.form.value)
                .pipe(takeUntil(this.ngUnsubscribe))
                .subscribe(() => {
                    this.toggleBlockUI(false);
                    this.ref.close(this.form.value);
                });
        }
    }

    buildForm() {
        this.form = this.fb.group({
            name: new FormControl(
                this.selectedEntity.name || null,
                Validators.compose([
                    Validators.required,
                    Validators.maxLength(255),
                    Validators.minLength(3),
                ])
            ),
            displayName: new FormControl(
                this.selectedEntity.displayName || null,
                Validators.required
            ),
        });
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
