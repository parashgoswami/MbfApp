using MbfApp.Data;
using MbfApp.Dtos.FinYear;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Services;
public interface IFinYearService
{
    public Task<FinYearResponse> getCurrentFinYear();
}

public class FinYearService(AppDbContext _context) : IFinYearService
{
    public Task<FinYearResponse> getCurrentFinYear()
    {
        var finyear = _context.FinYears
            .Where(fy => fy.IsCurrent)
            .Select(fy => new FinYearResponse
            {
                Id = fy.Id,
                FinYrLabel = fy.FinYrLabel,
                IntDeposit = fy.IntDeposit,
                IntLoan = fy.IntLoan
            })
            .FirstOrDefaultAsync();
        return finyear!;
    }
}
