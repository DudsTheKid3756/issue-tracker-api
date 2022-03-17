using System.Text.RegularExpressions;

namespace IssueTracker.Utils;

public class Validation
{
    private static string ValidateField(string input, string field, ICollection<string> nullFields)
    {
        var match = Regex.Match(input, Constants.Alphanumeric);
        if (nullFields.Contains(field)) return "";
        if (!match.Success || input.StartsWith(' ')) return field + Constants.Invalid;
        return "";
    }

    private static string ValidateEntity(object o, List<string> nullFields)
    {
        return o.GetType().GetProperties().Where(property => property.PropertyType.Name is "String")
            .Where(property => property.GetValue(o) is not null).Aggregate("",
                (current, property) =>
                    current + ValidateField(property.GetValue(o)!.ToString()!, property.Name, nullFields));
    }

    public static string GetErrors(object o)
    {
        var nullFields = (from property in o.GetType()
                .GetProperties()
            where property.GetValue(o) is null
            select property.Name).ToList();
        var nullErrors = string.Join(", ", nullFields.Select(field => field + Constants.Null));
        return ValidateEntity(o, nullFields) + nullErrors;
    }
}