using CsvHelper;
using CsvHelper.Configuration;
using MbfApp.Data;
using MbfApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MbfApp.Services;
public class MemberLedgerService : IMemberLedgerService
{
    private readonly AppDbContext _context;

    public MemberLedgerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task ImportFromCsvAsync(Stream csvStream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true
        };

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, config);

        // Register mapping so Id is ignored
        csv.Context.RegisterClassMap<MemberLedgerMap>();


        var records = new List<MemberLedger>();

        await foreach (var record in csv.GetRecordsAsync<MemberLedger>())
        {
            records.Add(record);
        }

        foreach (var record in records)
        {
            // Ensure EmpCode + YearMonth is unique
            bool exists = await _context.MemberLedgers
                .AnyAsync(m => m.EmpCode == record.EmpCode && m.YearMonth == record.YearMonth);

            if (!exists)
            {
                _context.MemberLedgers.Add(record);
            }
            else
            {
                var existing = await _context.MemberLedgers.FirstAsync(m => m.EmpCode == record.EmpCode && m.YearMonth == record.YearMonth);
                //existing.DepositDr = record.DepositDr;
                existing.DepositCr = record.DepositCr;
               // existing.IntDeposit = record.IntDeposit;
               // existing.LoanDr = record.LoanDr;
                existing.LoanCr = record.LoanCr;
                //existing.IntLoan = record.IntLoan;
            }
        }

        await _context.SaveChangesAsync();
    }
}
