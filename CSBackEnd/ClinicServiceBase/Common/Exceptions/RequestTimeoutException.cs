namespace ClinicServiceBase.Common.Exceptions
{
    public class RequestTimeoutException : Exception
    {
        public RequestTimeoutException(string? message = null) : base(message) { }
    }
}
