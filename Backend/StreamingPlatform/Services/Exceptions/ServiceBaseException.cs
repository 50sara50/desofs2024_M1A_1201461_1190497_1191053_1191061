namespace StreamingPlatform.Services.Exceptions
{
    /// <summary>
    /// Base class for all exceptions thrown by StreamingPlatform services.
    /// </summary>
    public class ServiceBaseException : Exception
    {
        public ServiceBaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceBaseException class with a specified error message and a reference to the inner exception that is the cause of the current exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ServiceBaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
