import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { MessageService } from './message.service';
import { LoginResponseContract } from '../model/response/LoginResponseContract';
import { catchError, shareReplay, tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import * as moment from 'moment';
import { NewUserContract } from '../model/contract/NewUserContract';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private Url = 'https://localhost:32774/Auth/';

  constructor(private httpClient: HttpClient) {}

  public login(email: string, password: string) {
    console.log('Logging in...');
    console.log(email, password);
    const requestUrl = this.Url + 'login';
    return this.httpClient
      .post<LoginResponseContract>(requestUrl, { email, password })
      .pipe(
        tap((response) => this.setSession(response)),
        shareReplay()
      );
  }

  private setSession(authResult: LoginResponseContract) {
    localStorage.setItem('userBearerToken', authResult.token);
    localStorage.setItem(
      'expiresAt',
      JSON.stringify(authResult.expirationDate.valueOf())
    );
  }

  public logout() {
    localStorage.removeItem('userBearerToken');
    localStorage.removeItem('expiresAt');
  }

  public isLoggedIn() {
    return moment().isBefore(this.getExpiration());
  }

  public isLoggedOut() {
    return !this.isLoggedIn();
  }

  public getExpiration() {
    const expiration = localStorage.getItem('expiresAt');
    if (!expiration) {
      return null;
    }
    const expiresAt = JSON.parse(expiration);
    return moment(expiresAt);
  }

  public register(newUser: NewUserContract) {
    const requestUrl = this.Url + 'register';
    return this.httpClient.post(requestUrl, newUser).pipe(shareReplay());
  }
}
