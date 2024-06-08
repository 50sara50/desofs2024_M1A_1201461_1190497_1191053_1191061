// services/subscription.service.ts
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {map, Observable} from 'rxjs';
import {Subscription} from '../pages/ui-components/domain/Subscription';
import {MessageService} from './message.service';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {

  private apiUrl = 'https://localhost:7255/Subscription'; // Altere para a URL correta da sua API

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


  createValidPlan(subscriptionData: Subscription): Observable<any> | null {
    if (!this.validateData(subscriptionData)) return null;
    return this.createSubscription(subscriptionData);
  }

  public extractData(res: any) {
    return res || {};
  }

  validateData(subscriptionData: Subscription): boolean {
    if (subscriptionData.planName == null) {
      this.log("ERROR: Plan Name can't be null.");
      return false;
    }
    if (subscriptionData.userEmail == null) {
      this.log("ERROR: User Email can't be null.");
      return false;
    }
    return true;
  }

  log(message: string) {
    this.messageService.add(`Created: ${message}`);
  }
}
