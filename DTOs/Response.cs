namespace FoodioAPI.DTOs;

/// <summary>
/// Non-generic Response class for backward compatibility
/// </summary>
public class Response
{
    public string Status { get; set; } = ResponseStatus.SUCCESS;
    public string? Message { get; set; }
    public object? Data { get; set; }
}

/// <summary>
/// Generic Response class for type-safe responses
/// </summary>
/// <typeparam name="T">Type of the data being returned</typeparam>
public class Response<T>
{
    public string Status { get; set; } = ResponseStatus.SUCCESS;
    public string? Message { get; set; }
    public T? Data { get; set; }
}
