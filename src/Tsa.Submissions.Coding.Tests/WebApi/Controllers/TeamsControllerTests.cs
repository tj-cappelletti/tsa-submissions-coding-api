using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tsa.Submissions.Coding.Tests.Data;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.Tests.WebApi.Controllers;

//TODO: Add for testing participants
public class TeamsControllerTests
{
    [Theory]
    [ClassData(typeof(InvalidTeamTestData))]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Post_Should_Return_Bad_Request(Team newTeam)
    {
        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.CreateAsync(It.IsAny<Team>()))
            .Callback((Team team) => team.Id = string.Empty);

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Post(newTeam);

        Assert.NotNull(actionResult);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Delete_By_Id_Should_Return_No_Content()
    {
        // Arrange
        const string expectedId = "1234567890";

        var expectedTeam = new Team
        {
            Id = expectedId,
            SchoolNumber = "9999",
            TeamNumber = "901"
        };

        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.GetAsync(It.Is<string>(s => s == expectedId)))
            .Returns(Task.FromResult(expectedTeam));

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Delete(expectedId);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);

        mockedTeamsService.Verify(m => m.GetAsync(It.IsAny<string>()), Times.Once);
        mockedTeamsService.Verify(m => m.RemoveAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Delete_By_Id_Should_Return_Not_Found()
    {
        // Arrange
        Team? nullTeam = null;

        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.GetAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(nullTeam));

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Delete("1234567890");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);

        mockedTeamsService.Verify(m => m.GetAsync(It.IsAny<string>()), Times.Once);
        mockedTeamsService.Verify(m => m.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Get_By_Id_Should_Return_A_Team()
    {
        // Arrange
        const string expectedId = "1234567890";

        var expectedTeam = new Team
        {
            Id = expectedId,
            SchoolNumber = "9999",
            TeamNumber = "901"
        };

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.GetAsync(It.Is<string>(s => s == expectedId)))
            .Returns(Task.FromResult(expectedTeam));

        var teamsController = new TeamsController(mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await teamsController.Get(expectedId);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(expectedTeam.Id, actionResult.Value!.Id);
        Assert.Equal(expectedTeam.SchoolNumber, actionResult.Value.SchoolNumber);
        Assert.Equal(expectedTeam.TeamId, actionResult.Value.TeamId);
        Assert.Equal(expectedTeam.TeamNumber, actionResult.Value.TeamNumber);

        mockedTeamsService.Verify(m => m.GetAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Get_By_Id_Should_Return_Not_Authorized()
    {
        // Arrange
        const string id = "1234567890";

        var team = new Team
        {
            Id = id,
            SchoolNumber = "9999",
            TeamNumber = "901",
            Participants = new List<Participant>
            {
                new() { ParticipantNumber = "100", SchoolNumber = "9999" }
            }
        };

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("8888-100");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.GetAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(team));

        var teamsController = new TeamsController(mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await teamsController.Get(id);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Result);
        Assert.IsType<UnauthorizedResult>(actionResult.Result);
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Get_By_Id_Should_Return_Not_Found()
    {
        // Arrange
        Team? nullTeam = null;

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.GetAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(nullTeam));

        var teamsController = new TeamsController(mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await teamsController.Get("1234567890");

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Result);
        Assert.IsType<NotFoundResult>(actionResult.Result);

        mockedTeamsService.Verify(m => m.GetAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Get_Should_Return_Empty_List()
    {
        // Arrange
        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.GetAsync())
            .Returns(Task.FromResult(new List<Team>()));

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actualTeams = await teamsController.Get();

        // Assert
        Assert.Empty(actualTeams);

        mockedTeamsService.Verify(m => m.GetAsync(), Times.Once);
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Get_Should_Return_None_Empty_List()
    {
        // Arrange
        var expectedTeams = new List<Team>
        {
            new() { Id = "1", SchoolNumber = "9999", TeamNumber = "901" },
            new() { Id = "2", SchoolNumber = "9999", TeamNumber = "902" },
            new() { Id = "3", SchoolNumber = "9999", TeamNumber = "903" }
        };

        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.GetAsync())
            .Returns(Task.FromResult(expectedTeams));

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actualTeams = (await teamsController.Get()).ToList();

        // Assert
        Assert.NotEmpty(actualTeams);
        Assert.True(expectedTeams.Count == actualTeams.Count);

        foreach (var actualTeam in actualTeams)
        {
            var expectedTeam = expectedTeams.SingleOrDefault(t => t.Id == actualTeam.Id);

            Assert.NotNull(expectedTeam);

            Assert.Equal(expectedTeam!.SchoolNumber, actualTeam.SchoolNumber);
            Assert.Equal(expectedTeam.TeamId, actualTeam.TeamId);
            Assert.Equal(expectedTeam.TeamNumber, actualTeam.TeamNumber);
        }

        mockedTeamsService.Verify(m => m.GetAsync(), Times.Once);
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Post_Should_Return_A_Team()
    {
        // Arrange
        const string expectedId = "1234567890";

        var expectedTeam = new Team
        {
            Id = expectedId,
            SchoolNumber = "9999",
            TeamNumber = "901"
        };

        var newTeam = new Team
        {
            SchoolNumber = expectedTeam.SchoolNumber,
            TeamNumber = expectedTeam.TeamNumber
        };

        Func<Team, bool> checkNewTeamFunc = submittedTeam => submittedTeam.SchoolNumber == newTeam.SchoolNumber && submittedTeam.TeamNumber == newTeam.TeamNumber;

        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.CreateAsync(It.IsAny<Team>()))
            .Callback((Team team) => team.Id = expectedId);

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Post(newTeam);

        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Result);
        Assert.IsType<CreatedAtActionResult>(actionResult.Result);

        var actualTeam = ((CreatedAtActionResult)actionResult.Result!).Value as Team;

        Assert.NotNull(actualTeam);
        Assert.Equal(expectedTeam.Id, actualTeam!.Id);
        Assert.Equal(expectedTeam.SchoolNumber, actualTeam.SchoolNumber);
        Assert.Equal(expectedTeam.TeamId, actualTeam.TeamId);
        Assert.Equal(expectedTeam.TeamNumber, actualTeam.TeamNumber);

        mockedTeamsService.Verify(m => m.CreateAsync(It.Is<Team>(data => checkNewTeamFunc(data))));
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Put_Should_Return_No_Content()
    {
        // Arrange
        const string expectedId = "1234567890";

        var expectedTeam = new Team
        {
            Id = expectedId,
            SchoolNumber = "9999",
            TeamNumber = "901"
        };

        var mockedTeamsService = new Mock<ITeamsService>();

        Func<Team, bool> checkNewTeamFunc = submittedTeam => submittedTeam.SchoolNumber == expectedTeam.SchoolNumber && submittedTeam.TeamNumber == expectedTeam.TeamNumber;

        mockedTeamsService.Setup(m => m.GetAsync(It.Is<string>(s => s == expectedId)))
            .Returns(Task.FromResult(expectedTeam));

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Put(expectedId, expectedTeam);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);

        mockedTeamsService.Verify(m => m.GetAsync(It.IsAny<string>()), Times.Once);
        mockedTeamsService.Verify(m => m.UpdateAsync(It.Is<string>(s => s == expectedId), It.Is<Team>(data => checkNewTeamFunc(data))));
    }

    [Fact]
    [Trait("AppLayer", "API")]
    [Trait("TestCategory", "UnitTest")]
    public async void TeamsController_Put_Should_Return_Not_Found()
    {
        // Arrange
        Team? nullTeam = null;

        const string expectedId = "1234567890";

        var expectedTeam = new Team
        {
            Id = expectedId,
            SchoolNumber = "9999",
            TeamNumber = "901"
        };

        var mockedTeamsService = new Mock<ITeamsService>();

        mockedTeamsService.Setup(m => m.GetAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(nullTeam));

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Put(expectedId, expectedTeam);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);

        mockedTeamsService.Verify(m => m.GetAsync(It.IsAny<string>()), Times.Once);
    }
}
