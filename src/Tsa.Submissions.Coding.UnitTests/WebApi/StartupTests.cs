using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Tsa.Submissions.Coding.WebApi;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi;

[ExcludeFromCodeCoverage]
public class StartupTests
{
    private readonly Mock<IConfiguration> _mockConfiguration = new();
    private readonly IServiceCollection _services = new ServiceCollection();

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ConfigureServices_RegistersAuthentication()
    {
        // Arrange
        var jwtSettingsSection = new Mock<IConfigurationSection>();
        jwtSettingsSection.Setup(x => x.Get<JwtSettings>()).Returns(new JwtSettings
        {
            Audience = "TestAudience",
            Issuer = "TestIssuer",
            Key = "TestKey",
            ExpirationInHours = 1
        });

        _mockConfiguration.Setup(x => x.GetSection("Jwt")).Returns(jwtSettingsSection.Object);

        var startup = new Startup(_mockConfiguration.Object);

        // Act
        startup.ConfigureServices(_services);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var authenticationSchemeProvider = serviceProvider.GetService<IAuthenticationSchemeProvider>();
        Assert.NotNull(authenticationSchemeProvider);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ConfigureServices_RegistersJwtSettings()
    {
        // Arrange
        var jwtSettingsSection = new Mock<IConfigurationSection>();
        jwtSettingsSection.Setup(x => x.Get<JwtSettings>()).Returns(new JwtSettings
        {
            Audience = "TestAudience",
            Issuer = "TestIssuer",
            Key = "TestKey",
            ExpirationInHours = 1
        });

        _mockConfiguration.Setup(x => x.GetSection("Jwt")).Returns(jwtSettingsSection.Object);

        var startup = new Startup(_mockConfiguration.Object);

        // Act
        startup.ConfigureServices(_services);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var jwtSettings = serviceProvider.GetService<IOptions<JwtSettings>>();
        Assert.NotNull(jwtSettings);
        Assert.Equal("TestAudience", jwtSettings.Value.Audience);
        Assert.Equal("TestIssuer", jwtSettings.Value.Issuer);
        Assert.Equal("TestKey", jwtSettings.Value.Key);
        Assert.Equal(1, jwtSettings.Value.ExpirationInHours);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ConfigureServices_RegistersMongoClient()
    {
        // Arrange
        var submissionsDatabaseSection = new Mock<IConfigurationSection>();
        submissionsDatabaseSection.Setup(x => x.Get<SubmissionsDatabase>()).Returns(new SubmissionsDatabase
        {
            Host = "localhost",
            Port = 27017,
            Username = "testuser",
            Password = "testpassword",
            LoginDatabase = "admin",
            Name = "testdb"
        });

        _mockConfiguration.Setup(x => x.GetSection(ConfigurationKeys.SubmissionsDatabaseSection))
            .Returns(submissionsDatabaseSection.Object);

        var startup = new Startup(_mockConfiguration.Object);

        // Act
        startup.ConfigureServices(_services);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var mongoClient = serviceProvider.GetService<IMongoClient>();
        Assert.NotNull(mongoClient);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ConfigureServices_RegistersSwagger()
    {
        // Arrange
        var startup = new Startup(_mockConfiguration.Object);

        // Act
        startup.ConfigureServices(_services);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var swaggerGenService = serviceProvider.GetService<IServiceCollection>();
        Assert.NotNull(swaggerGenService);
    }
}
