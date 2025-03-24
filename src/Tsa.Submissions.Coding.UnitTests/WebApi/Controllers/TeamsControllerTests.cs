using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class TeamsControllerTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Authorize_Attribute_With_Proper_Roles()
    {
        var teamsControllerType = typeof(TeamsController);

        var methodInfos = teamsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        Assert.NotEmpty(methodInfos);

        foreach (var methodInfo in methodInfos)
        {
            var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), false);

            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
            Assert.Single(attributes);

            var authorizeAttribute = (AuthorizeAttribute)attributes[0];

            switch (methodInfo.Name)
            {
                case "Delete":
                    Assert.Equal(SubmissionRoles.Judge, authorizeAttribute.Roles);
                    break;

                case "Get":
                    Assert.Equal(SubmissionRoles.All, authorizeAttribute.Roles);
                    break;

                case "Post":
                    Assert.Equal(SubmissionRoles.Judge, authorizeAttribute.Roles);
                    break;

                case "Put":
                    Assert.Equal(SubmissionRoles.Judge, authorizeAttribute.Roles);
                    break;

                default:
                    Assert.Fail($"A test case for the method `{methodInfo.Name}` does not exist");
                    break;
            }
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Http_Method_Attribute()
    {
        var teamsControllerType = typeof(TeamsController);

        var methodInfos = teamsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        Assert.NotEmpty(methodInfos);

        foreach (var methodInfo in methodInfos)
        {
            // Needs to be nullable so the compiler sees it's initialized
            // The Assert.Fail doesn't tell it that the line it's being used
            // will only ever be hit if it's initialized
            Type? attributeType = null;

            switch (methodInfo.Name.ToLower())
            {
                case "delete":
                    attributeType = typeof(HttpDeleteAttribute);
                    break;
                case "get":
                    attributeType = typeof(HttpGetAttribute);
                    break;
                case "head":
                    attributeType = typeof(HttpHeadAttribute);
                    break;
                case "options":
                    attributeType = typeof(HttpOptionsAttribute);
                    break;
                case "patch":
                    attributeType = typeof(HttpPatchAttribute);
                    break;
                case "post":
                    attributeType = typeof(HttpPostAttribute);
                    break;
                case "put":
                    attributeType = typeof(HttpPutAttribute);
                    break;
                default:
                    Assert.Fail("Unsupported public HTTP operation");
                    break;
            }

            var attributes = methodInfo.GetCustomAttributes(attributeType, false);

            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
            Assert.Single(attributes);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_ApiController_Attribute()
    {
        var teamsControllerType = typeof(TeamsController);

        var attributes = teamsControllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Produces_Attribute()
    {
        var teamsControllerType = typeof(TeamsController);

        var attributes = teamsControllerType.GetCustomAttributes(typeof(ProducesAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var producesAttribute = (ProducesAttribute)attributes[0];

        Assert.Contains("application/json", producesAttribute.ContentTypes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Route_Attribute()
    {
        var teamsControllerType = typeof(TeamsController);

        var attributes = teamsControllerType.GetCustomAttributes(typeof(RouteAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var routeAttribute = (RouteAttribute)attributes[0];

        Assert.Equal("api/[controller]", routeAttribute.Template);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Should_Return_No_Content()
    {
        // ArrangeE
        var teamsTestData = new TeamsTestData();

        var team = teamsTestData.First(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)[0] as Team;

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService.Setup(teamsService => teamsService.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(team);

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Delete("64639f6fcdde06187b09ecae", default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Should_Return_Not_Found()
    {
        // Arrange
        var mockedTeamsService = new Mock<ITeamsService>();

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Delete("64639f6fcdde06187b09ecae", default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Not_Found()
    {
        // Arrange
        var mockedTeamsService = new Mock<ITeamsService>();

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Get("64639f6fcdde06187b09ecae", default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Not_Found_For_Participant_Role()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var team = teamsTestData.First(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)[0] as Team;

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService.Setup(teamsService => teamsService.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(team);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var teamsController = new TeamsController(mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await teamsController.Get("64639f6fcdde06187b09ecae", default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Ok_For_Judge_Role()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var team = teamsTestData.First(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)[0] as Team;

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService.Setup(teamsService => teamsService.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(team);

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var teamsController = new TeamsController(mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await teamsController.Get("64639f6fcdde06187b09ecae", default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(team!.Id, actionResult.Value!.Id);
        Assert.Equal(team.Participants.Count, actionResult.Value.Participants.Count);

        foreach (var participantModel in actionResult.Value.Participants)
        {
            var participant = team.Participants.SingleOrDefault(p =>
                p.ParticipantNumber == participantModel.ParticipantNumber && p.SchoolNumber == participantModel.SchoolNumber);

            Assert.NotNull(participant);
        }

        Assert.Equal(team.SchoolNumber, actionResult.Value.SchoolNumber);
        Assert.Equal(team.TeamNumber, actionResult.Value.TeamNumber);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Ok_For_Participant_Role()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var team = teamsTestData.First(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)[0] as Team;

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService.Setup(teamsService => teamsService.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(team);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns(team!.Participants[0].ParticipantId);

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var teamsController = new TeamsController(mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await teamsController.Get("64639f6fcdde06187b09ecae", default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(team.Id, actionResult.Value!.Id);
        Assert.Equal(team.Participants.Count, actionResult.Value.Participants.Count);

        foreach (var participantModel in actionResult.Value.Participants)
        {
            var participant = team.Participants.SingleOrDefault(p =>
                p.ParticipantNumber == participantModel.ParticipantNumber && p.SchoolNumber == participantModel.SchoolNumber);

            Assert.NotNull(participant);
        }

        Assert.Equal(team.SchoolNumber, actionResult.Value.SchoolNumber);
        Assert.Equal(team.TeamNumber, actionResult.Value.TeamNumber);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Empty_For_Judge()
    {
        // Arrange
        var emptyTeamsList = new List<Team>();

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService.Setup(teamsService => teamsService.GetAsync(default))
            .ReturnsAsync(emptyTeamsList);

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var teamsController = new TeamsController(mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await teamsController.Get(default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value!);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Empty_For_Participant()
    {
        // Arrange
        var emptyTeamsList = new List<Team>();

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService.Setup(teamsService => teamsService.GetAsync(default))
            .ReturnsAsync(emptyTeamsList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var teamsController = new TeamsController(mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await teamsController.Get(default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value!);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Not_Empty_For_Judge()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var teamsList = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .ToList();

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService.Setup(teamsService => teamsService.GetAsync(default))
            .ReturnsAsync(teamsList);

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var teamsController = new TeamsController(mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await teamsController.Get(default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(teamsList.Count, actionResult.Value!.Count);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Not_Empty_For_Participant()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var teamsList = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .ToList();

        var team = teamsList.First();

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService.Setup(teamsService => teamsService.GetAsync(default))
            .ReturnsAsync(teamsList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns(team!.Participants[0].ParticipantId);

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var teamsController = new TeamsController(mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await teamsController.Get(default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Single(actionResult.Value!);

        var actualTeam = actionResult.Value![0];

        Assert.Equal(team.Id, actualTeam.Id);
        Assert.Equal(team.SchoolNumber, actualTeam.SchoolNumber);
        Assert.Equal(team.TeamNumber, actualTeam.TeamNumber);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Created()
    {
        // Arrange
        var newTeam = new TeamModel
        {
            CompetitionLevel = "MiddleSchool",
            Id = "12345",
            Participants = new[] { new ParticipantModel { ParticipantNumber = "123", SchoolNumber = "1234" } },
            SchoolNumber = "1234",
            TeamNumber = "901"
        };

        Func<Team, bool> validateTeam = teamToValidate =>
        {
            var idsMatch = teamToValidate.Id == newTeam.Id;
            var schoolNumbersMatch = teamToValidate.SchoolNumber == newTeam.SchoolNumber;
            var teamNumbersMatch = teamToValidate.TeamNumber == newTeam.TeamNumber;

            var participantsMatch = teamToValidate.Participants.Count == newTeam.Participants.Count &&
                                    teamToValidate.Participants[0].ParticipantNumber == newTeam.Participants[0].ParticipantNumber &&
                                    teamToValidate.Participants[0].SchoolNumber == newTeam.Participants[0].SchoolNumber;

            return idsMatch && participantsMatch && schoolNumbersMatch && teamNumbersMatch;
        };

        var mockedTeamsService = new Mock<ITeamsService>();

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var createdAtActionResult = await teamsController.Post(newTeam, default);

        // Assert
        Assert.NotNull(createdAtActionResult);

        Assert.IsType<TeamModel>(createdAtActionResult.Value);

        mockedTeamsService.Verify(teamsService => teamsService.CreateAsync(It.Is<Team>(c => validateTeam(c)), default), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_No_Content()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var team = teamsTestData.First(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)[0] as Team;

        var updatedTeam = new TeamModel
        {
            CompetitionLevel = "MiddleSchool",
            Id = team!.Id,
            Participants = new[] { new ParticipantModel { ParticipantNumber = "123", SchoolNumber = "1234" } },
            SchoolNumber = "1234",
            TeamNumber = "901"
        };

        Func<Team, bool> validateTeam = teamToValidate =>
        {
            var idsMatch = teamToValidate.Id == updatedTeam.Id;
            var schoolNumbersMatch = teamToValidate.SchoolNumber == updatedTeam.SchoolNumber;
            var teamNumbersMatch = teamToValidate.TeamNumber == updatedTeam.TeamNumber;

            var participantsMatch = teamToValidate.Participants.Count == updatedTeam.Participants.Count &&
                                    teamToValidate.Participants[0].ParticipantNumber == updatedTeam.Participants[0].ParticipantNumber &&
                                    teamToValidate.Participants[0].SchoolNumber == updatedTeam.Participants[0].SchoolNumber;

            return idsMatch && participantsMatch && schoolNumbersMatch && teamNumbersMatch;
        };

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService.Setup(teamsService => teamsService.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(team);

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Put(team.Id!, updatedTeam, default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);

        mockedTeamsService.Verify(teamsService => teamsService.UpdateAsync(It.Is<Team>(c => validateTeam(c)), default), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_Not_Found()
    {
        // Arrange
        var mockedTeamsService = new Mock<ITeamsService>();

        var teamsController = new TeamsController(mockedTeamsService.Object);

        // Act
        var actionResult = await teamsController.Put("64639f6fcdde06187b09ecae", new TeamModel(), default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }
}
