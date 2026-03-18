import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent) },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard-layout/dashboard-layout.component').then(m => m.DashboardLayoutComponent),
    canActivate: [authGuard],
    children: [
      { path: '', loadComponent: () => import('./features/dashboard/welcome/welcome.component').then(m => m.WelcomeComponent) },
      {
        path: 'users',
        canActivate: [roleGuard(['super_admin'])],
        loadComponent: () => import('./features/users/user-list/user-list.component').then(m => m.UserListComponent),
      },
      {
        path: 'artists',
        canActivate: [roleGuard(['super_admin', 'artist_manager'])],
        loadComponent: () => import('./features/artists/artist-list/artist-list.component').then(m => m.ArtistListComponent),
      },
      {
        path: 'artists/:artistId/songs',
        canActivate: [roleGuard(['super_admin', 'artist_manager', 'artist'])],
        loadComponent: () => import('./features/songs/song-list/song-list.component').then(m => m.SongListComponent),
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
