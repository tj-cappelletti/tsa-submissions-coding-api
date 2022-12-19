public class ApiErrorResponse
{
    public int ErrorCode { get; set; }

    public string? Message { get; set; }

    public static ApiErrorResponse Unauthorized => new() { ErrorCode = (int)ErrorCodes.Unauthorized, Message = "Client is unauthorized" };
}