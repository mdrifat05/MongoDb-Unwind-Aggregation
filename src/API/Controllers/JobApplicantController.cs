using API.Helpers;
using Domain.Dto;
using Domain.Params;
using Domain.Services.Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class JobApplicantController : ControllerBase
{
    private readonly IJobApplicantService _jobApplicantService;
    private readonly ILogger<JobApplicantController> _logger;

    public JobApplicantController(ILogger<JobApplicantController> logger,
        IJobApplicantService jobApplicantService)
    {
        _logger = logger;
        _jobApplicantService = jobApplicantService;
    }

    [HttpGet("")]
    public async Task<ActionResult<Pagination<JobApplicationShortInfo>>> Get([FromQuery] JobApplicantQuery query)
    { 
        var list = await _jobApplicantService.GetJobApplicants(query);
        return Ok(list);
    }
}