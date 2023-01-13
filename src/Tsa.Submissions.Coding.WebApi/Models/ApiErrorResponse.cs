namespace Tsa.Submissions.Coding.WebApi.Models;

public class ApiErrorResponse
{
    public int ErrorCode { get; set; }

    public string? Message { get; set; }

    public static ApiErrorResponse Unauthorized => new()
    {
        ErrorCode = (int)ErrorCodes.Unauthorized,
        Message = "Client is unauthorized"
    };

    public static ApiErrorResponse UnexpectedNullValue => new()
    {
        ErrorCode = (int)ErrorCodes.UnexpectedNullValue,
        Message = "A dependent resource could not be loaded while making this call."
    };

    public static ApiErrorResponse UnexpectedMissingResource => new()
    {
        ErrorCode = (int)ErrorCodes.UnexpectedMissingResource,
        Message = "A dependent resource could not be loaded while making this call."
    };
}
