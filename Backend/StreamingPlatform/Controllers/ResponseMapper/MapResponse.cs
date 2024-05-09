using StreamingPlatform.Controllers.Responses;

namespace StreamingPlatform.Controllers.ResponseMapper
{
    public class MapResponse
    {

        public static ErrorResponseObject BadRequest(string message)
        {
            return new ErrorResponseObject
            {
                Code = "BadRequest",
                Message = message,
            };
        }

        public static ErrorResponseObject NotFound(string message)
        {
            return new ErrorResponseObject
            {
                Code = "NotFound",
                Message = message,
            };
        }

        public static ErrorResponseObject InternalServerError(string message)
        {
            return new ErrorResponseObject
            {
                Code = "InternalServerError",
                Message = message,
            };
        }

        public static ErrorResponseObject InternalServerError()
        {
            return new ErrorResponseObject
            {
                Code = "InternalServerError",
                Message = "Unexpected Server Error Error. Please try again later or contact an admin",
            };
        }

        public static ErrorResponseObject Unauthorized(string message)
        {
            return new ErrorResponseObject
            {
                Code = "Unauthorized",
                Message = message,
            };
        }

        public static ErrorResponseObject Forbidden(string message)
        {
            return new ErrorResponseObject
            {
                Code = "Forbidden",
                Message = message,
            };
        }

        public static ErrorResponseObject Conflict(string message)
        {
            return new ErrorResponseObject
            {
                Code = "Conflict",
                Message = message,
            };
        }
    }
}
