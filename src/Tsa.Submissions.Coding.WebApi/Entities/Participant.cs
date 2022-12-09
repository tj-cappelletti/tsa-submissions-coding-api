namespace Tsa.Submissions.Coding.WebApi.Models;

public class Participant
{
    public string ParticipantId => $"{SchoolNumber}-{ParticipantNumber}";

    public string ParticipantNumber { get; set; }

    public string SchoolNumber { get; set; }
}
