using Domain.Dto;
using Domain.Params;
using Domain.Services.Contracts;
using Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Repositories.Contracts;

namespace Domain.Services;

public class JobApplicantService : IJobApplicantService
{
    private readonly IMongoDbDataContextProvider _mongoDbDataContextProvider;
    private readonly IRepository _repository;

    public JobApplicantService(IRepository repository, IMongoDbDataContextProvider mongoDbDataContextProvider)
    {
        _repository = repository;
        _mongoDbDataContextProvider = mongoDbDataContextProvider;
    }

    public async Task<List<JobApplicationShortInfo>> GetJobApplicants(JobApplicantQuery query)
    {
        var db = _mongoDbDataContextProvider.GetTenantDataContext();
        var collection = db.GetCollection<JobApplicant>($"{nameof(JobApplicant)}s");

        var jobQuery = collection.AsQueryable()
            .SelectMany(j => j.JobInformations, (j, a) => new JobApplicationShortInfo
            {
                Id = j.ItemId,
                JobId = a.JobId,
                Score = a.ApplicantSkillMatchingScore,
                ApplicantName = j.ApplicantName,
                AppliedAt = a.ApplyDate
            })
            // .Select(g => new JobApplicationReturnDto
            // {
            //     JobId = g.Key,
            //     ApplicantId = g.Select(x => x.id)!,
            //     ApplicantName = g.Select(x => x.applicantName),
            //     Score = g.Select(x => x.score)
            // })
            .Where(g => g.JobId == query.JobId);
            
        jobQuery = query.OrderBy == 0 ? jobQuery.OrderBy(o => o.Score) : jobQuery.OrderByDescending(o => o.Score);

        return await jobQuery.ToListAsync();
    }

    public Task<List<dynamic>> GetJobApplicantsCustomFields(JobApplicantQuery query)
    {
        var db = _mongoDbDataContextProvider.GetTenantDataContext();
        var collection = db.GetCollection<JobApplicant>($"{nameof(JobApplicant)}s");

        var filterBuilder = Builders<JobApplicant>.Filter;
        var filter = filterBuilder.Empty;

        var projectBuilder = Builders<JobApplicant>.Projection;
        var projection = 
            projectBuilder.Include(x => x.JobInformations.Select(y => y.IsRejectedInAllApplicantsStep))
            .Include(x => x.JobInformations.Select(y => y.ApplicantProcessInformation
                .Select(z => z.CurrentStepLevelStatus)));
        
        var result = collection.Find(filter).Project(projection).ToListAsync();
        
        Console.WriteLine(result.Result.ToList().ToJson());

        return (dynamic) result;
    }
    
    
    private async Task<List<JobApplicant>> GetJobApplicantWithBuilders(JobApplicantQuery query)
    {
        var db = _mongoDbDataContextProvider.GetTenantDataContext();
        var collection = db.GetCollection<JobApplicant>($"{nameof(JobApplicant)}s");

        var filterBuilder = Builders<JobApplicant>.Filter;
        var filter = filterBuilder.ElemMatch(x => x.JobInformations,
            Builders<JobInformation>.Filter.Eq(x => x.JobId, query.JobId));

        var sortBuilder = Builders<JobApplicant>.Sort;
        var sort = query.OrderBy == 0 ? sortBuilder.Ascending("JobInformations.ApplicantSkillMatchingScore") : sortBuilder.Descending("JobInformations.ApplicantSkillMatchingScore");

        return await collection.Find(filter).Sort(sort).ToListAsync();
    }
}