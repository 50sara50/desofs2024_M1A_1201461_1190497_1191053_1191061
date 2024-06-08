import { Component, OnInit } from '@angular/core';
import { Plan } from '../domain/Plan';
import { PlanService } from '../../../services/plan.service';
import {SubscriptionService} from "../../../services/subscription.service";
import {Subscription} from "../domain/Subscription";
import {AuthService} from "../../../services/auth.service";
@Component({
  selector: 'app-menu',
  templateUrl: './plans.component.html',
})
export class PlanComponent implements OnInit {
  plan: Plan;
  planName: string;
  monthlyFee: number;
  numberOfMinutes: number;
  pipe = false;

  searchPlanName: string;
  searchMonthlyFee: number;
  searchNumberOfMinutes: number;

  plans: Plan[];

  constructor(private planService: PlanService, private subscriptionService: SubscriptionService, private authService: AuthService) {}

  ngOnInit(): void {
    this.getPlans(); // Call getPlans to populate plans array
  }

  public createPlan(): void {
    // @ts-ignore
    this.planService
      .createValidPlan(this.planName, this.monthlyFee, this.numberOfMinutes)
      .subscribe();
    setTimeout(window.location.reload.bind(window.location), 200);
    this.getPlans();
  }

  public getPlans(): void {
    this.planService.getPlan().subscribe((data) => {
      this.plans = data;
    });
    console.log(this.planService.getPlan());
  }

  showButton = false;

  createSubscription(planName: string) {
    const userEmail = this.authService.getUserEmail();

    if (!userEmail) {
      console.error('User is not logged in.');
      return;
    }

    const subscriptionData: Subscription = {
      planName: planName,
      userEmail: userEmail
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
