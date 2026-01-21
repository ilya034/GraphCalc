namespace GraphCalc.Api.Responses;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    
    public string Message { get; set; }
    
    public string? ErrorCode { get; set; }
    
    public Dictionary<string, string[]>? Errors { get; set; }
    
    public string Timestamp { get; set; }

    public ErrorResponse(int statusCode, string message, string? errorCode = null)
    {
        StatusCode = statusCode;
        Message = message;
        ErrorCode = errorCode;
        Timestamp = DateTime.UtcNow.ToString("O");
    }

    public ErrorResponse(int statusCode, string message, Dictionary<string, string[]> errors)
    {
        StatusCode = statusCode;
        Message = message;
        Errors = errors;
        Timestamp = DateTime.UtcNow.ToString("O");
    }
}
