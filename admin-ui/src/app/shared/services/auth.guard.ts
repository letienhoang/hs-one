import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { TokenStorageService } from './token-storage.service';
import { UrlConstants } from '../constants/url.constants';

@Injectable()
export class AuthGuard {
  constructor(
    private router: Router,
    private tokenStorageService: TokenStorageService
  ) {}

  canActivate(activateRoute: ActivatedRouteSnapshot, routerState: RouterStateSnapshot): boolean {
    let requiredPolicy = activateRoute.data["requiredPolicy"] as string;
    var loggedInUser = this.tokenStorageService.getUser();
    if (loggedInUser) {
      var listPermissions = JSON.parse(loggedInUser.permissions);
      if (listPermissions != null && listPermissions != '' && listPermissions.filter((x: any) => x == requiredPolicy).length > 0) {
        return true;
      } else {
        this.router.navigate([UrlConstants.ACCESS_DENIED], { queryParams: { returnUrl: routerState.url } });
        return false;
      }
    }
    else {
        this.router.navigate([UrlConstants.LOGIN], { queryParams: { returnUrl: routerState.url } });
        return false;
    }
}
}
