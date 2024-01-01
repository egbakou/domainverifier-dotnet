using System;

namespace DomainOwnershipSample.Entities;

public class Domain
{
    public string DomainId { get; set; }
    public string DomainName { get; set; }
    public string VerificationCode { get; set; }
    public DateTime VerificationDate { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerificationCompletedDate { get; set; }
    public string UserId { get; set; }
    public  User User { get; set; }
}