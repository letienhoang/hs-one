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
import { ADMIN_API_BASE_URL, AdminApiAuthApiClient, AdminApiMediaApiClient, AdminApiPostApiClient, AdminApiPostCategoryApiClient, AdminApiRoleApiClient, AdminApiSeriesApiClient, AdminApiTestApiClient, AdminApiTokenApiClient, AdminApiUserApiClient } from './api/admin-api.service.generated';
import { environment } from '../environments/environment';
import { TokenStorageService } from './shared/services/token-storage.service';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { AuthGuard } from './shared/services/auth.guard';
import { TokenInterceptor } from './shared/interceptors/token.interceptor';
import { GlobalHttpInterceptorService } from './shared/interceptors/error-handler.interceptor';
import { ConfirmationService } from 'primeng/api';
import { UtilityService } from './shared/services/utility.service';
import { DialogService } from 'primeng/dynamicdialog';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { UploadService } from './shared/services/upload.service';
import { UserSharedModule } from './views/system/users/user-shared.module';

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
    provideAnimationsAsync(),
        providePrimeNG({ 
            theme: {
                preset: Aura
            }
        }),
    importProvidersFrom(SidebarModule, DropdownModule, IconModule),
    IconSetService,
    ConfirmationService,
    UtilityService,
    DialogService, 
    AdminApiAuthApiClient,
    AdminApiTestApiClient,
    AdminApiTokenApiClient,
    AdminApiRoleApiClient,
    AdminApiUserApiClient,
    AdminApiPostCategoryApiClient,
    AdminApiSeriesApiClient,
    AdminApiPostApiClient,
    AdminApiMediaApiClient,
    TokenStorageService,
    AuthGuard,
    UploadService,
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
