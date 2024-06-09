export class SubscriptionContractById {
  planName: string;
  userId: string;

  constructor(planName: string, userId: string) {
    this.planName = planName;
    this.userId = userId;
  }
}
