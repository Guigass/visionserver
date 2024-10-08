import { HttpInterceptorFn } from '@angular/common/http';
import { StorageService } from '../../services/storage/storage.service';
import { inject } from '@angular/core';
import { environment } from 'src/environments/environment';

export const apiInterceptor: HttpInterceptorFn = (request, next) => {

  console.log((request.url.startsWith('api')));
  if (request.url.startsWith('api')) {
    request = request.clone({
      url: `${environment.apiUrl}${request.url}`,
    });
  }

  return next(request);
};