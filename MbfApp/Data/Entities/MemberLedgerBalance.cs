namespace MbfApp.Data.Entities;

public class MemberLedgerBalance
{
    public int Id { get; set; }
    public int FinYearId { get; set; }
    public int MemberId { get; set; }
    public decimal OpBalDeposit { get; set; }
    public decimal OpBalLoan { get; set; }
    public decimal DepositBalance { get; set; }    
    public decimal LoanBalance { get; set; }   
    //public decimal IntDeposit { get; set; }
    public Member Member { get; set; } = default!;
    public FinYear FinYear { get; set; } = default!;
}
