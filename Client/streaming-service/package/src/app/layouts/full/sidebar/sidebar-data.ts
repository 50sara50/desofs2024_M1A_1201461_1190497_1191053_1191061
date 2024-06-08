import { NavItem } from './nav-item/nav-item';

export const navItems: NavItem[] = [
  {
    navCap: 'Home',
  },
  {
    displayName: 'Dashboard',
    iconName: 'layout-dashboard',
    route: '/dashboard',
  },
  {
    navCap: 'Ui Components',
  },
  {
    displayName: 'Playlists',
    iconName: 'list',
    route: '/ui-components/playlists',
  },
  {
    displayName: 'Plans',
    iconName: 'layout-navbar-expand',
    route: '/ui-components/plans',
   }
];
