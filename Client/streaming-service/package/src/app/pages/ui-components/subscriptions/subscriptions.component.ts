import { Component, OnInit } from '@angular/core';
import { Subscription } from '../domain/Subscription';
import { SubscriptionService } from '../../../services/subscription.service';

@Component({
  selector: 'app-subscriptions',
  templateUrl: './subscriptions.component.html',
})
export class SubscriptionsComponent{

  constructor(private subscriptionService: SubscriptionService) { }

}
