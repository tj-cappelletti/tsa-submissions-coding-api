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
        var apiErrorResponse = ApiErrorResponse.Unauthorized;

        // Assert
        Assert.Equal((int)ErrorCodes.Unauthorized, apiErrorResponse.ErrorCode);
        Assert.Equal("Client is unauthorized", apiErrorResponse.Message);
    }
}
