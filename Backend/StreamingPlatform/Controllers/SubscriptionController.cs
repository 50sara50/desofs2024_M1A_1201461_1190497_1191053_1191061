using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        [HttpPost("ById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status401Unauthorized)]
        [Consumes("application/json")]

        public async Task<IActionResult> CreateSubscriptionById([FromBody] CreateSubscriptionContractById subscriptionDto)
        {
            try
            {
                logger.LogInformation("Creating subscription");
                SubscriptionResponse subscriptionResponse = await subscriptionService.CreateSubscriptionById(subscriptionDto);
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
        
        [HttpGet("GetUserSubscriptions")]
        [Authorize]
        public async Task<IActionResult> GetUserSubscriptions([FromQuery] string userId)
        {
            try
            {
                var results = await subscriptionService.GetSubscriptions(userId);
                logger.LogInformation(@"Success");
                return this.Ok(results);
            }
            catch (ValidationException v)
            {
                logger.LogError($"Validation exception: ${v.Message}.");
                return this.BadRequest(v.Message);
            }
            catch (InvalidOperationException i)
            {
                logger.LogError($"Invalid operation exceptions: ${i.Message}.");
                return this.Conflict(i.Message);
            }
            catch (Exception e)
            {
                logger.LogError($"Exception: ${e.Message}.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

    }
}