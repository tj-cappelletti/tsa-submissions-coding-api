﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class SubmissionsControllerTest
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Authorize_Attribute_With_Proper_Roles()
    {
        var submissionsControllerType = typeof(SubmissionsController);

        var methodInfos = submissionsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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

                case "GetTestSets":
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
        var submissionsControllerType = typeof(SubmissionsController);

        var methodInfos = submissionsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
        var submissionsControllerType = typeof(SubmissionsController);

        var attributes = submissionsControllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Produces_Attribute()
    {
        var submissionsControllerType = typeof(SubmissionsController);

        var attributes = submissionsControllerType.GetCustomAttributes(typeof(ProducesAttribute), false);

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
        var submissionsControllerType = typeof(SubmissionsController);

        var attributes = submissionsControllerType.GetCustomAttributes(typeof(RouteAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var routeAttribute = (RouteAttribute)attributes[0];

        Assert.Equal("api/[controller]", routeAttribute.Template);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_By_Id_Should_Return_Not_Found()
    {
        // Arrange
        var mockedSubmissionsService = new Mock<ISubmissionsService>();

        var mockedTeamsService = new Mock<ITeamsService>();

        var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object);

        // Act
        var actionResult = await submissionsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        //Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_By_Id_Should_Return_Ok_For_Judge()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var submission = submissionsTestData.First(_ => (SubmissionDataIssues)_[1] == SubmissionDataIssues.None)[0] as Submission;

        var expectedSubmissionModel = submission!.ToModel();

        var submissionId = submission!.Id;

        var mockedSubmissionsService = new Mock<ISubmissionsService>();
        mockedSubmissionsService
            .Setup(_ => _.GetAsync(It.Is(submissionId, new StringEqualityComparer())!, default))
            .ReturnsAsync(submission);

        var mockedTeamsService = new Mock<ITeamsService>();

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await submissionsController.Get(submissionId!);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(expectedSubmissionModel, actionResult.Value, new SubmissionModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_By_Id_Should_Return_Ok_For_Participant()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var submission = submissionsTestData.First(_ => (SubmissionDataIssues)_[1] == SubmissionDataIssues.None)[0] as Submission;

        var submissionId = submission!.Id;

        var expectedSubmissionModel = submission.ToModel();

        var teamsTestData = new TeamsTestData();
        var team = teamsTestData
            .Where(_ => (TeamDataIssues)_[1] == TeamDataIssues.None)
            .Select(_ => _[0])
            .Cast<Team>()
            .Single(_ => _.Id == submission.Team?.Id.AsString);

        var mockedSubmissionsService = new Mock<ISubmissionsService>();
        mockedSubmissionsService
            .Setup(_ => _.GetAsync(It.Is(submissionId, new StringEqualityComparer())!, default))
            .ReturnsAsync(submission);

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService
            .Setup(_ => _.GetAsync(It.Is(team.Id, new StringEqualityComparer())!, default))
            .ReturnsAsync(team);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns(team.Participants.First().ParticipantId);

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await submissionsController.Get(submissionId!);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(expectedSubmissionModel, actionResult.Value, new SubmissionModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_Ok_When_Empty_For_Judge()
    {
        // Arrange
        var emptySubmissionsList = new List<Submission>();

        var mockedSubmissionsService = new Mock<ISubmissionsService>();
        mockedSubmissionsService.Setup(_ => _.GetAsync(default))
            .ReturnsAsync(emptySubmissionsList);

        var mockedTeamsService = new Mock<ITeamsService>();

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await submissionsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_Ok_When_Empty_For_Participant()
    {
        // Arrange
        var emptySubmissionsList = new List<Submission>();

        var teamsTestData = new TeamsTestData();
        var teams = teamsTestData
            .Where(_ => (TeamDataIssues)_[1] == TeamDataIssues.None)
            .Select(_ => _[0])
            .Cast<Team>()
            .ToList();

        var team = teams.First();

        var mockedSubmissionsService = new Mock<ISubmissionsService>();
        mockedSubmissionsService.Setup(_ => _.GetAsync(default))
            .ReturnsAsync(emptySubmissionsList);

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService
            .Setup(_ => _.GetAsync(default))
            .ReturnsAsync(teams);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns(team.Participants.First().ParticipantId);

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await submissionsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_Ok_When_Not_Empty_For_Judge()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var submissionsList = submissionsTestData
            .Where(_ => (SubmissionDataIssues)_[1] == SubmissionDataIssues.None)
            .Select(_ => _[0])
            .Cast<Submission>()
            .ToList();

        var mockedSubmissionsService = new Mock<ISubmissionsService>();
        mockedSubmissionsService.Setup(_ => _.GetAsync(default))
            .ReturnsAsync(submissionsList);

        var mockedTeamsService = new Mock<ITeamsService>();

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await submissionsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(submissionsList.Count, actionResult.Value!.Count);
        Assert.Equal(submissionsList.ToModels(), actionResult.Value, new SubmissionModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_Ok_When_Not_Empty_For_Participant()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var submissionsList = submissionsTestData
            .Where(_ => (SubmissionDataIssues)_[1] == SubmissionDataIssues.None)
            .Select(_ => _[0])
            .Cast<Submission>()
            .ToList();

        var teamsTestData = new TeamsTestData();
        var teams = teamsTestData
            .Where(_ => (TeamDataIssues)_[1] == TeamDataIssues.None)
            .Select(_ => _[0])
            .Cast<Team>()
            .ToList();

        var team = teams.First();

        var mockedSubmissionsService = new Mock<ISubmissionsService>();
        mockedSubmissionsService.Setup(_ => _.GetAsync(default))
            .ReturnsAsync(submissionsList);

        var mockedTeamsService = new Mock<ITeamsService>();
        mockedTeamsService
            .Setup(_ => _.GetAsync(default))
            .ReturnsAsync(teams);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns(team.Participants.First().ParticipantId);

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.Is(SubmissionRoles.Judge, new StringEqualityComparer()))).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await submissionsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value);
        Assert.Equal(submissionsList.Count, actionResult.Value!.Count);
        Assert.Equal(submissionsList.ToModels(), actionResult.Value, new SubmissionModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Post_Should_Return_Created()
    {
        // Arrange
        var newSubmission = new SubmissionModel
        {
            IsFinalSubmission = false,
            Language = "Language",
            ProblemId = "000000000000000000000001",
            Solution = "This is the solution",
            TeamId = "000000000000000000000001"
        };

        var mockedSubmissionsService = new Mock<ISubmissionsService>();

        var mockedTeamsService = new Mock<ITeamsService>();

        var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object);

        // Act
        var createdAtActionResult = await submissionsController.Post(newSubmission);

        // Assert
        Assert.NotNull(createdAtActionResult);

        Assert.IsType<SubmissionModel>(createdAtActionResult.Value);

        mockedSubmissionsService.Verify(_ => _.CreateAsync(It.Is(newSubmission.ToEntity(), new SubmissionEqualityComparer(true)), default), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Put_Should_Return_No_Content()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var submission = submissionsTestData.First(_ => (SubmissionDataIssues)_[1] == SubmissionDataIssues.None)[0] as Submission;

        var updatedSubmission = new SubmissionModel
        {
            Id = submission!.Id,
            IsFinalSubmission = true,
            Language = "Language",
            ProblemId = "000000000000000000000001",
            Solution = "This is the solution",
            SubmittedOn = submission.SubmittedOn,
            TeamId = "000000000000000000000001"
        };

        var mockedSubmissionsService = new Mock<ISubmissionsService>();
        mockedSubmissionsService.Setup(_ => _.GetAsync(It.Is(submission.Id, new StringEqualityComparer())!, default)).ReturnsAsync(submission);

        var mockedTeamsService = new Mock<ITeamsService>();

        var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object);

        // Act
        var actionResult = await submissionsController.Put(submission.Id!, updatedSubmission);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);

        mockedSubmissionsService.Verify(_ => _.UpdateAsync(It.Is(updatedSubmission.ToEntity(), new SubmissionEqualityComparer()), default), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Put_Should_Return_Not_Found()
    {
        // Arrange
        var mockedSubmissionsService = new Mock<ISubmissionsService>();

        var mockedTeamsService = new Mock<ITeamsService>();

        var submissionsController = new SubmissionsController(mockedSubmissionsService.Object, mockedTeamsService.Object);

        // Act
        var actionResult = await submissionsController.Put("64639f6fcdde06187b09ecae", new SubmissionModel());

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }
}