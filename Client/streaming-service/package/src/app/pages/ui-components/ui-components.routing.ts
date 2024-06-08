import { Routes } from '@angular/router';

// ui
import { AppBadgeComponent } from './badge/badge.component';
import { AppChipsComponent } from './personal-data/personal-data.component';
import { AppListsComponent } from './playlists/playlists.component';
import {PlanComponent} from './plans/plans.component';
import { AppTooltipsComponent } from './tooltips/tooltips.component';
import {SubscriptionsComponent} from "./subscriptions/subscriptions.component";

export const UiComponentsRoutes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'badge',
        component: AppBadgeComponent,
      },
      {
        path: 'chips',
        component: AppChipsComponent,
      },
      {
        path: 'playlists',
        component: AppListsComponent,
      },
      {
        path: 'plans',
        component: PlanComponent,
      },
      {
        path: 'subscriptions',
        component: SubscriptionsComponent,
      },
      {
        path: 'tooltips',
        component: AppTooltipsComponent,
      },
    ],
  },
];
