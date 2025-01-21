import {
  Component,
  OnInit,
  EventEmitter,
  OnDestroy,
} from '@angular/core';
import {
  Validators,
  FormControl,
  FormGroup,
  FormBuilder,
} from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import {
  AdminApiPostApiClient,
  AdminApiPostCategoryApiClient,
  AdminApiTagApiClient,
  PostCategoryDto,
  PostDto,
  TagDto,
} from '../../../api/admin-api.service.generated';
import { UtilityService } from '../../../shared/services/utility.service';
import { UploadService } from '../../../shared/services/upload.service';
import { PostSharedModule } from './post-shared.module';
import { environment } from '../../../../environments/environment';
import { ToastService } from '../../../shared/services/toast.service';

interface AutoCompleteCompleteEvent {
  originalEvent: Event;
  query: string;
}

@Component({
  templateUrl: 'post-detail.component.html',
  standalone: true,
  imports: [
    PostSharedModule
],
})
export class PostDetailComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  // Default
  public blockedPanelDetail: boolean = false;
  public form: FormGroup = new FormGroup({});
  public title: string = '';
  public btnDisabled = false;
  public saveBtnName: string = '';
  selectedEntity = {} as PostDto;
  public avatarImage = '';
  public thumbnailImage = '';
  public canImportImage = false;
  public postCategories: any[] = [];

  public tagList: any[] = [];
  public filteredTags: any[] = [];

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private postApiService: AdminApiPostApiClient,
    private postCategoryApiService: AdminApiPostCategoryApiClient,
    private tagApiService: AdminApiTagApiClient,
    private utilService: UtilityService,
    private fb: FormBuilder,
    private uploadService: UploadService,
    public toastService: ToastService,
  ) {}

  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    title: [
      { type: 'required', message: 'You must enter a title' },
      { type: 'minlength', message: 'The title must be at least 3 characters long' },
      { type: 'maxlength', message: 'The title cannot be more than 256 characters long' },
    ],
    slug: [{ type: 'required', message: 'You must enter an slug' }],
    categoryId: [{ type: 'required', message: 'You must enter a slug category' }],
    description: [{ type: 'maxlength', message: 'The description cannot be more than 256 characters long' }],
    source: [{ type: 'maxlength', message: 'The source cannot be more than 512 characters long' }],
    tags: [{ type: 'maxlength', message: 'The tags cannot be more than 256 characters long' }],
    seoDescription: [{ type: 'maxlength', message: 'The seo description cannot be more than 160 characters long' }],
  };

  ngOnInit() {
    this.buildForm();
    this.toggleBlockUI(true);
    this.loadPostCategories();
    this.loadTags();
    if (this.utilService.isEmpty(this.config.data?.id) == false) {
      this.loadFormDetails(this.config.data?.id);
    } else {
      this.toggleBlockUI(false);
    }
  }

  loadFormDetails(id: string) {
    this.postApiService
      .getPost(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostDto) => {
          this.selectedEntity = response;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadPostCategories() {
    this.postCategoryApiService
      .getAllPostCategories()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostCategoryDto[]) => {
          response.forEach((element) => {
            this.postCategories.push({
              label: element.name,
              value: element.id,
            });
          });
        },
        error: (error) => {
          this.toastService.showError(error);
        },
      });
  }

  loadTags() {
    this.tagApiService
      .getAllNameTags()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: string[]) => {
          this.tagList = response;
        },
        error: (error) => {
          this.toastService.showError(error);
        },
      });
  }
    
  buildForm() {
    this.form = this.fb.group({
      title: new FormControl(this.selectedEntity.title || null,Validators.compose([
        Validators.required,
        Validators.maxLength(256),
        Validators.minLength(3)
      ])),
      slug: new FormControl(this.selectedEntity.slug || null,Validators.required),
      categoryId: new FormControl(this.selectedEntity.categoryId || null,Validators.required),
      description: new FormControl(this.selectedEntity.description || null, Validators.maxLength(256)),
      thumbnail: new FormControl(this.selectedEntity.thumbnail || null),
      content: new FormControl(this.selectedEntity.content || null),
      source: new FormControl(this.selectedEntity.source || null, Validators.maxLength(512)),
      tags: new FormControl(this.convertStringToArray(this.selectedEntity.tags || '') || [], Validators.maxLength(256)),
      seoDescription: new FormControl(this.selectedEntity.seoDescription || null, Validators.maxLength(160)),
    });
    if (this.selectedEntity.thumbnail) {
      this.thumbnailImage = environment.API_URL + this.selectedEntity.thumbnail;
    }
    this.generateSlug();
  }

  onFileChange(event: any) {
    if (event.target.files && event.target.files.length) {
      this.uploadService.uploadImage('posts', this.utilService.makeSeoTitle(this.form.value.title), event.target.files)
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
      this.postApiService
        .createPost(this.form.value)
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
      this.postApiService
        .updatePost(this.config.data?.id, this.form.value)
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
    var slug = this.utilService.makeSeoTitle(this.form.value.title);
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

  filterTags(event: AutoCompleteCompleteEvent) {
    let filtered: any[] = [];
    let query = event.query;

    for (let i = 0; i < (this.tagList as any[]).length; i++) {
        let item = (this.tagList as any[])[i];
        if (item.toLowerCase().indexOf(query.toLowerCase()) == 0) {
            filtered.push(item);
        }
    }

    if (filtered.length == 0) {
        filtered.push(query.trim());
    }

    this.filteredTags = filtered;
  }

  convertStringToArray(tagsString: string): string[] {
    const arr = tagsString.split(',').map(tag => tag.trim());
    if (arr.length == 1 && tagsString == '') {
      return [];
    }
    return arr || [];
  }

  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
