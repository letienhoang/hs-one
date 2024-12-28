import { Component, OnInit } from "@angular/core";
import {
  FormBuilder,
} from "@angular/forms";
import { DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";
import { forkJoin, map, Subject, switchMap, takeUntil } from "rxjs";
import { AddPostSeriesRequest, AdminApiPostApiClient, AdminApiSeriesApiClient, PostInListDto } from "../../../api/admin-api.service.generated";
import { MessageConstants } from "../../../shared/constants/messages.constants";
import { ToastService } from "../../../shared/services/toast.service";
import { SeriesSharedModule } from "./series-shared.module";

@Component({
  templateUrl: "series-posts.component.html",
  standalone: true,
  imports: [
    SeriesSharedModule
  ],
})
export class SeriesPostsComponent implements OnInit {
  private ngUnsubscribe = new Subject<void>();

  public isBlockUI: boolean = false;
  public items: any[] = [];
  public seriesName: string = '';

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private seriesApiClient: AdminApiSeriesApiClient,
    private postApiService: AdminApiPostApiClient,
    private toastService: ToastService
  ) { }

  ngOnInit() {
    this.initData();
  }
  
  initData() {
    this.toggleBlockUI(true);
    this.loadDatas(this.config.data?.id);
    this.seriesName = this.config.data?.seriesName;
  }

  loadDatas(id: string) {
      var allPostInSeries = this.seriesApiClient.getAllPostInSeries(id);
      this.toggleBlockUI(false);
      allPostInSeries
        .pipe(
          takeUntil(this.ngUnsubscribe),
          switchMap((allPostInSeries: PostInListDto[]) => {
            const postInSeriesRequests = allPostInSeries.map(post =>
              this.postApiService.getPostsInSeries(post.id, this.config.data.id)
              .pipe(
                map(postInSeries => ({
                  seriesId: this.config.data.id,
                  postId: post.id,
                  postName: post.title,
                  displayOrder: postInSeries.displayOrder 
                }))
              )
            );
            return forkJoin(postInSeriesRequests);
          })
        
        )
        .subscribe({
          next: (items: any[]) => {
            this.items = items || [];
            this.toggleBlockUI(false);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    }

  removePost(id: string, displayOrder: number) {
    var body: AddPostSeriesRequest = new AddPostSeriesRequest({
      postId: id,
      seriesId: this.config.data.id,
      sortOrder: displayOrder
    });
    console.log('Payload:', body);
    this.toggleBlockUI(true);
    this.seriesApiClient
      .deletePostSeries(body)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.loadDatas(this.config.data?.id);
          this.toastService.showSuccess(MessageConstants.DELETED_OK_MSG);
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.isBlockUI = true;
    } else {
      setTimeout(() => {
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
