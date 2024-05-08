namespace StreamingPlatform.Services.Exceptions
{
    public class ServiceBaseException : Exception
    {
        public ServiceBaseException(string message)
            : base(message)
        {
        }

        public ServiceBaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
