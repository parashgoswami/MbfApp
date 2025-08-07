namespace MbfApp.Data.Entities;

public class MemberLedger
{
    public int Id { get; set; }
    public string EmpCode { get; set; } = string.Empty;
    public int YearMonth { get; set; }
    public decimal DepositDr { get; set; }
    public decimal DepositCr { get; set; }
    public decimal IntDeposit { get; set; }
    public decimal LoanDr { get; set; }
    public decimal LoanCr { get; set; }
    public decimal IntLoan { get; set; }
}
