using StreamingPlatform.Controllers.Responses;

namespace StreamingPlatform.Controllers.ResponseMapper
{
    public class MapResponse
    {
        /// <summary>
        /// Creates an error response object with a "BadRequest" code and a custom message.
        /// Used to indicate that the request was invalid due to invalid input data.
        /// </summary>
        /// <param name="message">The specific error message to include in the response.</param>
        /// <returns>An ErrorResponseObject instance with the specified code and message.</returns>
        public static ErrorResponseObject BadRequest(string message)
        {
            return new ErrorResponseObject
            {
                Code = "BadRequest",
                Message = message,
            };
        }

        /// <summary>
        /// Creates an error response object with a "NotFound" code and a custom message.
        /// 
        /// Used to indicate that the requested resource could not be found.
        /// </summary>
        /// <param name="message">The specific error message to include in the response.</param>
        /// <returns>An ErrorResponseObject instance with the specified code and message.</returns>
        public static ErrorResponseObject NotFound(string message)
        {
            return new ErrorResponseObject
            {
                Code = "NotFound",
                Message = message,
            };
        }

        /// <summary>
        /// Creates an error response object with a "InternalServerErro" code and a custom message.
        /// Used to indicate that the request failed due to an unexpected server error.
        /// </summary>
        /// <param name="message">The specific error message to include in the response.</param>
        /// <returns>An ErrorResponseObject instance with the specified code and message.</returns>
        public static ErrorResponseObject InternalServerError(string message)
        {
            return new ErrorResponseObject
            {
                Code = "InternalServerError",
                Message = message,
            };
        }

        /// <summary>
        /// Creates an error response object with a "Internal Server" code.
        /// Used to indicate that the request failed due to an unexpected server error.
        /// </summary>
        /// <returns>An ErrorResponseObject instance with the specified code and static message.</returns>
        public static ErrorResponseObject InternalServerError()
        {
            return new ErrorResponseObject
            {
                Code = "InternalServerError",
                Message = "Unexpected Server Error Error. Please try again later or contact an admin",
            };
        }

        /// <summary>
        /// Creates an error response object with a "Unauthorized" code and a custom message.
        /// Used to indicate that the request failed due to lack of authorization.
        /// </summary>
        /// <param name="message">The specific error message to include in the response.</param>
        /// <returns>An ErrorResponseObject instance with the specified code and message.</returns>
        public static ErrorResponseObject Unauthorized(string message)
        {
            return new ErrorResponseObject
            {
                Code = "Unauthorized",
                Message = message,
            };
        }

        /// <summary>
        /// Creates an error response object with a "Forbidden" code and a custom message.
        /// Used to indicate that the request failed due to lack of permissions.
        /// </summary>
        /// <param name="message">The specific error message to include in the response.</param>
        /// <returns>An ErrorResponseObject instance with the specified code and message.</returns>
        public static ErrorResponseObject Forbidden(string message)
        {
            return new ErrorResponseObject
            {
                Code = "Forbidden",
                Message = message,
            };
        }

        /// <summary>
        /// Creates an error response object with a "Conflict" code and a custom message.
        /// Used to indicate that the request failed due to a conflict with the current state of the resource.
        /// </summary>
        /// <param name="message">The specific error message to include in the response.</param>
        /// <returns>An ErrorResponseObject instance with the specified code and message.</returns>
        public static ErrorResponseObject Conflict(string message)
        {
            return new ErrorResponseObject
            {
                Code = "Conflict",
                Message = message,
            };
        }

        /// <summary>
        /// Creates an error response object with a "ToManyRequests" code and a custom message.
        /// Used to indicate that the request failed due to too many requests being made in a short period of time.
        /// </summary>
        /// <param name="retryAfter">The number of seconds the client should wait before retrying the request.</param>
        /// <returns>An ErrorResponseObject instance with the specified code and message.</returns>
        public static ErrorResponseObject TooManyRequests()
        {
            return new ErrorResponseObject
            {
                Code = "ToManyRequests",
                Message = "Too many requests. Please try again later.",
            };
        }
    }
}
