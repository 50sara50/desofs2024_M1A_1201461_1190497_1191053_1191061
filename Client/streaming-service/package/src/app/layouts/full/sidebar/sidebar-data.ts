import { NavItem } from './nav-item/nav-item';

export const navItems: NavItem[] = [
  {
    navCap: 'Home',
  },
  {
    displayName: 'Dashboard',
    iconName: 'layout-dashboard',
    route: '/app/dashboard',
  },
  {
    navCap: 'Ui Components',
  },
  {
    displayName: 'Playlists',
    iconName: 'list',
    route: '/app/ui-components/playlists',
  },
  {
    displayName: 'Plans',
    iconName: 'layout-navbar-expand',
    route: '/app/ui-components/plans',
  },
];
