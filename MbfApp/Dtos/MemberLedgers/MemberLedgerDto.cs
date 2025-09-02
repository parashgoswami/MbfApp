using MbfApp.Validators;
using System.ComponentModel.DataAnnotations;

namespace MbfApp.Dtos.MemberLedgers;

public class MemberLedgerDto
{
    [EmployeeCode]
    public string EmpCode { get; set; } = string.Empty;
    
    [YearMonth]
    public int YearMonth { get; set; }

    [Range(0, (double)decimal.MaxValue)]
    public decimal DepositCr { get; set; }

    [Range(0, (double)decimal.MaxValue)]
    public decimal LoanCr { get; set; }
}
