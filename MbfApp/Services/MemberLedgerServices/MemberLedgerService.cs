using CsvHelper;
using CsvHelper.Configuration;
using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Services.VoucherNoService;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MbfApp.Services;
public class MemberLedgerService : IMemberLedgerService
{
    private readonly AppDbContext _context;
    private readonly IVoucherNumberService _voucherService;
    private readonly IFinYearService _finYearService;
    public MemberLedgerService(AppDbContext context, IVoucherNumberService voucherNumberService, IFinYearService finYearService)
    {
        _context = context;
        _voucherService = voucherNumberService;
        _finYearService = finYearService;
    }

    public async Task ImportFromCsvAsync(Stream csvStream, string narration)
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
        var totalDepositCr = 0m;
        var totalLoanCr = 0m;
        await foreach (var record in csv.GetRecordsAsync<MemberLedger>())
        {
            records.Add(record);
        }
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
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

                    existing.DepositCr = record.DepositCr;
                    existing.LoanCr = record.LoanCr;
                }
                totalDepositCr += record.DepositCr;
                totalLoanCr += record.LoanCr;
            }
            var finYear = await _finYearService.getCurrentFinYear();
            var journal = new Journal
            {
                JvNo = await _voucherService.GetNextVoucherNumberAsync(),
                FinYearId = finYear.Id,
                JournalDate = DateTime.UtcNow,
                Narration = narration.Trim(),
                Lines = new List<JournalLine>
            {
                new JournalLine
                {
                    AccountId = 1, // Deposit Account Id
                    DbAmt = 0,
                    CrAmt = totalDepositCr,
                    Note = "Total Deposit Cr from CSV"
                },
                new JournalLine
                {
                    AccountId = 2, // Loan Account Id
                    DbAmt = 0,
                    CrAmt = totalLoanCr,
                    Note = "Total Loan Cr from CSV"
                },
                new JournalLine
                {
                    AccountId = 3, // Cash/Bank Account Id
                    CrAmt = 0,
                    DbAmt = totalDepositCr + totalLoanCr,
                    Note = "Total Bank Cr from CSV"
                }
            }
            };
            _context.Journals.Add(journal);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
