#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class TeamsService : ITeamsService
{
    private readonly IMongoCollection<Team> _teamsCollection;

    public TeamsService(IOptions<SubmissionsDatabase> submissionsDatabaseConfiguration)
    {
        var mongoClient = new MongoClient(submissionsDatabaseConfiguration.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(submissionsDatabaseConfiguration.Value.DatabaseName);

        _teamsCollection = mongoDatabase.GetCollection<Team>(submissionsDatabaseConfiguration.Value.TeamsCollectionName);
    }

    public async Task CreateAsync(Team newTeam)
    {
        await _teamsCollection.InsertOneAsync(newTeam);
    }

    public async Task<List<Team>> GetAsync()
    {
        return await _teamsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Team?> GetAsync(string id)
    {
        return await _teamsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task RemoveAsync(string id)
    {
        await _teamsCollection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(string id, Team updatedTeam)
    {
        await _teamsCollection.ReplaceOneAsync(x => x.Id == id, updatedTeam);
    }
}
