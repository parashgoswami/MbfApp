using MbfApp.Data;
using MbfApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Services.VoucherNoService;
public interface IVoucherNumberService
{
    Task<string> GetNextVoucherNumberAsync();
}
public class VoucherNumberService : IVoucherNumberService
{
    private readonly AppDbContext _context;
    public VoucherNumberService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetNextVoucherNumberAsync()
    {
        var fy = FinancialYearHelper.GetFinancialYear(DateTime.Now);
        var sequence = await _context.VoucherSequences
           .SingleOrDefaultAsync(v => v.FinancialYear == fy);

        if (sequence == null)
        {
            sequence = new VoucherSequence
            {
                FinancialYear = fy,
                LastNumber = 1,
                UpdatedAt = DateTime.UtcNow
            };
            _context.VoucherSequences.Add(sequence);
        }
        else
        {
            sequence.LastNumber += 1;
            sequence.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return $"{fy}-{sequence.LastNumber:D4}";
    }
}
