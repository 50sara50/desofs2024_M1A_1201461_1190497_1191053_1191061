using StreamingPlatform.Models.Enums;

namespace StreamingPlatform.Dtos.Responses
{
    public class SubscriptionResponse(string subscriptionId, string planName, string userEmail, DateTime createdOn, DateTime renewDate,  SubscriptionStatus status)
    {
        
        /// <summary>
        /// The id of the subscription.
        /// </summary>
        public string SubscriptionId { get; set; } = subscriptionId;
        
        /// <summary>
        /// The name of the streaming plan subscribed.
        /// </summary>
        public string PlanName { get; set; } = planName;
        
        /// <summary>
        /// The user id of the subscription.
        /// </summary>
        public string UserEmail { get; set; } = userEmail;


        /// <summary>
        /// The creation date of the subscription
        /// </summary>
        public DateTime CreatedOn { get; set; } = createdOn;

        /// <summary>
        /// The renew date of the subscription.
        /// </summary>
        public DateTime RenewDate { get; set; } = renewDate;

        /// <summary>
        /// The status of the subscription (Active, Inactive).
        /// </summary>
        public SubscriptionStatus Status { get; set; } = status;
    }
}