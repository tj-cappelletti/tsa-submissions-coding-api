namespace Tsa.Submissions.Coding.WebApi.Models;

public class ApiErrorResponseModel
{
    public int ErrorCode { get; set; }

    public string? Message { get; set; }

    public static ApiErrorResponseModel Unauthorized => new()
    {
        ErrorCode = (int)ErrorCodes.Unauthorized,
        Message = "Client is unauthorized"
    };

    public static ApiErrorResponseModel UnexpectedNullValue => new()
    {
        ErrorCode = (int)ErrorCodes.UnexpectedNullValue,
        Message = "A dependent resource could not be loaded while making this call."
    };

    public static ApiErrorResponseModel UnexpectedMissingResource => new()
    {
        ErrorCode = (int)ErrorCodes.UnexpectedMissingResource,
        Message = "A dependent resource could not be loaded while making this call."
    };
}
