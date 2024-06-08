using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingPlatform.Controllers.ResponseMapper;
using StreamingPlatform.Controllers.Responses;
using StreamingPlatform.Dtos.Contracts;
using StreamingPlatform.Dtos.Responses;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionController(ILogger<SubscriptionController> logger, ISubscriptionService subscriptionService) : ControllerBase
    {
        /// <summary>
        /// Creates a new subscription based on the provided subscription data.
        /// </summary>
        /// <param name="subscriptionDto">The data for the subscription to be created.</param>
        /// <returns>
        ///     A response indicating the outcome of the subscription creation operation.
        ///     If successful, returns 200 (OK) along with the created subscription details.
        ///     If the input data is invalid, returns 400 (Bad Request) with validation error details.
        ///     If a subscription with the same name already exists, returns 409 (Conflict) with an appropriate error message.
        ///     If an unexpected error occurs during subscription creation, returns 500 (Internal Server Error) with error details.
        ///     If the user is not authorized to create a subscription, returns 401 (Unauthorized).
        /// </returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status401Unauthorized)]
        [Consumes("application/json")]

        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionContract subscriptionDto)
        {
            try
            {
                logger.LogInformation("Creating subscription");
                SubscriptionResponse subscriptionResponse = await subscriptionService.CreateSubscription(subscriptionDto);
                return this.Ok(subscriptionResponse);
            }
            catch (ValidationException e)
            {
                logger.LogError($"Validation error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.BadRequest("Invalid data for a subscription");
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

    }
}