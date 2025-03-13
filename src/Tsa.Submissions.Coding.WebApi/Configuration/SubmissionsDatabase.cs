namespace Tsa.Submissions.Coding.WebApi.Configuration;

public class SubmissionsDatabase
{
    public string? Host { get; set; }

    public string? LoginDatabase { get; set; }

    public string? Name { get; set; }

    public string? Password { get; set; }

    public int Port { get; set; }

    public string? Username { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Host) &&
               !string.IsNullOrWhiteSpace(LoginDatabase) &&
               !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password) &&
               Port > 0;
    }
}
