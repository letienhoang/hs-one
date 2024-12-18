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
import { DomSanitizer } from '@angular/platform-browser';
import { formatDate } from '@angular/common';
import {
  AdminApiPostCategoryApiClient,
  PostCategoryDto,
} from '../../../api/admin-api.service.generated';
import { UtilityService } from '../../../shared/services/utility.service';
import { PostCategorySharedModule } from './post-category-shared.module';


@Component({
  templateUrl: 'post-category-detail.component.html',
  standalone: true,
  imports: [
    PostCategorySharedModule,
],
})
export class PostCategoryDetailComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  // Default
  public blockedPanelDetail: boolean = false;
  public form: FormGroup = new FormGroup({});
  public title: string = '';
  public btnDisabled = false;
  public saveBtnName: string = '';
  public parentIds: any[] = [];
  selectedEntity = {} as PostCategoryDto;
  public avatarImage = '';

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private postCategoryApiService: AdminApiPostCategoryApiClient,
    private utilService: UtilityService,
    private fb: FormBuilder,
    private cd: ChangeDetectorRef,
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
  };

  ngOnInit() {
    this.buildForm();
    var postCategories = this.postCategoryApiService.getPostCategories();
    this.toggleBlockUI(true);
    forkJoin({
      postCategories,
    })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (repsonse: any) => {
          var postCategories = repsonse.postCategories as PostCategoryDto[];
          postCategories.forEach((element) => {
            this.parentIds.push({
              value: element.id,
              label: element.name,
            });
          });

          if (this.utilService.isEmpty(this.config.data?.id) == false) {
            this.loadFormDetails(this.config.data?.id);
          } else {
            this.toggleBlockUI(false);
          }
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadFormDetails(id: string) {
    this.postCategoryApiService
      .getPostCategory(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostCategoryDto) => {
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
      parentId: new FormControl(this.selectedEntity.parentId || null),
      isActive: new FormControl(this.selectedEntity.isActive || true),
      seoKeywords: new FormControl(this.selectedEntity.seoKeywords || null),
      seoDescription: new FormControl(this.selectedEntity.seoDescription || null)
    });
  }

  onFileChange(event: any) {
    const reader = new FileReader();

    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.form.patchValue({
          avatarFileName: file.name,
          avatarFileContent: reader.result,
        });

        // need to run CD since file load runs outside of zone
        this.cd.markForCheck();
      };
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
        .createPostCategory(this.form.value)
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
        .updatePostCategory(this.config.data?.id, this.form.value)
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
