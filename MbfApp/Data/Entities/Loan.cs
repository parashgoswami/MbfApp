using MbfApp.Data.Enums;
using MbfApp.Validators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MbfApp.Data.Entities;

public class Loan 
{
    public int Id { get; set; }
    [DateNotInFuture, Column(TypeName = "date")]
    public DateTime ApplicationDate { get; set; } = DateTime.Now;
    [Required]
    public decimal AppliedAmt { get; set; }
    public decimal SanctionedAmt { get; set; }
    [DateNotInFuture, Column(TypeName = "date")]
    public DateTime? SanctionDate { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.NEW;

    [StringLength(120)]
    public string? Remarks { get; set; }
    public int MemberId { get; set; }
    public Member? Member { get; set; }
}