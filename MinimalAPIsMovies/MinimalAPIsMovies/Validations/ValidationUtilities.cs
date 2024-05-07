namespace MinimalAPIsMovies.Validations;

public static class ValidationUtilities
{
    public const string NonEmptyMessage = "The field {PropertyName} is required.";

    public const string MaxLengthMessage = "The field {propertyName} should be less than {MaxLength} characters.";

    public const string FirstLetterIsUpperCaseMessage = "The field {propertyName} should start with uppercase";

    public static string GreaterThanDate(DateTime value) =>
        "The field {PropertyName} should be greater than " + value.ToString("yyyy-MM-dd");
    
    public static bool FirstLetterIsUpperCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        var firstLetter = value[0].ToString();

        return firstLetter.Equals(firstLetter, StringComparison.CurrentCultureIgnoreCase);
    }
    
}