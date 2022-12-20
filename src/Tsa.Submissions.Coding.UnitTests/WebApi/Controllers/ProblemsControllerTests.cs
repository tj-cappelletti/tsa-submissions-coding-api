using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
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
public class ProblemsControllerTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Authorize_Attribute_With_Proper_Roles()
    {
        var problemsControllerType = typeof(ProblemsController);

        var methodInfos = problemsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
        var problemsControllerType = typeof(ProblemsController);

        var methodInfos = problemsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
                case "gettestsets":
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
        var problemsControllerType = typeof(ProblemsController);

        var attributes = problemsControllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Produces_Attribute()
    {
        var problemsControllerType = typeof(ProblemsController);

        var attributes = problemsControllerType.GetCustomAttributes(typeof(ProducesAttribute), false);

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
        var problemsControllerType = typeof(ProblemsController);

        var attributes = problemsControllerType.GetCustomAttributes(typeof(RouteAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var routeAttribute = (RouteAttribute)attributes[0];

        Assert.Equal("api/[controller]", routeAttribute.Template);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Delete_Should_Return_No_Content()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)[0] as Problem;

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(_ => _.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(problem);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Delete("64639f6fcdde06187b09ecae", default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Delete_Should_Return_Not_Found()
    {
        // Arrange
        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Delete("64639f6fcdde06187b09ecae", default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_By_Id_Should_Return_Not_Found()
    {
        // Arrange
        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae", default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_By_Id_Should_Return_Ok()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)[0] as Problem;

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(_ => _.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(problem);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae", default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(problem!.Description, actionResult.Value!.Description);
        Assert.Equal(problem.Id, actionResult.Value.Id);
        Assert.Equal(problem.IsActive, actionResult.Value.IsActive);
        Assert.Equal(problem.Title, actionResult.Value.Title);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_Ok_When_Empty()
    {
        // Arrange
        var emptyProblemsList = new List<Problem>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(_ => _.GetAsync(default))
            .ReturnsAsync(emptyProblemsList);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Get(default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value!);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_Ok_When_Not_Empty()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problemsList = problemsTestData
            .Where(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)
            .Select(_ => _[0])
            .Cast<Problem>()
            .ToList();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(_ => _.GetAsync(default))
            .ReturnsAsync(problemsList);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Get(default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(problemsList.Count, actionResult.Value!.Count);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Post_Should_Return_Created()
    {
        // Arrange
        var newProblem = new ProblemModel
        {
            Description = "This is the description",
            Id = "12345",
            IsActive = true,
            Title = "This is the title"
        };

        Func<Problem, bool> validateProblem = problemToValidate =>
        {
            var descriptionsMatch = problemToValidate.Description == newProblem.Description;
            var idsMatch = problemToValidate.Id == newProblem.Id;
            var isActiveMatch = problemToValidate.IsActive == newProblem.IsActive;
            var titlesMatch = problemToValidate.Title == newProblem.Title;

            return descriptionsMatch && idsMatch && isActiveMatch && titlesMatch;
        };

        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var createdAtActionResult = await problemsController.Post(newProblem, default);

        // Assert
        Assert.NotNull(createdAtActionResult);

        Assert.IsType<ProblemModel>(createdAtActionResult.Value);

        mockedProblemsService.Verify(_ => _.CreateAsync(It.Is<Problem>(c => validateProblem(c)), default), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Put_Should_Return_No_Content()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)[0] as Problem;

        var updatedProblem = new ProblemModel
        {
            Description = "This is the description",
            Id = "12345",
            IsActive = true,
            Title = "This is the title"
        };

        Func<Problem, bool> validateProblem = problemToValidate =>
        {
            var descriptionsMatch = problemToValidate.Description == updatedProblem.Description;
            var idsMatch = problemToValidate.Id == updatedProblem.Id;
            var isActiveMatch = problemToValidate.IsActive == updatedProblem.IsActive;
            var titlesMatch = problemToValidate.Title == updatedProblem.Title;

            return descriptionsMatch && idsMatch && isActiveMatch && titlesMatch;
        };

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(_ => _.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(problem);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Put(problem!.Id!, updatedProblem, default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);

        mockedProblemsService.Verify(_ => _.UpdateAsync(It.Is<Problem>(c => validateProblem(c)), default), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Put_Should_Return_Not_Found()
    {
        // Arrange
        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Put("64639f6fcdde06187b09ecae", new ProblemModel(), default);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }
}
