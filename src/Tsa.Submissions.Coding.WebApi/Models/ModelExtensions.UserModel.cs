using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static partial class ModelExtensions
{
    public static User ToEntity(this UserModel userModel, string? passwordHash = default)
    {
        var user = new User
        {
            Id = userModel.Id,
            PasswordHash = passwordHash,
            Role = userModel.Role,
            UserName = userModel.UserName
        };

        if (userModel.Team != null)
        {
            user.Team = new MongoDBRef(TeamsService.MongoDbCollectionName, userModel.Team.Id);
        }

        return user;
    }
}
