import { INavData } from '@coreui/angular';

export const navItems: INavData[] = [
  {
    name: 'Dashboard',
    url: '/dashboard',
    iconComponent: { name: 'cil-speedometer' },
    // badge: {
    //   color: 'info',
    //   text: 'NEW'
    // }
  },
  {
    name: 'Content',
    title: true
  },
  {
    name: 'Posts',
    url: '/content/posts',
    iconComponent: { name: 'cil-spreadsheet' },
    children: [
      {
        name: 'All Posts',
        url: '/content/posts',
        icon: 'nav-icon-bullet'
      },
      {
        name: 'Categories',
        url: '/content/post-categories',
        icon: 'nav-icon-bullet'
      },
      {
        name: 'Series',
        url: '/content/series',
        icon: 'nav-icon-bullet'
      },
      {
        name: 'Royalty',
        url: '/content/royalty',
        icon: 'nav-icon-bullet'
      }
    ]
  },
  {
    title: true,
    name: 'System'
  },
  {
    name: 'Users',
    url: '/system',
    iconComponent: { name: 'cil-star' },
    children: [
      {
        name: 'All Users',
        url: '/system/users',
        icon: 'nav-icon-bullet'
      },
      {
        name: 'Roles',
        url: '/system/roles',
        icon: 'nav-icon-bullet'
      },
    ]
  }
];
