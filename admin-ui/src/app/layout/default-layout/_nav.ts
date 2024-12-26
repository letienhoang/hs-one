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
    attributes: {
      "policyName": "Permissions.Dashboard.View"
    }
  },
  {
    name: 'Content',
    title: true,
    attributes: {
      "policyName": "Permissions.Posts.View"
    }
  },
  {
    name: 'Posts',
    url: '/content/posts',
    iconComponent: { name: 'cil-spreadsheet' },
    children: [
      {
        name: 'All Posts',
        url: '/content/posts',
        icon: 'nav-icon-bullet',
        attributes: {
          "policyName": "Permissions.Posts.View"
        }
      },
      {
        name: 'Categories',
        url: '/content/post-categories',
        icon: 'nav-icon-bullet',
        attributes: {
          "policyName": "Permissions.PostCategories.View"
        }
      },
      {
        name: 'Series',
        url: '/content/series',
        icon: 'nav-icon-bullet',
        attributes: {
          "policyName": "Permissions.Series.View"
        }
      },
    ]
  },
  {
    name: 'Royalty',
    url: '/royalty',
    iconComponent: { name: 'cil-star' },
    children: [
      {
        name: 'Month Report',
        url: '/royalty/royalty-month',
        icon: 'nav-icon-bullet',
        attributes: {
          "policyName": "Permissions.Royalty.View"
        }
      },
      {
        name: 'User Report',
        url: '/royalty/royalty-user',
        icon: 'nav-icon-bullet',
        attributes: {
          "policyName": "Permissions.Royalty.View"
        }
      },
      {
        name: 'Transaction',
        url: '/royalty/transactions',
        icon: 'nav-icon-bullet',
        attributes: {
          "policyName": "Permissions.Royalty.View"
        }
      },
    ]
  },
  {
    title: true,
    name: 'System',
    attributes: {
      "policyName": "Permissions.Users.View"
    }
  },
  {
    name: 'Identity',
    url: '/system',
    iconComponent: { name: 'cil-star' },
    children: [
      {
        name: 'Users',
        url: '/system/users',
        icon: 'nav-icon-bullet',
        attributes: {
          "policyName": "Permissions.Users.View"
        }
      },
      {
        name: 'Roles',
        url: '/system/roles',
        icon: 'nav-icon-bullet',
        attributes: {
          "policyName": "Permissions.Roles.View"
        }
      },
    ]
  },
];
