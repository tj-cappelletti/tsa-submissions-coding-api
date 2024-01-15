using System.Collections.Generic;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class ApiErrorResponseModel
{
    public Dictionary<string, string> Data { get; } = [];

    public int ErrorCode { get; set; }

    public string? Message { get; set; }


    public static ApiErrorResponseModel Unauthorized => new()
    {
        ErrorCode = (int)ErrorCodes.Unauthorized,
        Message = "Client is unauthorized"
    };

    public static ApiErrorResponseModel UnexpectedMissingResource => new()
    {
        ErrorCode = (int)ErrorCodes.UnexpectedMissingResource,
        Message = "A dependent resource could not be loaded while making this call."
    };

    public static ApiErrorResponseModel UnexpectedNullValue => new()
    {
        ErrorCode = (int)ErrorCodes.UnexpectedNullValue,
        Message = "A required value was unexpectedly missing."
    };

    public static ApiErrorResponseModel EntityNotFound(string entityName, string id)
    {
        var apiErrorResponseModel = new ApiErrorResponseModel
        {
            ErrorCode = (int)ErrorCodes.EntityNotFound,
            Message = "The requested resource could not be found."
        };

        apiErrorResponseModel.Data.Add("entityName", entityName);
        apiErrorResponseModel.Data.Add("id", id);

        return apiErrorResponseModel;
    }
}
