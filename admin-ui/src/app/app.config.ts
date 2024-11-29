import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import {
  provideRouter,
  withEnabledBlockingInitialNavigation,
  withHashLocation,
  withInMemoryScrolling,
  withRouterConfig,
  withViewTransitions
} from '@angular/router';

import { DropdownModule, SidebarModule } from '@coreui/angular';
import { IconSetService, IconModule } from '@coreui/icons-angular';
import { routes } from './app.routes';
import { ADMIN_API_BASE_URL, AdminApiAuthApiClient } from './api/admin-api.service.generated';
import { environment } from '../environments/environment';
import { TokenStorageService } from './shared/services/token-storage.service';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes,
      withRouterConfig({
        onSameUrlNavigation: 'reload'
      }),
      withInMemoryScrolling({
        scrollPositionRestoration: 'top',
        anchorScrolling: 'enabled'
      }),
      withEnabledBlockingInitialNavigation(),
      withViewTransitions(),
      withHashLocation()
    ),
    importProvidersFrom(SidebarModule, DropdownModule, IconModule),
    IconSetService,
    AdminApiAuthApiClient,
    TokenStorageService,
    provideAnimations(),
    {provide: ADMIN_API_BASE_URL, useValue: environment.API_URL},
    provideHttpClient(withInterceptorsFromDi())
  ]
};
