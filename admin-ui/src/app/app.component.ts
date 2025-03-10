import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { delay, filter, map, tap } from 'rxjs/operators';

import { ColorModeService } from '@coreui/angular';
import { iconSubset } from './icons/icon-subset';
import { ToastContainerComponent } from './shared/components/toast-container.component';
import { IconSetService  } from '@coreui/icons-angular';
import { brandSet, flagSet, freeSet, } from '@coreui/icons';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DynamicDialog } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-root',
  template: `
    <app-toast-container></app-toast-container>
    <p-confirmDialog header="Confirm" acceptLabel="Yes" rejectLabel="No" icon="pi pi-exclamation-triangle"
     rejectButtonStyleClass="p-button-outlined me-3 p-button-contrast" acceptButtonStyleClass="p-button-outlined">
    </p-confirmDialog>
    <router-outlet />
  `,
  standalone: true,
  imports: [RouterOutlet, ToastContainerComponent, ConfirmDialogModule, DynamicDialog]
})
export class AppComponent implements OnInit {
  title = 'HSOne CMS Admin UI';

  readonly #destroyRef: DestroyRef = inject(DestroyRef);
  readonly #activatedRoute: ActivatedRoute = inject(ActivatedRoute);
  readonly #router = inject(Router);
  readonly #titleService = inject(Title);

  readonly #colorModeService = inject(ColorModeService);
  readonly #iconSetService = inject(IconSetService);

  constructor(public iconSetService: IconSetService) {
    this.#titleService.setTitle(this.title);
    // iconSet singleton
    this.#iconSetService.icons = { ...iconSubset, ...brandSet, ...freeSet, ...flagSet };
    this.#colorModeService.localStorageItemName.set('coreui-free-angular-admin-template-theme-default');
    this.#colorModeService.eventName.set('ColorSchemeChange');
  }

  ngOnInit(): void {

    this.#router.events.pipe(
        takeUntilDestroyed(this.#destroyRef)
      ).subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
    });

    this.#activatedRoute.queryParams
      .pipe(
        delay(1),
        map(params => <string>params['theme']?.match(/^[A-Za-z0-9\s]+/)?.[0]),
        filter(theme => ['dark', 'light', 'auto'].includes(theme)),
        tap(theme => {
          this.#colorModeService.colorMode.set(theme);
        }),
        takeUntilDestroyed(this.#destroyRef)
      )
      .subscribe();
  }
}
