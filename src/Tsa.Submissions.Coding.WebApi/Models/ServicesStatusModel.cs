namespace Tsa.Submissions.Coding.WebApi.Models
{
    public class ServicesStatusModel
    {
        public bool TeamsServiceIsAlive { get; set; }

        public bool IsHealthy => TeamsServiceIsAlive;
    }
}
