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
import { forkJoin, map, Subject, switchMap, takeUntil } from 'rxjs';
import {
  AddPostSeriesRequest,
  AdminApiPostApiClient,
  AdminApiSeriesApiClient,
  PostCategoryDto,
  PostInSeriesDto,
  SeriesInListDto,
} from '../../../api/admin-api.service.generated';
import { UtilityService } from '../../../shared/services/utility.service';
import { PostSharedModule } from './post-shared.module';
import { ToastService } from '../../../shared/services/toast.service';


@Component({
  templateUrl: 'post-series.component.html',
  standalone: true,
  imports: [
    PostSharedModule
],
})
export class PostSeriesComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  // Default
  public isBlockUI: boolean = false;
  public form: FormGroup = new FormGroup({});
  public btnDisabled = false;
  public saveBtnName: string = '';
  selectedEntity = {} as AddPostSeriesRequest;
  public series: any[] = [];
  public items: any[] = [];
  public selectedItems: SeriesInListDto[] = [];

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private postApiService: AdminApiPostApiClient,
    private seriesApiService: AdminApiSeriesApiClient,
    private utilService: UtilityService,
    private fb: FormBuilder,
    public toastService: ToastService,
  ) {}

  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    seriesId: [{ type: 'required', message: 'You must enter an Series' }],
    sortOrder: [{ type: 'required', message: 'You must enter a Sort Order' }],
  };

  ngOnInit() {
    this.buildForm();
    this.toggleBlockUI(true);
    this.loadSeries();
    if (this.utilService.isEmpty(this.config.data.id) == false) {
      this.loadSeriesForPost(this.config.data.id);
    } else {
      this.toggleBlockUI(false);
    }
  }

  loadSeriesForPost(id: string) {
    var allSeriesForPost = this.postApiService.getAllSeriesForPost(id);
    this.toggleBlockUI(false);
    allSeriesForPost
      .pipe(
        takeUntil(this.ngUnsubscribe),
        switchMap((allSeriesForPost: SeriesInListDto[]) => {
          const postInSeriesRequests = allSeriesForPost.map(series =>
            this.postApiService.getPostsInSeries(this.config.data.id, series.id)
            .pipe(
              map(postInSeries => ({
                postId: this.config.data.id,
                seriesId: series.id,
                seriesName: series.name,
                displayOrder: postInSeries.displayOrder 
              }))
            )
          );
          return forkJoin(postInSeriesRequests);
        })
      
      )
      .subscribe({
        next: (items: any[]) => {
          this.items = items;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadSeries() {
    this.seriesApiService
      .getAllSeries()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostCategoryDto[]) => {
          response.forEach((element) => {
            this.series.push({
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
    
  buildForm() {
    this.form = this.fb.group({
      postId: new FormControl(this.config.data.id),
      seriesId: new FormControl(this.selectedEntity.seriesId || null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || 1, Validators.required),
    });
  }

  saveChange() {
    this.toggleBlockUI(true);
    this.saveData();
  }

  private saveData() {
    this.toggleBlockUI(true);
    this.seriesApiService
      .addPostSeries(this.form.value)
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
  }

  removeSeries(id: string) {}

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.btnDisabled = true;
      this.isBlockUI = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.isBlockUI = false;
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
