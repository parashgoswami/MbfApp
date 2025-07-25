using MbfApp.Data.Enums;
using MbfApp.Validators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MbfApp.Data.Entities;

public class Withdrawal
{
    public int Id { get; set; }

    [DateNotInFuture, Column(TypeName = "date")]
    public DateTime ApplicationDate { get; set; }

    [Required]
    public decimal AppliedAmt { get; set; }
    public decimal SanctionedAmt { get; set; }

    [DateNotInFuture, Column(TypeName = "date")]
    public DateTime? SanctionDate { get; set; }
    public WithdrawalStatus Status { get; set; }

    [StringLength(120)]
    public string? Remarks { get; set; }
    public int MemberId { get; set; }
    public Member? Member { get; set; }
}
