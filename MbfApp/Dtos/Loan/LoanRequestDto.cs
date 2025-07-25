using System.ComponentModel.DataAnnotations;

namespace MbfApp.Dtos.Loan;

public class LoanRequestDto
{
    public int Id { get; set; }
    [Required]
    public decimal AppliedAmt { get; set; }

    [StringLength(120)]
    public string? Remarks { get; set; }
}

