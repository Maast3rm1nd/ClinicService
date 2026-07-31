namespace ClinicServiceBase.Common.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string? message = null) : base(message) { }
    }
}
