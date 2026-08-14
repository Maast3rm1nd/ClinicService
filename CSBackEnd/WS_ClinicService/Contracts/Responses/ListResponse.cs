namespace WS_ClinicService.Contracts.Responses
{
    public class ListResponse<T>
    {
        public List<T> Data { get; set; }
    }
}