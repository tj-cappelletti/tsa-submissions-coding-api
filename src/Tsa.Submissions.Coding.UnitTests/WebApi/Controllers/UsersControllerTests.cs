using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class UsersControllerTests
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
        var statusControllerType = typeof(StatusController);

        var methodInfos = statusControllerType.GetMethods(BindingFlags.DeclaredOnly);

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
    public async Task Post_Should_Return_Created()
    {
        var teamModel = new TeamModel
        {
            SchoolNumber = "9001",
            TeamNumber = "901",
        };
        _ = new UserModel
        {
            Id = "000000000000000000000002",
            Role = "participant",
            Team = teamModel,
            UserName = "9000-901"
        };

        var mockedCacheService = new Mock<ICacheService>();
        var mockedTeamsService = new Mock<ITeamsService>();
        var mockedUsersService = new Mock<IUsersService>();
        _ = new UsersController(mockedCacheService.Object, mockedTeamsService.Object, mockedUsersService.Object);
    }
}
