import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component } from '@angular/core';
import { MatChipEditedEvent, MatChipInputEvent } from '@angular/material/chips';
import {ThemePalette} from '@angular/material/core';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { SubscriptionService } from '../../../services/subscription.service';
import {PlanService} from "../../../services/plan.service";
import {AuthService} from "../../../services/auth.service";
import {SubscriptionResponse} from "../domain/SubscriptionResponse";

@Component({
  selector: 'app-chips',
  templateUrl: './personal-data.component.html',
  styleUrls: ['./personal-data.component.scss'],
})
export class AppChipsComponent {

  subscriptions: SubscriptionResponse[];
  userEmail: 'user@example.com';
  constructor(private subscriptionService: SubscriptionService, private authService: AuthService) {}

  ngOnInit(): void {
    this.getSubscriptions(); // Call getPlans to populate plans array
  }


  public getSubscriptions(): void {
    this.subscriptionService.getSubscriptions().subscribe((data) => {
      this.subscriptions = data;
    });
  }

}
function isDragDrop(object: any): object is CdkDragDrop<string[]> {
  return 'previousIndex' in object;
}
