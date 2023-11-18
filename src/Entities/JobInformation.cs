namespace Entities;

public class JobInformation
{
    public int JobId { get; set; }
    
    public int ApplyId { get; set; }
    public DateTime ApplyDate { get; set; }
    public int ApplicantSkillMatchingScore { get; set; }

    public bool IsRejectedInAllApplicantsStep { get; set; }

    public List<ApplicantProcessInformation> ApplicantProcessInformation { get; set; }
}
