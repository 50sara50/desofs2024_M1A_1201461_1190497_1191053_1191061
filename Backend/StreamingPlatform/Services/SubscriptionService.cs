using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dtos.Contracts;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Dtos.Responses;
using StreamingPlatform.Models;
using StreamingPlatform.Models.Enums;
using StreamingPlatform.Services.Exceptions;
using StreamingPlatform.Services.Interfaces;
using SubscriptionResponse = StreamingPlatform.Dtos.Responses.SubscriptionResponse;

namespace StreamingPlatform.Services
{
    public class SubscriptionService : ISubscriptionService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public SubscriptionService(IUnitOfWork unitOfWork, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates a new subscription based on the provided subscription data.
        /// </summary>
        /// <param name="subscriptionDto">The data for the subscription to be created.</param>
        /// <returns>The newly created subscription as a response.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to create a subscription that already exists.</exception>
        /// <exception cref="InvalidOperationException">Thrown when attempting to create a subscription with a user that doesn't exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown when attempting to create a subscription with a plan that doesn't exist.</exception>
        /// <exception cref="ValidationException">Thrown when input data fails validation.</exception>
        /// <exception cref="ServiceBaseException">Thrown for unexpected errors during subscription creation.</exception>
        public async Task<SubscriptionResponse> CreateSubscription(CreateSubscriptionContract subscriptionDto)
        {
            var validationContext = new ValidationContext(subscriptionDto, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(subscriptionDto, validationContext, validationResults,
                validateAllProperties: true);

            if (!isValid)
            {
                var errorMessages = validationResults.Select(r => r.ErrorMessage);
                throw new ArgumentException(string.Join(" ", errorMessages));
            }

            IGenericRepository<Subscription> subscriptionRepository = _unitOfWork.Repository<Subscription>();

            //IGenericRepository<AspNetUsers> userRepository = this.unitOfWork.Repository<User>();

            var user = await _userManager.FindByEmailAsync(subscriptionDto.UserEmail);
            if (user == null)
            {
                throw new InvalidOperationException("User doesn't exist");
            }

            IGenericRepository<Plan> planRepository = _unitOfWork.Repository<Plan>();

            Plan? plan = await planRepository.GetRecordAsync(p => p.PlanName == subscriptionDto.PlanName);
            if (plan == null)
            {
                throw new InvalidOperationException("Plan doesn't exist");
            }

            Subscription? existingSubscription =
                await subscriptionRepository.GetRecordAsync(s => Equals(s.UserId, user.Id));
            if (existingSubscription != null)
            {
                throw new InvalidOperationException("Subscription already exists");
            }

            Subscription subscription = new(user.Id, plan.PlanId);
            subscriptionRepository.Create(subscription);
            await _unitOfWork.SaveChangesAsync();

            SubscriptionResponse subscriptionResponse = new SubscriptionResponse(subscription.Id.ToString(),
                plan.PlanName, user.Email, subscription.CreatedOn, subscription.RenewDate, subscription.Status);
            return subscriptionResponse;
        }
        
        /// <summary>
        /// Creates a new subscription based on the provided subscription data.
        /// </summary>
        /// <param name="subscriptionDto">The data for the subscription to be created.</param>
        /// <returns>The newly created subscription as a response.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to create a subscription that already exists.</exception>
        /// <exception cref="InvalidOperationException">Thrown when attempting to create a subscription with a user that doesn't exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown when attempting to create a subscription with a plan that doesn't exist.</exception>
        /// <exception cref="ValidationException">Thrown when input data fails validation.</exception>
        /// <exception cref="ServiceBaseException">Thrown for unexpected errors during subscription creation.</exception>
        public async Task<SubscriptionResponse> CreateSubscriptionById(CreateSubscriptionContractById subscriptionDto)
        {
            var validationContext = new ValidationContext(subscriptionDto, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(subscriptionDto, validationContext, validationResults,
                validateAllProperties: true);

            if (!isValid)
            {
                var errorMessages = validationResults.Select(r => r.ErrorMessage);
                throw new ArgumentException(string.Join(" ", errorMessages));
            }

            IGenericRepository<Subscription> subscriptionRepository = _unitOfWork.Repository<Subscription>();

            //IGenericRepository<AspNetUsers> userRepository = this.unitOfWork.Repository<User>();

            var user = await _userManager.FindByIdAsync(subscriptionDto.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User doesn't exist");
            }

            IGenericRepository<Plan> planRepository = _unitOfWork.Repository<Plan>();

            Plan? plan = await planRepository.GetRecordAsync(p => p.PlanName == subscriptionDto.PlanName);
            if (plan == null)
            {
                throw new InvalidOperationException("Plan doesn't exist");
            }

            Subscription? existingSubscription =
                await subscriptionRepository.GetRecordAsync(s => Equals(s.UserId, user.Id));
            if (existingSubscription != null)
            {
                throw new InvalidOperationException("Subscription already exists");
            }

            Subscription subscription = new(user.Id, plan.PlanId);
            subscriptionRepository.Create(subscription);
            await _unitOfWork.SaveChangesAsync();

            SubscriptionResponse subscriptionResponse = new SubscriptionResponse(subscription.Id.ToString(),
                plan.PlanName, user.Email, subscription.CreatedOn, subscription.RenewDate, subscription.Status);
            return subscriptionResponse;
        }

        /// <summary>
        /// Gets user's playlists.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SubscriptionResponse>> GetSubscriptions(string userId)
        {
            //verify if user exists
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User doesn't exist");
            }

            IGenericRepository<Subscription> repository = _unitOfWork.Repository<Subscription>();
            var subscriptions = await repository.GetRecordsAsync(s => s.UserId.ToString().Equals(user.Id))
                ?? throw new Exception("That user does not have any subscriptions.");
            
            //TODO: create mapper
            var results = new List<SubscriptionResponse>();
            foreach (var subscription in subscriptions)
            {
                IGenericRepository<Plan> planRepository = _unitOfWork.Repository<Plan>();
                Plan? plan = await planRepository.GetRecordAsync(p => p.PlanId == subscription.PlanId);
                if (plan == null)
                {
                    throw new InvalidOperationException("Plan doesn't exist");
                }
                results.Add(new SubscriptionResponse(subscription.Id.ToString(), plan.PlanName, user.Email, subscription.CreatedOn, subscription.RenewDate, subscription.Status));
            }

            return results;
        }
    }
}
