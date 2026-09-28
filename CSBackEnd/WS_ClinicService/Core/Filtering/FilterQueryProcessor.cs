using System.Reflection;
using ClinicServiceBase.Common.Exceptions;
using Newtonsoft.Json.Linq;

namespace WS_ClinicService.Core.Filtering
{
    public static class FilterQueryProcessor
    {
        public static List<T> Apply<T>(IEnumerable<T> items, JArray? filter, string? sortBy, bool sortDesc, int? takeCount)
        {
            IEnumerable<T> query = items;

            var parsed = FilterExpressionParser.Parse(filter);

            if (parsed.Conditions.Count > 0)
            {
                query = query.Where(item => Evaluate(item!, parsed));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var property = GetProperty<T>(sortBy);

                query = sortDesc
                    ? query.OrderByDescending(item => property.GetValue(item))
                    : query.OrderBy(item => property.GetValue(item));
            }

            if (takeCount.HasValue)
            {
                query = query.Take(takeCount.Value);
            }

            return query.ToList();
        }

        private static bool Evaluate<T>(T item, ParsedFilter filter)
        {
            var result = EvaluateCondition(item, filter.Conditions[0]);

            for (var i = 0; i < filter.Operators.Count; i++)
            {
                var next = EvaluateCondition(item, filter.Conditions[i + 1]);
                result = filter.Operators[i] == FilterLogicalOperator.And ? result && next : result || next;
            }

            return result;
        }

        private static bool EvaluateCondition<T>(T item, FilterCondition condition)
        {
            var property = GetProperty<T>(condition.PropertyName);
            var propertyValue = property.GetValue(item);

            if (condition.Operation == FilterOperation.AnyOf)
            {
                var values = condition.Value as JArray
                    ?? throw new BadRequestException($"'AnyOf' operation requires an array value for property '{condition.PropertyName}'.");

                return values.Any(v => AreEqual(propertyValue, v, property.PropertyType));
            }

            if (condition.Operation == FilterOperation.Contains)
            {
                var stringValue = propertyValue?.ToString() ?? string.Empty;
                var searchValue = condition.Value.ToString();
                return stringValue.Contains(searchValue, StringComparison.OrdinalIgnoreCase);
            }

            if (condition.Operation == FilterOperation.Equal)
            {
                return AreEqual(propertyValue, condition.Value, property.PropertyType);
            }

            var comparableValue = ConvertValue(condition.Value, property.PropertyType);
            var comparison = Compare(propertyValue, comparableValue);

            return condition.Operation switch
            {
                FilterOperation.GreaterThan => comparison > 0,
                FilterOperation.GreaterThanOrEqual => comparison >= 0,
                FilterOperation.LessThan => comparison < 0,
                FilterOperation.LessThanOrEqual => comparison <= 0,
                _ => throw new BadRequestException($"Unsupported filter operation: {condition.Operation}")
            };
        }

        private static PropertyInfo GetProperty<T>(string propertyName)
        {
            return typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                ?? throw new BadRequestException($"Property '{propertyName}' was not found on type '{typeof(T).Name}'.");
        }

        private static bool AreEqual(object? propertyValue, JToken value, Type propertyType)
        {
            var converted = ConvertValue(value, propertyType);

            if (propertyValue == null || converted == null)
            {
                return propertyValue == null && converted == null;
            }

            return propertyValue.Equals(converted);
        }

        private static int Compare(object? propertyValue, object? comparableValue)
        {
            if (propertyValue == null || comparableValue == null)
            {
                return Comparer<object>.Default.Compare(propertyValue, comparableValue);
            }

            if (propertyValue is IComparable comparable)
            {
                return comparable.CompareTo(comparableValue);
            }

            throw new BadRequestException($"Type '{propertyValue.GetType().Name}' does not support comparison.");
        }

        private static object? ConvertValue(JToken value, Type targetType)
        {
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (value.Type == JTokenType.Null)
            {
                return null;
            }

            if (underlyingType == typeof(Guid))
            {
                return Guid.Parse(value.ToString());
            }

            if (underlyingType.IsEnum)
            {
                return Enum.Parse(underlyingType, value.ToString(), true);
            }

            return value.ToObject(underlyingType);
        }
    }
}
