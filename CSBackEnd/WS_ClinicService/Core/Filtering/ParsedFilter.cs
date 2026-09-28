namespace WS_ClinicService.Core.Filtering
{
    public class ParsedFilter
    {
        public ParsedFilter(IReadOnlyList<FilterCondition> conditions, IReadOnlyList<FilterLogicalOperator> operators)
        {
            Conditions = conditions;
            Operators = operators;
        }

        public IReadOnlyList<FilterCondition> Conditions { get; }

        public IReadOnlyList<FilterLogicalOperator> Operators { get; }
    }
}
