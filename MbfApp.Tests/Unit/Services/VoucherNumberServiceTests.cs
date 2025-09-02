
using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Services;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Tests.Unit.Services;

public class VoucherNumberServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetNextVoucherNumberAsync_ShouldReturnFirstVoucher_WhenNoSequenceExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new VoucherNumberService(context);
        var financialYear = FinancialYearHelper.GetFinancialYear(DateTime.Now);

        // Act
        var result = await service.GetNextVoucherNumberAsync();

        // Assert      
        Assert.Equal($"{financialYear}0001", result);
        
        var sequence = await context.VoucherSequences.SingleAsync();
        Assert.Equal(financialYear, sequence.FinancialYear);
        Assert.Equal(1, sequence.LastNumber);
    }

    [Fact]
    public async Task GetNextVoucherNumberAsync_ShouldReturnNextVoucher_WhenSequenceExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var fy = FinancialYearHelper.GetFinancialYear(DateTime.Now);

        context.VoucherSequences.Add(new VoucherSequence { FinancialYear = fy, LastNumber = 41 });
        await context.SaveChangesAsync();

        var service = new VoucherNumberService(context);

        // Act
        var result = await service.GetNextVoucherNumberAsync();

        // Assert
        Assert.Equal($"{fy}0042", result);
        var sequence = await context.VoucherSequences.SingleAsync();
        Assert.Equal(fy, sequence.FinancialYear);
        Assert.Equal(42, sequence.LastNumber);
    }

    [Theory]
    [InlineData(2023, 4, 1, "202324")]
    [InlineData(2023, 3, 31, "202223")]
    [InlineData(2024, 5, 15, "202425")]
    public void GetFinancialYear_ReturnsCorrectFinancialYear(int year, int month, int day, string expected)
    {
        // Arrange
        var date = new DateTime(year, month, day);

        // Act
        var result = FinancialYearHelper.GetFinancialYear(date);

        // Assert
        Assert.Equal(expected, result);
    }
}
