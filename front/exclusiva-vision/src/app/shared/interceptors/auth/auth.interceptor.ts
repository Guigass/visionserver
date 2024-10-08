import { HttpInterceptorFn } from '@angular/common/http';
import { StorageService } from '../../services/storage/storage.service';
import { inject } from '@angular/core';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const storage = inject(StorageService);

  const at = storage.get('at');
  const auth = `Bearer ${at}`;

  if (request.url.indexOf('api') > 0 || request.url.indexOf('localhost') > 0) {
    request = request.clone({
      setHeaders: {
        'Content-Type': 'application/json',
        Authorization: auth
      }
    });
  }

  return next(request);
};
