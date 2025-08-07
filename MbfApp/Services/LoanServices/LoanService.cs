using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Data.Enums;
using MbfApp.Dtos.Loan;

namespace MbfApp.Services;

public interface ILoanService
{
    public Task CreateNewLoan(LoanRequestDto loanRequest);
    public Task<Loan> EditNonSanctionedLoan(LoanRequestDto loanRequest);
}
public class LoanService : ILoanService
{
    private AppDbContext _context;
    public LoanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateNewLoan(LoanRequestDto request)
    {
        var loan = new Loan
        {
            ApplicationDate = DateTime.Now,
            AppliedAmt = request.AppliedAmt,
            Status = LoanStatus.NEW,
            MemberId = 1,
        };
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync(); ;
    }

    public Task<Loan> EditNonSanctionedLoan(LoanRequestDto request)
    {
        var loan = _context.Loans.FirstOrDefault(l => l.Id == request.Id && l.Status == LoanStatus.NEW);
        throw new NotImplementedException();
    }
}
