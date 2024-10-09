import { Routes } from '@angular/router';
import { authGuard } from './shared/guards/auth/auth.guard';
import { claimGuard } from './shared/guards/claim/claim.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.page').then( m => m.LoginPage)
  },
  {
    path: '',
    loadComponent: () => import('./layouts/app-layout/app-layout.component').then( m => m.AppLayoutComponent),
    canActivate: [authGuard],
    children: [
      {
        path: 'home',
        loadComponent: () => import('./pages/app/home/home.page').then( m => m.HomePage)
      }
    ]
  },
  {
    path: 'admin',
    loadComponent: () => import('./layouts/admin-layout/admin-layout.component').then( m => m.AdminLayoutComponent),
    canActivate: [authGuard, claimGuard],
    data: {claimType: 'role', claimValue: 'Admin'},
    children: [
      {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full',
      },
      {
        path: 'home',
        loadComponent: () => import('./pages/admin/home/home.page').then( m => m.HomePage)
      },
      {
        path: 'cameras',
        loadComponent: () => import('./pages/admin/cameras/cameras.page').then( m => m.CamerasPage)
      },
      {
        path: 'groups',
        loadComponent: () => import('./pages/admin/groups/groups.page').then( m => m.GroupsPage)
      },
      {
        path: 'users',
        loadComponent: () => import('./pages/admin/users/users.page').then( m => m.UsersPage)
      },
      {
        path: 'settings',
        loadComponent: () => import('./pages/admin/settings/settings.page').then( m => m.SettingsPage)
      }
    ]
  },
  {
    path: '**',
    loadComponent: () => import('./pages/not-found/not-found.page').then( m => m.NotFoundPage)
  }
];
