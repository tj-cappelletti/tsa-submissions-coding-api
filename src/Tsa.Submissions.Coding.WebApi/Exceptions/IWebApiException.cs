using System.Net;

namespace PointOfSales.BackOffice.Services.WebApi.Exceptions;

public interface IWebApiException
{
    HttpStatusCode HttpStatusCode { get; }

    ApiErrorResponse ToApiErrorResponseModel();
}