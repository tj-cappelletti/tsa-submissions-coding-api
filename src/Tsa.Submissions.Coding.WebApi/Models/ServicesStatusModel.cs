namespace Tsa.Submissions.Coding.WebApi.Models
{
    public class Status
    {
        public bool TeamsServiceIsAlive { get; set; }

        public bool IsHealthy => TeamsServiceIsAlive;
    }
}
