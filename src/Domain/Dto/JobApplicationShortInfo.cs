namespace Domain.Dto;

public class JobApplicationShortInfo
{
   public string? Id { get; set; }
   public int JobId { get; set; }
   public string? ApplicantName { get; set; }
   public int Score { get; set; }
   public DateTime AppliedAt { get; set; }
}