using Microsoft.AspNetCore.Mvc;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;

namespace StreamingPlatform.Services.Interfaces
{
    public interface IPlanService
    {

        public Task<PlanResponse> CreatePlan(CreatePlanContract planDto);

        public Task<PlanResponse?> GetPlan(string planName, bool isAdmin = false);

        public Task<PagedResponseDTO<PlanResponse>> GetPlans(int pageSize, int currentPage);

        public Task<IEnumerable<PlanResponse>> GetPlans();
    }
}
