import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { LocalStorageConstants } from '../constants';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (sessionStorage.getItem(LocalStorageConstants.authToken) != null) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${sessionStorage.getItem(LocalStorageConstants.authToken)}`
        }
      });

      return next.handle(req);
    }

    return next.handle(req);
  }

}
