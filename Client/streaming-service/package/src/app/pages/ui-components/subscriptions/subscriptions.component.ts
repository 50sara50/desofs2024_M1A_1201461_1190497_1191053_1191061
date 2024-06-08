import { Component, OnInit } from '@angular/core';
import { Subscription } from '../domain/Subscription';
import { SubscriptionService } from '../../../services/subscription.service';

@Component({
  selector: 'app-subscriptions',
  templateUrl: './subscriptions.component.html',
})
export class SubscriptionsComponent{

  constructor(private subscriptionService: SubscriptionService) { }

  createSubscription() {
    const subscriptionData: Subscription = {
      planName: 'Basic Plan',
      userEmail: 'user@example.com'
    };

    this.subscriptionService.createSubscription(subscriptionData).subscribe({
      next: response => {
        console.log('Subscription created successfully', response);
      },
      error: error => {
        console.error('Error creating subscription', error);
      }
    });
  }
}
