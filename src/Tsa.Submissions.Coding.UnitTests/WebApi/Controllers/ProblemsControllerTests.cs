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
        var actionResult = await problemsController.Delete("64639f6fcdde06187b09ecae");

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
        var actionResult = await problemsController.Delete("64639f6fcdde06187b09ecae");

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
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae");

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
        var actionResult = await problemsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(problem!.ToModel(), actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_By_Id_Should_Return_Ok_With_Test_Sets_Expanded_For_Judge()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)[0] as Problem;

        var problemId = problem!.Id;

        var testSetsTestData = new TestSetsTestData();

        var testSetList = testSetsTestData
            .Where(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None &&
                        ((TestSet)_[0]).Problem?.Id.AsString == problemId)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .ToList();

        var expectedProblemModel = problem.ToModel();
        expectedProblemModel.TestSets = testSetList.ToModels();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.GetAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.Is(problem, new ProblemEqualityComparer()), default))
            .ReturnsAsync(testSetList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get(problemId!, true);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(expectedProblemModel, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_By_Id_Should_Return_Ok_With_Test_Sets_Expanded_For_Participant()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)[0] as Problem;

        var problemId = problem!.Id;

        var testSetsTestData = new TestSetsTestData();

        var testSetList = testSetsTestData
            .Where(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None &&
                        ((TestSet)_[0]).Problem?.Id.AsString == problemId)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .ToList();

        var expectedProblemModel = problem.ToModel();
        expectedProblemModel.TestSets = testSetList
            .Where(_ => _.IsPublic)
            .ToList()
            .ToModels();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.GetAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.Is(problem, new ProblemEqualityComparer()), default))
            .ReturnsAsync(testSetList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get(problemId!, true);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(expectedProblemModel, actionResult.Value, new ProblemModelEqualityComparer());
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
        var actionResult = await problemsController.Get();

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
        var actionResult = await problemsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(problemsList.Count, actionResult.Value!.Count);
        Assert.Equal(problemsList.ToModels(), actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_Ok_With_Test_Sets_Expanded_For_Judge()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problemsList = problemsTestData
            .Where(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)
            .Select(_ => _[0])
            .Cast<Problem>()
            .ToList();

        var testSetsTestData = new TestSetsTestData();

        var testSetList = testSetsTestData
            .Where(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .ToList();

        var expectedProblemModels = problemsList.ToModels();

        foreach (var expectedProblemModel in expectedProblemModels)
        {
            expectedProblemModel.TestSets = testSetList
                .ToModels()
                .Where(_ => _.ProblemId == expectedProblemModel.Id)
                .ToList();
        }

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(_ => _.GetAsync(default))
            .ReturnsAsync(problemsList);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        foreach (var problem in problemsList)
        {
            var testSets = testSetList.Where(_ => _.Problem?.Id == problem.Id).ToList();

            mockedTestSetsService
                .Setup(_ => _.GetAsync(It.Is(problem, new ProblemEqualityComparer()), default))
                .ReturnsAsync(testSets);
        }

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get(true);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(expectedProblemModels.Count, actionResult.Value!.Count);
        Assert.Equal(expectedProblemModels, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_Ok_With_Test_Sets_Expanded_For_Participant()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problemsList = problemsTestData
            .Where(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)
            .Select(_ => _[0])
            .Cast<Problem>()
            .ToList();

        var testSetsTestData = new TestSetsTestData();

        var testSetList = testSetsTestData
            .Where(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .ToList();

        var expectedProblemModels = problemsList.ToModels();

        foreach (var expectedProblemModel in expectedProblemModels)
        {
            expectedProblemModel.TestSets = testSetList
                .ToModels()
                .Where(_ => _.ProblemId == expectedProblemModel.Id && _.IsPublic)
                .ToList();
        }

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(_ => _.GetAsync(default))
            .ReturnsAsync(problemsList);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        foreach (var problem in problemsList)
        {
            var testSets = testSetList.Where(_ => _.Problem?.Id == problem.Id).ToList();

            mockedTestSetsService
                .Setup(_ => _.GetAsync(It.Is(problem, new ProblemEqualityComparer()), default))
                .ReturnsAsync(testSets);
        }

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.Get(true);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(expectedProblemModels.Count, actionResult.Value!.Count);
        Assert.Equal(expectedProblemModels, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetTestSets_Should_Return_Not_Found()
    {
        // Arrange
        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.GetTestSets("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetTestSets_Should_Return_Ok_When_Empty_For_Judge()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var problemId = ((TestSet)testSetsTestData.First(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)[0]).Problem?.Id.AsString;

        var problem = new Problem { Id = problemId };

        var testSetList = new List<TestSet>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.GetAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.Is(problem, new ProblemEqualityComparer()), default))
            .ReturnsAsync(testSetList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.GetTestSets(problemId!);

        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value!);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetTestSets_Should_Return_Ok_When_Empty_For_Participant()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var problemId = ((TestSet)testSetsTestData.First(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)[0]).Problem?.Id.AsString;

        var problem = new Problem { Id = problemId };

        var testSetList = new List<TestSet>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.GetAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.Is(problem, new ProblemEqualityComparer()), default))
            .ReturnsAsync(testSetList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.GetTestSets(problemId!);

        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value!);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetTestSets_Should_Return_Ok_When_Not_Empty_For_Judge()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var problemId = ((TestSet)testSetsTestData.First(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)[0]).Problem?.Id.AsString;

        var problem = new Problem { Id = problemId };

        var testSetList = testSetsTestData
            .Where(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None &&
                        ((TestSet)_[0]).Problem?.Id.AsString == problemId)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .ToList();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.GetAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.Is(problem, new ProblemEqualityComparer()), default))
            .ReturnsAsync(testSetList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.GetTestSets(problemId!);

        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(testSetList.Count, actionResult.Value!.Count);
        Assert.Equal(testSetList.ToModels(), actionResult.Value, new TestSetModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetTestSets_Should_Return_Ok_When_Not_Empty_For_Participant()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var problemId = ((TestSet)testSetsTestData.First(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None)[0]).Problem?.Id.AsString;

        var problem = new Problem { Id = problemId };

        var testSetList = testSetsTestData
            .Where(_ => (TestSetDataIssues)_[1] == TestSetDataIssues.None &&
                        ((TestSet)_[0]).Problem?.Id.AsString == problemId)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .ToList();

        var participantTestSetList = testSetList.Where(_ => _.IsPublic).ToList();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(_ => _.GetAsync(It.Is(problemId, new StringEqualityComparer())!, default))
            .ReturnsAsync(problem);

        var mockedTestSetsService = new Mock<ITestSetsService>();
        mockedTestSetsService.Setup(_ => _.GetAsync(It.Is(problem, new ProblemEqualityComparer()), default))
            .ReturnsAsync(testSetList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = await problemsController.GetTestSets(problemId!);

        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(participantTestSetList.Count, actionResult.Value!.Count);
        Assert.Equal(participantTestSetList.ToModels(), actionResult.Value, new TestSetModelEqualityComparer());
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

        var mockedProblemsService = new Mock<IProblemsService>();

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var createdAtActionResult = await problemsController.Post(newProblem);

        // Assert
        Assert.NotNull(createdAtActionResult);

        Assert.IsType<ProblemModel>(createdAtActionResult.Value);

        mockedProblemsService.Verify(_ => _.CreateAsync(It.Is(newProblem.ToEntity(), new ProblemEqualityComparer()), default), Times.Once);
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

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(_ => _.GetAsync(It.Is(problem!.Id, new StringEqualityComparer())!, default)).ReturnsAsync(problem);

        var mockedTestSetsService = new Mock<ITestSetsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object, mockedTestSetsService.Object);

        // Act
        var actionResult = await problemsController.Put(problem!.Id!, updatedProblem);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);

        mockedProblemsService.Verify(_ => _.UpdateAsync(It.Is(updatedProblem.ToEntity(), new ProblemEqualityComparer()), default), Times.Once);
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
        var actionResult = await problemsController.Put("64639f6fcdde06187b09ecae", new ProblemModel());

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }
}
