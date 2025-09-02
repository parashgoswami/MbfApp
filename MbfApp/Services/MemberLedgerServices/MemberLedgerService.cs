using CsvHelper;
using CsvHelper.Configuration;
using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Dtos.MemberLedgers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MbfApp.Services;

public class MemberLedgerService : IMemberLedgerService
{
    private readonly AppDbContext _context;
    private readonly IVoucherNumberService _voucherService;
    private readonly IFinYearService _finYearService;
    public MemberLedgerService(AppDbContext context, IVoucherNumberService voucherService, IFinYearService finYearService)
    {
        _context = context;
        _voucherService = voucherService;
        _finYearService = finYearService;
    }

    public async Task ImportFromCsvAsync(Stream csvStream, string narration)
    {
        List<MemberLedgerDto> records = await ReadMemberLedgerFromCsv(csvStream);

        // if records is empty, return
        if (records.Count == 0) return;

        decimal totalLoanCr = records.Sum(r => r.LoanCr);
        decimal totalDepositCr = records.Sum(r => r.DepositCr);

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
                    _context.MemberLedgers.Add(new MemberLedger
                    {
                        EmpCode = record.EmpCode,
                        YearMonth = record.YearMonth,
                        DepositCr = record.DepositCr,
                        LoanCr = record.LoanCr
                    });
                }
                else
                {
                    var existing = await _context.MemberLedgers.FirstAsync(m => m.EmpCode == record.EmpCode && m.YearMonth == record.YearMonth);

                    existing.DepositCr = record.DepositCr;
                    existing.LoanCr = record.LoanCr;
                }
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
                        AccountId = EntityConstants.DepositAccountId, // Deposit Account Id
                        DbAmt = 0,
                        CrAmt = totalDepositCr,
                        Note = "Total Deposit Cr from CSV"
                    },
                    new JournalLine
                    {
                        AccountId = EntityConstants.LoanAccountId, // Loan Account Id
                        DbAmt = 0,
                        CrAmt = totalLoanCr,
                        Note = "Total Loan Cr from CSV"
                    },
                    new JournalLine
                    {
                        AccountId = EntityConstants.BankAccountId, // Bank Account Id
                        DbAmt = 0,
                        CrAmt = totalDepositCr + totalLoanCr,
                        Note = "Total from CSV upload"
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

    private async Task<List<MemberLedgerDto>> ReadMemberLedgerFromCsv(Stream csvStream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true
        };

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, config);

        // Try to read the first row
        if (!await csv.ReadAsync() || csv.Parser.Record == null)
        {
            throw new MemberLedgerException("CSV file is empty.");
        }

        // Try to read header
        csv.ReadHeader();

        // Validate that required headers exist
        var headers = csv.HeaderRecord ?? Array.Empty<string>();
        var requiredHeaders = new[] { "EmpCode", "YearMonth", "DepositCr", "LoanCr" };

        var missingHeaders = requiredHeaders.Except(headers, StringComparer.OrdinalIgnoreCase).ToList();
        if (missingHeaders.Any())
        {
            throw new MemberLedgerException(
                $"CSV file is missing required headers: {string.Join(", ", missingHeaders)}");
        }

        var records = new List<MemberLedgerDto>();
        var errorRows = new List<MemberLedgerErrorDto>();

        while (await csv.ReadAsync())
        {
            string? errorMessage = null;
            MemberLedgerDto? record = null;

            try
            {
                record = csv.GetRecord<MemberLedgerDto>();
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to parse row. Details: {ex.Message}";
            }

            if (record is not null)
            {
                var validationContext = new ValidationContext(record);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(record, validationContext, results, true))
                {
                    var validationErrors = string.Join("; ", results.Select(r => r.ErrorMessage));
                    errorMessage = string.IsNullOrEmpty(errorMessage)
                        ? validationErrors
                        : $"{errorMessage}; {validationErrors}";
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                // Capture the row with all errors merged
                errorRows.Add(new MemberLedgerErrorDto
                {
                    EmpCode = record?.EmpCode ?? csv.GetField("EmpCode") ?? string.Empty,
                    YearMonth = record?.YearMonth.ToString() ?? csv.GetField("YearMonth") ?? string.Empty,
                    DepositCr = record?.DepositCr.ToString() ?? csv.GetField("DepositCr") ?? string.Empty,
                    LoanCr = record?.LoanCr.ToString() ?? csv.GetField("LoanCr") ?? string.Empty,
                    Error = errorMessage
                });
            }
            else if (record is not null)
            {
                records.Add(record);
            }
        }

        if (errorRows.Any())
        {
            throw new MemberLedgerException("Errors found in CSV file.", errorRows);
        }

        return records;
    }
}