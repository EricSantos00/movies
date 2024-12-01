namespace MoviesApi.Validations;

public class ValidationException(ValidationError[] errors) : Exception
{
    public ValidationError[] Errors { get; } = errors;
}

public record ValidationError(string PropertyName, string ErrorMessage);