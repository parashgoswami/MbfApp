namespace MbfApp.Dtos.MemberLedgers;

public class MemberLedgerErrorDto
{
    public string EmpCode { get; set; } = string.Empty;
    public string YearMonth { get; set; } = string.Empty;
    public string DepositCr { get; set; } = string.Empty;
    public string LoanCr { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}
