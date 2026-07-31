namespace ClinicServiceBase.Common.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string? message = null) : base(message) { }
    }
}
