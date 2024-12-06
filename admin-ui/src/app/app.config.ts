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
import { ADMIN_API_BASE_URL, AdminApiAuthApiClient, AdminApiRoleApiClient, AdminApiTestApiClient, AdminApiTokenApiClient, AdminApiUserApiClient } from './api/admin-api.service.generated';
import { environment } from '../environments/environment';
import { TokenStorageService } from './shared/services/token-storage.service';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { AuthGuard } from './shared/services/auth.guard';
import { TokenInterceptor } from './shared/interceptors/token.interceptor';
import { GlobalHttpInterceptorService } from './shared/interceptors/error-handler.interceptor';
import { BlockUIModule } from 'primeng/blockui';
import { ConfirmationService } from 'primeng/api';
import { UtilityService } from './shared/services/utility.service';
import { DialogService } from 'primeng/dynamicdialog';

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
    importProvidersFrom(SidebarModule, DropdownModule, IconModule, BlockUIModule),
    IconSetService,
    ConfirmationService,
    UtilityService,
    DialogService, 
    AdminApiAuthApiClient,
    AdminApiTestApiClient,
    AdminApiTokenApiClient,
    AdminApiRoleApiClient,
    AdminApiUserApiClient,
    TokenStorageService,
    AuthGuard,
    provideAnimations(),
    {provide: ADMIN_API_BASE_URL, useValue: environment.API_URL},
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: GlobalHttpInterceptorService,
      multi: true,
    }
  ]
};
