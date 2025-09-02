using MbfApp.Data;
using MbfApp.Tests.Functional.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using System.Text;

namespace MbfApp.Tests.Functional.Pages;

[Collection("Database")]
public class MemberLedgerPagesTests : BlazorPageTest
{
    private readonly DatabaseFixture _fixture;

    public MemberLedgerPagesTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    protected override string ConnectionString => _fixture.ConnectionString;

    [Fact]
    public async Task Upload_Ledger_Works()
    {
        // Arrange
        await Page.GotoBlazorServerPageAsync("/upload-ledger");

        var narration = "Test CSV Upload";
        var csvContent = GetCSVContent();

        // Act
        await Page.GetByLabel("Narration").FillAsync(narration);

        await Page.GetByLabel("CSV File").SetInputFilesAsync(new FilePayload
        {
            Name = "test.csv",
            MimeType = "text/csv",
            Buffer = Encoding.UTF8.GetBytes(csvContent)
        });

        await Assertions.Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Upload File" })).ToBeEnabledAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Upload File" }).ClickAsync();

        // Assert
        // Check for success message on the page
        await Assertions.Expect(Page.GetByText("CSV uploaded and processed successfully!")).ToBeVisibleAsync();

        // Verify data in the database
        await using var scope = Host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var member = await context.MemberLedgers.FirstOrDefaultAsync(m => m.EmpCode == "6001");
        Assert.NotNull(member);

        var journal = await context.Journals.FirstOrDefaultAsync(j => j.Narration == narration);
        Assert.NotNull(journal);
    }


    [Fact]
    public async Task Upload_Fails_When_No_Narration_Or_File_Is_Provided()
    {
        // Arrange
        await Page.GotoBlazorServerPageAsync("/upload-ledger");

        // Act & Assert
        // Initially, the button should be disabled
        await Assertions.Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Upload File" })).ToBeDisabledAsync();

        // Provide only narration
        await Page.GetByLabel("Narration").FillAsync("Test Narration");
        await Assertions.Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Upload File" })).ToBeDisabledAsync();

        // Provide only a file
        await Page.GetByLabel("Narration").FillAsync(""); // Clear narration
        
        var csvContent = GetCSVContent();
        await Page.GetByLabel("CSV File").SetInputFilesAsync(new FilePayload
        {
            Name = "test.csv",
            MimeType = "text/csv",
            Buffer = Encoding.UTF8.GetBytes(csvContent)
        });
        await Assertions.Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Upload File" })).ToBeDisabledAsync();
    }

    private string GetCSVContent()
    {
        var csvContent = "EmpCode,YearMonth,DepositCr,LoanCr\n6001,202504,1500,500";
        return csvContent;
    }
}
