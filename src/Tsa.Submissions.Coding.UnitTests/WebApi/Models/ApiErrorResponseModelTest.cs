using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Models;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Models;

[ExcludeFromCodeCoverage]
public class ApiErrorResponseTest
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Unauthorized_Should_Return_ApiErrorResponse_For_Unauthorized()
    {
        // Arrange
        // Act
        var apiErrorResponse = ApiErrorResponseModel.Unauthorized;

        // Assert
        Assert.Equal((int)ErrorCodes.Unauthorized, apiErrorResponse.ErrorCode);
        Assert.Equal("Client is unauthorized", apiErrorResponse.Message);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void UnexpectedMissingResource_Should_Return_ApiErrorResponse_For_UnexpectedMissingResource()
    {
        // Arrange
        // Act
        var apiErrorResponse = ApiErrorResponseModel.UnexpectedMissingResource;

        // Assert
        Assert.Equal((int)ErrorCodes.UnexpectedMissingResource, apiErrorResponse.ErrorCode);
        Assert.Equal("A dependent resource could not be loaded while making this call.", apiErrorResponse.Message);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void UnexpectedNullValue_Should_Return_ApiErrorResponse_For_UnexpectedNullValue()
    {
        // Arrange
        // Act
        var apiErrorResponse = ApiErrorResponseModel.UnexpectedNullValue;

        // Assert
        Assert.Equal((int)ErrorCodes.UnexpectedNullValue, apiErrorResponse.ErrorCode);
        Assert.Equal("A dependent resource could not be loaded while making this call.", apiErrorResponse.Message);
    }
}
