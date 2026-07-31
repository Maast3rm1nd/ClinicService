namespace ClinicServiceBase.Common.Exceptions
{
    public class UnprocessableEntityException : Exception
    {
        public UnprocessableEntityException(string? message = null) : base(message) { }
    }
}
