import { Component, OnInit } from "@angular/core";
import {
  FormBuilder,
} from "@angular/forms";
import { DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";
import { Subject, takeUntil } from "rxjs";
import { AddPostSeriesRequest, AdminApiSeriesApiClient, PostInListDto } from "../../../api/admin-api.service.generated";
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
  public posts: PostInListDto[] = [];
  public seriesName: string = '';

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private seriesApiClient: AdminApiSeriesApiClient,
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
    this.toggleBlockUI(true);

    this.seriesApiClient.getAllPostInSeries(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostInListDto[]) => {
          this.posts = response;
          this.toggleBlockUI(false);
        },
        error: (error) => {
          this.toggleBlockUI(false);
        }
      }
      );
  }
  removePost(id: string) {
    var body: AddPostSeriesRequest = new AddPostSeriesRequest({
      postId: id,
      seriesId: this.config.data.id
    });
    this.seriesApiClient
      .deletePostSeries(body)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.toastService.showSuccess(MessageConstants.DELETED_OK_MSG);
          this.loadDatas(this.config.data?.id);
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
