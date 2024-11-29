import { Component } from '@angular/core';
import { NgFor, AsyncPipe } from '@angular/common';
import { ButtonModule, ToastModule } from '@coreui/angular';
import { ToastService, Toast } from '../services/toast.service';

@Component({
  selector: 'app-toast-container',
  template: `
    <div class="toast-container position-fixed top-20 end-0 p-3">
      <c-toast
        *ngFor="let toast of toasts$ | async"
        [autohide]="toast.autohide"
        [visible]="true"
        [color]="toast.color"
        class="align-items-center text-white border-0"
      >
        <div class="d-flex">
          <c-toast-body>
            <span>{{ toast.message }}</span>
          </c-toast-body>
          <button aria-label="close" cButtonClose class="me-2 m-auto" (click)="removeToast(toast)"></button>

        </div>
      </c-toast>
    </div>
  `,
  standalone: true,
  imports: [NgFor, ToastModule, AsyncPipe, ButtonModule],
})
export class ToastContainerComponent {
  toasts$ = this.toastService.toasts$;

  constructor(private toastService: ToastService) {}

  removeToast(toast: Toast) {
    this.toastService.removeToast(toast);
  }
}
