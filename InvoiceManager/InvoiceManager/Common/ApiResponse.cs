namespace InvoiceManager.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    
    public static ApiResponse<T> SuccessResult(T? data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = "Success"
        };
    }

    public static ApiResponse<T> FailureResult(string message = "Failure")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
}