using System.Collections.Generic;
using System.Threading.Tasks;
using StreamingPlatform.Dtos.Contracts;
using StreamingPlatform.Dtos.Responses;

namespace StreamingPlatform.Services.Interfaces
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Creates a new subscription based on the provided details.
        /// </summary>
        /// <param name="subscriptionDto">DTO containing information about the subscription to be created.</param>
        /// <returns>A task that resolves to a DTO containing details of the created subscription.</returns>
        public Task<SubscriptionResponse> CreateSubscription(CreateSubscriptionContract subscriptionDto);
        
        public Task<IEnumerable<SubscriptionResponse>> GetSubscriptions(string userEmail);

    }
}