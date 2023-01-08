using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class StatusControllerTests
{
    private static void AssertServiceStatus(ServicesStatusModel servicesStatusModel, PingableServiceFailures pingableServiceFailures)
    {
        Assert.Equal(servicesStatusModel.ProblemsServiceIsAlive, !pingableServiceFailures.HasFlag(PingableServiceFailures.Problems));
        Assert.Equal(servicesStatusModel.TeamsServiceIsAlive, !pingableServiceFailures.HasFlag(PingableServiceFailures.Teams));
    }

    private static IList<IPingableService> BuildHealthyPingableServices()
    {
        var pingableServices = new List<IPingableService>();

        foreach (var serviceType in GetServiceTypes())
        {
            var mockedPingableService = new Mock<IPingableService>();

            mockedPingableService
                .Setup(_ => _.PingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            mockedPingableService
                .Setup(_ => _.ServiceName)
                .Returns(serviceType.Name.Replace("Service", string.Empty));

            pingableServices.Add(mockedPingableService.Object);
        }

        return pingableServices;
    }

    private static IList<IPingableService> BuildUnhealthyPingableServices(PingableServiceFailures pingableServiceFailures,
        ServiceFailureType serviceFailureType)
    {
        var pingableServices = new List<IPingableService>();

        foreach (var serviceType in GetServiceTypes())
        {
            var serviceName = serviceType.Name.Replace("Service", string.Empty);

            var pingableServiceFailureFlag = Enum.Parse<PingableServiceFailures>(serviceName);

            var mockedPingableService = new Mock<IPingableService>();

            mockedPingableService
                .Setup(_ => _.ServiceName)
                .Returns(serviceName);

            if (pingableServiceFailures.HasFlag(pingableServiceFailureFlag))
            {
                switch (serviceFailureType)
                {
                    case ServiceFailureType.ExceptionThrown:
                        mockedPingableService
                            .Setup(_ => _.PingAsync(It.IsAny<CancellationToken>()))
                            .Throws(new Exception("Test handling exceptions"));
                        break;

                    case ServiceFailureType.PingFailed:
                        mockedPingableService.Setup(_ => _.PingAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(false);
                        break;
                }
            }
            else
            {
                mockedPingableService
                    .Setup(_ => _.PingAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);
            }

            pingableServices.Add(mockedPingableService.Object);
        }

        return pingableServices;
    }

    [Theory]
    // Use this tool to generate the permutations
    // https://www.easyunitconverter.com/permutation-calculator

    #region Inline Data

    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.Teams | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.Teams)]
    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Teams | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Problems)]
    [InlineData(PingableServiceFailures.Teams)]
    [InlineData(PingableServiceFailures.TestSets)]

    #endregion

    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_InternalServerError(PingableServiceFailures pingableServiceFailures)
    {
        // Arrange
        var pingableServices = BuildUnhealthyPingableServices(pingableServiceFailures, ServiceFailureType.PingFailed);

        var statusController = new StatusController(pingableServices);

        // Act
        var actionResult = await statusController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<ServicesStatusModel>>(actionResult);

        var objectResult = actionResult.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.NotNull(objectResult.StatusCode);
        Assert.Equal(500, objectResult.StatusCode!.Value);
        Assert.NotNull(objectResult.Value);
        Assert.IsType<ServicesStatusModel>(objectResult.Value);

        var servicesStatus = objectResult.Value as ServicesStatusModel;
        Assert.False(servicesStatus!.IsHealthy);

        AssertServiceStatus(servicesStatus, pingableServiceFailures);
    }

    [Theory]

    #region Inline Data

    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.Teams | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.Teams)]
    [InlineData(PingableServiceFailures.Problems | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Teams | PingableServiceFailures.TestSets)]
    [InlineData(PingableServiceFailures.Problems)]
    [InlineData(PingableServiceFailures.Teams)]
    [InlineData(PingableServiceFailures.TestSets)]

    #endregion

    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_InternalServerError_When_Exception_Is_Thrown(PingableServiceFailures pingableServiceFailures)
    {
        // Arrange
        var pingableServices = BuildUnhealthyPingableServices(pingableServiceFailures, ServiceFailureType.ExceptionThrown);

        var statusController = new StatusController(pingableServices);

        // Act
        var actionResult = await statusController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<ServicesStatusModel>>(actionResult);

        var objectResult = actionResult.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.NotNull(objectResult.StatusCode);
        Assert.Equal(500, objectResult.StatusCode!.Value);
        Assert.NotNull(objectResult.Value);
        Assert.IsType<ServicesStatusModel>(objectResult.Value);

        var servicesStatus = objectResult.Value as ServicesStatusModel;

        Assert.False(servicesStatus!.IsHealthy);

        AssertServiceStatus(servicesStatus, pingableServiceFailures);
    }

    private static Type[] GetServiceTypes()
    {
        return new[] { typeof(ProblemsService), typeof(TeamsService), typeof(TestSetsService) };
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
    public void Controller_Should_Have_ApiController_Attribute()
    {
        var statusControllerType = typeof(StatusController);

        var attributes = statusControllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Produces_Attribute()
    {
        var statusControllerType = typeof(StatusController);

        var attributes = statusControllerType.GetCustomAttributes(typeof(ProducesAttribute), false);

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
        var statusControllerType = typeof(StatusController);

        var attributes = statusControllerType.GetCustomAttributes(typeof(RouteAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var routeAttribute = (RouteAttribute)attributes[0];

        Assert.Equal("api/[controller]", routeAttribute.Template);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_Ok()
    {
        // Arrange
        var pingableServices = BuildHealthyPingableServices();

        var statusController = new StatusController(pingableServices);

        // Act
        var actionResult = await statusController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<ServicesStatusModel>>(actionResult);

        var okObjectResult = actionResult.Result as ObjectResult;
        Assert.NotNull(okObjectResult);
        Assert.NotNull(okObjectResult.StatusCode);
        Assert.Equal(200, okObjectResult.StatusCode!.Value);
        Assert.NotNull(okObjectResult.Value);
        Assert.IsType<ServicesStatusModel>(okObjectResult.Value);

        var servicesStatus = okObjectResult.Value as ServicesStatusModel;
        Assert.True(servicesStatus!.IsHealthy);
        Assert.True(servicesStatus.ProblemsServiceIsAlive);
        Assert.True(servicesStatus.TeamsServiceIsAlive);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Get_Should_Return_Ok_When_Unknown_Service_Is_Injected()
    {
        // Arrange
        var pingableServices = BuildHealthyPingableServices();

        var mockedUnknownService = new Mock<IPingableService>();
        mockedUnknownService
            .Setup(_ => _.PingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        mockedUnknownService
            .Setup(_ => _.ServiceName)
            .Returns("Bomb");

        pingableServices.Add(mockedUnknownService.Object);

        var statusController = new StatusController(pingableServices);

        // Act
        var actionResult = await statusController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<ServicesStatusModel>>(actionResult);

        var okObjectResult = actionResult.Result as ObjectResult;
        Assert.NotNull(okObjectResult);
        Assert.NotNull(okObjectResult.StatusCode);
        Assert.Equal(200, okObjectResult.StatusCode!.Value);
        Assert.NotNull(okObjectResult.Value);
        Assert.IsType<ServicesStatusModel>(okObjectResult.Value);

        var servicesStatus = okObjectResult.Value as ServicesStatusModel;
        Assert.True(servicesStatus!.IsHealthy);
        Assert.True(servicesStatus.TeamsServiceIsAlive);
    }
}

[Flags]
public enum PingableServiceFailures
{
    None = 0,
    Problems = 1 << 0,
    Teams = 1 << 1,
    TestSets = 1 << 2
}

public enum ServiceFailureType
{
    None,
    ExceptionThrown,
    PingFailed
}
