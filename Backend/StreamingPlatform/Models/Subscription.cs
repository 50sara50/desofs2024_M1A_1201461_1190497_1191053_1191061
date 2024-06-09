using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StreamingPlatform.Models.Enums;

namespace StreamingPlatform.Models
{
    [Table("Subscriptions")]
    public sealed class Subscription
    {
        /// <summary>
        /// The subscription's unique identifier.
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// The unique identifier of the owner of the subscription.
        /// </summary>
        [Required(ErrorMessage = "User Id is required.")]
        public Guid UserId { get; set; }
        
        /// <summary>
        /// The unique identifier of the plan.
        /// </summary>
        [Required(ErrorMessage = "Plan Id is required.")]
        public Guid PlanId { get; set; }
        
        /// <summary>
        /// The subscription's creation date.
        /// </summary>
        public DateTime CreatedOn { get; set; }
        
        /// <summary>
        /// The subscription's renew date (when applicable).
        /// </summary>
        public DateTime RenewDate { get; set; }
        
        /// <summary>
        /// The subscription's status.
        /// </summary>
        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(SubscriptionStatus), ErrorMessage = "Invalid status.")]
        public SubscriptionStatus Status { get; set; }

        public Subscription()
        {
            this.PlanId = Guid.NewGuid();
            this.CreatedOn = DateTime.Now;
            this.RenewDate = DateTime.Now.AddYears(1);
        }

        public Subscription(Guid userId, Guid planId)
        {
            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.PlanId = planId;
            this.CreatedOn = DateTime.Now;
            this.RenewDate = DateTime.Now.AddYears(1);
            this.Status = SubscriptionStatus.Active;
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