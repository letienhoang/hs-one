import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Toast {
  message: string;
  color: string;
  autohide: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private toastsSubject = new BehaviorSubject<Toast[]>([]);
  readonly toasts$: Observable<Toast[]> = this.toastsSubject.asObservable();

  addToast(message: string, color: string = 'primary', autohide: boolean = true) {
    const currentToasts = this.toastsSubject.getValue();
    const newToast: Toast = { message, color, autohide };
    this.toastsSubject.next([...currentToasts, newToast]);

    if (autohide) {
      setTimeout(() => {
        this.removeToast(newToast);
      }, 2000);
    }
  }

  removeToast(toast: Toast) {
    const currentToasts = this.toastsSubject.getValue();
    this.toastsSubject.next(currentToasts.filter(t => t !== toast));
  }

  show(message: string, color: string = 'primary', autohide: boolean = true) {
    this.addToast(message, color, autohide);
  }

  showSuccess(message: string) {
    this.show(message, 'success', true);
  }

  showError(message: string) {
    this.show(message, 'danger', true);
  }

  showInfo(message: string) {
    this.show(message, 'info', true);
  }

  showWarning(message: string) {
    this.show(message, 'warning', true);
  }
}
