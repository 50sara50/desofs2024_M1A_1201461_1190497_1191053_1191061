// services/subscription.service.ts
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {map, Observable, of} from 'rxjs';
import {SubscriptionResponse} from '../pages/ui-components/domain/SubscriptionResponse';
import {SubscriptionContractById} from '../pages/ui-components/domain/SubscriptionContractById';
import {MessageService} from './message.service';
import {switchMap} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {

  private apiUrl = 'https://localhost:7255/Subscription';
  private apiUrlById = 'https://localhost:7255/Subscription/ById';
  private userEmail = 'bea@gmail.com';
  public AuthUrl = 'https://localhost:7255/Auth/';
  userIdData: string;
  userId: string;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) {
  }


  public extractData(res: any) {
    return res || {};
  }

  createValidSubscription(plan: string): Observable<any> | null {
    return this.getUserId().pipe(
      switchMap(userId => {
        this.userId = userId.message;
        const body = {
          planName: plan,
          userId: this.userId
        };
        return this.httpClient.post(this.apiUrlById, body).pipe(
          map(this.extractData)
        );
      })
    );
  }

    getSubscriptions(): Observable<any> {
    const url = `${this.apiUrl}/GetUserSubscriptions`;

    return this.getUserId().pipe(
      switchMap(userIdData => {
        this.userIdData = userIdData.message; // Store userIdData if needed
        return this.httpClient.get(`${url}?userId=${this.userIdData}`);
      }),
      map(this.extractData)
    );
  }

  getUserId(): Observable<any> {
    return this.httpClient.get(`${this.AuthUrl}user-id`).pipe(this.extractData);
  }

  validateData(subscriptionData: SubscriptionContractById): boolean {
    if (subscriptionData.planName == null) {
      this.log("ERROR: Plan Name can't be null.");
      return false;
    }
    if (subscriptionData.userId == null) {
      this.log("ERROR: User Id can't be null.");
      return false;
    }
    return true;
  }



  log(message: string) {
    this.messageService.add(`Created: ${message}`);
  }
}
