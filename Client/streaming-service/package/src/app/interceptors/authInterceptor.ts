import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const requestWithAuth = req.clone({
      withCredentials: true,
    });
    /*const token = sessionStorage.getItem('userBearerToken');
    if (token) {
      
      return next.handle(requestWithAuth);
    }*/
    return next.handle(requestWithAuth);
  }
}
