import {
  Component,
  OnInit,
  EventEmitter,
  OnDestroy,
  ChangeDetectorRef,
} from '@angular/core';
import {
  Validators,
  FormControl,
  FormGroup,
  FormBuilder,
} from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import {
  AdminApiSeriesApiClient,
  SeriesDto,
} from '../../../api/admin-api.service.generated';
import { UtilityService } from '../../../shared/services/utility.service';
import { UploadService } from '../../../shared/services/upload.service';
import { SeriesSharedModule } from './series-shared.module';
import { environment } from '../../../../environments/environment';


@Component({
  templateUrl: 'series-detail.component.html',
  standalone: true,
  imports: [
    SeriesSharedModule,
],
})
export class SeriesDetailComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  // Default
  public blockedPanelDetail: boolean = false;
  public form: FormGroup = new FormGroup({});
  public title: string = '';
  public btnDisabled = false;
  public saveBtnName: string = '';
  public parentIds: any[] = [];
  selectedEntity = {} as SeriesDto;
  public avatarImage = '';
  public thumbnailImage = '';
  public canImportImage = false;

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private postCategoryApiService: AdminApiSeriesApiClient,
    private utilService: UtilityService,
    private fb: FormBuilder,
    private uploadService: UploadService
  ) {}

  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    name: [
      { type: 'required', message: 'You must enter a name' },
      { type: 'minlength', message: 'The name must be at least 3 characters long' },
      { type: 'maxlength', message: 'The name cannot be more than 256 characters long' },
    ],
    slug: [{ type: 'required', message: 'You must enter an slug' }],
    sortOrder: [{ type: 'required', message: 'You must enter a sort order' }],
    description: [{ type: 'maxlength', message: 'The description cannot be more than 256 characters long' }],
    seoDescription: [{ type: 'maxlength', message: 'The seo description cannot be more than 160 characters long' }],
  };

  ngOnInit() {
    this.buildForm();
    this.toggleBlockUI(true);
    if (this.utilService.isEmpty(this.config.data?.id) == false) {
      this.loadFormDetails(this.config.data?.id);
    } else {
      this.toggleBlockUI(false);
    }
  }

  loadFormDetails(id: string) {
    this.postCategoryApiService
      .getSeries(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: SeriesDto) => {
          this.selectedEntity = response;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }
    
  buildForm() {
    this.form = this.fb.group({
      name: new FormControl(this.selectedEntity.name || null,Validators.compose([
        Validators.required,
        Validators.maxLength(256),
        Validators.minLength(3)
      ])),
      slug: new FormControl(this.selectedEntity.slug || null,Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || 1,Validators.required),
      description: new FormControl(this.selectedEntity.description || null, Validators.maxLength(256)),
      isActive: new FormControl(this.selectedEntity.isActive || true),
      seoDescription: new FormControl(this.selectedEntity.seoDescription || null, Validators.maxLength(160)),
      thumbnail: new FormControl(this.selectedEntity.thumbnail || null),
      content: new FormControl(this.selectedEntity.content || null),
    });
    if (this.selectedEntity.thumbnail) {
      this.thumbnailImage = environment.API_URL + this.selectedEntity.thumbnail;
    }
    this.generateSlug();
  }

  onFileChange(event: any) {
    if (event.target.files && event.target.files.length) {
      this.uploadService.uploadImage('series', this.utilService.makeSeoTitle(this.form.value.name), event.target.files)
        .subscribe({
          next: (response: any) => {
            this.form.controls['thumbnail'].setValue(response.path);
            this.thumbnailImage = environment.API_URL + response.path;
          },
          error: (err: any) => {
            console.log(err);
          }
        });
    }
  }

  saveChange() {
    this.toggleBlockUI(true);
    this.saveData();
  }

  private saveData() {
    this.toggleBlockUI(true);

    if (this.utilService.isEmpty(this.config.data?.id)) {
      this.postCategoryApiService
        .createSeries(this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.ref.close(this.form.value);
            this.toggleBlockUI(false);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    } else {
      this.postCategoryApiService
        .updateSeries(this.config.data?.id, this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.toggleBlockUI(false);

            this.ref.close(this.form.value);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    }
  }

  public generateSlug() {
    var slug = this.utilService.makeSeoTitle(this.form.value.name);
    this.form.patchValue({
      slug: slug,
    });
    if (slug.length > 0) {
      this.canImportImage = true;
    }
    else {
      this.canImportImage = false;
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

  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
