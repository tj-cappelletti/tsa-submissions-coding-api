using System;
using System.Net;
using PointOfSales.BackOffice.Services.WebApi.Exceptions;

namespace Tsa.Submissions.Coding.WebApi.Exceptions;

public class RequiredEntityNotFoundException : Exception, IWebApiException
{
    public string EntityName { get; }

    public HttpStatusCode HttpStatusCode { get; }

    public RequiredEntityNotFoundException(string entityName) : base($"Could not locate required resource for the `{entityName}` entity.")
    {
        EntityName = entityName;
        HttpStatusCode = HttpStatusCode.NotFound;
    }

    public ApiErrorResponse ToApiErrorResponseModel()
    {
        return new ApiErrorResponse
        {
            ErrorCode = (int)ErrorCodes.RequiredAttributeNotFound,
            Message = Message
        };
    }
}
