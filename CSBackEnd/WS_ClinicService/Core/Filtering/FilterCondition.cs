using Newtonsoft.Json.Linq;

namespace WS_ClinicService.Core.Filtering
{
    public class FilterCondition
    {
        public FilterCondition(string propertyName, FilterOperation operation, JToken value)
        {
            PropertyName = propertyName;
            Operation = operation;
            Value = value;
        }

        public string PropertyName { get; }

        public FilterOperation Operation { get; }

        public JToken Value { get; }
    }
}
