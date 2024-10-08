import { NavController } from '@ionic/angular/standalone';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../../services/api/auth/auth.service';
import { inject } from '@angular/core';

export const claimGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const navCtrl = inject(NavController);
  
  if (route.data && route.data['claimType'] != null && route.data['claimValue'] != null) {
    if (authService.hasClaim(route.data['claimType'], route.data['claimValue'])) {
      return true;
    }
  }

  navCtrl.navigateRoot('/');

  return false;
};
