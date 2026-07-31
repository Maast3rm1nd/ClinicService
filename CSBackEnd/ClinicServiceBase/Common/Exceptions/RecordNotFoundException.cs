namespace ClinicServiceBase.Common.Exceptions
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException(string? message = null) : base(message) { }
    }
}
