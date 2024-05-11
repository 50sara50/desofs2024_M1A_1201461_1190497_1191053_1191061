using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using StreamingPlatform.Controllers.ResponseMapper;
using StreamingPlatform.Controllers.Responses;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform.Controllers
{
    [ApiController]
    [Route("api/plan")]
    [EnableRateLimiting("fixed-by-user-id-or-ip")]
    public class PlanController(ILogger<PlanController> logger, IPlanService planService) : ControllerBase
    {
        /// <summary>
        /// Creates a new plan based on the provided plan data.
        /// </summary>
        /// <param name="planDto">The data for the plan to be created.</param>
        /// <returns>
        ///     A response indicating the outcome of the plan creation operation.
        ///     If successful, returns 200 (OK) along with the created plan details.
        ///     If the input data is invalid, returns 400 (Bad Request) with validation error details.
        ///     If a plan with the same name already exists, returns 409 (Conflict) with an appropriate error message.
        ///     If an unexpected error occurs during plan creation, returns 500 (Internal Server Error) with error details.
        ///     If the user is not authorized to create a plan, returns 401 (Unauthorized).
        ///     If the user has exceeded the rate limit returns 429 (Too Many Requests).
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status429TooManyRequests)]
        [Consumes("application/json")]

        public async Task<IActionResult> CreatePlan([FromBody] CreatePlanContract planDto, IOutputCacheStore cache)
        {
            try
            {
                logger.LogInformation("Creating plan");
                PlanResponse planResponse = await planService.CreatePlan(planDto);
                await cache.EvictByTagAsync("tag-plan", default);
                return this.Ok(planResponse);
            }
            catch (ValidationException e)
            {
                logger.LogError($"Validation error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.BadRequest("Invalid data for a plan");
                return this.BadRequest(errorResponseObject);
            }
            catch (InvalidOperationException e)
            {
                logger.LogError($"Error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.Conflict(e.Message);
                return this.Conflict(errorResponseObject);
            }
            catch (Exception e)
            {
                logger.LogError($"Error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.InternalServerError();
                return this.StatusCode(StatusCodes.Status500InternalServerError, errorResponseObject);
            }
        }

        /// <summary>
        /// Gets the plan with the specified name.
        /// </summary>
        /// <param name="planName">the name of the plan</param>
        /// <returns>
        /// A response indicating the outcome of the plan creation operation.
        /// If successful, returns 200 (OK) along with the created plan details.
        /// If no plan with the specified name exists, returns 404 (Not Found).
        /// If an unexpected error occurs during the get, returns 500 (Internal Server Error) with error details.
        /// If the user has exceeded the rate limit, returns 429 (Too Many Requests).
        /// </returns>
        [Produces("application/json")]
        [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status429TooManyRequests)]
        [OutputCache(PolicyName = "evict", Duration = 60)]
        [HttpGet("{planName}")]
        public async Task<IActionResult> GetPlanByName([FromRoute][Required] string planName)
        {
            try
            {
                bool canAccessInactivePlans = this.User.IsInRole("Admin");
                logger.LogInformation($"Getting plan {planName}");
                PlanResponse? planResponse = await planService.GetPlan(planName, canAccessInactivePlans);
                if (planResponse == null)
                {
                    ErrorResponseObject errorResponseObject = MapResponse.NotFound("No plan with the specified name");
                    return this.NotFound(errorResponseObject);
                }

                return this.Ok(planResponse);
            }
            catch (Exception e)
            {
                logger.LogError($"Error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.InternalServerError();
                return this.StatusCode(StatusCodes.Status500InternalServerError, errorResponseObject);
            }
        }

        [HttpGet]
        [RequestTimeout(20)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<PlanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PagedResponseDTO<PlanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status429TooManyRequests)]
        [OutputCache(PolicyName = "evict", Duration = 60, VaryByQueryKeys = ["pageSize", "currentPage"])]
        /// <summary>
        /// Gets all the plans in the system.
        /// </summary>
        /// <returns>
        /// A response with all the plans.
        ///  Returns 200 (OK) along with the list of plans
        ///  Returns 400(BAD Request) if only one of the header values (page size or current Page is Provided).        
        ///  If an unexpected error occurs during the get, returns 500 (Internal Server Error) with error details.
        ///  If the user has exceeded the rate limit, returns 429 (Too Many Requests).
        /// </returns>
        public async Task<IActionResult> GetPlans([FromQuery] int? pageSize, [FromQuery] int? currentPage)
        {
            try
            {
                logger.LogInformation($"Getting plans");
                if ((pageSize == null && currentPage != null) || (pageSize != null && currentPage == null))
                {
                    ErrorResponseObject errorResponseObject = MapResponse.BadRequest("Both pageSize and currentPage must be provided");
                    return this.BadRequest(errorResponseObject);
                }

                bool canAccessInactivePlans = this.User.IsInRole("Admin"); //Ideally we would do this with claims but for simplicity we are using roles
                if (pageSize == null && currentPage == null)
                {
                    IEnumerable<PlanResponse> plans = await planService.GetPlans(canAccessInactivePlans);
                    return this.Ok(plans);
                }
                else
                {
#pragma warning disable CS8629 // We are sure that pageSize and currentPage are not null
                    PagedResponseDTO<PlanResponse> paginatedPlans = await planService.GetPlans(pageSize.Value, currentPage.Value, canAccessInactivePlans);
                    return this.Ok(paginatedPlans);
#pragma warning restore CS8629 // Possible null reference argument.
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.InternalServerError();
                return this.StatusCode(StatusCodes.Status500InternalServerError, errorResponseObject);
            }
        }
    }
}
