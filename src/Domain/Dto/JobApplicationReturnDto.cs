namespace Domain.Dto;

public class JobApplicationReturnDto
{
    public IEnumerable<string> ApplicantId { get; set; }
    public int JobId { get; set; }
    public IEnumerable<string> ApplicantName { get; set; }
    public IEnumerable<int> Score { get; set; }
}