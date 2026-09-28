using ClinicServiceBase.Common.Exceptions;
using Newtonsoft.Json.Linq;

namespace WS_ClinicService.Core.Filtering
{
    public static class FilterExpressionParser
    {
        public static ParsedFilter Parse(JArray? filterArray)
        {
            var conditions = new List<FilterCondition>();
            var operators = new List<FilterLogicalOperator>();

            if (filterArray == null || filterArray.Count == 0)
            {
                return new ParsedFilter(conditions, operators);
            }

            foreach (var token in filterArray)
            {
                if (token.Type == JTokenType.Array)
                {
                    var conditionArray = (JArray)token;

                    if (conditionArray.Count != 3)
                    {
                        throw new BadRequestException($"Filter condition must contain exactly 3 elements: [field, operator, value]. Got: {conditionArray}");
                    }

                    var propertyName = conditionArray[0].ToString();

                    if (!Enum.TryParse<FilterOperation>(conditionArray[1].ToString(), true, out var operation))
                    {
                        throw new BadRequestException($"Unknown filter operation: {conditionArray[1]}");
                    }

                    conditions.Add(new FilterCondition(propertyName, operation, conditionArray[2]));
                }
                else if (token.Type == JTokenType.String)
                {
                    if (!Enum.TryParse<FilterLogicalOperator>(token.ToString(), true, out var logicalOperator))
                    {
                        throw new BadRequestException($"Unknown logical operator: {token}");
                    }

                    operators.Add(logicalOperator);
                }
                else
                {
                    throw new BadRequestException($"Unexpected token in filter array: {token}");
                }
            }

            if (operators.Count != Math.Max(conditions.Count - 1, 0))
            {
                throw new BadRequestException("Filter array must alternate between condition arrays and logical operators ('And'/'Or').");
            }

            return new ParsedFilter(conditions, operators);
        }
    }
}
