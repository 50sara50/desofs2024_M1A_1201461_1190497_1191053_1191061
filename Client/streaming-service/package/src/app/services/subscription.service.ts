// services/subscription.service.ts
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {map, Observable} from 'rxjs';
import {Subscription} from '../pages/ui-components/domain/Subscription';
import {MessageService} from './message.service';
import {switchMap} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {

  private apiUrl = 'https://localhost:7255/Subscription'; // Altere para a URL correta da sua API
  private userEmail = 'bea@gmail.com';
  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) {
  }

  createSubscription(subscriptionData: Subscription): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.httpClient.post(this.apiUrl, subscriptionData, {headers}).pipe(map(this.extractData));
  }

  public extractData(res: any) {
    return res || {};
  }

  getSubscriptions(params: string): Observable<any> {
    return this.httpClient
      .get(this.apiUrl + '/GetUserSubscriptions' + `?userEmail=${this.userEmail}`)
      .pipe(map(this.extractData));

  }

  log(message: string) {
    this.messageService.add(`Created: ${message}`);
  }
}
