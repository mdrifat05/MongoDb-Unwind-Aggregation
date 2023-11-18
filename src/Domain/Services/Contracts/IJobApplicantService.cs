using Domain.Dto;
using Domain.Params;
using Entities;

namespace Domain.Services.Contracts;

public interface IJobApplicantService
{
    Task<List<JobApplicationShortInfo>> GetJobApplicants(JobApplicantQuery genericParams);
    Task<List<dynamic>> GetJobApplicantsCustomFields(JobApplicantQuery query);
}