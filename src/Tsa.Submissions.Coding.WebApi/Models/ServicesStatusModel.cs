namespace Tsa.Submissions.Coding.WebApi.Models;

public class ServicesStatusModel
{
    public bool IsHealthy => ProblemsServiceIsAlive && TeamsServiceIsAlive;
    public bool ProblemsServiceIsAlive { get; set; }

    public bool TeamsServiceIsAlive { get; set; }
}
