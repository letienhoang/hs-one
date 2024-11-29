import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { NgScrollbar } from 'ngx-scrollbar';

import {
  ContainerComponent,
  ShadowOnScrollDirective,
  SidebarBrandComponent,
  SidebarComponent,
  SidebarFooterComponent,
  SidebarHeaderComponent,
  SidebarNavComponent,
  SidebarToggleDirective,
  SidebarTogglerDirective,
} from '@coreui/angular';

import { DefaultFooterComponent, DefaultHeaderComponent } from './';
import { navItems } from './_nav';
import { TokenStorageService } from 'src/app/shared/services/token-storage.service';
import { UrlConstants } from 'src/app/shared/constants/url.constants';

function isOverflown(element: HTMLElement) {
  return (
    element.scrollHeight > element.clientHeight ||
    element.scrollWidth > element.clientWidth
  );
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html',
  styleUrls: ['./default-layout.component.scss'],
  standalone: true,
  imports: [
    SidebarComponent,
    SidebarHeaderComponent,
    SidebarBrandComponent,
    RouterLink,
    NgScrollbar,
    SidebarNavComponent,
    SidebarFooterComponent,
    SidebarToggleDirective,
    SidebarTogglerDirective,
    DefaultHeaderComponent,
    ShadowOnScrollDirective,
    ContainerComponent,
    RouterOutlet,
    DefaultFooterComponent,
  ],
})
export class DefaultLayoutComponent implements OnInit {
  public navItems = navItems;

  constructor(
    private tokenStorageService: TokenStorageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    var user = this.tokenStorageService.getUser();
    if (user == null) {
      this.router.navigate([UrlConstants.LOGIN]);
    }
    var permissions = JSON.parse(user.permissions);
    for (var i = 0; i < this.navItems.length; i++) {
      for (var chili = 0; chili < this.navItems[i].children?.length; chili++) {
        if (
          this.navItems[i].children[chili].attributes &&
          permissions.filter(
            (x) =>
              x == this.navItems[i].children[chili].attributes['policyName']
          ).length == 0
        ) {
          this.navItems[i].children[chili].attributes['hidden'] = true;
        }
      }
    }
  }

  onScrollbarUpdate($event: any) {
    // if ($event.verticalUsed) {
    // console.log('verticalUsed', $event.verticalUsed);
    // }
  }
}
