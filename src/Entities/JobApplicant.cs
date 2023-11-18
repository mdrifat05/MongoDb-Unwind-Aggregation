namespace Entities;

public class JobApplicant : BaseEntity
{
    public string ApplicantName { get; set; }
    public List<JobInformation> JobInformations { get; set; }
}