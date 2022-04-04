using Microsoft.AspNetCore.Mvc;
using Moq;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.Tests.WebApi.Controllers;

/// <summary>
///     Tests the status controller to ensure the right status is being returned
/// </summary>
public class StatusControllerTests
{
    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    //TODO: Improve naming convention as additional tests get written
    public async void StatusController_Get_Should_Return_InternalServerError()
    {
        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.Ping())
            .ReturnsAsync(false);

        var statusController = new StatusController(mockedTeamsService.Object);

        // Act
        var actionResult = await statusController.Get();

        Assert.NotNull(actionResult);
        Assert.IsType<ObjectResult>(actionResult);

        var objectResult = (ObjectResult)actionResult;

        Assert.Equal(500, objectResult.StatusCode);
        Assert.NotNull(objectResult.Value);
        Assert.IsType<Status>(objectResult.Value);

        var status = (Status)objectResult.Value!;

        Assert.False(status.IsHealthy);
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    //TODO: Improve naming convention as additional tests get written
    public async void StatusController_Get_Should_Return_Ok()
    {
        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.Ping())
            .ReturnsAsync(true);

        var statusController = new StatusController(mockedTeamsService.Object);

        // Act
        var actionResult = await statusController.Get();

        Assert.NotNull(actionResult);
        Assert.IsType<OkObjectResult>(actionResult);

        var okObjectResult = (OkObjectResult)actionResult;

        Assert.NotNull(okObjectResult.Value);
        Assert.IsType<Status>(okObjectResult.Value);

        var status = (Status)okObjectResult.Value!;

        Assert.True(status.IsHealthy);
    }
}
