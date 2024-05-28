using System.ComponentModel.DataAnnotations;

namespace StreamingPlatform.Dtos.Contracts
{
    public class CreateSubscriptionContract
    {
        /// <summary>
        /// The name of the streaming plan to be subscribed.
        /// </summary>
        [Required(ErrorMessage = "Plan name is required.")]
        [MaxLength(50, ErrorMessage = "Plan name is too long. Max length is 50 characters.")]
        required public string PlanName { get; set; }

        /// <summary>
        /// The unique identifier of the owner.
        /// </summary>
        [Required (ErrorMessage = "You must provide the user email of the owner of the subscription.")]
        public string UserEmail { get; set; }

    }
}