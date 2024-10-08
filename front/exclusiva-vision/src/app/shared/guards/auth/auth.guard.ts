import { NavController } from '@ionic/angular/standalone';
import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../../services/api/auth/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const navCtrl = inject(NavController);

  const isAuth = authService.checkAuthenticated();

  if (!isAuth) {
    navCtrl.navigateRoot(['/login'], { queryParams: { redirectUrl: state.url } });
  }

  return isAuth;
};
