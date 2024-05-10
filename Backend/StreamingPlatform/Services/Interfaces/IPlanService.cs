using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;

namespace StreamingPlatform.Services.Interfaces
{
    /// <summary>
    /// Interface defining methods for managing streaming platform plans.
    /// </summary>
    public interface IPlanService
    {
        /// <summary>
        /// Creates a new streaming plan based on the provided details.
        /// </summary>
        /// <param name="planDto">DTO containing information about the plan to be created.</param>
        /// <returns>A task that resolves to a DTO containing details of the created plan.</returns>
        public Task<PlanResponse> CreatePlan(CreatePlanContract planDto);

        /// <summary>
        /// Retrieves details of a streaming plan by its name.
        /// </summary>
        /// <param name="planName">The name of the plan to retrieve.</param>
        /// <param name="isAdmin">Optional flag indicating if the request is coming from an admin (default false).
        /// Admins may have access to additional plan details.</param>
        /// <returns>A task that resolves to a DTO containing details of the requested plan, or null if not found.</returns>
        public Task<PlanResponse?> GetPlan(string planName, bool isAdmin = false);

        /// <summary>
        /// Retrieves a paginated list of streaming plans.
        /// </summary>
        /// <param name="pageSize">The number of plans to include in each page.</param>
        /// <param name="currentPage">The current page number (starting from 1).</param>
        /// <returns>A task that resolves to a DTO containing a list of plan details and pagination information.</returns>
        public Task<PagedResponseDTO<PlanResponse>> GetPlans(int pageSize, int currentPage);

        /// <summary>
        /// Retrieves a list of all streaming plans.
        /// </summary>
        /// <returns>A task that resolves to an IEnumerable containing details of all available plans.</returns>
        public Task<IEnumerable<PlanResponse>> GetPlans();
    }
}
