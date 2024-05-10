using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using StreamingPlatform.Controllers.ResponseMapper;
using StreamingPlatform.Controllers.Responses;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        /// </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePlan([FromBody] CreatePlanContract planDto)
        {
            try
            {
                logger.LogInformation("Creating plan");
                PlanResponse planResponse = await planService.CreatePlan(planDto);
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
        /// </returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 60 * 60)] //assuming the plan details are not updated frequently
        [HttpGet("{planName}")]
        public async Task<IActionResult> GetPlanByName([FromRoute][Required] string planName)
        {
            try
            {
                logger.LogInformation($"Getting plan {planName}");
                PlanResponse? planResponse = await planService.GetPlan(planName);
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [RequestTimeout(20)]
        [HttpGet]

        /// <summary>
        /// Gets all the plans in the system.
        /// </summary>
        /// <returns>
        /// A response with all the plans.
        ///  Returns 200 (OK) along with the list of plans
        ///  Returns 400(BAD Request) if only one of the header values (page size or current Page is Provided).        
        ///  If an unexpected error occurs during the get, returns 500 (Internal Server Error) with error details.
        /// </returns>
        public async Task<IActionResult> GetPlans([FromHeader] int? pageSize, [FromHeader] int? currentPage)
        {
            try
            {
                logger.LogInformation($"Getting plans");
                if ((pageSize == null && currentPage != null) || (pageSize != null && currentPage == null))
                {
                    ErrorResponseObject errorResponseObject = MapResponse.BadRequest("Both pageSize and currentPage must be provided");
                    return this.BadRequest(errorResponseObject);
                }

                if (pageSize == null && currentPage == null)
                {
                    IEnumerable<PlanResponse> plans = await planService.GetPlans();
                    return this.Ok(plans);
                }
                else
                {
#pragma warning disable CS8629 // We are sure that pageSize and currentPage are not null
                    PagedResponseDTO<PlanResponse> paginatedPlans = await planService.GetPlans(pageSize.Value, currentPage.Value);
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
