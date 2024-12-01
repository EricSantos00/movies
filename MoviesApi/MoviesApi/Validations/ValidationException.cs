namespace MoviesApi.Validations;

public record ValidationError(string PropertyName, string ErrorMessage);

public class ValidationException(ValidationError[] errors) : Exception
{
    public ValidationError[] Errors { get; } = errors;

    public ValidationException(string propertyName, string errorMessage)
        : this([new ValidationError(propertyName, errorMessage)])
    {
    }
}