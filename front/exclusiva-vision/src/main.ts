import { bootstrapApplication } from '@angular/platform-browser';
import { RouteReuseStrategy, provideRouter, withPreloading, PreloadAllModules } from '@angular/router';
import { IonicRouteStrategy, provideIonicAngular } from '@ionic/angular/standalone';
import { JwtModule } from '@auth0/angular-jwt';

import { routes } from './app/app.routes';
import { AppComponent } from './app/app.component';
import { DEFAULT_CURRENCY_CODE, importProvidersFrom, LOCALE_ID } from '@angular/core';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './app/shared/interceptors/auth/auth.interceptor';
import { apiInterceptor } from './app/shared/interceptors/api/api.interceptor';

import { VgCoreModule } from '@videogular/ngx-videogular/core';

import { registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';

registerLocaleData(localePt, 'pt-BR');

bootstrapApplication(AppComponent, {
  providers: [
    { provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
    provideIonicAngular(),
    provideRouter(routes, withPreloading(PreloadAllModules)),
    importProvidersFrom(
      VgCoreModule,
      JwtModule.forRoot({
        config: {
          throwNoTokenError: false,
          tokenGetter: () => {
            if (typeof Storage !== 'undefined') {
              // Primeiro, tenta obter o token do sessionStorage
              const tokenFromSession = sessionStorage.getItem('at');
              if (tokenFromSession) {
                return tokenFromSession;
              }

              // Se não estiver no sessionStorage, tenta obter do localStorage
              const tokenFromLocal = localStorage.getItem('at');
              return tokenFromLocal ? tokenFromLocal : '';
            }

            // Retorna uma string vazia se o Storage não estiver disponível
            return '';
          },
        }
      }),
    ),
    provideHttpClient(
      withFetch(),
      withInterceptors([
        apiInterceptor,
        authInterceptor
      ])
    ),
    { provide: LOCALE_ID, useValue: 'pt-BR' },
    { provide: DEFAULT_CURRENCY_CODE, useValue: 'BRL' },
  ],
});
