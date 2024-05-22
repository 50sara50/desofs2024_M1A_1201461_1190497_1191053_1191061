namespace StreamingPlatform.Controllers.Responses
{
    /// <summary>
    /// Class to represent an error response object.
    /// </summary>
    public class ErrorResponseObject
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        required public string Code { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        required public string Message { get; set; }
    }
}
