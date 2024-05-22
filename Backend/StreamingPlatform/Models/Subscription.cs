using System.ComponentModel.DataAnnotations.Schema;
using StreamingPlatform.Models.Enums;

namespace StreamingPlatform.Models
{
    [Table("Subscriptions")]
    public class Subscription
    {
        /// <summary>
        /// The subscription's unique identifier.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The unique identifier of the owner of the subscription.
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// The unique identifier of the plan.
        /// </summary>
        public Guid PlanId { get; set; }
        
        /// <summary>
        /// The subscription's creation date.
        /// </summary>
        public DateTime CreatedOn { get; set; }
        
        /// <summary>
        /// The subscription's renew date (when applicable).
        /// </summary>
        public DateTime? RenewDate { get; set; }
        
        /// <summary>
        /// The subscription's status.
        /// </summary>
        public SubscriptionStatus Status { get; set; }

        public Subscription()
        {
            this.UserId = Guid.NewGuid();
            this.PlanId = Guid.NewGuid();
            this.CreatedOn = DateTime.Now;
        }

        public Subscription(Guid id, Guid userId, Guid planId, SubscriptionStatus status)
        {
            this.Id = id;
            this.UserId = userId;
            this.PlanId = planId;
            this.Status = status;
        }

        /// <summary>
        /// Renew subscription
        /// </summary>
        public void RenewSubscription()
        {
            this.RenewDate = DateTime.Now;
        }
    }
}