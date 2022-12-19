using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface ITestSetsService: IMongoEntityService<TestSet>, IPingableService { }
