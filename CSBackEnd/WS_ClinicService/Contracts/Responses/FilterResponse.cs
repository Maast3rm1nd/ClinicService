namespace WS_ClinicService.Contracts.Responses
{
    public class FilterResponse<T>
    {
        public List<T> Items { get; set; } = [];
    }
}
