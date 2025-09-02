using MbfApp.Data;
using MbfApp.Dtos.FinYear;
using MbfApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System.Text;

namespace MbfApp.Tests.Unit.Services;

public class MemberLedgerServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB for each test
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task ImportFromCsvAsync_ShouldImportDataAndCreateJournal()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockVoucherService = new Mock<IVoucherNumberService>();
        var mockFinYearService = new Mock<IFinYearService>();

        var csvContent = "EmpCode,YearMonth,DepositCr,LoanCr\n" +
                            "6001,202301,1000,500\n" +
                            "6002,202301,2000,1000";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var finYear = new FinYearResponse() { Id = 1 };

        mockVoucherService.Setup(s => s.GetNextVoucherNumberAsync()).ReturnsAsync("2025-26-0001");
        mockFinYearService.Setup(s => s.getCurrentFinYear()).ReturnsAsync(finYear);

        var service = new MemberLedgerService(context, mockVoucherService.Object, mockFinYearService.Object);

        // Act
        await service.ImportFromCsvAsync(stream, "Test Import");

        // Assert
        var memberLedgers = await context.MemberLedgers.ToListAsync();
        Assert.Equal(2, memberLedgers.Count);
        Assert.Equal("6001", memberLedgers[0].EmpCode);
        Assert.Equal(1000, memberLedgers[0].DepositCr);
        Assert.Equal(500, memberLedgers[0].LoanCr);

        var journal = await context.Journals.Include(j => j.Lines).FirstOrDefaultAsync();
        Assert.NotNull(journal);
        Assert.Equal("2025-26-0001", journal.JvNo);
        Assert.Equal("Test Import", journal.Narration);
        Assert.Equal(finYear.Id, journal.FinYearId);
        Assert.Equal(3, journal.Lines.Count);

        var depositLine = journal.Lines.FirstOrDefault(l => l.AccountId == EntityConstants.DepositAccountId);
        Assert.NotNull(depositLine);
        Assert.Equal(3000, depositLine.CrAmt);

        var loanLine = journal.Lines.FirstOrDefault(l => l.AccountId == EntityConstants.LoanAccountId);
        Assert.NotNull(loanLine);
        Assert.Equal(1500, loanLine.CrAmt);

        var bankLine = journal.Lines.FirstOrDefault(l => l.AccountId == EntityConstants.BankAccountId);
        Assert.NotNull(bankLine);
        Assert.Equal(4500, bankLine.CrAmt);
    }

    [Fact]
    public async Task ImportFromCsvAsync_WithEmptyCsv_ShouldThrow()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockVoucherService = new Mock<IVoucherNumberService>();
        var mockFinYearService = new Mock<IFinYearService>();

        var stream = new MemoryStream(Encoding.UTF8.GetBytes("")); // Empty file
        var service = new MemberLedgerService(context, mockVoucherService.Object, mockFinYearService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<MemberLedgerException>(() => service.ImportFromCsvAsync(stream, "Test Import"));

        // Verify no side effects
        Assert.Empty(await context.MemberLedgers.ToListAsync());
        Assert.Null(await context.Journals.FirstOrDefaultAsync());
        mockVoucherService.Verify(s => s.GetNextVoucherNumberAsync(), Times.Never);
        mockFinYearService.Verify(s => s.getCurrentFinYear(), Times.Never);
    }

    [Fact]
    public async Task ImportFromCsvAsync_WithHeaderOnlyCsv_ShouldNotCreateJournal()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockVoucherService = new Mock<IVoucherNumberService>();
        var mockFinYearService = new Mock<IFinYearService>();

        var csvContent = "EmpCode,YearMonth,DepositCr,LoanCr"; // Header only
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        var service = new MemberLedgerService(context, mockVoucherService.Object, mockFinYearService.Object);

        // Act
        await service.ImportFromCsvAsync(stream, "Test Import");

        // Assert
        Assert.Empty(await context.MemberLedgers.ToListAsync());
        Assert.Null(await context.Journals.FirstOrDefaultAsync());
        
        mockVoucherService.Verify(s => s.GetNextVoucherNumberAsync(), Times.Never);
        mockFinYearService.Verify(s => s.getCurrentFinYear(), Times.Never);
    }
}

