// If the values are null, that means the app was miss configured
// If the app was miss configured, let the error bubble up

#pragma warning disable CS8618
namespace Tsa.Submissions.Coding.WebApi.Configuration;

public class SubmissionsDatabase
{
    public string DatabaseName { get; set; }
}
