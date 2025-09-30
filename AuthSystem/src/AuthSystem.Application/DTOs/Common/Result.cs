namespace AuthSystem.Application.DTOs.Common;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static Result<T> Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage,
        Errors = new List<string> { errorMessage }
    };

    public static Result<T> Failure(List<string> errors) => new()
    {
        IsSuccess = false,
        Errors = errors,
        ErrorMessage = string.Join(", ", errors)
    };
}
