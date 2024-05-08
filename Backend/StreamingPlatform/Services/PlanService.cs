using System.ComponentModel.DataAnnotations;
using StreamingPlatform.Dao.Helper;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Models;
using StreamingPlatform.Models.Enums;
using StreamingPlatform.Services.Exceptions;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform.Services
{
    public class PlanService(IUnitOfWork unitOfWork) : IPlanService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        /// <summary>
        /// Creates a new plan based on the provided plan data.
        /// </summary>
        /// <param name="planDto">The data for the plan to be created.</param>
        /// <returns>The newly created plan as a response.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to create a plan that already exists.</exception>
        /// <exception cref="ValidationException">Thrown when input data fails validation.</exception>
        /// <exception cref="ServiceBaseException">Thrown for unexpected errors during plan creation.</exception>
        public async Task<PlanResponse> CreatePlan(CreatePlanContract planDto)
        {
            try
            {
                var validationContext = new ValidationContext(planDto, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(planDto, validationContext, validationResults, validateAllProperties: true);

                if (!isValid)
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    throw new ArgumentException(string.Join(" ", errorMessages));
                }

                IGenericRepository<Plan> planRepository = this.unitOfWork.Repository<Plan>();

                Plan? existingPlan = await planRepository.GetRecordAsync(p => p.PlanName == planDto.PlanName);
                if (existingPlan != null)
                {
                    throw new InvalidOperationException("Plan already exists");
                }

                Plan plan = new(planDto.PlanName, planDto.MonthlyFee, planDto.NumberOfMinutes);
                planRepository.Create(plan);
                await this.unitOfWork.SaveChangesAsync();

                PlanResponse planResponse = new(plan.PlanName, plan.MontlyFee, plan.NumberOfMinutes, plan.Status);
                return planResponse;
            }
            catch (ArgumentException e)
            {
                throw new ValidationException($"Validation error: {e.Message}");
            }
            catch (Exception e)
            {
                throw new ServiceBaseException($"Unexpected error while creating plan; {e.Message}");
            }
        }

        /// <summary>
        /// Retrieves details of a plan based on the provided plan name.
        /// </summary>
        /// <param name="planName">The name of the plan to retrieve.</param>
        /// <param name="isAdmin">Indicates whether the caller is an administrator (default is false).</param>
        /// <returns>
        ///     A <see cref="PlanResponse"/> object representing the plan details, or null if no matching plan is found.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if the provided plan name is null, empty, or contains only whitespace characters.
        /// </exception>
        public async Task<PlanResponse?> GetPlan(string planName, bool isAdmin = false)
        {
            if (string.IsNullOrWhiteSpace(planName))
            {
                throw new ArgumentException("Plan name cannot be null or empty");
            }

            IGenericRepository<Plan> planRepository = this.unitOfWork.Repository<Plan>();
            Plan? plan;
            if (isAdmin)
            {
                plan = await planRepository.GetRecordAsync(p => p.PlanName == planName);
            }
            else
            {
                plan = await planRepository.GetRecordAsync(p => p.PlanName == planName && p.Status == PlanStatus.Active);
            }

            return plan == null ? null : new PlanResponse(plan.PlanName, plan.MontlyFee, plan.NumberOfMinutes, plan.Status);
        }

        /// <summary>
        /// Retrieves a paginated list of plans.
        /// </summary>
        /// <param name="pageSize">The number of plans to include per page.</param>
        /// <param name="currentPage">The current page number of the paginated results.</param>
        /// <returns>
        ///     A <see cref="PagedResponseOffsetDto{PlanResponse}"/> object containing the paginated list of plan details.
        /// </returns>
        public async Task<PagedResponseDTO<PlanResponse>> GetPlans(int pageSize, int currentPage)
        {
            IGenericRepository<Plan> genericRepository = this.unitOfWork.Repository<Plan>();
            PagedResponseOffset<Plan> plans = await genericRepository.GetAllRecordsAsync(pageSize, currentPage);

            return new PagedResponseDTO<PlanResponse>
            {
                Data = plans.Data.Select(p => new PlanResponse(p.PlanName, p.MontlyFee, p.NumberOfMinutes, p.Status)).ToList(),
                PageNumber = plans.PageNumber,
                PageSize = plans.PageSize,
                TotalRecords = plans.TotalRecords,
                TotalPages = plans.TotalPages,
                HasNextPage = plans.HasNextPage
            };
        }

        /// <summary>
        /// Retrieves a list of all plans.
        /// </summary>
        /// <returns>
        ///     An enumerable collection of <see cref="PlanResponse"/> objects representing the details of all plans.
        /// </returns>
        public async Task<IEnumerable<PlanResponse>> GetPlans()
        {
            IGenericRepository<Plan> genericRepository = this.unitOfWork.Repository<Plan>();
            IEnumerable<Plan> plans = await genericRepository.GetAllRecordsAsync();
            return plans.Select(p => new PlanResponse(p.PlanName, p.MontlyFee, p.NumberOfMinutes, p.Status));
        }
    }
}